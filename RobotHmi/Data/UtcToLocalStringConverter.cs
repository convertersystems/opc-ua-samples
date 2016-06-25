// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Windows.Data;
using Workstation.Windows.Data;

namespace RobotHmi.Data
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class UtcToLocalStringConverter : ValueConverter<DateTime, string>
    {
        protected override string Convert(DateTime value, object parameter, CultureInfo culture)
        {
            var s = parameter as string;
            if (s != null)
            {
                return value.ToLocalTime().ToString(s, culture);
            }

            return value.ToLocalTime().ToString(culture);
        }
    }
}