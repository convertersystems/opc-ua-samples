// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

namespace DataLoggingConsole
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
            var discoveryUrl = $"opc.tcp://192.168.1.107:49320";
            var cycleTime = 5000;

            // setup logger
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddDebug(LogLevel.Trace);

            // Describe this app.
            var appDescription = new ApplicationDescription()
            {
                ApplicationName = "DataLoggingConsole",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:DataLoggingConsole",
                ApplicationType = ApplicationType.Client,
            };

            // Create a certificate store on disk.
            var certificateStore = new DirectoryStore(
                Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\DataLoggingConsole\pki"));


            // Prepare array of nodes to read.
            var readRequest = new ReadRequest
            {
                NodesToRead = new[]
                {
                    new ReadValueId { NodeId = NodeId.Parse("i=2258"), AttributeId = AttributeIds.Value },
                }
            };

            Console.WriteLine("Press any key to end the program...");

            while (!Console.KeyAvailable)
            {
                try
                {
                    await Task.Delay(cycleTime);

                    // Discover endpoints.
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

                    // Choose the endpoint with highest security level.
                    var remoteEndpoint = getEndpointsResponse.Endpoints.OrderBy(e => e.SecurityLevel).Last();

                    // Choose a User Identity.
                    IUserIdentity userIdentity = null;
                    if (remoteEndpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.Anonymous))
                    {
                        userIdentity = new AnonymousIdentity();
                    }
                    else if (remoteEndpoint.UserIdentityTokens.Any(p => p.TokenType == UserTokenType.UserName))
                    {
                        // If a username / password is requested, provide from .config file.
                        userIdentity = new UserNameIdentity("root", "secret");
                    }
                    else
                    {
                        Console.WriteLine("Program supports servers requesting Anonymous and UserName identity.");
                    }

                    // Create a session with the server.
                    var session = new UaTcpSessionChannel(appDescription, certificateStore, userIdentity, remoteEndpoint, loggerFactory);
                    try
                    {
                        await session.OpenAsync();

                        while (!Console.KeyAvailable)
                        {
                            // Read the nodes.
                            var readResponse = await session.ReadAsync(readRequest).ConfigureAwait(false);

                            // Write the results.
                            for (int i = 0; i < readRequest.NodesToRead.Length; i++)
                            {
                                Console.WriteLine($"{readRequest.NodesToRead[i].NodeId}; value: {readResponse.Results[i]}");
                            }

                            await Task.Delay(cycleTime);
                        }
                        await session.CloseAsync();
                    }
                    catch
                    {
                        await session.AbortAsync();
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
