// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RobotApp.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RobotApp.Views
{
    public sealed partial class SettingsPage : Page
    {
        Template10.Services.SerializationService.ISerializationService _SerializationService;

        public SettingsPage()
        {
            this.InitializeComponent();
            this._SerializationService = Template10.Services.SerializationService.SerializationService.Json;
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        public SettingsPageViewModel ViewModel => this.DataContext as SettingsPageViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var index = int.Parse(this._SerializationService.Deserialize(e.Parameter?.ToString()).ToString());
            this.MyPivot.SelectedIndex = index;
        }
    }
}
