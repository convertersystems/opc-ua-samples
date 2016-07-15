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
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigableSubscriptionBase"/> class.
        /// </summary>
        /// <param name="session">The session service.</param>
        /// <param name="publishingInterval">The publishing interval in milliseconds.</param>
        /// <param name="keepAliveCount">The number of PublishingIntervals before the server should return an empty Publish response.</param>
        public NavigableSubscriptionBase(UaTcpSessionClient session, double publishingInterval = 1000f, uint keepAliveCount = 10)
            : base(session, publishingInterval: publishingInterval, keepAliveCount: keepAliveCount)
        {
        }

        public virtual Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
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
