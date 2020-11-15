// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MahApps.Metro;
using RobotHmi.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RobotHmi.Data
{
    [ValueConversion(typeof(AccentColorMenuData), typeof(Brush))]
    public class AccentToBrushConverter : ValueConverter<AccentColorMenuData, Brush>
    {
        protected override Brush Convert(AccentColorMenuData value, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return value.ColorBrush;
        }
    }
}