using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections;
using System.Linq;
using System.Numerics;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Expression = System.Linq.Expressions.Expression;

namespace Workstation.UI.Xaml.Controls
{
    public class Trendline : UserControl
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
        public static readonly DependencyProperty ShowAxisProperty;
        public static readonly DependencyProperty AutoRangeProperty;

        private readonly ScaleTransform renderScale;
        private readonly TranslateTransform renderTranslate;
        private readonly ScaleTransform scale;
        private readonly TranslateTransform translate;
        private readonly TimeSpan frameDuration = TimeSpan.FromSeconds(1.0 / 12.0); // 12 fps
        private bool isAnimationRunning;
        private DateTime previousUpdateTime;
        private Func<object, DateTime> timeGetter;
        private Func<object, IConvertible> valueGetter;
        private CanvasControl canvas;
        private TransformGroup transformGroup;

        static Trendline()
        {
            ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(Trendline), new PropertyMetadata(null, OnItemsSourceChanged));
            ValuePathProperty = DependencyProperty.Register("ValuePath", typeof(string), typeof(Trendline), new PropertyMetadata("Value"));
            TimePathProperty = DependencyProperty.Register("TimePath", typeof(string), typeof(Trendline), new PropertyMetadata("SourceTimestamp"));
            MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(Trendline), new PropertyMetadata(0.0, OnMinMaxChanged));
            MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(Trendline), new PropertyMetadata(100.0, OnMinMaxChanged));
            StartTimeProperty = DependencyProperty.Register("StartTime", typeof(DateTime), typeof(Trendline), new PropertyMetadata(DateTime.MinValue));
            EndTimeProperty = DependencyProperty.Register("EndTime", typeof(DateTime), typeof(Trendline), new PropertyMetadata(DateTime.MaxValue));
            StrokeProperty = DependencyProperty.Register("Stroke", typeof(SolidColorBrush), typeof(Trendline), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnPenChanged));
            StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(Trendline), new PropertyMetadata(2.0, OnPenChanged));
            TimeSpanProperty = DependencyProperty.Register("TimeSpan", typeof(TimeSpan), typeof(Trendline), new PropertyMetadata(new TimeSpan(0, 0, 60)));
            ShowAxisProperty = DependencyProperty.Register("ShowAxis", typeof(bool), typeof(Trendline), new PropertyMetadata(false));
            AutoRangeProperty = DependencyProperty.Register("AutoRange", typeof(bool), typeof(Trendline), new PropertyMetadata(true));
        }

        public Trendline()
        {
            DefaultStyleKey = typeof(Trendline);
            this.translate = new TranslateTransform();
            this.scale = new ScaleTransform();
            this.renderTranslate = new TranslateTransform();
            this.renderScale = new ScaleTransform();
            this.transformGroup = new TransformGroup { Children = new TransformCollection { this.translate, this.scale, this.renderScale, this.renderTranslate } };
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        public double MinValue
        {
            get { return (double)this.GetValue(MinValueProperty); }
            set { this.SetValue(MinValueProperty, value); }
        }

        public double MaxValue
        {
            get { return (double)this.GetValue(MaxValueProperty); }
            set { this.SetValue(MaxValueProperty, value); }
        }

        public TimeSpan TimeSpan
        {
            get { return (TimeSpan)this.GetValue(TimeSpanProperty); }
            set { this.SetValue(TimeSpanProperty, value); }
        }

        public string TimePath
        {
            get { return (string)this.GetValue(TimePathProperty); }
            set { this.SetValue(TimePathProperty, value); }
        }

        public string ValuePath
        {
            get { return (string)this.GetValue(ValuePathProperty); }
            set { this.SetValue(ValuePathProperty, value); }
        }

        public SolidColorBrush Stroke
        {
            get { return (SolidColorBrush)this.GetValue(StrokeProperty); }
            set { this.SetValue(StrokeProperty, value); }
        }

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

        private static void OnPenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Trendline)d).OnPenChanged(e);
        }

        private void OnPenChanged(DependencyPropertyChangedEventArgs e)
        {
            this.canvas?.Invalidate();
        }

        private Size currentSize = Size.Empty;

        private void UpdateGeometry(CanvasDrawingSession ds, Size size)
        {
            try
            {
                var now = DateTime.UtcNow;
                this.EndTime = now;
                this.StartTime = now - this.TimeSpan;
                var isEmpty = true;
                var minValue = double.MaxValue;
                var maxValue = double.MinValue;
                var startTicks = this.StartTime.Ticks;
                var endTicks = this.EndTime.Ticks;
                var rangeTicks = endTicks - startTicks;
                var startDrawingTicks = startTicks - (rangeTicks / 10);
                var endDrawingTicks = endTicks + (rangeTicks / 10);

                if (!size.Equals(this.currentSize))
                {
                    this.currentSize = size;
                    this.renderScale.ScaleX = Math.Max(size.Width - this.StrokeThickness, 0.0);
                    this.renderScale.ScaleY = Math.Max(size.Height - this.StrokeThickness, 0.0);
                    this.renderTranslate.X = this.StrokeThickness / 2.0;
                    this.renderTranslate.Y = this.StrokeThickness / 2.0;
                }

                var matrix = transformGroup.Value;

                var penColor = this.Stroke?.Color ?? Colors.Transparent;
                var penThickness = (float)StrokeThickness;

                var bgColor = (this.Background as SolidColorBrush)?.Color ?? Colors.Transparent;
                if (bgColor != Colors.Transparent)
                {
                    ds.Clear(bgColor);
                }

                ds.Antialiasing = CanvasAntialiasing.Antialiased;

                if (this.ItemsSource == null || this.valueGetter == null || this.timeGetter == null)
                {
                    return;
                }

                var p0 = default(Vector2);
                var p1 = default(Vector2);

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
                            //context.BeginFigure(, false, false);
                            p1 = matrix.Transform(new Point((double)(tick - startTicks) / rangeTicks, value)).ToVector2();
                            isEmpty = false;
                        }
                        else
                        {
                            //context.LineTo(new Point((double)(tick - startTicks) / rangeTicks, value), true, true);
                            p0 = p1;
                            p1 = matrix.Transform(new Point((double)(tick - startTicks) / rangeTicks, value)).ToVector2();
                            ds.DrawLine(p0, p1, penColor, penThickness);
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
            canvas = new CanvasControl();
            canvas.ClearColor = (this.Background as SolidColorBrush)?.Color ?? Colors.Transparent;
            canvas.Draw += OnDraw; ;
            Content = canvas;

            if (!this.isAnimationRunning)
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

            // Explicitly remove references to allow the Win2D controls to get garbage collected
            if (canvas != null)
            {
                canvas.RemoveFromVisualTree();
                canvas = null;
            }
        }

        private void OnRendering(object sender, object e)
        {
            if (DateTime.UtcNow.Subtract(this.previousUpdateTime) > this.frameDuration)
            {
                this.previousUpdateTime = DateTime.UtcNow;
                canvas.Invalidate();
            }
        }

        private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            this.UpdateGeometry(args.DrawingSession, sender.Size);
        }
    }
}
