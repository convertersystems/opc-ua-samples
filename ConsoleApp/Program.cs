// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
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
            var discoveryUrl = "opc.tcp://localhost:26543"; // Workstation.NodeServer
            //var discoveryUrl = "opc.tcp://localhost:48010"; // UaCppServer - see  http://www.unified-automation.com/

            Console.WriteLine("Step 1 - Describe this app.");
            var appDescription = new ApplicationDescription()
            {
                ApplicationName = "MyHomework",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:MyHomework",
                ApplicationType = ApplicationType.Client,
            };

            Console.WriteLine($"Step 2 - Discover endpoints of '{discoveryUrl}'.");
            var getEndpointsRequest = new GetEndpointsRequest
            {
                EndpointUrl = discoveryUrl,
                ProfileUris = new[] { TransportProfileUris.UaTcpTransport }
            };
            var getEndpointsResponse = await UaTcpDiscoveryClient.GetEndpointsAsync(getEndpointsRequest).ConfigureAwait(false);
            if (getEndpointsResponse.Endpoints == null || getEndpointsResponse.Endpoints.Length == 0)
            {
                throw new InvalidOperationException($"'{discoveryUrl}' returned no endpoints.");
            }

            Console.WriteLine("Step 3 - Choose the endpoint with highest security level.");
            var remoteEndpoint = getEndpointsResponse.Endpoints.OrderBy(e => e.SecurityLevel).Last();
            Console.WriteLine(remoteEndpoint.SecurityPolicyUri);

            IUserIdentity userIdentity = null;
            if (remoteEndpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.Anonymous))
            {
                userIdentity = new AnonymousIdentity();
            }
            else if (remoteEndpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.UserName))
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

            Console.WriteLine("Step 4 - Create a session with your server.");
            var session = new UaTcpSessionChannel(
                appDescription,
                new DirectoryStore(Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Workstation.ConsoleApp\pki")),
                userIdentity,
                remoteEndpoint);
            try
            {
                await session.OpenAsync();

                Console.WriteLine("Step 5 - Browse the server namespace.");
                Console.WriteLine("+ Root");
                BrowseRequest browseRequest = new BrowseRequest
                {
                    NodesToBrowse = new BrowseDescription[] { new BrowseDescription { NodeId = NodeId.Parse(ObjectIds.RootFolder), BrowseDirection = BrowseDirection.Forward, ReferenceTypeId = NodeId.Parse(ReferenceTypeIds.HierarchicalReferences), NodeClassMask = (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, IncludeSubtypes = true, ResultMask = (uint)BrowseResultMask.All } },
                };
                BrowseResponse browseResponse = await session.BrowseAsync(browseRequest);
                foreach (var rd1 in browseResponse.Results[0].References ?? new ReferenceDescription[0])
                {
                    Console.WriteLine("  + {0}: {1}, {2}", rd1.DisplayName, rd1.BrowseName, rd1.NodeClass);
                    browseRequest = new BrowseRequest
                    {
                        NodesToBrowse = new BrowseDescription[] { new BrowseDescription { NodeId = ExpandedNodeId.ToNodeId(rd1.NodeId, session.NamespaceUris), BrowseDirection = BrowseDirection.Forward, ReferenceTypeId = NodeId.Parse(ReferenceTypeIds.HierarchicalReferences), NodeClassMask = (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, IncludeSubtypes = true, ResultMask = (uint)BrowseResultMask.All } },
                    };
                    browseResponse = await session.BrowseAsync(browseRequest);
                    foreach (var rd2 in browseResponse.Results[0].References ?? new ReferenceDescription[0])
                    {
                        Console.WriteLine("    + {0}: {1}, {2}", rd2.DisplayName, rd2.BrowseName, rd2.NodeClass);
                        browseRequest = new BrowseRequest
                        {
                            NodesToBrowse = new BrowseDescription[] { new BrowseDescription { NodeId = ExpandedNodeId.ToNodeId(rd2.NodeId, session.NamespaceUris), BrowseDirection = BrowseDirection.Forward, ReferenceTypeId = NodeId.Parse(ReferenceTypeIds.HierarchicalReferences), NodeClassMask = (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, IncludeSubtypes = true, ResultMask = (uint)BrowseResultMask.All } },
                        };
                        browseResponse = await session.BrowseAsync(browseRequest);
                        foreach (var rd3 in browseResponse.Results[0].References ?? new ReferenceDescription[0])
                        {
                            Console.WriteLine("      + {0}: {1}, {2}", rd3.DisplayName, rd3.BrowseName, rd3.NodeClass);
                        }
                    }
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);

                Console.WriteLine("Step 6 - Create a subscription.");
                var subscriptionRequest = new CreateSubscriptionRequest
                {
                    RequestedPublishingInterval = 1000,
                    RequestedMaxKeepAliveCount = 10,
                    RequestedLifetimeCount = 30,
                    PublishingEnabled = true
                };
                var subscriptionResponse = await session.CreateSubscriptionAsync(subscriptionRequest).ConfigureAwait(false);
                var id = subscriptionResponse.SubscriptionId;

                Console.WriteLine("Step 7 - Add items to the subscription.");
                var itemsToCreate = new MonitoredItemCreateRequest[]
                {
                    new MonitoredItemCreateRequest { ItemToMonitor = new ReadValueId { NodeId = NodeId.Parse("i=2258"), AttributeId = AttributeIds.Value }, MonitoringMode = MonitoringMode.Reporting, RequestedParameters = new MonitoringParameters { ClientHandle = 12345, SamplingInterval = -1, QueueSize = 0, DiscardOldest = true } }
                };
                var itemsRequest = new CreateMonitoredItemsRequest
                {
                    SubscriptionId = id,
                    ItemsToCreate = itemsToCreate,
                };
                var itemsResponse = await session.CreateMonitoredItemsAsync(itemsRequest).ConfigureAwait(false);

                Console.WriteLine("Step 8 - Publish the subscription.");
                var publishRequest = new PublishRequest
                {
                    SubscriptionAcknowledgements = new SubscriptionAcknowledgement[0]
                };
                Console.WriteLine("Press any key to delete the subscription...");
                while (!Console.KeyAvailable)
                {
                    var publishResponse = await session.PublishAsync(publishRequest).ConfigureAwait(false);

                    // loop thru all the data change notifications
                    var dcns = publishResponse.NotificationMessage.NotificationData.OfType<DataChangeNotification>();
                    foreach (var dcn in dcns)
                    {
                        foreach (var min in dcn.MonitoredItems)
                        {
                            Console.WriteLine($"subscription: {publishResponse.SubscriptionId}; clientHandle: {min.ClientHandle}; value: {min.Value}");
                        }
                    }

                    publishRequest = new PublishRequest
                    {
                        SubscriptionAcknowledgements = new[] { new SubscriptionAcknowledgement { SequenceNumber = publishResponse.NotificationMessage.SequenceNumber, SubscriptionId = publishResponse.SubscriptionId } }
                    };
                }

                Console.ReadKey(true);

                Console.WriteLine("Step 9 - Delete the subscription.");
                var request = new DeleteSubscriptionsRequest
                {
                    SubscriptionIds = new uint[] { id }
                };
                await session.DeleteSubscriptionsAsync(request).ConfigureAwait(false);

                Console.WriteLine("Press any key to close the session...");
                Console.ReadKey(true);
                Console.WriteLine("Step 10 - Close the session.");
                await session.CloseAsync();
            }
            catch (ServiceResultException ex)
            {
                if ((uint)ex.HResult == StatusCodes.BadSecurityChecksFailed)
                {
                    Console.WriteLine("Error connecting to endpoint. Did the server reject our certificate?");
                }

                await session.AbortAsync();
                throw;
            }
        }
    }
}
