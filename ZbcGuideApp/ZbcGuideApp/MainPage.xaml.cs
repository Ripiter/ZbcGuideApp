using Android.Content;
using Android.Locations;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ZbcGuideApp
{
    public partial class MainPage : ContentPage
    {
        bool isSelected = false;
        bool doneSearchin = false;
        WifiConnection wific = new WifiConnection();
        public MainPage()
        {
            InitializeComponent();

            AskPermision();
            wific.ErrorLoading += ErrorOccured;
            wific.PathFound += Wific_PathFound;
            isSelected = false;
            ImportingMap.StartLoading();

        }

        private void Wific_PathFound(object sender, EventArgs e)
        {
            doneSearchin = true;
            DisplayAlert("Found Path", "Found Path", "ok");
        }

        private void ErrorOccured(object sender, EventArgs e)
        {
            if (isSelected == true)
                DisplayAlert("Error Occurreed", "Please try again later", "ok");
        }

        async private void AskPermision()
        {
            LocationManager mc = (LocationManager)WifiConnection.context.GetSystemService(Context.LocationService);
            if (mc.IsProviderEnabled(LocationManager.GpsProvider))
                Debug.WriteLine("Enabled");
            else
            {
                bool x = await DisplayAlert("Need Gps", "Need Gps", "ok", "no");

                if (x == true)
                {
                    Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                    intent.AddFlags(ActivityFlags.NewTask);
                    intent.AddFlags(ActivityFlags.MultipleTask);
                    Android.App.Application.Context.StartActivity(intent);
                }
            }

        }
        async private void NewPage(object sender, EventArgs e)
        {
            //if (isSelected == false)
            //    return;

            //if (doneSearchin == false)
            //    return;

            if (isSelected == true && doneSearchin == true)
                await Navigation.PushAsync(new SingalStrenght());

            //await Navigation.PushAsync(new GpsLocationXaml());
        }
        async private void Camera(object sender, EventArgs e)
        {
            if (isSelected == true && doneSearchin == true)
            {
                bool x = await GetCameraPermission();
                if (x == true)
                    await Navigation.PushAsync(new CameraPage());
            }

        }

        Image Zbc = new Image
        {
            HeightRequest = 100,
            Source = ImageSource.FromResource("ZbcGuideApp.Img.zbc.jpg")
        };

        async Task<bool> GetCameraPermission()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                    {
                        bool result = await DisplayAlert("Camera access needed", "App needs Camera access enabled to work.", "ENABLE", "CANCEL");

                        if (!result)
                            return false;
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Camera))
                        status = results[Permission.Camera];
                }

                if (status == PermissionStatus.Granted)
                {
                    return true;
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Could not access Camera", "App needs Camera access to work. Go to Settings >> App to enable Camera access ", "GOT IT");
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        Picker picker;
        private void Location_SelectedIndexChanged(object sender, EventArgs e)
        {
            picker = (Picker)sender;
            wific.SetGoPos(picker.SelectedItem.ToString());
            wific.GetWifiNetworks();
            Debug.WriteLine(picker.SelectedItem);
            isSelected = true;
            doneSearchin = false;
        }
    }
}

#region Suprice(open me)                                                                                                                                             
//                                                                                             .,,,,,**,                                                         
//                                                                                           ,,,,...,,,,/.                                                       
//                                                                                         .,,,..,,,,,,,**/.                                                     
//                                                                                         ,,,..,,,,,,,,,**/                                                     
//                                                                                         ,,,.,,,,,,,,,,**/*                                                    
//                                                                                         ,,,,,,,,,,,,,,,**/,                                                   
//                                                                                         ,,,,,,,,,,,,,,,***/.                                                  
//                                                                                         .,*,,*,,,,,,,,,***/*,.                                                
//                                                                                         .***,,,,,,,,,,,,**/*/***//,.                             .*/((##(((/,.
//       ..*/(#######((*.                                                                  ,*,.,,,,,,,,,,,,,,*,,,,,,****/.                        .(%%%%%%%%%%%%%
//    .*##################*                                                               ,,.,,,,,,,,,,,,*//*,,,,,,,,******                        ./%%%%%%%%%%%%
// .*######################,                                                            ./////#/,,,,,,(#*,,,,,,,***********                          *(((((##%%%%
//(########################(.                                                         .#&&&&&&&&&&&&&(,,,,,,,****//*******,                                   .(%
//#########(/,..,(#########/.                                                     .*,...,,,%/*/#%&&&&&@@#//***/*,******/*.                           ,/(//*,,*(%%
//#######*.     .(#########.                                                     **..,,,,,(/.../%,,/&@@@@@#**/.                                     *%%%%%%%%%%%%
//   ...     ./#########(*                                                       **,/,,,./%,,*%/.(%/.,**#&@%.                                     ,%%%%%%%%%%%%%%
//        .*##########/,                                                          ./*,,,,#(.*%/,#(,,,,*****.                                     *%%%%%%%%%/.,,,,
//        *#########/                                                             **,.,,*#/,%/*%(,,,,,******.                                     *#%%%%%%%,     
//        ,##########,                                                           ./,.,,.,#*#(*%*.,,,********,                                   .... ,/#%%(      
//        .(##########*                                                          .*,,#&&&#*(%#%%#/****,,..                                  (%%%%%%(.  .       
//         *###########,                                                         .*#%%&&&&@&/%@@@@@@@(*.                                     .%%%%%%%%%/         
//          .,,....                                                             ,#&@@&(/#@@@@@@@&&@@@@&*                                      .(%%%%%%%(.        
//              ,/#####(*                                                      *%&@@/     (@@@#.   *&@@@.                                        .,//*.          
//            *##########(.                                                   *%@@&*       &@#      .%@@/                                                        
//            (##########/.             .*/*                                  #&@&*        /&.       ,&@@#.                                                      
//            .* (#####/,.            ,%&&&@@&,                              ,#&@@(         ,(.        /@@&&,                                                     
//                                 ,%&%%&&@@&,                             ,%%@@&,         .           **@@&@(
//                                (&&%%&&&@@#                             *%&&@@#.         .          ,&@@@#                                                     
//                               %&%%%&&&@@%.                           #%&&&&@@(          .          ,&@@@@#,                                                   
//                              /&%#%&&&@@&,                          .#&&&&&&@@(      .(&%..(%%.    .*@@@@@@@(                                                  
//                             *&%#%&&&&@@*                        .,(%@&&&&&&@@(     .##%@*(&%@#    ./@@@@@@@@#                                                 
//                            .%%&&&&@@*                    .*#&&@@#,/##%&&&@@#.    *@@@@(%@@@&    .%@@@@&&%&@@@#/.                                            
//                            /&&%&&&&@@(                 ./%&&&@@%,      ,&&&&@&,    /@@@@/%@@@&    /@@@@#    ,#@@@@@&%/,.                                      
//                           .#&%&&&&&@@,             ,/#&&&@@@&(          (&&&&@(    /@@@@.(@@@#   **..,.      .*#@@@@@@&&&&&%#(/*,.                          
//                           .#&&&&&&&@@@#(((##%&&&&&@@@@&(,            ....,*#&/   .%@@#.,(#(*,,,.......,,          .*(&@@@&&&&&&&&&&&@@@,                  
//                            /&&&&&&&&@&&@&&&&&&&&@@@@@&(.             .,...,,,..*,.,*,,..............,,..,.               ./#&@@@@&&&&&&&&&@@@&/.              
//                             (@&&&&&&&&&&&&&@@@@@@&/.                  ,,...,,,.,...,,,,,,...........*,,.                      ,/%@@@@&&%%&&&@@@@&/            
//             .,....      ,     /%@@@@@@@@@@/,.                     .,,,,......,..* (%&&&&&&&&&&&&%*........,,,,..                  ./@@@&&&%&&&&@@@&*         
//           .,...        ..,         ....                          .,,...........*%(((#&&&&&&&&&@@&,.............,.                   ,%@@@&&%%&&&@@@&/       
//          ,,..         ...*.                                    .,.............(&&&&&&&&&&&@@@@@@@#,.................                     /&@@&&%%&&&@@@@,     
//         .,..         ...,,                                    .................*#&@@@@@@@@@&%#*,...................                        /&@@&&%%&&@@@@%,   
//         *,.        ....*                                      ...................................................,.                           /&@@&&&&&@@@@&, 
//         *,.       ...,,                                        ....................,,,****,,,................,,*,                               ,#@@@&&&@@@@@*
//         .,.      ....,                                          ,,..........,,,****,.........,,,,,,,,*******..                                    ,%@@@@@@@@@&
//         .,,.     .,,,.                                              .,,,,..    ...,.....,*,,,....... ...,*,                                          .(&@@@@@&
//        ,.....     ..,.                                            .*******/(,.,,**...,,..........,,,*.                                            ,,,.  .*//, 
//      ..            ..,,                                         ,/*,,******/#/.....................,/(/.                                  .         ..,,      
//     ,                ..,.                                     */*,,*********/#*..................,*/(((((*                               .,           ..*.    
//   ,.                 ...,.                                  .*/,,,**********/#(,................,*((((((((/                               ,            ..*.   
// ,,                    ...,                                 .//,,************/(%&(,............,*(((((//(((#.                              .,            .,,   
//.                     ....,,                               */*,,*************/#(/%@&(*,,.,,,,*/&@%((/***/((((                               ,,.          .,,   
//          .,,..      ......,,,.                           */*,,**************/#(**(@@@@@(((((#&@%/****/(((#.                            .,             .,.   
//      .,.              ....,,.,.                         */*,,**************/(%#/**/%@@@@((((#(#&@@#*****/((#*                         .,,              ..     
//    ,.                  ...,,..,.                       ./*,,***************/#&@#******/(((/(((&&&@@(****/((#/                        *.               ..      
//  ,.                    ...,,..,.                      ./*,,***************/(%@@&/*********/(##%@&@@&/***/((#(                      .,                ..,      
// ,.                    ........,.                     .*/,,****************/#&@@@(******///(((##&&&@@#***/((#(                     .,                  ..,     
//.                     ..... ...*,.                    */*,,***************/((%@@@%*********/(###&&&@@&***/((((                    .,                    ..,.   
//                    ......  .,*@%* (/.               .*/*,***************//(((%@@@&(//////((##((#&&&&@@/**/((#(                   .,             .,,      ..,,. 
//                   ......  ../&@@#*/(*             .*/*,,**************/(((((%@@@@#*//(((((((((#@&&&@@(***/(#(                  .,.                .,.     ..,,
//                .....,,.  .,(@@@@&/*/(.            */*,,**************/((((((&@@@@%*****/((((((#@@&&&@%/**/(((                 ,.,.                   ,.     ..
//            ......,,.   ./%@@@@@@&(**//.         .***,**************/(((((((#@@@@@%/*****((((((#@@&&&@@(**/(((.              ., .,                      ..     
//         ......,*,...,*#&@@@@@@@@@#**/((*       ./****************//((((((((%@@@@@&/*****/(((((%@&&&&@@#***/((.             ,(,  ..                      ..    
//     .....,,,,.        *&&&&&&&@@@(**/((((,.  .*/****************/((((#(((((@@@@@@&/*****/(((((%@&&&&@@%***/((.            /&/,   .                       ..   
//...,,,,,.             .**#&&&&&@@&(**/((((//**///**************/(((((%#((((&@@@@@@&/*****/(((((&@&&&&&@&/**/((*         .((&@(,                             .  
//,,.                  .*** ((%&&&@@%/**/((((/*//***//***********/((((%#(((((&@@@@@@@&/*****/(((((&@&&&&&@@(***(#/        ,#(#@@&*.                               
//                     */**(((#&&@&(**/(((#(/**//****//********/(((##(((((%@@@@@@@@@&/*****/(((((@@&&&&&@@(**/(((,     *(#//%@@@%,.     .                        
//                     */**(((((#%/**/((((#(/****//****/*****/(((#(((((#&@@@@@@@@@@@%//****/(((((@@&&&&&@@#**/((((. ./(/(/*/%@@@@&/,......,,,                    
//                     , (*, (####(***/((((##(******///******/((((%%%&&@@@@@@@@@@@@@@@%/*****/((((#@&&&&&&@@#*/(####%#(#///**/%@@@@@@*,......,,,.                
//                      /#/*******//((/((##/********//****/(((#&@@@@@@@@@@@@@@@@@@@@#/*****/((((%@&&&&&&@@#/(/(#(/*/((*****/%@@@@@@@@@%(/,,,,,,..,,,.            
//                        *##((((((//*/(##/**************/((#&@@@@&&@@@@@@@@@@@@@@@@#/*****/((((&@&&&&&&@@#*/#******#/******(&@@@@%(((((/.           .,*,..      
//                         ,/*******//(#(/*************/(((%@@@&&&&&&&&@@@@@@@@@@@@&(/*****/(((#@@&&&&&&@@(/(******/#*******/#@@%(((((((*                .,*,... 
//                           , (#(((####/*************/((((%@@&&&&&&&&&&&&@@@@@@@@@@&(/*****((((%@&&&&&&@@@#/*******/#********/#(((//*/(#.                    .,*,
//                               */*****************/(((%@@@&&&&&&&&&&&&&@@@@@@@@@@%(/*****((((&@&&&&&&@@@(********/#/*********/(/**//((                         
//                                 ./(/*********//(((%&@@@&&&&&&&&&&&&&&&&@@@@@@@@&((/*****(((#@@&&&&&&@@@/*********((*************/(#*                          
//                                    ./##(((((((###%@@&&&&&&&&&&&&&&&&&&&@@@@@@@@%((/****/(((%@@&&&&&&@@&/*********/#/****((((//((#/.                           
//                                       .,*/((/*. *&&&&&&&&&&&&&&&&&&&&&&@@@@@@@@#((*****/(((&@&&&&&&&@@&/*********/##(/**/((((((/.                             
//                                                .***/(#&@@&&&&&&&&&&&&&@@@@@@@@&((/*****(((%@@&&&&&&@@@%******//(((((###((((##/                                
//                                               .********* (%@@@@@&&&&&&@@@@@@@@@#((/****/(((&@@&&&&&&@@@#((((((((((((((*..,..                                 
#endregion(open me)