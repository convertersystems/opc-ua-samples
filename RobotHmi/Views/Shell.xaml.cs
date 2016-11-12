// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Prism.Common;
using Prism.Regions;
using Workstation.ServiceModel.Ua;

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

        /// <summary>
        /// Show a Sign In dialog if the Session demands a UserNameIdentity token.
        /// </summary>
        /// <param name="endpoint">The remote endpoint.</param>
        /// <returns>A UserIdentity</returns>
        public Task<IUserIdentity> ProvideUserIdentity(EndpointDescription endpoint)
        {
            if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.Anonymous))
            {
                return Task.FromResult<IUserIdentity>(new AnonymousIdentity());
            }

            if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.UserName))
            {
                var tcs = new TaskCompletionSource<IUserIdentity>();

                this.Dispatcher.InvokeAsync(
                    async () =>
                    {
                        var userNamesDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(RobotHmi.Properties.Settings.Default.UserNames) ?? new Dictionary<string, string>();
                        var userNameKey = $"userName_{endpoint.EndpointUrl}";

                        var initialUserName = string.Empty;
                        if (userNamesDictionary.ContainsKey(userNameKey))
                        {
                            initialUserName = userNamesDictionary[userNameKey];
                        }

                        LoginDialogSettings loginSettings = new LoginDialogSettings { InitialUsername = initialUserName };
                        var result = await this.ShowLoginAsync("SIGN IN", $"Connecting to server '{endpoint.Server.ApplicationName}' at '{endpoint.EndpointUrl}'.", loginSettings);
                        if (result != null && !string.IsNullOrEmpty(result.Username))
                        {
                            userNamesDictionary[userNameKey] = result.Username;
                            RobotHmi.Properties.Settings.Default.UserNames = JsonConvert.SerializeObject(userNamesDictionary);
                            RobotHmi.Properties.Settings.Default.Save();

                            tcs.TrySetResult(new UserNameIdentity(result.Username, result.Password));
                        }
                        tcs.TrySetResult(new AnonymousIdentity());
                    },
                    DispatcherPriority.ApplicationIdle);
                return tcs.Task;
            }

            throw new NotImplementedException("ProvideUserIdentity supports only UserName and Anonymous identity, for now.");
        }

    }
}