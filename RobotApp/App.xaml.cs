// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using RobotApp.Services;
using RobotApp.Services.SettingsServices;
using RobotApp.ViewModels;
using RobotApp.Views;
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
        private EventListener eventListener;
        private UnityContainer container = new UnityContainer();

        public App()
        {
            this.InitializeComponent();
            this.SplashFactory = (e) => new Splash(e);
            this.eventListener = new DebugEventListener();
            this.eventListener.EnableEvents(Workstation.ServiceModel.Ua.EventSource.Log, EventLevel.Verbose);
            var settings = SettingsService.Instance;
            this.RequestedTheme = settings.AppTheme;
            this.CacheMaxDuration = settings.CacheMaxDuration;
            this.ShowShellBackButton = settings.UseShellBackButton;
            IServiceLocator locator = new UnityServiceLocator(this.container);
            ServiceLocator.SetLocatorProvider(() => locator);
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
                    Content = new Shell(nav),
                    ModalContent = new Busy(),
                };
            }

            await Task.CompletedTask;
        }

        public override Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // Prepare for constructing the shared PLC1Session.
            var appDescription = new ApplicationDescription()
            {
                ApplicationName = "Workstation.RobotApp",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.RobotApp",
                ApplicationType = ApplicationType.Client
            };
            var appCertificate = appDescription.GetCertificate();
            var userIdentityProvider = new Func<EndpointDescription, Task<IUserIdentity>>(ep => this.ProvideUserIdentity(ep));
            var session = new PLC1Session(appDescription, appCertificate, userIdentityProvider);

            // Register shared services with the application's dependency injection container.
            this.container.RegisterInstance(session);

            // Register view models with the container using the name of the view.
            this.container.RegisterType<INavigable, MainPageViewModel>(nameof(MainPage), new ContainerControlledLifetimeManager());
            this.container.RegisterType<INavigable, AxisPageViewModel>(nameof(AxisPage), new ContainerControlledLifetimeManager());
            this.container.RegisterType<IAxisViewModel, Axis1ViewModel>(nameof(Axis1ViewModel));
            this.container.RegisterType<IAxisViewModel, Axis2ViewModel>(nameof(Axis2ViewModel));
            this.container.RegisterType<IAxisViewModel, Axis3ViewModel>(nameof(Axis3ViewModel));
            this.container.RegisterType<IAxisViewModel, Axis4ViewModel>(nameof(Axis4ViewModel));
            this.container.RegisterType<INavigable, SettingsPageViewModel>(nameof(SettingsPage));

            // Show the MainPage.
            this.NavigationService.Navigate(typeof(MainPage));
            return Task.CompletedTask;
        }

        public async override Task OnSuspendingAsync(object s, SuspendingEventArgs e, bool prelaunchActivated)
        {
            await this.container.Resolve<PLC1Session>().SuspendAsync();
        }

        public override void OnResuming(object s, object e, AppExecutionState previousExecutionState)
        {
            this.container.Resolve<PLC1Session>().Resume();
        }

        public override INavigable ResolveForPage(Page page, NavigationService navigationService)
        {
            // Search container for the view model registered with the name of the given view.
            return this.container.Resolve<INavigable>(page.GetType().Name);
        }

        private Task<IUserIdentity> ProvideUserIdentity(EndpointDescription endpoint)
        {
            if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.Anonymous))
            {
                return Task.FromResult<IUserIdentity>(new AnonymousIdentity());
            }

            if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.UserName))
            {
                var tcs = new TaskCompletionSource<IUserIdentity>();

                this.NavigationService.Dispatcher.DispatchIdleAsync(async () =>
                {
                    var d = new UserIdentityDialog(endpoint);
                    var result = await d.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        tcs.TrySetResult(d.UserIdentity);
                    }
                    tcs.TrySetResult(new AnonymousIdentity());
                });
                return tcs.Task;
            }

            throw new NotImplementedException("ProvideUserIdentity supports only UserName and Anonymous identity, for now.");
        }
    }
}