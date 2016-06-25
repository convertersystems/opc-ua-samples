// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Workstation.ServiceModel.Ua;
using Workstation.UI.Xaml.Data;

namespace RobotApp.Views
{
    /// <summary>
    /// Return string value of AlarmCondition
    /// </summary>
    public class AlarmConditionToStringConverter : ValueConverter<AlarmCondition, string>
    {
        protected override string Convert(AlarmCondition value, object parameter, CultureInfo cultureInfo)
        {
            return $"{value.Time.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", cultureInfo)}  {value.SourceName ?? "System"}: {value.Message.Text}";
        }
    }
}