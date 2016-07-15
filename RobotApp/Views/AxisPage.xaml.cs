// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RobotApp.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RobotApp.Views
{
    public sealed partial class AxisPage : Page
    {
        private Template10.Services.SerializationService.ISerializationService serializationService;

        public AxisPage()
        {
            this.InitializeComponent();
            this.serializationService = Template10.Services.SerializationService.SerializationService.Json;
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        public AxisPageViewModel ViewModel
        {
            get { return this.DataContext as AxisPageViewModel; }
            set { this.DataContext = value; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var index = int.Parse(this.serializationService.Deserialize(e.Parameter?.ToString()).ToString());
            this.MyPivot.SelectedIndex = index;
        }
    }
}
