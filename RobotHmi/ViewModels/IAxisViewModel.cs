// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace RobotHmi.ViewModels
{
    public interface IAxisViewModel
    {
        float Axis { get; }

        string DisplayName { get; }
    }

    internal class AxisViewModelDesignInstance : IAxisViewModel
    {
        public float Axis => 42.0f;

        public string DisplayName => "Axis 1";
    }
}
