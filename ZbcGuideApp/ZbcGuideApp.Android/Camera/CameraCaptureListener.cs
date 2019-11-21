using Android.Hardware.Camera2;
using Java.Lang;
using System;

namespace ZbcGuideApp.Droid.Camera
{
    public class CameraCaptureListener : CameraCaptureSession.CaptureCallback
    {
        private readonly CameraDroid owner;

        public CameraCaptureListener(CameraDroid owner)
        {
            this.owner = owner ?? throw new System.ArgumentNullException("owner");
        }

        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {
            Process(result);
        }

        public override void OnCaptureProgressed(CameraCaptureSession session, CaptureRequest request, CaptureResult partialResult)
        {
            Process(partialResult);
        }

        private void Process(CaptureResult result)
        {
            switch (owner.mState)
            {
                case CameraDroid.stateWaitingLock:
                    {
                        Integer afState = (Integer)result.Get(CaptureResult.ControlAfState);
                        if (afState == null)
                        {
                            owner.mState = CameraDroid.statePictureTaken;
                        }
                        else if ((((int)ControlAFState.FocusedLocked) == afState.IntValue()) ||
                                (((int)ControlAFState.NotFocusedLocked) == afState.IntValue()))
                        {
                            // ControlAeState can be null on some devices
                            Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);

                            if (aeState == null || aeState.IntValue() == ((int)ControlAEState.Converged))
                            {
                                owner.mState = CameraDroid.statePictureTaken;
                            }
                            else
                            {
                                owner.RunPrecaptureSequence();
                            }
                        }
                        break;
                    }
                case CameraDroid.stateWaitingPrecapture:
                    {
                        // ControlAeState can be null on some devices
                        Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                        if (aeState == null ||
                            aeState.IntValue() == ((int)ControlAEState.Precapture) ||
                            aeState.IntValue() == ((int)ControlAEState.FlashRequired))
                        {
                            owner.mState = CameraDroid.stateWaitingNonPrecapture;
                        }
                        break;
                    }
                case CameraDroid.stateWaitingNonPrecapture:
                    {
                        // ControlAeState can be null on some devices
                        Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                        if (aeState == null || aeState.IntValue() != ((int)ControlAEState.Precapture))
                        {
                            owner.mState = CameraDroid.statePictureTaken;
                        }
                        break;
                    }
            }
        }
    }
}