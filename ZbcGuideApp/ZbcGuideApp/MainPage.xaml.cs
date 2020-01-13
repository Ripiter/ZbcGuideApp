﻿using Android.Content;
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
        bool mapLoaded = false;
        WifiConnection wific;
        public MainPage()
        {
            InitializeComponent();
            wific = new WifiConnection();
            wific.PathFound += PathFound;
            AskPermision();
            isSelected = false;

            // Loads map of roskilde
            // TODO: Add thread to do it
            ImportingMap.StartLoading();
        }


        Image Zbc = new Image
        {
            HeightRequest = 100,
            Source = ImageSource.FromResource("ZbcGuideApp.Img.zbc.jpg")
        };

        private void PathFound(object sender, EventArgs e)
        {
            //Location.IsEnabled = true;
            mapLoaded = true;
            StateLabel.Text = "State: Path Found";
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
        async private void MapPage(object sender, EventArgs e)
        {
            if (isSelected == true && mapLoaded == true && OurPosition.scanned == true)
            {
                wific.GeneratePath();
                await Navigation.PushAsync(new SingalStrenght());
            }
        }
        async private void Camera(object sender, EventArgs e)
        {
            bool x = await GetCameraPermission();
            if (x == true)
                await Navigation.PushAsync(new QrCodeMain());
        }

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
        private void Location_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;
            StateLabel.Text = "State: Searching";

            // Sets x and y position of where we want to go
            wific.SetGoPos(picker.SelectedItem.ToString());
            mapLoaded = false;
            wific.GenerateMap();
            isSelected = true;
            //Location.IsEnabled = false;

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