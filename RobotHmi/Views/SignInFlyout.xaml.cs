// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Input;

namespace RobotHmi.Views
{
    /// <summary>
    /// Interaction logic for SignInFlyout.xaml
    /// </summary>
    public partial class SignInFlyout : Flyout
    {
        public SignInFlyout()
        {
            this.InitializeComponent();
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
            e.Handled = true;
        }

        private void OnBrowseBackExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.IsOpen = false;
            e.Handled = true;
        }
    }
}