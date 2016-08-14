// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RobotApp.Services;
using System.Collections.ObjectModel;
using Template10.Mvvm;

namespace RobotApp.ViewModels
{
    public class AxisPageViewModel : ViewModelBase
    {
        public AxisPageViewModel() // design-time only
        {
            this.Axes = new Collection<IAxisViewModel>();
            this.Axes.Add(new AxisViewModelDesignInstance());
        }

        public AxisPageViewModel(PLC1Service service)
        {
            this.Axes = new Collection<IAxisViewModel>();
            this.Axes.Add(new Axis1ViewModel(service));
            this.Axes.Add(new Axis2ViewModel(service));
            this.Axes.Add(new Axis3ViewModel(service));
            this.Axes.Add(new Axis4ViewModel(service));
        }

        public Collection<IAxisViewModel> Axes { get; }
    }
}