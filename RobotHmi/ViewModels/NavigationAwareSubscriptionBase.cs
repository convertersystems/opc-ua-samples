// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Prism.Regions;
using System;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;

namespace RobotHmi.ViewModels
{
    /// <summary>
    /// A base class that implements Subscription and Prism's INavigationAware
    /// </summary>
    public class NavigationAwareSubscriptionBase : Subscription, INavigationAware, IRegionMemberLifetime
    {
        public NavigationAwareSubscriptionBase(UaTcpSessionService service)
            : base(service)
        {
        }

        public bool KeepAlive { get; set; }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
            if (!this.KeepAlive)
            {
                this.Session.UnsubscribeAsync(this)
                    .ContinueWith(
                        t =>
                        {
                            foreach (var ex in t.Exception.InnerExceptions)
                            {
                                Log.Warn($"Error unsubscribing {this.GetType().Name}. {ex.Message}");
                            }
                        }, TaskContinuationOptions.OnlyOnFaulted);

            }
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.Session.SubscribeAsync(this)
                .ContinueWith(
                    t =>
                    {
                        foreach (var ex in t.Exception.InnerExceptions)
                        {
                            Log.Warn($"Error subscribing {this.GetType().Name}. {ex.Message}");
                        }
                    }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}