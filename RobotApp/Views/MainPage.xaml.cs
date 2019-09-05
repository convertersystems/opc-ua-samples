using System;
using RobotApp.ViewModels;
using Windows.UI.Xaml.Controls;

namespace RobotApp.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; } = Helpers.Singleton<MainPageViewModel>.Instance;

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
