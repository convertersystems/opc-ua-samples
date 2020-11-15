// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ControlzEx.Theming;
using MahApps.Metro;
using RobotHmi.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RobotHmi.Data
{
    [ValueConversion(typeof(AppThemeMenuData), typeof(Brush))]
    public class ThemeToBrushConverter : ValueConverter<AppThemeMenuData, Brush>
    {
        protected override Brush Convert(AppThemeMenuData value, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            //return value.Resources["WindowBackgroundBrush"] as Brush ?? Brushes.White;
            return value.ColorBrush;
        }
    }
}