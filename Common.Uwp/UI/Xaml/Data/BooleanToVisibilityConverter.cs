// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Windows.UI.Xaml;

namespace Workstation.UI.Xaml.Data
{
    /// <summary>
    /// Converts Boolean to Visibility
    /// </summary>
    public class BooleanToVisibilityConverter : ValueConverter<bool, Visibility>
    {
        /// <summary>
        /// If set to True, conversion is reversed: True will become Collapsed.
        /// </summary>
        public bool IsReversed { get; set; }

        protected override Visibility Convert(bool value, object parameter, CultureInfo cultureInfo)
        {
            if (this.IsReversed)
            {
                return value ? Visibility.Collapsed : Visibility.Visible;
            }

            return value ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}