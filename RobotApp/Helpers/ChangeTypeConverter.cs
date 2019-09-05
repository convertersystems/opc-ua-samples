// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace RobotApp.Views
{
    /// <summary>
    /// Converts a base data type implementing IConvertible. 
    /// </summary>
    public class ChangeTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ChangeType(value, targetType, language != null ? new CultureInfo(language) : CultureInfo.CurrentUICulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return this.Convert(value, targetType, parameter, language);
        }
    }
}
