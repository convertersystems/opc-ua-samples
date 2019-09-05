// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RobotApp.ViewModels;
using System.Globalization;

namespace RobotApp.Views
{
    /// <summary>
    /// Return true if Value is equal Parameter
    /// </summary>
    public class IsModeHandConverter : ValueConverter<short, bool>
    {
        /// <summary>
        /// If set to True, conversion is reversed: Return true if Value is not equal Hand
        /// </summary>
        public bool IsReversed { get; set; }

        protected override bool Convert(short value, object parameter, CultureInfo cultureInfo)
        {
            if (this.IsReversed)
            {
                return value != (short)HandOffAuto.Hand;
            }
            return value == (short)HandOffAuto.Hand;
        }
    }
}
