// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Workstation.Windows.Data;
using MahApps.Metro;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RobotHmi.Data
{
    [ValueConversion(typeof(Accent), typeof(Brush))]
    public class AccentToBrushConverter : ValueConverter<Accent, Brush>
    {
        protected override Brush Convert(Accent value, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return value.Resources["AccentColorBrush"] as Brush ?? Brushes.Blue;
        }
    }
}