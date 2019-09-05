// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows.Data;

namespace RobotHmi.Data
{
    [ValueConversion(typeof(float), typeof(double))]
    public class SingleToDoubleConverter : ValueConverter<float, double>
    {
        protected override double Convert(float obj, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(obj);
        }

        protected override float ConvertBack(double obj, object parameter, CultureInfo culture)
        {
            return System.Convert.ToSingle(obj);
        }
    }
}