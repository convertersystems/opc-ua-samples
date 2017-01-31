// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
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
                var cts = new CancellationTokenSource();
                Console.WriteLine("Press Ctrl-C to close the program...");
                Console.CancelKeyPress += (s, e) => { e.Cancel = true; cts.Cancel(); };

                Task.Run(() => TestAsync(cts.Token)).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to close the program...");
                Console.ReadKey(true);
            }
        }

        private static async Task TestAsync(CancellationToken token = default(CancellationToken))
        {
            var discoveryUrl = $"opc.tcp://localhost:48010";
            var cycleTime = 5000;

            // setup logger
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(LogLevel.Information);
            var logger = loggerFactory?.CreateLogger<Program>();

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
            
            // Create array of NodeIds to log.
            var nodeIds = new[]
            {
                NodeId.Parse("i=2258")
            };

            while (!token.IsCancellationRequested)
            {
                try
                {
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
                        throw new InvalidOperationException("Server must accept Anonymous or UserName identity.");
                    }

                    // Create a session with the server.
                    var session = new UaTcpSessionChannel(appDescription, certificateStore, userIdentity, remoteEndpoint, loggerFactory);
                    try
                    {
                        await session.OpenAsync();

                        RegisterNodesResponse registerNodesResponse = null;

                        if (true) // True registers the nodeIds to improve performance of the server.
                        {
                            // Register array of nodes to read.
                            var registerNodesRequest = new RegisterNodesRequest
                            {
                                NodesToRegister = nodeIds
                            };
                            registerNodesResponse = await session.RegisterNodesAsync(registerNodesRequest);
                        }

                        // Prepare read request.
                        var readRequest = new ReadRequest
                        {
                            NodesToRead = (registerNodesResponse?.RegisteredNodeIds ?? nodeIds)
                            .Select(n => new ReadValueId { NodeId = n, AttributeId = AttributeIds.Value })
                            .ToArray()
                        };

                        while (!token.IsCancellationRequested)
                        {
                            // Read the nodes.
                            var readResponse = await session.ReadAsync(readRequest).ConfigureAwait(false);

                            // Write the results.
                            for (int i = 0; i < readRequest.NodesToRead.Length; i++)
                            {
                                logger?.LogInformation($"{nodeIds[i]}; value: {readResponse.Results[i]}");
                            }

                            try
                            {
                                await Task.Delay(cycleTime, token);
                            }
                            catch { }
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
                    logger?.LogError(ex.Message);
                }

                try
                {
                    await Task.Delay(cycleTime, token);
                }
                catch { }
            }
        }
    }
}
