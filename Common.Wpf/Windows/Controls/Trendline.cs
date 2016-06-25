// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Expression = System.Linq.Expressions.Expression;

namespace Workstation.Windows.Controls
{
    public class Trendline : Control
    {
        public static readonly DependencyProperty ItemsSourceProperty;
        public static readonly DependencyProperty ValuePathProperty;
        public static readonly DependencyProperty TimePathProperty;
        public static readonly DependencyProperty MinValueProperty;
        public static readonly DependencyProperty MaxValueProperty;
        public static readonly DependencyProperty StrokeProperty;
        public static readonly DependencyProperty StrokeThicknessProperty;
        public static readonly DependencyProperty TimeSpanProperty;
        public static readonly DependencyProperty StartTimeProperty;
        public static readonly DependencyProperty EndTimeProperty;
        public static readonly DependencyProperty GeometryProperty;
        public static readonly DependencyProperty ShowAxisProperty;
        public static readonly DependencyProperty AutoRangeProperty;

        private readonly ScaleTransform renderScale;
        private readonly TranslateTransform renderTranslate;
        private readonly ScaleTransform scale;
        private readonly TranslateTransform translate;
        private readonly TimeSpan frameDuration = TimeSpan.FromMilliseconds(1000.0 / 24.0); // 24 fps
        private FrameworkElement chartArea;
        private bool isAnimationRunning;
        private DateTime previousUpdateTime;
        private Func<object, DateTime> timeGetter;
        private Func<object, IConvertible> valueGetter;

        static Trendline()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Trendline), new FrameworkPropertyMetadata(typeof(Trendline)));
            ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(Trendline), new FrameworkPropertyMetadata(null, OnItemsSourceChanged));
            ValuePathProperty = DependencyProperty.Register("ValuePath", typeof(string), typeof(Trendline), new FrameworkPropertyMetadata("Value"), IsPathValid);
            TimePathProperty = DependencyProperty.Register("TimePath", typeof(string), typeof(Trendline), new FrameworkPropertyMetadata("SourceTimestamp"), IsPathValid);
            MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(Trendline), new FrameworkPropertyMetadata(0.0, OnMinMaxChanged), IsMinMaxValid);
            MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(Trendline), new FrameworkPropertyMetadata(100.0, OnMinMaxChanged), IsMinMaxValid);
            StartTimeProperty = DependencyProperty.Register("StartTime", typeof(DateTime), typeof(Trendline), new FrameworkPropertyMetadata(DateTime.MinValue));
            EndTimeProperty = DependencyProperty.Register("EndTime", typeof(DateTime), typeof(Trendline), new FrameworkPropertyMetadata(DateTime.MaxValue));
            StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(Trendline), new FrameworkPropertyMetadata(null));
            StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(Trendline), new FrameworkPropertyMetadata(1.0));
            TimeSpanProperty = DependencyProperty.Register("TimeSpan", typeof(TimeSpan), typeof(Trendline), new FrameworkPropertyMetadata(new TimeSpan(0, 0, 60)));
            GeometryProperty = DependencyProperty.Register("Geometry", typeof(StreamGeometry), typeof(Trendline));
            ShowAxisProperty = DependencyProperty.Register("ShowAxis", typeof(bool), typeof(Trendline), new FrameworkPropertyMetadata(false));
            AutoRangeProperty = DependencyProperty.Register("AutoRange", typeof(bool), typeof(Trendline), new FrameworkPropertyMetadata(true));
        }

        public Trendline()
        {
            this.translate = new TranslateTransform();
            this.scale = new ScaleTransform();
            this.renderTranslate = new TranslateTransform();
            this.renderScale = new ScaleTransform();
            this.Geometry = new StreamGeometry { Transform = new TransformGroup { Children = new TransformCollection { this.translate, this.scale, this.renderScale, this.renderTranslate } } };
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        [Category("Common")]
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        [Category("Common")]
        public double MinValue
        {
            get { return (double)this.GetValue(MinValueProperty); }
            set { this.SetValue(MinValueProperty, value); }
        }

        [Category("Common")]
        public double MaxValue
        {
            get { return (double)this.GetValue(MaxValueProperty); }
            set { this.SetValue(MaxValueProperty, value); }
        }

        [Category("Common")]
        public TimeSpan TimeSpan
        {
            get { return (TimeSpan)this.GetValue(TimeSpanProperty); }
            set { this.SetValue(TimeSpanProperty, value); }
        }

        [Category("Common")]
        public string TimePath
        {
            get { return (string)this.GetValue(TimePathProperty); }
            set { this.SetValue(TimePathProperty, value); }
        }

        [Category("Common")]
        public string ValuePath
        {
            get { return (string)this.GetValue(ValuePathProperty); }
            set { this.SetValue(ValuePathProperty, value); }
        }

        [Category("Brush")]
        public Brush Stroke
        {
            get { return (Brush)this.GetValue(StrokeProperty); }
            set { this.SetValue(StrokeProperty, value); }
        }

        [Category("Appearance")]
        [TypeConverter(typeof(LengthConverter))]
        public double StrokeThickness
        {
            get { return (double)this.GetValue(StrokeThicknessProperty); }
            set { this.SetValue(StrokeThicknessProperty, value); }
        }

        public bool AutoRange
        {
            get { return (bool)this.GetValue(AutoRangeProperty); }
            set { this.SetValue(AutoRangeProperty, value); }
        }

        public bool ShowAxis
        {
            get { return (bool)this.GetValue(ShowAxisProperty); }
            set { this.SetValue(ShowAxisProperty, value); }
        }

        public DateTime StartTime
        {
            get { return (DateTime)this.GetValue(StartTimeProperty); }
            private set { this.SetValue(StartTimeProperty, value); }
        }

        public DateTime EndTime
        {
            get { return (DateTime)this.GetValue(EndTimeProperty); }
            private set { this.SetValue(EndTimeProperty, value); }
        }

        public StreamGeometry Geometry
        {
            get { return (StreamGeometry)this.GetValue(GeometryProperty); }
            private set { this.SetValue(GeometryProperty, value); }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Trendline)d).OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            this.valueGetter = null;
            this.timeGetter = null;

            if (newValue != null)
            {
                var elementType = newValue.AsQueryable().ElementType;
                this.valueGetter = TryMakePropertyGetFromObjectDelegate<IConvertible>(elementType, this.ValuePath);
                this.timeGetter = TryMakePropertyGetFromObjectDelegate<DateTime>(elementType, this.TimePath);
            }
        }

        /// <summary>
        /// Builds a delegate that casts an object to a target type and then gets the value a property or field.
        /// </summary>
        /// <typeparam name="T">The expected return type of the property or field.</typeparam>
        /// <param name="targetType">The expected target instance type.</param>
        /// <param name="propertyOrField">The name of the target property or field.</param>
        /// <returns>A delegate.</returns>
        private static Func<object, T> TryMakePropertyGetFromObjectDelegate<T>(Type targetType, string propertyOrField)
        {
            try
            {
                var target = Expression.Parameter(typeof(object), "target");
                var expr = Expression.Lambda<Func<object, T>>(Expression.Convert(Expression.PropertyOrField(Expression.Convert(target, targetType), propertyOrField), typeof(T)), target);
                return expr.Compile();
            }
            catch
            {
                return null;
            }
        }

        private static bool IsPathValid(object value)
        {
            var path = (string)value;
            return path.IndexOfAny(new[] { '.' }) == -1;
        }

        private static void OnMinMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Trendline)d).OnMinMaxChanged(e);
        }

        private void OnMinMaxChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((this.MaxValue - this.MinValue) < 1.53E-06)
            {
                this.translate.Y = -this.MaxValue - 0.5; // as if max/min = value +/- 0.5
                this.scale.ScaleY = -1.0;
            }
            else
            {
                this.translate.Y = -this.MaxValue;
                this.scale.ScaleY = -1.0 / (this.MaxValue - this.MinValue);
            }
        }

        private static bool IsMinMaxValid(object value)
        {
            var num = (double)value;
            return !double.IsNaN(num) && !double.IsInfinity(num);
        }

        private void UpdateGeometry()
        {
            try
            {
                this.previousUpdateTime = DateTime.UtcNow;
                this.EndTime = this.previousUpdateTime;
                this.StartTime = this.previousUpdateTime - this.TimeSpan;
                var isEmpty = true;
                var minValue = double.MaxValue;
                var maxValue = double.MinValue;
                using (var context = this.Geometry.Open())
                {
                    if (this.ItemsSource == null || this.valueGetter == null || this.timeGetter == null)
                    {
                        return;
                    }

                    var startTicks = this.StartTime.Ticks;
                    var endTicks = this.EndTime.Ticks;
                    var rangeTicks = endTicks - startTicks;
                    var startDrawingTicks = startTicks - (rangeTicks / 10);
                    var endDrawingTicks = endTicks + (rangeTicks / 10);

                    foreach (var item in this.ItemsSource)
                    {
                        if (item == null)
                        {
                            continue;
                        }

                        var time = this.timeGetter(item);
                        var tick = time.Ticks;
                        if (tick >= startDrawingTicks)
                        {
                            if (tick > endDrawingTicks)
                            {
                                break;
                            }

                            var value = this.valueGetter(item).ToDouble(null);
                            if (isEmpty)
                            {
                                context.BeginFigure(new Point((double)(tick - startTicks) / rangeTicks, value), false, false);
                                isEmpty = false;
                            }
                            else
                            {
                                context.LineTo(new Point((double)(tick - startTicks) / rangeTicks, value), true, true);
                            }

                            if (maxValue < value)
                            {
                                maxValue = value;
                            }

                            if (minValue > value)
                            {
                                minValue = value;
                            }
                        }
                    }
                }

                if (this.AutoRange && !isEmpty)
                {
                    this.MaxValue = maxValue;
                    this.MinValue = minValue;
                }
            }
            catch
            {
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!this.isAnimationRunning && !DesignerProperties.GetIsInDesignMode(this))
            {
                this.isAnimationRunning = true;
                CompositionTarget.Rendering += this.OnRendering;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (this.isAnimationRunning)
            {
                this.isAnimationRunning = false;
                CompositionTarget.Rendering -= this.OnRendering;
            }
        }

        private void OnRendering(object sender, EventArgs eventArgs)
        {
            if (DateTime.UtcNow.Subtract(this.previousUpdateTime) > this.frameDuration)
            {
                this.UpdateGeometry();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.chartArea = (this.GetTemplateChild("PART_ChartArea") as FrameworkElement) ?? this;
            this.chartArea.SizeChanged += this.OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.renderScale.ScaleX = Math.Max(e.NewSize.Width - this.StrokeThickness, 0.0);
            this.renderScale.ScaleY = Math.Max(e.NewSize.Height - this.StrokeThickness, 0.0);
            this.renderTranslate.X = this.StrokeThickness / 2.0;
            this.renderTranslate.Y = this.StrokeThickness / 2.0;
        }
    }
}