// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Urho;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RobotApp.Views
{
    public sealed partial class RobotControl : UserControl
    {
        private RobotGame robotGame;

        public RobotControl()
        {
            this.InitializeComponent();
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        public float Axis1
        {
            get { return (float)this.GetValue(Axis1Property); }
            set { this.SetValue(Axis1Property, value); }
        }

        public static readonly DependencyProperty Axis1Property =
            DependencyProperty.Register("Axis1", typeof(float), typeof(RobotControl), new PropertyMetadata(0f, new PropertyChangedCallback(OnAxis1Changed)));

        private static void OnAxis1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RobotControl)d;
            var game = control.robotGame;
            if (game != null)
            {
                game.Axis1 = (float)e.NewValue;
            }
        }

        public float Axis2
        {
            get { return (float)this.GetValue(Axis2Property); }
            set { this.SetValue(Axis2Property, value); }
        }

        public static readonly DependencyProperty Axis2Property =
            DependencyProperty.Register("Axis2", typeof(float), typeof(RobotControl), new PropertyMetadata(0f, new PropertyChangedCallback(OnAxis2Changed)));

        private static void OnAxis2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RobotControl)d;
            var game = control.robotGame;
            if (game != null)
            {
                control.robotGame.Axis2 = (float)e.NewValue;
            }
        }

        public float Axis3
        {
            get { return (float)this.GetValue(Axis3Property); }
            set { this.SetValue(Axis3Property, value); }
        }

        public static readonly DependencyProperty Axis3Property =
            DependencyProperty.Register("Axis3", typeof(float), typeof(RobotControl), new PropertyMetadata(0f, new PropertyChangedCallback(OnAxis3Changed)));

        private static void OnAxis3Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RobotControl)d;
            var game = control.robotGame;
            if (game != null)
            {
                control.robotGame.Axis3 = (float)e.NewValue;
            }
        }

        public float Axis4
        {
            get { return (float)this.GetValue(Axis4Property); }
            set { this.SetValue(Axis4Property, value); }
        }

        public static readonly DependencyProperty Axis4Property =
            DependencyProperty.Register("Axis4", typeof(float), typeof(RobotControl), new PropertyMetadata(0f, new PropertyChangedCallback(OnAxis4Changed)));

        private static void OnAxis4Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RobotControl)d;
            var game = control.robotGame;
            if (game != null)
            {
                control.robotGame.Axis4 = (float)e.NewValue;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.robotGame == null)
            {
                this.robotGame = this.UrhoSurface.Run<RobotGame>(new ApplicationOptions("Assets") { Width = (int)this.UrhoSurface.ActualWidth, Height = (int)this.UrhoSurface.ActualHeight });
                this.robotGame.Axis1 = this.Axis1;
                this.robotGame.Axis2 = this.Axis2;
                this.robotGame.Axis3 = this.Axis3;
                this.robotGame.Axis4 = this.Axis4;
            }

            this.UrhoSurface.Resume();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.UrhoSurface.Pause();
        }
    }
}
