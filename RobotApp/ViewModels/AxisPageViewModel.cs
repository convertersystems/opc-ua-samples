// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.ServiceLocation;
using Template10.Mvvm;

namespace RobotApp.ViewModels
{
    public class AxisPageViewModel : ViewModelBase
    {
        public AxisPageViewModel(IAxisViewModel[] axes)
        {
            this.Axes = axes;
        }

        public IEnumerable<IAxisViewModel> Axes { get; }
    }

    public class AxisPageViewModelDesignInstance : AxisPageViewModel
    {
        public AxisPageViewModelDesignInstance()
            : base(new[] { new Axis1ViewModel() })
        {
        }
    }
}