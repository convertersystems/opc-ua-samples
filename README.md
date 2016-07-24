![Workstation][1] ![NodeServer][2]

# Workstation.Samples
*New!* Supports Universal Windows Platform (UWP) and Windows Presentation Framework (WPF) applications.

Build a free HMI using OPC Unified Architecture and Visual Studio. With this library, your app can browse, read, write and subscribe to the live data published by the OPC UA servers on your network.

Get the companion Visual Studio extension 'Workstation.UaBrowser' and you can:
- Browse OPC UA servers directly from the Visual Studio IDE.
- Drag and drop the variables, methods and events onto your view model.
- Use XAML bindings to connect your UI elements to live data.


### Main Projects
- ConsoleApp - An example console application. Demonstates use of SessionChannel for creating subscriptions and logging of data changes or events.
- RobotHMI - An example HMI for .NETFramework 4.6.1. Demonstrates features of Workstation.UaClient, such as reading /writing variables, logging events, calling methods. 
- RobotApp - An example HMI for Universal Window Platform (UWP). Demonstrates features of Workstation.UaClient, such as reading /writing variables, logging events, calling methods. 
- NodeServer - An example OPC UA Server for Node.js. Demonstates features of Node-OpcUa package. Requires installation of:

1. Node.js version 6.2.1 or higher - https://nodejs.org/en/
2. Node.js Tools for Visual Studio version 1.1 or higher - http://aka.ms/explorentvs 

### Releases
v1.2.0 Added ConsoleApp.
v1.1.0 Expanded AxisDetail page.
v1.0.0 First commit.

[1]: WorkstationRuntime.png
[2]: nodeserver.png
  