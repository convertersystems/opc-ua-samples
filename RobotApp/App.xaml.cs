// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
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
            // Prepare for constructing the shared UaTcpSessionClient.
            var appDescription = new ApplicationDescription()
            {
                ApplicationName = "Workstation.RobotApp",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.RobotApp",
                ApplicationType = ApplicationType.Client
            };
            var appCertificate = appDescription.GetCertificate();
            var userIdentityProvider = new Func<UaTcpSessionClient, Task<IUserIdentity>>(s => this.ProvideUserIdentity(s));
            var discoveryUrl = SettingsService.Instance.EndpointUrl;

            // Register shared services with the application's dependency injection container.
            this.container.RegisterInstance("PLC1", new UaTcpSessionClient(appDescription, appCertificate, userIdentityProvider, discoveryUrl));

            // Register subscriptions with the container using a factory mathod.
            this.container.RegisterType<MainPageViewModel>(new InjectionFactory(c => c.Resolve<UaTcpSessionClient>("PLC1").CreateSubscription<MainPageViewModel>()));
            this.container.RegisterType<IAxisViewModel, Axis1ViewModel>(nameof(Axis1ViewModel), new InjectionFactory(c => c.Resolve<UaTcpSessionClient>("PLC1").CreateSubscription<Axis1ViewModel>()));
            this.container.RegisterType<IAxisViewModel, Axis2ViewModel>(nameof(Axis2ViewModel), new InjectionFactory(c => c.Resolve<UaTcpSessionClient>("PLC1").CreateSubscription<Axis2ViewModel>()));
            this.container.RegisterType<IAxisViewModel, Axis3ViewModel>(nameof(Axis3ViewModel), new InjectionFactory(c => c.Resolve<UaTcpSessionClient>("PLC1").CreateSubscription<Axis3ViewModel>()));
            this.container.RegisterType<IAxisViewModel, Axis4ViewModel>(nameof(Axis4ViewModel), new InjectionFactory(c => c.Resolve<UaTcpSessionClient>("PLC1").CreateSubscription<Axis4ViewModel>()));

            // Register view models with the container using the name of the view.
            this.container.RegisterType<INavigable, MainPageViewModel>(nameof(MainPage), new ContainerControlledLifetimeManager(), new InjectionFactory(c => c.Resolve<MainPageViewModel>()));
            this.container.RegisterType<INavigable, AxisPageViewModel>(nameof(AxisPage), new ContainerControlledLifetimeManager());
            this.container.RegisterType<INavigable, SettingsPageViewModel>(nameof(SettingsPage));

            // Show the MainPage.
            this.NavigationService.Navigate(typeof(MainPage));
            return Task.CompletedTask;
        }

        public async override Task OnSuspendingAsync(object s, SuspendingEventArgs e, bool prelaunchActivated)
        {
            foreach (var session in this.container.ResolveAll<UaTcpSessionClient>())
            {
                await session.SuspendAsync();
            }
        }

        public override void OnResuming(object s, object e, AppExecutionState previousExecutionState)
        {
            foreach (var session in this.container.ResolveAll<UaTcpSessionClient>())
            {
                session.Resume();
            }
        }

        public override INavigable ResolveForPage(Page page, NavigationService navigationService)
        {
            // Search container for the view model registered with the name of the given view.
            return this.container.Resolve<INavigable>(page.GetType().Name);
        }

        private Task<IUserIdentity> ProvideUserIdentity(UaTcpSessionClient session)
        {
            if (session.RemoteEndpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.Anonymous))
            {
                return Task.FromResult<IUserIdentity>(new AnonymousIdentity());
            }

            if (session.RemoteEndpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.UserName))
            {
                var tcs = new TaskCompletionSource<IUserIdentity>();

                this.NavigationService.Dispatcher.DispatchIdleAsync(async () =>
                {
                    var d = new UserIdentityDialog(session);
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