// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows.Data;

namespace RobotHmi.Data
{
    /// <summary>
    /// Returns TrueResult if Value equals true, FalseResult if Value equals false
    /// </summary>
    [ValueConversion(typeof(bool), typeof(string))]
    public class BooleanToStringConverter : ValueConverter<bool, string>
    {
        private string falseResult = "False";
        private string trueResult = "True";

        public string TrueResult
        {
            get { return this.trueResult; }
            set { this.trueResult = value; }
        }

        public string FalseResult
        {
            get { return this.falseResult; }
            set { this.falseResult = value; }
        }

        protected override string Convert(bool value, object parameter, CultureInfo culture)
        {
            return value ? this.TrueResult : this.FalseResult;
        }
    }
}