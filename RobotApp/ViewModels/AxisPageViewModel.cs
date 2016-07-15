// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RobotApp.Services;
using Template10.Mvvm;

namespace RobotApp.ViewModels
{
    public class AxisPageViewModel : ViewModelBase
    {
        private PLC1Service service;

        public AxisPageViewModel(PLC1Service service)
        {
            this.service = service;
        }

        public IAxisViewModel Axis1ViewModel
        {
            get { return new Axis1ViewModel(this.service); }
        }

        public IAxisViewModel Axis2ViewModel
        {
            get { return new Axis2ViewModel(this.service); }
        }

        public IAxisViewModel Axis3ViewModel
        {
            get { return new Axis3ViewModel(this.service); }
        }

        public IAxisViewModel Axis4ViewModel
        {
            get { return new Axis4ViewModel(this.service); }
        }
    }
}