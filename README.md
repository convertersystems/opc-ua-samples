![Workstation][1] ![RobotApp][2]

# opc-ua-samples
Supports Universal Windows Platform (UWP) and Windows Presentation Framework (WPF) applications and Xamarin.

Build a free HMI using OPC Unified Architecture and Visual Studio. With this library, your app can browse, read, write and subscribe to the live data published by the OPC UA servers on your network.

Get the companion Visual Studio extension 'Workstation.UaBrowser' and you can:
- Browse OPC UA servers directly from the Visual Studio IDE.
- Drag and drop the variables, methods and events onto your view model.
- Use XAML bindings to connect your UI elements to live data.


### Main Projects
- ConsoleApp - A simple example to demonstrates use of SessionChannel for creating subscriptions and logging of data changes or events.
- StatusHmi - A simple example HMI for .NETFramework 4.7.2. Demonstrates basic subscription feature of Workstation.UaClient. 
- RobotHmi - A more complex example HMI for .NETFramework 4.7.2. Demonstrates features of Workstation.UaClient, such as reading /writing variables, logging events, calling methods. 
- RobotApp - A more complex example HMI for Universal Window Platform (UWP). Demonstrates features of Workstation.UaClient, such as reading /writing variables, logging events, calling methods. 
- MobileHmi.Droid - A more complex example HMI for Xamarin Forms. Demonstrates features of Workstation.UaClient working in the Android Emulator. 
- RobotServer - A example OPC UA Server for RobotHmi. Demonstrates features of OPCFoundation.NetStandard.Opc.Ua.SDK package. 


[1]: WorkstationRuntime.png
[2]: RobotApp.png
