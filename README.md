![Workstation][1] ![RobotApp][2] ![mobilehmi-droid][3]

# opc-ua-samples
Supports Universal Windows Platform (UWP) and Windows Presentation Framework (WPF) applications and now Xamarin Forms.

Build a free HMI using OPC Unified Architecture and Visual Studio. With this library, your app can browse, read, write and subscribe to the live data published by the OPC UA servers on your network.

Get the companion Visual Studio extension 'Workstation.UaBrowser' and you can:
- Browse OPC UA servers directly from the Visual Studio IDE.
- Drag and drop the variables, methods and events onto your view model.
- Use XAML bindings to connect your UI elements to live data.


### Main Projects
- ConsoleApp - A simple example to demonstrates use of SessionChannel for creating subscriptions and logging of data changes or events.
- DataLoggingConsole - A simple example to demonstrates use of SessionChannel for reading a list of nodes and logging the values to the Console.
- StatusHmi - A simple example HMI for .NETFramework 4.6.1. Demonstrates basic subscription feature of Workstation.UaClient. 
- RobotHmi - A more complex example HMI for .NETFramework 4.6.1. Demonstrates features of Workstation.UaClient, such as reading /writing variables, logging events, calling methods. 
- RobotApp - A more complex example HMI for Universal Window Platform (UWP). Demonstrates features of Workstation.UaClient, such as reading /writing variables, logging events, calling methods. 
- MobileHmi.Droid - A more complex example HMI for Xamarin Forms. Demonstrates features of Workstation.UaClient working in the Android Emulator. 
- RobotServer - A example OPC UA Server for RobotHmi. Demonstrates features of OPCFoundation.NetStandard.Opc.Ua.SDK package. 

### Releases

v1.5.7 Replaced NodeServer with RobotServer to demonstrate interopability with the OPCFoundation Reference Server.

v1.5.6 Added 3d robot to Xamarin Forms based MobileHmi.

v1.5.5 Packages upgrade. Added 3d robot to UWP based RobotApp. 

v1.5.2 Packages upgraded. Added DataLoggingConsole app.

v1.5.1 UaClient upgraded to 1.5.1. Switched Xamarin Forms common project from PCL to Shared. 

v1.5.0 Added support for Xamarin Forms. Introduced ICertificateStore and DirectoryStore.

v1.4.2 Upgraded Trendline control for UWP. Adjusted UaTcpSessionClient constructors to use an async certificate provider. 

v1.4.1 Revamped the view models to depend on a shared session client passed in the constructor. Changed some view models to subscribe when navigated to and unsubscribe when navigated away.

v1.4.0 Added "Sign In" dialog to support servers that require a UserNameIdentity. NodeServer has new command line argument "--allowAnonymous" | "-a" 

v1.3.0 Added StatusHmi to demonstrate the basic subscription feature.

v1.2.0 Added ConsoleApp to demonstrate that SessionChannel is all you need for logging of data or events.

v1.1.0 Expanded AxisDetail page.

v1.0.0 First commit.

[1]: WorkstationRuntime.png
[2]: RobotApp.png
[3]: mobilehmi-droid.png  
