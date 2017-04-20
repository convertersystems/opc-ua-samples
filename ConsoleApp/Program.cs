// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
                Task.Run(TestAsync).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to close the program...");
                Console.ReadKey(true);
            }
        }

        private static async Task TestAsync()
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddDebug(LogLevel.Trace);

            var discoveryUrl = "opc.tcp://localhost:26543"; // Workstation.NodeServer
            //var discoveryUrl = "opc.tcp://localhost:48010"; // UaCppServer - see  http://www.unified-automation.com/

            Console.WriteLine("Step 1 - Describe this app.");
            var appDescription = new ApplicationDescription()
            {
                ApplicationName = "MyHomework",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:MyHomework",
                ApplicationType = ApplicationType.Client,
            };

            Console.WriteLine("Step 2 - Create a session with your server.");
            var channel = new UaTcpSessionChannel(
                appDescription,
                new DirectoryStore(@"%LOCALAPPDATA%\Workstation.ConsoleApp\pki"),
                ShowSignInDialog,
                discoveryUrl,
                loggerFactory: loggerFactory);
            try
            {
                await channel.OpenAsync();

                Console.WriteLine($"  Opened channel with endpoint '{channel.RemoteEndpoint.EndpointUrl}'.");
                Console.WriteLine($"  SecurityPolicyUri: '{channel.RemoteEndpoint.SecurityPolicyUri}'.");
                Console.WriteLine($"  SecurityMode: '{channel.RemoteEndpoint.SecurityMode}'.");
                Console.WriteLine($"  UserIdentity: '{channel.UserIdentity}'.");

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);

                Console.WriteLine("Step 3 - Browse the server namespace.");
                Console.WriteLine("+ Root");
                BrowseRequest browseRequest = new BrowseRequest
                {
                    NodesToBrowse = new BrowseDescription[] { new BrowseDescription { NodeId = NodeId.Parse(ObjectIds.RootFolder), BrowseDirection = BrowseDirection.Forward, ReferenceTypeId = NodeId.Parse(ReferenceTypeIds.HierarchicalReferences), NodeClassMask = (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, IncludeSubtypes = true, ResultMask = (uint)BrowseResultMask.All } },
                };
                BrowseResponse browseResponse = await channel.BrowseAsync(browseRequest);
                foreach (var rd1 in browseResponse.Results[0].References ?? new ReferenceDescription[0])
                {
                    Console.WriteLine("  + {0}: {1}, {2}", rd1.DisplayName, rd1.BrowseName, rd1.NodeClass);
                    browseRequest = new BrowseRequest
                    {
                        NodesToBrowse = new BrowseDescription[] { new BrowseDescription { NodeId = ExpandedNodeId.ToNodeId(rd1.NodeId, channel.NamespaceUris), BrowseDirection = BrowseDirection.Forward, ReferenceTypeId = NodeId.Parse(ReferenceTypeIds.HierarchicalReferences), NodeClassMask = (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, IncludeSubtypes = true, ResultMask = (uint)BrowseResultMask.All } },
                    };
                    browseResponse = await channel.BrowseAsync(browseRequest);
                    foreach (var rd2 in browseResponse.Results[0].References ?? new ReferenceDescription[0])
                    {
                        Console.WriteLine("    + {0}: {1}, {2}", rd2.DisplayName, rd2.BrowseName, rd2.NodeClass);
                        browseRequest = new BrowseRequest
                        {
                            NodesToBrowse = new BrowseDescription[] { new BrowseDescription { NodeId = ExpandedNodeId.ToNodeId(rd2.NodeId, channel.NamespaceUris), BrowseDirection = BrowseDirection.Forward, ReferenceTypeId = NodeId.Parse(ReferenceTypeIds.HierarchicalReferences), NodeClassMask = (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, IncludeSubtypes = true, ResultMask = (uint)BrowseResultMask.All } },
                        };
                        browseResponse = await channel.BrowseAsync(browseRequest);
                        foreach (var rd3 in browseResponse.Results[0].References ?? new ReferenceDescription[0])
                        {
                            Console.WriteLine("      + {0}: {1}, {2}", rd3.DisplayName, rd3.BrowseName, rd3.NodeClass);
                        }
                    }
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);

                Console.WriteLine("Step 4 - Create a subscription.");
                var subscriptionRequest = new CreateSubscriptionRequest
                {
                    RequestedPublishingInterval = 1000,
                    RequestedMaxKeepAliveCount = 10,
                    RequestedLifetimeCount = 30,
                    PublishingEnabled = true
                };
                var subscriptionResponse = await channel.CreateSubscriptionAsync(subscriptionRequest);
                var id = subscriptionResponse.SubscriptionId;

                Console.WriteLine("Step 5 - Add items to the subscription.");
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

                Console.WriteLine("Step 6 - Subscribe to PublishResponse stream.");
                var token = channel.Where(pr => pr.SubscriptionId == id).Subscribe(pr =>
                {
                    // loop thru all the data change notifications
                    var dcns = pr.NotificationMessage.NotificationData.OfType<DataChangeNotification>();
                    foreach (var dcn in dcns)
                    {
                        foreach (var min in dcn.MonitoredItems)
                        {
                            Console.WriteLine($"sub: {pr.SubscriptionId}; handle: {min.ClientHandle}; value: {min.Value}");
                        }
                    }
                });

                Console.WriteLine("Press any key to delete the subscription...");
                while (!Console.KeyAvailable)
                {
                    await Task.Delay(500);
                }

                Console.ReadKey(true);

                Console.WriteLine("Step 7 - Delete the subscription.");
                var request = new DeleteSubscriptionsRequest
                {
                    SubscriptionIds = new uint[] { id }
                };
                await channel.DeleteSubscriptionsAsync(request);
                token.Dispose();

                Console.WriteLine("Press any key to close the session...");
                Console.ReadKey(true);

                Console.WriteLine("Step 8 - Close the session.");
                await channel.CloseAsync();
            }
            catch (ServiceResultException ex)
            {
                if ((uint)ex.HResult == StatusCodes.BadSecurityChecksFailed)
                {
                    Console.WriteLine("Error connecting to endpoint. Did the server reject our certificate?");
                }

                await channel.AbortAsync();
                throw;
            }
        }

        /// <summary>
        /// Show a Sign In dialog if the remote endpoint demands a UserNameIdentity token.
        /// </summary>
        /// <param name="endpoint">The remote endpoint.</param>
        /// <returns>A UserIdentity</returns>
        private static async Task<IUserIdentity> ShowSignInDialog(EndpointDescription endpoint)
        {
            IUserIdentity userIdentity = null;
            if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.Anonymous))
            {
                userIdentity = new AnonymousIdentity();
            }
            else if (endpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.UserName))
            {
                Console.WriteLine("Server is requesting UserName identity...");
                Console.Write("Enter user name: ");
                var userName = Console.ReadLine();
                Console.Write("Enter password: ");
                var password = Console.ReadLine();
                userIdentity = new UserNameIdentity(userName, password);
            }
            else
            {
                Console.WriteLine("Program supports servers requesting Anonymous and UserName identity.");
            }

            return userIdentity;
        }
    }
}
