// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Windows.UI.Xaml.Controls;
using Workstation.ServiceModel.Ua;

namespace RobotApp.Views
{
    public sealed partial class UserIdentityDialog : ContentDialog
    {
        private string userNameKey = "userName";

        public UserIdentityDialog()
        {
            this.InitializeComponent();
        }

        public UserIdentityDialog(EndpointDescription endpoint)
            : this()
        {
            this.userNameKey = $"userName_{endpoint.EndpointUrl}";
            this.body.Text = $"Connecting to server '{endpoint.Server.ApplicationName}' at '{endpoint.EndpointUrl}' ";
        }

        public IUserIdentity UserIdentity { get; private set; }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.userNameTextBox.Text))
            {
                this.UserIdentity = new UserNameIdentity(this.userNameTextBox.Text, this.passwordTextBox.Password);
            }
            else
            {
                this.UserIdentity = new AnonymousIdentity();
            }

            if (this.saveUserNameCheckBox.IsChecked == true)
            {
                Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
                roamingSettings.Values[this.userNameKey] = this.userNameTextBox.Text;
            }
            else
            {
                Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
                roamingSettings.Values[this.userNameKey] = null;
                this.userNameTextBox.Text = string.Empty;
            }
        }

        private void ContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            // If the user name is saved, get it and populate the user name field.
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (roamingSettings.Values.ContainsKey(this.userNameKey))
            {
                this.userNameTextBox.Text = roamingSettings.Values[this.userNameKey].ToString();
                this.saveUserNameCheckBox.IsChecked = true;
            }
        }
    }
}
