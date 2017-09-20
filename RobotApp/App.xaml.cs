// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using RobotApp.Services.SettingsServices;
using RobotApp.ViewModels;
using RobotApp.Views;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
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
        private ILoggerFactory loggerFactory;
        private UaApplication application;
        private UnityContainer container = new UnityContainer();

        public App()
        {
            this.InitializeComponent();
            this.SplashFactory = (e) => new Splash(e);
            var settings = SettingsService.Instance;
            this.RequestedTheme = settings.AppTheme;
            this.CacheMaxDuration = settings.CacheMaxDuration;
            this.ShowShellBackButton = settings.UseShellBackButton;
            IServiceLocator locator = new UnityServiceLocator(this.container);
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        /// <summary>
        /// Gets the current instance of the local application.
        /// </summary>
        public static new App Current => (App)Application.Current;

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
            // Setup a logger.
            this.loggerFactory = new LoggerFactory();
            this.loggerFactory.AddDebug(LogLevel.Trace);

            // Build and run an OPC UA application instance.
            this.application = new UaApplicationBuilder()
                .SetApplicationUri($"urn:{Dns.GetHostName()}:Workstation.RobotApp")
                .SetDirectoryStore(Path.Combine(ApplicationData.Current.LocalFolder.Path, "pki"))
                .SetIdentity(this.ShowSignInDialog)
                .SetLoggerFactory(this.loggerFactory)
                .Build();

            this.application.Run();

            // Register view models with the container using the name of the view.
            this.container.RegisterType<INavigable, MainPageViewModel>(nameof(MainPage), new ContainerControlledLifetimeManager());
            this.container.RegisterType<INavigable, AxisPageViewModel>(nameof(AxisPage), new ContainerControlledLifetimeManager());
            this.container.RegisterType<INavigable, SettingsPageViewModel>(nameof(SettingsPage));

            // Show the MainPage.
            this.NavigationService.Navigate(typeof(MainPage));
            return Task.CompletedTask;
        }

        public async override Task OnSuspendingAsync(object s, SuspendingEventArgs e, bool prelaunchActivated)
        {
            await this.application.SuspendAsync();
        }

        public override void OnResuming(object s, object e, AppExecutionState previousExecutionState)
        {
            this.application.Run();
        }

        public override INavigable ResolveForPage(Page page, NavigationService navigationService)
        {
            // Search container for the view model registered with the name of the given view.
            return this.container.Resolve<INavigable>(page.GetType().Name);
        }

        /// <summary>
        /// Show a Sign In dialog if the remote endpoint demands a UserNameIdentity token.
        /// </summary>
        /// <param name="endpoint">The remote endpoint.</param>
        /// <returns>A UserIdentity</returns>
        public async Task<IUserIdentity> ShowSignInDialog(EndpointDescription endpoint)
        {
            if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.Anonymous))
            {
                return new AnonymousIdentity();
            }

            if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.UserName))
            {
                var tcs = new TaskCompletionSource<IUserIdentity>();

                await this.NavigationService.Dispatcher.DispatchIdleAsync(async () =>
                {
                    var d = new UserIdentityDialog(endpoint);
                    var result = await d.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        tcs.TrySetResult(d.UserIdentity);
                    }
                    tcs.TrySetResult(new AnonymousIdentity());
                });

                return await tcs.Task;
            }

            throw new NotImplementedException("SignInDialog supports only UserName and Anonymous identity, for now.");
        }
    }
}