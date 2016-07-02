// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using Workstation.ServiceModel.Ua;

namespace RobotApp.ViewModels
{
    /// <summary>
    /// A base class that implements Subscription and Template10's INavigable
    /// </summary>
    public class NavigableSubscriptionBase : Subscription, INavigable
    {
        public NavigableSubscriptionBase(UaTcpSessionService service)
            : base(service)
        {
        }

        public bool KeepAlive { get; set; }

        public virtual Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            if (!this.KeepAlive)
            {
                this.Session.Subscriptions.Remove(this);
            }

            return Task.CompletedTask;
        }

        public virtual Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {

            if (!this.Session.Subscriptions.Contains(this))
            {
                this.Session.Subscriptions.Add(this);
            }

            return Task.CompletedTask;
        }

        [JsonIgnore]
        public virtual INavigationService NavigationService { get; set; }

        [JsonIgnore]
        public virtual IDispatcherWrapper Dispatcher { get; set; }

        [JsonIgnore]
        public virtual IStateItems SessionState { get; set; }
    }
}
