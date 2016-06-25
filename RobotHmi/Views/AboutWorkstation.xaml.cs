// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace RobotHmi.Views
{
    /// <summary>
    /// Interaction logic for AboutWorkstation.xaml
    /// </summary>
    public partial class AboutWorkstation
    {
        public AboutWorkstation()
        {
            this.InitializeComponent();
        }

        public AboutWorkstation(MetroWindow parentWindow, MetroDialogSettings settings)
            : base(parentWindow, settings)
        {
            this.InitializeComponent();
        }

        private async void OnClick(object sender, RoutedEventArgs e)
        {
            var w = this.OwningWindow;
            if (w != null)
            {
                await w.HideMetroDialogAsync(this);
            }
        }
    }
}