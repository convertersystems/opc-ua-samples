// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
            var discoveryUrl = "opc.tcp://localhost:48010"; // UaCppServer - see  http://www.unified-automation.com/
            // var discoveryUrl = $"opc.tcp://localhost:26543"; // Workstation.RobotServer
            var cycleTime = 5000;

            // Describe this app.
            var appDescription = new ApplicationDescription()
            {
                ApplicationName = "DataLoggingConsole",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:DataLoggingConsole",
                ApplicationType = ApplicationType.Client,
            };

            // Create a certificate store on disk.
            var certificateStore = new DirectoryStore(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DataLoggingConsole", "pki"));

            // Create array of NodeIds to log.
            var nodeIds = new[]
            {
                NodeId.Parse("i=2258") // CurrentTime
            };

            // Create a session with the server.
            var session = new UaTcpSessionChannel(
                appDescription,
                certificateStore,
                new AnonymousIdentity(),
                discoveryUrl);
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
                        Console.WriteLine($"{nodeIds[i]}; value: {readResponse.Results[i]}");
                    }

                    await Task.Delay(cycleTime, token);
                }
                await session.CloseAsync();
            }
            catch (TaskCanceledException)
            {
                // Ctrl-C was pressed.
                await session.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await session.AbortAsync();
            }
        }
    }
}