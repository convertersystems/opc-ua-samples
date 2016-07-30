// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml;

namespace RobotApp.Services.SettingsServices
{
    public class SettingsService
    {
        private Template10.Services.SettingsService.ISettingsHelper helper;

        static SettingsService()
        {
            // implement singleton pattern
            Instance = Instance ?? new SettingsService();
        }

        private SettingsService()
        {
            this.helper = new Template10.Services.SettingsService.SettingsHelper();
        }

        public static SettingsService Instance { get; }

        public bool UseShellBackButton
        {
            get { return this.helper.Read<bool>(nameof(this.UseShellBackButton), true); }

            set
            {
                this.helper.Write(nameof(this.UseShellBackButton), value);
                BootStrapper.Current.NavigationService.Dispatcher.Dispatch(() =>
                {
                    BootStrapper.Current.ShowShellBackButton = value;
                    BootStrapper.Current.UpdateShellBackButton();
                    BootStrapper.Current.NavigationService.Refresh();
                });
            }
        }

        public ApplicationTheme AppTheme
        {
            get
            {
                var theme = ApplicationTheme.Light;
                var value = this.helper.Read<string>(nameof(this.AppTheme), theme.ToString());
                return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
            }

            set
            {
                this.helper.Write(nameof(this.AppTheme), value.ToString());
                (Window.Current.Content as FrameworkElement).RequestedTheme = value.ToElementTheme();
                Views.Shell.HamburgerMenu.RefreshStyles(value);
            }
        }

        public TimeSpan CacheMaxDuration
        {
            get { return this.helper.Read<TimeSpan>(nameof(this.CacheMaxDuration), TimeSpan.FromDays(2)); }

            set
            {
                this.helper.Write(nameof(this.CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }

        public string PLC1EndpointUrl
        {
            get { return this.helper.Read<string>(nameof(this.PLC1EndpointUrl), "opc.tcp://localhost:26543"); }
        }
    }
}