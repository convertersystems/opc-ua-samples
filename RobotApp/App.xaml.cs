// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Practices.Unity;
using RobotApp.Services;
using RobotApp.ViewModels;
using RobotApp.Views;
using System;
using System.Threading.Tasks;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Workstation.ServiceModel.Ua;

namespace RobotApp
{
    /// <summary>
    /// Documentation on APIs used in this page: https://github.com/Windows-XAML/Template10/wiki
    /// </summary>
    public sealed partial class App : Template10.Common.BootStrapper
    {
        private UnityContainer container = new UnityContainer();

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
            // Register shared services with the application's dependency injection container.
            var appDescription = new ApplicationDescription()
            {
                ApplicationName = "Workstation.RobotApp",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.RobotApp",
                ApplicationType = ApplicationType.Client
            };
            var appCertificate = appDescription.GetCertificate();
            var userIdentity = new AnonymousIdentity();
            var endpointUrl = Services.SettingsServices.SettingsService.Instance.PLC1EndpointUrl;
            this.container.RegisterType<PLC1Service>(new ContainerControlledLifetimeManager(), new InjectionConstructor(appDescription, appCertificate, userIdentity, endpointUrl));

            // Register view models using the name of the view.
            this.container.RegisterType<INavigable, MainPageViewModel>(nameof(MainPage)/*, new ContainerControlledLifetimeManager()*/);
            this.container.RegisterType<INavigable, SettingsPageViewModel>(nameof(SettingsPage)/*, new ContainerControlledLifetimeManager()*/);
            this.container.RegisterType<INavigable, AxisPageViewModel>(nameof(AxisPage)/*, new ContainerControlledLifetimeManager()*/);

            this.NavigationService.Navigate(typeof(Views.MainPage));
            await Task.CompletedTask;
        }

        public async override Task OnSuspendingAsync(object s, SuspendingEventArgs e, bool prelaunchActivated)
        {
            var session = this.container.Resolve<PLC1Service>();
            await session.SuspendAsync();
        }

        public override void OnResuming(object s, object e, AppExecutionState previousExecutionState)
        {
            var session = this.container.Resolve<PLC1Service>();
            session.Resume();
        }

        public override INavigable ResolveForPage(Page page, NavigationService navigationService)
        {
            // Search container for the view model registered with the name of the given view.
            return this.container.Resolve<INavigable>(page.GetType().Name);
        }
    }
}