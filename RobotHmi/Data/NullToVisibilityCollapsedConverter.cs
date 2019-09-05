// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RobotHmi.Data
{
    /// <summary>
    /// Returns Visibility.Collapsed if Value is Null
    /// </summary>
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullToVisibilityCollapsedConverter : ValueConverter<object, Visibility>
    {
        protected override Visibility Convert(object obj, object parameter, CultureInfo culture)
        {
            return obj != null ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}