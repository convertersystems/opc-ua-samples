using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RobotApp.Helpers;
using RobotApp.Services;
using RobotApp.Views;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Workstation.ServiceModel.Ua;

namespace RobotApp
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;
        private UaApplication uaApplication;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();
            EnteredBackground += App_EnteredBackground;
            Resuming += App_Resuming;

            // Setup a logger.
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddDebug());
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            // Build and run an OPC UA application instance.
            this.uaApplication = new UaApplicationBuilder()
                .SetApplicationUri($"urn:{System.Net.Dns.GetHostName()}:Workstation.RobotApp")
                .SetDirectoryStore(Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "pki"))
                .SetIdentity(this.ShowSignInDialog)
                .SetLoggerFactory(loggerFactory)
                .Build();

            this.uaApplication.Run();
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.MainPage), new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }

        private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            await this.uaApplication.SuspendAsync();
            deferral.Complete();
        }

        private void App_Resuming(object sender, object e)
        {
            this.uaApplication.Run();
        }

        /// <summary>
        /// Show a Sign In dialog if the remote endpoint demands a UserNameIdentity token.
        /// </summary>
        /// <param name="endpoint">The remote endpoint.</param>
        /// <returns>A UserIdentity</returns>
        private async Task<IUserIdentity> ShowSignInDialog(EndpointDescription endpoint)
        {
            if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.Anonymous))
            {
                return new AnonymousIdentity();
            }

            if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.UserName))
            {
                var tcs = new TaskCompletionSource<IUserIdentity>();

                var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
                if (dispatcher != null)
                {
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                    {
                        var d = new UserIdentityDialog(endpoint);
                        var result = await d.ShowAsync();
                        if (result == ContentDialogResult.Primary)
                        {
                            tcs.TrySetResult(d.UserIdentity);
                        }
                        tcs.TrySetResult(new AnonymousIdentity());
                    });
                }
                return await tcs.Task;
            }

            throw new NotImplementedException("SignInDialog supports only UserName and Anonymous identity, for now.");
        }
    }
}

