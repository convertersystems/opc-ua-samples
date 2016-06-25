// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Windows.UI.Xaml;

namespace Workstation.UI.Xaml.Data
{
    /// <summary>
    /// Return true if Value is equal Parameter
    /// </summary>
    public class IsEqualConverter : ValueConverter<object, bool>
    {
        /// <summary>
        /// If set to True, conversion is reversed: Return true if Value is not equal Parameter
        /// </summary>
        public bool IsReversed { get; set; }

        protected override bool Convert(object value, object parameter, CultureInfo cultureInfo)
        {
            if (this.IsReversed)
            {
                return !(Equals(value, parameter));
            }
            return Equals(value, parameter);
        }
    }
}