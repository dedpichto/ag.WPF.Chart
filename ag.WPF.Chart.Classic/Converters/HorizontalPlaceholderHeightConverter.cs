using ag.WPF.Chart.Series;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ag.WPF.Chart.Converters
{
#nullable disable
    /// <summary>
    /// Defines height of row of numeric/custom values drawn next to x-axis
    /// </summary>
    public class HorizontalPlaceholderHeightConverter : IMultiValueConverter
    {
        /// <summary>Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.</summary>
        /// <returns>A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty" />.<see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding" />.<see cref="F:System.Windows.Data.Binding.DoNothing" /> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> or the default value.</returns>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding" /> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var height = 8.0;

            if (values == null
                || values[1] is not ChartStyle chartStyle
                || chartStyle == ChartStyle.Funnel
                || values[2] is not string formatX
                || values[3] is not FontFamily fontFamily
                || values[4] is not double fontSize
                || values[5] is not FontStyle fontStyle
                || values[6] is not FontWeight fontWeight
                || values[7] is not FontStretch fontStretch
                || values[9] is not AutoAdjustmentMode autoAdjust
                || values[10] is not double maxX
                || values[12] is not int linesCountX
                || values[13] is not string formatY)
                return height;

            var seriesEnumerable = values[0] as IEnumerable<ISeries>;
            var chartSeries = values[14] as IEnumerable<ISeries>;

            if (seriesEnumerable != null && seriesEnumerable.Any())
                seriesEnumerable = seriesEnumerable.Where(s => s.IsVisible);
            if (chartSeries != null && chartSeries.Any())
                chartSeries = chartSeries.Where(s => s.IsVisible);
            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return height;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            var customValuesX = values[8] is IEnumerable<string> customEnumerableX ? customEnumerableX.ToArray() : Array.Empty<string>();
            var customValuesY = values[11] is IEnumerable<string> customEnumerableY ? customEnumerableY.ToArray() : Array.Empty<string>();

            Directions dir;
            int maxFromValues = 0, maxForBars = 0;

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return height;
                var totalValues = chartStyle != ChartStyle.Waterfall
                    ? seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue))
                    : seriesArray[0].Values.Select(v => v.CompositeValue.PlainValue);
                dir = Utils.GetDirection(totalValues, chartStyle);
                if (Utils.StyleBars(chartStyle))
                {
                    maxForBars = autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Horizontal) ? seriesArray.Max(s => s.Values.Count) : linesCountX;
                    if (dir.In(Directions.NorthWest, Directions.NorthEastNorthWest))
                        maxForBars *= -1;
                }
                else
                {
                    var maxV = chartStyle != ChartStyle.Waterfall ? seriesArray.Max(s => s.Values.Count).ToString(culture).Length : seriesArray[0].Values.Count.ToString(culture).Length;
                    maxFromValues = autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Horizontal) ? maxV : maxX.ToString(culture).Length;
                }
            }
            else if (seriesArray.All(s => s.IsStockSeries()))
            {
                if (!chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return height;

                if (!seriesArray[0].Values.Any())
                    return height;
                if (chartStyle == ChartStyle.HighLowClose && seriesArray[0] is not HighLowCloseSeries)
                    return height;
                else if (chartStyle == ChartStyle.OpenHighLowClose && seriesArray[0] is not OpenHighLowCloseSeries)
                    return height;

                var totalValues = seriesArray[0].Values.Select(v => (v.CompositeValue.HighValue, v.CompositeValue.LowValue));
                dir = Utils.GetDirectionFinancial(totalValues);
                maxFromValues = autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Vertical) ? seriesArray[0].Values.Count.ToString(culture).Length : maxX.ToString(culture).Length;
            }
            else
                return height;

            string maxString;
            if (!Utils.StyleBars(chartStyle))
            {
                maxString = customValuesX.Any()
                    ? customValuesX.FirstOrDefault(c => c.Length == customValuesX.Max(v => v.Length))
                    : new string('0', maxFromValues);
            }
            else
            {
                if (customValuesY.Any())
                {
                    var cv = customValuesY.FirstOrDefault(c => c.Length == customValuesY.Max(v => v.Length));
                    if (cv.Length > maxForBars.ToString(formatX, culture).Length)
                        maxString = cv;
                    else
                        maxString = maxForBars.ToString(formatX, culture);
                }
                else
                {
                    maxString = maxForBars.ToString(formatX, culture);
                }
            }


            var fmt = new FormattedText(maxString, culture, FlowDirection.LeftToRight,
                            new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip)
            {
                TextAlignment = TextAlignment.Right
            };
            var pt = new Point(0, 0);
            var ngm = fmt.BuildGeometry(pt);

            var trgr = new TransformGroup();
            trgr.Children.Add(new RotateTransform(-45, pt.X + fmt.Width, pt.Y));
            ngm.Transform = trgr;

            height = ngm.Bounds.Height;
            // add some extra space
            return height + 8;
        }
        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
    }
#nullable restore
}
