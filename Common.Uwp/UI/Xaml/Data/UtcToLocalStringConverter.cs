// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace Workstation.UI.Xaml.Data
{
    /// <summary>
    /// Converts DateTime from UTC to LocalTime
    /// </summary>
    public class UtcToLocalStringConverter : ValueConverter<DateTime, string>
    {
        protected override string Convert(DateTime value, object parameter, CultureInfo cultureInfo)
        {
            var format = parameter as string;
            if (format != null)
            {
                return value.ToLocalTime().ToString(format, cultureInfo);
            }

            return value.ToLocalTime().ToString(cultureInfo);
        }
    }
}