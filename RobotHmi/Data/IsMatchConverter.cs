// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace RobotHmi.Data
{
    /// <summary>
    /// Return true if Value matches the regular expression (=Parameter) ?
    /// </summary>
    [ValueConversion(typeof(string), typeof(bool), ParameterType = typeof(string))]
    public class IsMatchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value == null) || (parameter == null))
            {
                return false;
            }

            var regex = new Regex((string)parameter);
            return regex.IsMatch((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}