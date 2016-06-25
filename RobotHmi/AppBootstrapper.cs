// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;
using RobotHmi.Services;
using RobotHmi.Views;
using System;
using System.Windows;

namespace RobotHmi
{
    /// <summary>
    /// Runs a bootstrapping sequence that initializes the Prism services.
    /// </summary>
    public class AppBootstrapper : UnityBootstrapper, IDisposable
    {
        public void Dispose()
        {
            // Disposing the container will dispose all the shared component parts.
            if (this.Container != null)
            {
                this.Container.Dispose();
                this.Container = null;
            }
        }

        protected override void ConfigureModuleCatalog()
        {
            ModuleCatalog catalog = (ModuleCatalog)this.ModuleCatalog;
            catalog.AddModule(typeof(MainModule));
        }

        protected override DependencyObject CreateShell()
        {
            // Creates the Shell window.
            return ServiceLocator.Current.GetInstance<Shell>();
        }

        protected override void InitializeShell()
        {
            // Shows the Shell window.
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            this.Container.RegisterType<PLC1Service>(new ContainerControlledLifetimeManager());
        }
    }
}