// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace RobotApp.ViewModels
{
    public interface IAxisViewModel
    {
        float Axis { get; }

        string DisplayName { get; }
    }
}
