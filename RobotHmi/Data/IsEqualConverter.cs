// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RobotHmi.Data
{
    /// <summary>
    /// Return true if Value equals Parameter
    /// </summary>
    [ValueConversion(typeof(object), typeof(bool), ParameterType = typeof(object))]
    public class IsEqualConverter : ValueConverter<object, bool>
    {
        // Methods
        protected override bool Convert(object value, object parameter, CultureInfo culture)
        {
            return Equals(value, parameter);
        }

    }
}