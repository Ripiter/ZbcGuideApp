using Android.Content;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using ZbcGuideApp.CustomViews;
using ZbcGuideApp.Droid;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Size = Android.Util.Size;

namespace ZbcGuideApp.Droid.Camera
{
    public class CameraDroid : FrameLayout, TextureView.ISurfaceTextureListener
    {
        #region Camera States

        // Camera state: Showing camera preview.
        public const int statePreview = 0;

        // Camera state: Waiting for the focus to be locked.
        public const int stateWaitingLock = 1;

        // Camera state: Waiting for the exposure to be precapture state.
        public const int stateWaitingPrecapture = 2;

        //Camera state: Waiting for the exposure state to be something other than precapture.
        public const int stateWaitingNonPrecapture = 3;

        // Camera state: Picture was taken.
        public const int statePictureTaken = 4;

        #endregion

        // The current state of camera state for taking pictures.
        public int mState = statePreview;

        private static readonly SparseIntArray Orientations = new SparseIntArray();

        public event EventHandler<byte[]> Photo;

        public bool OpeningCamera { private get; set; }

        public CameraDevice CameraDevice;

        private readonly CameraStateListener cameraStateListener;
        private readonly CameraCaptureListener cameraCaptureListener;

        private CaptureRequest.Builder previewBuilder;
        private CaptureRequest.Builder captureBuilder;
        private CaptureRequest previewRequest;
        private CameraCaptureSession previewSession;
        private SurfaceTexture viewSurface;
        private readonly TextureView cameraTexture;
        private Size previewSize;
        private readonly Context context;
        private CameraManager manager;

        private bool flashSupported;
        private Size[] supportedJpegSizes;
        private Size idealPhotoSize = new Size(480, 640);

        private HandlerThread backgroundThread;
        private Handler backgroundHandler;

        private ImageReader imageReader;
        private string cameraId;

        private LensFacing lensFacing;

        public void SetCameraOption(CameraOptions cameraOptions)
        {
            this.lensFacing = (cameraOptions == CameraOptions.Rear) ? LensFacing.Front : LensFacing.Back;
        }

        public CameraDroid(Context context) : base(context)
        {
            this.context = context;

            var inflater = LayoutInflater.FromContext(context);

            if (inflater == null)
                return;

            View view = inflater.Inflate(Resource.Layout.CameraLayout, this);

            cameraTexture = view.FindViewById<TextureView>(Resource.Id.cameraTexture);

            cameraTexture.SurfaceTextureListener = this;

            cameraStateListener = new CameraStateListener { Camera = this };
        }

        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            viewSurface = surface;

            StartBackgroundThread();

            OpenCamera(width, height);
        }

        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            StopBackgroundThread();

            return true;
        }

        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
        {

        }

        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {

        }

        private void SetUpCameraOutputs(int width, int height)
        {
            manager = (CameraManager)context.GetSystemService(Context.CameraService);

            string[] cameraIds = manager.GetCameraIdList();

            cameraId = cameraIds[0];

            for (int i = 0; i < cameraIds.Length; i++)
            {
                CameraCharacteristics chararc = manager.GetCameraCharacteristics(cameraIds[i]);

                var facing = (Integer)chararc.Get(CameraCharacteristics.LensFacing);
                if (facing != null && facing == (Integer.ValueOf((int)lensFacing)))
                    continue;

                cameraId = cameraIds[i];
            }

            var characteristics = manager.GetCameraCharacteristics(cameraId);
            var map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);

            if (supportedJpegSizes == null && characteristics != null)
            {
                supportedJpegSizes = ((StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap)).GetOutputSizes((int)ImageFormatType.Jpeg);
            }

            if (supportedJpegSizes != null && supportedJpegSizes.Length > 0)
            {
                //MAGIC NUMBER WHICH HAS PROVEN TO BE THE BEST
                idealPhotoSize = GetOptimalSize(supportedJpegSizes, 950, 1200);
            }

            imageReader = ImageReader.NewInstance(idealPhotoSize.Width, idealPhotoSize.Height, ImageFormatType.Jpeg, 1);

            ImageAvailableListener readerListener = new ImageAvailableListener();

            readerListener.Photo += (sender, buffer) =>
            {
                Photo?.Invoke(this, buffer);
            };

            var available = (Java.Lang.Boolean)characteristics.Get(CameraCharacteristics.FlashInfoAvailable);

            if (available == null)
            {
                flashSupported = false;
            }
            else
            {
                flashSupported = (bool)available;
            }

            imageReader.SetOnImageAvailableListener(readerListener, backgroundHandler);

            previewSize = GetOptimalSize(map.GetOutputSizes(Class.FromType(typeof(SurfaceTexture))), width, height);
        }

        public void OpenCamera(int width, int height)
        {
            if (context == null || OpeningCamera)
            {
                return;
            }

            OpeningCamera = true;

            SetUpCameraOutputs(width, height);

            manager.OpenCamera(cameraId, cameraStateListener, null);
        }

        public void StartPreview()
        {
            if (CameraDevice == null || !cameraTexture.IsAvailable || previewSize == null) return;

            SurfaceTexture texture = cameraTexture.SurfaceTexture;

            texture.SetDefaultBufferSize(previewSize.Width, previewSize.Height);

            Surface surface = new Surface(texture);

            previewBuilder = CameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
            previewBuilder.AddTarget(surface);

            List<Surface> surfaces = new List<Surface>();
            surfaces.Add(surface);
            surfaces.Add(imageReader.Surface);

            CameraDevice.CreateCaptureSession(surfaces, new CameraCaptureStateListener
            {
                OnConfigureFailedAction = session =>
                {
                },
                OnConfiguredAction = session =>
                {
                    previewSession = session;
                    UpdatePreview();
                }
            },backgroundHandler);
        }

        private void UpdatePreview()
        {
            if (CameraDevice == null || previewSession == null) return;

            previewBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture);
            SetAutoFlash(previewBuilder);

            previewRequest = previewBuilder.Build();
            previewSession.SetRepeatingRequest(previewRequest, cameraCaptureListener, backgroundHandler);
        }

        Size GetOptimalSize(IList<Size> sizes, int h, int w)
        {
            double AspectTolerance = 0.1;
            double targetRatio = (double)w / h;

            if (sizes == null)
            {
                return null;
            }

            Size optimalSize = null;
            double minDiff = double.MaxValue;
            int targetHeight = h;

            while (optimalSize == null)
            {
                foreach (Size size in sizes)
                {
                    double ratio = (double)size.Width / size.Height;

                    if (System.Math.Abs(ratio - targetRatio) > AspectTolerance)
                        continue;
                    if (System.Math.Abs(size.Height - targetHeight) < minDiff)
                    {
                        optimalSize = size;
                        minDiff = System.Math.Abs(size.Height - targetHeight);
                    }
                }

                if (optimalSize == null)
                    AspectTolerance += 0.1f;
            }

            return optimalSize;
        }

        public void SetAutoFlash(CaptureRequest.Builder requestBuilder)
        {
            if (flashSupported)
            {
                requestBuilder.Set(CaptureRequest.ControlAeMode, (int)ControlAEMode.OnAutoFlash);
            }
        }

        private void StartBackgroundThread()
        {
            backgroundThread = new HandlerThread("CameraBackground");
            backgroundThread.Start();
            backgroundHandler = new Handler(backgroundThread.Looper);
        }

        private void StopBackgroundThread()
        {
            backgroundThread.QuitSafely();
            try
            {
                backgroundThread.Join();
                backgroundThread = null;
                backgroundHandler = null;
            }
            catch (InterruptedException e)
            {
                e.PrintStackTrace();
            }
        }

        public void RunPrecaptureSequence()
        {
            try
            {
                previewBuilder.Set(CaptureRequest.ControlAePrecaptureTrigger, (int)ControlAEPrecaptureTrigger.Start);
                mState = stateWaitingPrecapture;
                previewSession.Capture(previewBuilder.Build(), cameraCaptureListener, backgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }
    }
}
