// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;

namespace RobotHmi.ViewModels
{
    /// <summary>
    /// A base class that implements Subscription
    /// </summary>
    public class SubscriptionBase : Subscription
    {
        public SubscriptionBase(UaTcpSessionService service)
            : base(service)
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