// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows.Data;

namespace Workstation.Windows.Data
{
    /// <summary>
    /// Return inverted boolean (=!Value)
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class IsNotConverter : ValueConverter<bool, bool>
    {
        protected override bool Convert(bool value, object parameter, CultureInfo culture)
        {
            return !value;
        }

        protected override bool ConvertBack(bool value, object parameter, CultureInfo culture)
        {
            return !value;
        }
    }
}