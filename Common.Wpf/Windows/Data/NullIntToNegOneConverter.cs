// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace Workstation.Windows.Data
{
    [ValueConversion(typeof(int?), typeof(int))]
    public class NullIntToNegOneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var i = value as int?;
            if (i != null)
            {
                return i.GetValueOrDefault(-1);
            }

            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                var i = (int)value;
                if (i == -1)
                {
                    return default(int?);
                }

                return (int?)i;
            }

            return Binding.DoNothing;
        }
    }
}