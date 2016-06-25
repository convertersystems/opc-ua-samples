// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Workstation.Windows.Data
{
    /// <summary>
    /// Returns a string selected by index.
    /// Provide list of strings in parameter, eg "string0,string1,string2".
    /// </summary>
    [ValueConversion(typeof(int), typeof(string), ParameterType = typeof(string))]
    public class StringSelectionConverter : IValueConverter
    {
        private static readonly Dictionary<string, List<string>> Cache = new Dictionary<string, List<string>>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = System.Convert.ToInt32(value, culture);
            string key = System.Convert.ToString(parameter, culture);

            if (!string.IsNullOrEmpty(key))
            {
                List<string> list;
                if (!Cache.TryGetValue(key, out list))
                {
                    try
                    {
                        list = new List<string>(key.Split(new[] { ',' }));
                    }
                    catch
                    {
                        list = new List<string>(0);
                    }

                    Cache.Add(key, list);
                }

                return list.ElementAtOrDefault(index);
            }

            return default(string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}