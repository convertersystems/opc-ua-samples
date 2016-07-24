using System;
using System.Linq;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(TestAsync).GetAwaiter().GetResult();
        }

        private static async Task TestAsync()
        {
            Console.WriteLine("Step 1 - Describe this app.");
            var appDescription = new ApplicationDescription()
            {
                ApplicationName = "MyHomework",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:MyHomework",
                ApplicationType = ApplicationType.Client,
            };

            var discoveryUrl = "opc.tcp://localhost:26543";
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

            Console.WriteLine("Step 3 - Choose endpoint with highest security level.");
            var remoteEndpoint = getEndpointsResponse.Endpoints.OrderBy(e => e.SecurityLevel).Last();
            Console.WriteLine(remoteEndpoint.SecurityPolicyUri);

            Console.WriteLine("Step 4 - Create a session with your server.");
            using (var session = new UaTcpSessionChannel(appDescription, appDescription.GetCertificate(), null, remoteEndpoint))
            {
                await session.OpenAsync();

                Console.WriteLine("Step 5 - Browse the server namespace.");
                BrowseRequest browseRequest = new BrowseRequest
                {
                    NodesToBrowse = new BrowseDescription[] { new BrowseDescription { NodeId = NodeId.Parse(ObjectIds.ObjectsFolder), BrowseDirection = BrowseDirection.Forward, ReferenceTypeId = NodeId.Parse(ReferenceTypeIds.HierarchicalReferences), NodeClassMask = (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, IncludeSubtypes = true, ResultMask = (uint)BrowseResultMask.All } },
                };
                BrowseResponse browseResponse = await session.BrowseAsync(browseRequest);
                Console.WriteLine("DisplayName: BrowseName, NodeClass");
                foreach (var rd in browseResponse.Results[0].References)
                {
                    Console.WriteLine("{0}: {1}, {2}", rd.DisplayName, rd.BrowseName, rd.NodeClass);
                    browseRequest = new BrowseRequest
                    {
                        NodesToBrowse = new BrowseDescription[] { new BrowseDescription { NodeId = ExpandedNodeId.ToNodeId(rd.NodeId, session.NamespaceUris), BrowseDirection = BrowseDirection.Forward, ReferenceTypeId = NodeId.Parse(ReferenceTypeIds.HierarchicalReferences), NodeClassMask = (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, IncludeSubtypes = true, ResultMask = (uint)BrowseResultMask.All } },
                    };
                    browseResponse = await session.BrowseAsync(browseRequest);
                    foreach (var nextRd in browseResponse.Results[0].References)
                    {
                        Console.WriteLine("+ {0}: {1}, {2}", nextRd.DisplayName, nextRd.BrowseName, nextRd.NodeClass);
                    }
                }

                Console.WriteLine("Press any key to create a subscription...");
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

                Console.WriteLine("Step 7 - Add a list of items you wish to monitor to the subscription.");
                var items = browseResponse.Results[0].References.Where(r => r.NodeClass == NodeClass.Variable).ToArray();
                var itemsToCreate = items.Select((r, i) => new MonitoredItemCreateRequest { ItemToMonitor = new ReadValueId { NodeId = ExpandedNodeId.ToNodeId(r.NodeId, session.NamespaceUris), AttributeId = AttributeIds.Value }, MonitoringMode = MonitoringMode.Reporting, RequestedParameters = new MonitoringParameters { ClientHandle = (uint)i, SamplingInterval = -1, QueueSize = 0, DiscardOldest = true } } ).ToArray();
                var itemsRequest = new CreateMonitoredItemsRequest
                {
                    SubscriptionId = id,
                    ItemsToCreate = itemsToCreate,
                };
                var itemsResponse = await session.CreateMonitoredItemsAsync(itemsRequest).ConfigureAwait(false);
                for (int i = 0; i < itemsResponse.Results.Length; i++)
                {
                    var item = itemsToCreate[i];
                    var result = itemsResponse.Results[i];
                    if (StatusCode.IsBad(result.StatusCode))
                    {
                        Console.WriteLine($"Error response from MonitoredItemCreateRequest for {item.ItemToMonitor.NodeId}. {result.StatusCode}");
                    }
                }

                Console.WriteLine("Step 8 - Publish the subscription.");
                var publishRequest = new PublishRequest
                {
                    SubscriptionAcknowledgements = new SubscriptionAcknowledgement[0]
                };
                Console.WriteLine("Press any key to delete the subscription...");
                while (!Console.KeyAvailable)
                {
                    var publishResponse = await session.PublishAsync(publishRequest).ConfigureAwait(false);

                    // loop thru all the notifications
                    var ndarray = publishResponse.NotificationMessage.NotificationData;
                    foreach (var nd in ndarray)
                    {
                        // if data change.
                        var dcn = nd as DataChangeNotification;
                        if (dcn != null)
                        {
                            foreach (var min in dcn.MonitoredItems)
                            {
                                Console.WriteLine($"name: {items[min.ClientHandle].DisplayName}; value: {min.Value}");
                            }

                            continue;
                        }
                    }
                    publishRequest = new PublishRequest
                    {
                        SubscriptionAcknowledgements = new[] { new SubscriptionAcknowledgement { SequenceNumber = publishResponse.NotificationMessage.SequenceNumber, SubscriptionId = publishResponse.SubscriptionId } }
                    };
                }
                Console.ReadKey(true);
                Console.WriteLine("Deleting subscription.");
                var request = new DeleteSubscriptionsRequest
                {
                    SubscriptionIds = new uint[] { id }
                };
                await session.DeleteSubscriptionsAsync(request).ConfigureAwait(false);


                Console.WriteLine("Press any key to close the session...");
                Console.ReadKey(true);
                Console.WriteLine("Closing session.");
                await session.CloseAsync();
            }
        }
    }
}

