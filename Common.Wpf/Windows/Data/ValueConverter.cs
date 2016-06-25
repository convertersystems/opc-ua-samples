// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Workstation.Windows.Data
{
    public class ValueConverter<TSource, TTarget> : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == Binding.DoNothing)
            {
                return value;
            }

            if (!(value is TSource) && ((value != null) || typeof(TSource).IsValueType))
            {
                Trace.TraceError("Error in '{0}', expected value of type '{1}', received value of type '{2}' ", this.GetType().FullName, typeof(TSource).FullName, value != null ? value.GetType().FullName : "null");
                return DependencyProperty.UnsetValue;
            }

            if (!targetType.IsAssignableFrom(typeof(TTarget)))
            {
                Trace.TraceError("Error in '{0}', expected target of type '{1}', received target of type '{2}' ", this.GetType().FullName, typeof(TTarget).FullName, targetType.FullName);
                return DependencyProperty.UnsetValue;
            }

            return this.Convert((TSource)value, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TTarget) && ((value != null) || typeof(TTarget).IsValueType))
            {
                Trace.TraceError("Error in '{0}', expected value of type '{1}', received value of type '{2}' ", this.GetType().FullName, typeof(TTarget).FullName, value != null ? value.GetType().FullName : "null");
                return DependencyProperty.UnsetValue;
            }

            if (!targetType.IsAssignableFrom(typeof(TSource)))
            {
                Trace.TraceError("Error in '{0}', expected target of type '{1}', received target of type '{2}' ", this.GetType().FullName, typeof(TSource).FullName, targetType.FullName);
                return DependencyProperty.UnsetValue;
            }

            return this.ConvertBack((TTarget)value, parameter, culture);
        }

        protected virtual TTarget Convert(TSource value, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Error_ConverterFunctionNotDefined {0}", new object[] { "Convert" }));
        }

        protected virtual TSource ConvertBack(TTarget value, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Error_ConverterFunctionNotDefined {0}", new object[] { "ConvertBack" }));
        }
    }
}