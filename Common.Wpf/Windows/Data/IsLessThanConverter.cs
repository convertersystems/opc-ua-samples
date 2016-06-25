// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace Workstation.Windows.Data
{
    /// <summary>
    /// Return true if Value is less than Parameter
    /// </summary>
    [ValueConversion(typeof(double), typeof(bool), ParameterType = typeof(double))]
    public class IsLessThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
            {
                throw new ArgumentException("Target must be a boolean");
            }

            if ((value == null) || (parameter == null))
            {
                return false;
            }

            double convertedValue;
            if (!double.TryParse(value.ToString(), out convertedValue))
            {
                throw new InvalidOperationException("The Value can not be converted to a Double");
            }

            double convertedParameter;
            if (!double.TryParse(parameter.ToString(), out convertedParameter))
            {
                throw new InvalidOperationException("The Parameter can not be converted to a Double");
            }

            return convertedValue < convertedParameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }
}