// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Workstation.UI.Xaml.Data
{
    public class ValueConverter<TSource, TTarget> : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is TSource) && ((value != null) || typeof(TSource).GetTypeInfo().IsValueType))
            {
                Debug.WriteLine("Error in '{0}', expected value of type '{1}', received value of type '{2}' ", this.GetType().FullName, typeof(TSource).FullName, value != null ? value.GetType().FullName : "null");
                return DependencyProperty.UnsetValue;
            }

            if (!targetType.IsAssignableFrom(typeof(TTarget)))
            {
                Debug.WriteLine("Error in '{0}', expected target of type '{1}', received target of type '{2}' ", this.GetType().FullName, typeof(TTarget).FullName, targetType.FullName);
                return DependencyProperty.UnsetValue;
            }

            return this.Convert((TSource)value, parameter, language != null ? new CultureInfo(language) : CultureInfo.CurrentUICulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (!(value is TTarget) && ((value != null) || typeof(TTarget).GetTypeInfo().IsValueType))
            {
                Debug.WriteLine("Error in '{0}', expected value of type '{1}', received value of type '{2}' ", this.GetType().FullName, typeof(TTarget).FullName, value != null ? value.GetType().FullName : "null");
                return DependencyProperty.UnsetValue;
            }

            if (!targetType.IsAssignableFrom(typeof(TSource)))
            {
                Debug.WriteLine("Error in '{0}', expected target of type '{1}', received target of type '{2}' ", this.GetType().FullName, typeof(TSource).FullName, targetType.FullName);
                return DependencyProperty.UnsetValue;
            }

            return this.ConvertBack((TTarget)value, parameter, language != null ? new CultureInfo(language) : CultureInfo.CurrentUICulture);
        }

        protected virtual TTarget Convert(TSource value, object parameter, CultureInfo cultureInfo)
        {
            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Error_ConverterFunctionNotDefined {0}", new object[] { "Convert" }));
        }

        protected virtual TSource ConvertBack(TTarget value, object parameter, CultureInfo cultureInfo)
        {
            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Error_ConverterFunctionNotDefined {0}", new object[] { "ConvertBack" }));
        }
    }
}