// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Practices.Unity;
using RobotApp.Services;
using RobotApp.ViewModels;
using RobotApp.Views;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RobotApp
{
    /// <summary>
    /// Documentation on APIs used in this page: https://github.com/Windows-XAML/Template10/wiki
    /// </summary>
    public sealed partial class App : Template10.Common.BootStrapper
    {
        private UnityContainer container = new UnityContainer();

        static App()
        {
            MetroLog.LogManagerFactory.DefaultConfiguration = new MetroLog.LoggingConfiguration();
#if DEBUG
            MetroLog.LogManagerFactory.DefaultConfiguration.AddTarget(MetroLog.LogLevel.Trace, MetroLog.LogLevel.Fatal, new MetroLog.Targets.DebugTarget());
#else
            MetroLog.LogManagerFactory.DefaultConfiguration.AddTarget(MetroLog.LogLevel.Info, MetroLog.LogLevel.Fatal, new MetroLog.Targets.StreamingFileTarget());
#endif
        }

        public App()
        {
            this.InitializeComponent();
            this.SplashFactory = (e) => new Views.Splash(e);
            var settings = Services.SettingsServices.SettingsService.Instance;
            this.RequestedTheme = settings.AppTheme;
            this.CacheMaxDuration = settings.CacheMaxDuration;
            this.ShowShellBackButton = settings.UseShellBackButton;
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            // Register services with container's lifetime to create singletons that will be disposed when the container is disposed.
            this.container.RegisterType<PLC1Service>(new ContainerControlledLifetimeManager());

            // Register view models using the name of the view.
            this.container.RegisterType<INavigable, MainPageViewModel>(nameof(MainPage)/*, new ContainerControlledLifetimeManager()*/);
            this.container.RegisterType<INavigable, SettingsPageViewModel>(nameof(SettingsPage)/*, new ContainerControlledLifetimeManager()*/);
            this.container.RegisterType<INavigable, AxisPageViewModel>(nameof(AxisPage)/*, new ContainerControlledLifetimeManager()*/);

            if (Window.Current.Content as ModalDialog == null)
            {
                // create a new frame
                var nav = this.NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

                // create modal root
                Window.Current.Content = new ModalDialog
                {
                    DisableBackButtonWhenModal = true,
                    Content = new Views.Shell(nav),
                    ModalContent = new Views.Busy(),
                };
            }

            await Task.CompletedTask;
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            this.NavigationService.Navigate(typeof(Views.MainPage));
            await Task.CompletedTask;
        }

        public override Task OnSuspendingAsync(object s, SuspendingEventArgs e, bool prelaunchActivated)
        {
            this.container.Dispose();
            return base.OnSuspendingAsync(s, e, prelaunchActivated);
        }

        public override INavigable ResolveForPage(Page page, NavigationService navigationService)
        {
            // Search container for the view model registered with the name of the given view.
            return this.container.Resolve<INavigable>(page.GetType().Name);
        }
    }
}