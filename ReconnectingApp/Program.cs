// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

namespace ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var cts = new CancellationTokenSource();
                var task1 = ConnectAndPublish(cts.Token);

                Console.WriteLine("Press any key to close the program...");
                Console.ReadKey(true);

                // exiting program, cancel the task.
                cts.Cancel();
                // wait for our task to complete.
                task1.GetAwaiter().GetResult(); 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to close the program...");
                Console.ReadKey(true);
            }
        }


        private static async Task ConnectAndPublish(CancellationToken token = default)
        {
            var discoveryUrl = "opc.tcp://localhost:48010"; // UaCppServer - see  http://www.unified-automation.com/

            var appDescription = new ApplicationDescription()
            {
                ApplicationName = "MyHomework",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:MyHomework",
                ApplicationType = ApplicationType.Client,
            };

            var certificateStore = new DirectoryStore(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Workstation.ConsoleApp", "pki"));

            while (!token.IsCancellationRequested)
            {
                var channel = new UaTcpSessionChannel(
                    appDescription,
                    certificateStore,
                    new AnonymousIdentity(),
                    discoveryUrl);
                try
                {
                    await channel.OpenAsync();

                    var subscriptionRequest = new CreateSubscriptionRequest
                    {
                        RequestedPublishingInterval = 1000,
                        RequestedMaxKeepAliveCount = 10,
                        RequestedLifetimeCount = 30,
                        PublishingEnabled = true
                    };
                    var subscriptionResponse = await channel.CreateSubscriptionAsync(subscriptionRequest);
                    var id = subscriptionResponse.SubscriptionId;

                    var itemsToCreate = new MonitoredItemCreateRequest[]
                   {
                        new MonitoredItemCreateRequest { ItemToMonitor = new ReadValueId { NodeId = NodeId.Parse("i=2258"), AttributeId = AttributeIds.Value }, MonitoringMode = MonitoringMode.Reporting, RequestedParameters = new MonitoringParameters { ClientHandle = 12345, SamplingInterval = -1, QueueSize = 0, DiscardOldest = true } }
                   };
                    var itemsRequest = new CreateMonitoredItemsRequest
                    {
                        SubscriptionId = id,
                        ItemsToCreate = itemsToCreate,
                    };
                    var itemsResponse = await channel.CreateMonitoredItemsAsync(itemsRequest);

                    var subtoken = channel.Where(pr => pr.SubscriptionId == id).Subscribe(
                        pr =>
                        {
                            var dcns = pr.NotificationMessage.NotificationData.OfType<DataChangeNotification>();
                            foreach (var dcn in dcns)
                            {
                                foreach (var min in dcn.MonitoredItems)
                                {
                                    Console.WriteLine($"sub: {pr.SubscriptionId}; handle: {min.ClientHandle}; value: {min.Value}");
                                }
                            }
                        },
                        // need to handle error when server closes
                        ex => { });

                    try
                    {
                        Task.WaitAny(new Task[] { channel.Completion }, token);
                    }
                    catch (OperationCanceledException) { }

                    var request = new DeleteSubscriptionsRequest
                    {
                        SubscriptionIds = new uint[] { id }
                    };
                    await channel.DeleteSubscriptionsAsync(request);
                    subtoken.Dispose();

                    await channel.CloseAsync();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error connecting and publishing. {ex.Message}");
                    await channel.AbortAsync();
                    try
                    {
                        await Task.Delay(5000, token);
                    }
                    catch (TaskCanceledException) { }
                }
            }
        }

    }
}
