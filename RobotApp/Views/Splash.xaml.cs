// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RobotApp.Views
{
    public sealed partial class Splash : UserControl
    {
        public Splash(SplashScreen splashScreen)
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += (s, e) => this.Resize(splashScreen);
            this.Resize(splashScreen);
        }

        private void Resize(SplashScreen splashScreen)
        {
            if (splashScreen.ImageLocation.Top == 0)
            {
                this.splashImage.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                this.rootCanvas.Background = null;
                this.splashImage.Visibility = Visibility.Visible;
            }
            this.splashImage.Height = splashScreen.ImageLocation.Height;
            this.splashImage.Width = splashScreen.ImageLocation.Width;
            this.splashImage.SetValue(Canvas.TopProperty, splashScreen.ImageLocation.Top);
            this.splashImage.SetValue(Canvas.LeftProperty, splashScreen.ImageLocation.Left);
            this.ProgressTransform.TranslateY = this.splashImage.Height / 2;
        }
    }
}

