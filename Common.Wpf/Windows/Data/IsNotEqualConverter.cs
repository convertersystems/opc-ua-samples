// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Workstation.Windows.Data
{
    /// <summary>
    /// Return true if Value is not equal Parameter
    /// </summary>
    [ValueConversion(typeof(object), typeof(bool), ParameterType = typeof(object))]
    public class IsNotEqualConverter : ValueConverter<object, bool>
    {
        protected override bool Convert(object value, object parameter, CultureInfo culture)
        {
            return !(Equals(value, parameter));
        }
    }
}