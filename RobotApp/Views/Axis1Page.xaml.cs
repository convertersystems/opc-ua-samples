using System;

using RobotApp.ViewModels;

using Windows.UI.Xaml.Controls;

namespace RobotApp.Views
{
    public sealed partial class Axis1Page : Page
    {
        public Axis1ViewModel ViewModel { get; } = new Axis1ViewModel();

        public Axis1Page()
        {
            InitializeComponent();
        }
    }
}
