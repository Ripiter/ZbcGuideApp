﻿using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ZbcGuideApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        async private void NewPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GpsLocationXaml());
        }
    }
}
