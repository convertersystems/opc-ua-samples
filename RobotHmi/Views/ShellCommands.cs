// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace RobotHmi.Views
{
    public static class ShellCommands
    {
        public static readonly RoutedUICommand About = new RoutedUICommand("About", "About", typeof(ShellCommands));
        public static readonly RoutedUICommand Settings = new RoutedUICommand("Settings", "Settings", typeof(ShellCommands));
        public static readonly RoutedUICommand SignIn = new RoutedUICommand("SignIn", "SignIn", typeof(ShellCommands));
    }
}