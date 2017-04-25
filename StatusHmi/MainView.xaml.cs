// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MahApps.Metro.Controls;
using Workstation.ServiceModel.Ua;

namespace StatusHmi
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            this.InitializeComponent();
            this.StateChanged += this.MainView_StateChanged;
        }

        private void MainView_StateChanged(object sender, System.EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Minimized)
            {
                UaApplication.Current?.SuspendAsync().Wait();
                return;
            }

            UaApplication.Current?.Run();
        }
    }
}
