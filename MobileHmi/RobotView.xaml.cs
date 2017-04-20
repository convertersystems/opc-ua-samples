// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Urho;
using Xamarin.Forms;

namespace Workstation.MobileHmi
{
    public partial class RobotView : ContentView
    {
        private RobotGame robotGame;
        private bool initialized;

        public RobotView()
        {
            this.InitializeComponent();
        }

        protected async override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (!this.initialized)
            {
                if (height > 0 && width > 0)
                {
                    this.initialized = true;
                    this.robotGame = await this.UrhoSurface.Show<RobotGame>(new ApplicationOptions(assetsFolder: "Data") { Orientation = ApplicationOptions.OrientationType.LandscapeAndPortrait });
                }
            }
        }

        public float Axis1
        {
            get { return (float)this.GetValue(Axis1Property); }
            set { this.SetValue(Axis1Property, value); }
        }

        public static readonly BindableProperty Axis1Property =
            BindableProperty.Create(nameof(Axis1), typeof(float), typeof(RobotView), 0f, propertyChanged: new BindableProperty.BindingPropertyChangedDelegate(OnAxis1Changed));

        private static void OnAxis1Changed(BindableObject d, object oldValue, object newValue)
        {
            var control = (RobotView)d;
            var game = control.robotGame;
            if (game != null)
            {
                game.Axis1 = (float)newValue;
            }
        }

        public float Axis2
        {
            get { return (float)this.GetValue(Axis2Property); }
            set { this.SetValue(Axis2Property, value); }
        }

        public static readonly BindableProperty Axis2Property =
            BindableProperty.Create(nameof(Axis2), typeof(float), typeof(RobotView), 0f, propertyChanged: new BindableProperty.BindingPropertyChangedDelegate(OnAxis2Changed));

        private static void OnAxis2Changed(BindableObject d, object oldValue, object newValue)
        {
            var control = (RobotView)d;
            var game = control.robotGame;
            if (game != null)
            {
                control.robotGame.Axis2 = (float)newValue;
            }
        }

        public float Axis3
        {
            get { return (float)this.GetValue(Axis3Property); }
            set { this.SetValue(Axis3Property, value); }
        }

        public static readonly BindableProperty Axis3Property =
            BindableProperty.Create(nameof(Axis3), typeof(float), typeof(RobotView), 0f, propertyChanged: new BindableProperty.BindingPropertyChangedDelegate(OnAxis3Changed));

        private static void OnAxis3Changed(BindableObject d, object oldValue, object newValue)
        {
            var control = (RobotView)d;
            var game = control.robotGame;
            if (game != null)
            {
                control.robotGame.Axis3 = (float)newValue;
            }
        }

        public float Axis4
        {
            get { return (float)this.GetValue(Axis4Property); }
            set { this.SetValue(Axis4Property, value); }
        }

        public static readonly BindableProperty Axis4Property =
            BindableProperty.Create(nameof(Axis4), typeof(float), typeof(RobotView), 0f, propertyChanged: new BindableProperty.BindingPropertyChangedDelegate(OnAxis4Changed));

        private static void OnAxis4Changed(BindableObject d, object oldValue, object newValue)
        {
            var control = (RobotView)d;
            var game = control.robotGame;
            if (game != null)
            {
                control.robotGame.Axis4 = (float)newValue;
            }
        }
    }
}
