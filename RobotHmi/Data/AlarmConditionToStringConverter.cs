// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Workstation.ServiceModel.Ua;
using Workstation.Windows.Data;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RobotHmi.Data
{
    [ValueConversion(typeof(AlarmCondition), typeof(string))]
    public class AlarmConditionToStringConverter : ValueConverter<AlarmCondition, string>
    {
        protected override string Convert(AlarmCondition value, object parameter, CultureInfo culture)
        {
            return $"{value.Time.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", culture)}  {value.SourceName ?? "System"}: {value.Message?.Text ?? "Default message."}";
        }
    }
}