// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Prism.Common;
using Prism.Regions;

namespace RobotHmi.Views
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public sealed partial class Shell
    {
        public Shell(IRegionManager regionManager)
        {
            this.InitializeComponent();
            this.RegionManager = regionManager;
            this.Loaded += this.OnLoaded;
        }

        public IRegionManager RegionManager { get; private set; }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.RegionManager.Regions[RegionNames.MainContent].NavigationService.NavigationFailed += this.OnNavigationFailed;
        }

        private async void OnAboutExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            await this.ShowMetroDialogAsync(new AboutWorkstation(this, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Accented }));
        }

        private void OnBrowseBackCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.RegionManager.Regions[RegionNames.MainContent].NavigationService.Journal.CanGoBack;
            e.Handled = true;
        }

        private void OnBrowseBackExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.RegionManager.Regions[RegionNames.MainContent].NavigationService.Journal.GoBack();
            e.Handled = true;
        }

        private void OnGoToPageExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var uri = new Uri(e.Parameter as string, UriKind.Relative);
            var target = UriParsingHelper.GetAbsolutePath(uri);
            var parameters = UriParsingHelper.ParseQuery(uri);
            this.RegionManager.RequestNavigate(RegionNames.MainContent, target, parameters);
            e.Handled = true;
        }

        private void OnSettingsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SignInFlyout.IsOpen = false;
            this.SettingsFlyout.IsOpen = true;
            e.Handled = true;
        }

        private void OnSignInExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SettingsFlyout.IsOpen = false;
            this.SignInFlyout.IsOpen = true;
            e.Handled = true;
        }

        private void OnNavigationFailed(object sender, RegionNavigationFailedEventArgs e)
        {
            this.ShowMessageAsync("Navigation Service", e.Error.Message ?? "Navigation failed.");
        }
    }
}