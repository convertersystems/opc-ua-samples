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
    [ValueConversion(typeof(AppTheme), typeof(Brush))]
    public class ThemeToBrushConverter : ValueConverter<AppTheme, Brush>
    {
        protected override Brush Convert(AppTheme value, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return value.Resources["WindowBackgroundBrush"] as Brush ?? Brushes.White;
        }
    }
}