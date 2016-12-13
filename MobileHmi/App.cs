// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Workstation.ServiceModel.Ua;
using Xamarin.Forms;

namespace Workstation.MobileHmi
{
    public class App : Application
    {
        private UaTcpSessionClient session;

        public App(UaTcpSessionClient session)
        {
            this.session = session;
            var viewModel = new MainViewModel(this.session);
            var view = new MainPage { BindingContext = viewModel };

            this.MainPage = new NavigationPage(view);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            this.session?.SuspendAsync().Wait();
        }

        protected override void OnResume()
        {
            this.session?.Resume();
        }
    }
}
