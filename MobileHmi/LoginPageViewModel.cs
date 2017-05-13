// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;
using Xamarin.Forms;

namespace Workstation.MobileHmi
{
    /// <summary>
    /// A model for LoginPage.
    /// </summary>
    public class LoginPageViewModel : ViewModelBase
    {
        TaskCompletionSource<IUserIdentity> tcs;

        public LoginPageViewModel()
        {
            this.tcs = new TaskCompletionSource<IUserIdentity>();
        }

        /// <summary>
        /// Gets or sets the value of UserName.
        /// </summary>
        public string UserName
        {
            get { return this.userName; }
            set { this.SetProperty(ref this.userName, value); }
        }

        private string userName;

        /// <summary>
        /// Gets or sets the value of Password.
        /// </summary>
        public string Password
        {
            get { return this.password; }
            set { this.SetProperty(ref this.password, value); }
        }

        private string password;

        /// <summary>
        /// Gets or sets the value of Endpoint.
        /// </summary>
        public EndpointDescription Endpoint
        {
            get { return this.endpoint; }
            set { this.SetProperty(ref this.endpoint, value); }
        }

        private EndpointDescription endpoint;

        /// <summary>
        /// Gets the LoginCommand.
        /// </summary>
        public Command LoginCommand
        {
            get
            {
                return new Command(() =>
                {
                    this.tcs.TrySetResult(new UserNameIdentity(this.UserName, this.Password));
                });
            }
        }

        /// <summary>
        /// Gets the IUserIdentity.
        /// </summary>
        public Task<IUserIdentity> Task => this.tcs.Task;

    }
}
