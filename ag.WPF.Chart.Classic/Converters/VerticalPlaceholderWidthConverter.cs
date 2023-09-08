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
    /// Defines width of column of numeric/custom values drawn next to y-axis
    /// </summary>
    public class VerticalPlaceholderWidthConverter : IMultiValueConverter
    {
        /// <summary>Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.</summary>
        /// <returns>A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty" />.<see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding" />.<see cref="F:System.Windows.Data.Binding.DoNothing" /> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> or the default value.</returns>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding" /> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null
                || values[1] is not ChartStyle chartStyle
                || values[2] is not string formatY
                || values[3] is not FontFamily fontFamily
                || values[4] is not double fontSize
                || values[5] is not FontStyle fontStyle
                || values[6] is not FontWeight fontWeight
                || values[7] is not FontStretch fontStretch
                || values[9] is not AutoAdjustmentMode autoAdjust
                || values[10] is not double maxY
                || values[11] is not double height
                || values[12] is not int linesCountY
                || values[14] is not string formatX)
                return 0.0;
            var width = 28.0;

            var seriesEnumerable = values[0] as IEnumerable<ISeries>;
            var chartSeries = values[15] as IEnumerable<ISeries>;

            if (seriesEnumerable != null && seriesEnumerable.Any())
                seriesEnumerable = seriesEnumerable.Where(s => s.IsVisible);
            if (chartSeries != null && chartSeries.Any())
                chartSeries = chartSeries.Where(s => s.IsVisible);
            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return 0.0;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            var customValuesY = values[8] is IEnumerable<string> customEnumerableY ? customEnumerableY.ToArray() : Array.Empty<string>();
            var customValuesX = values[13] is IEnumerable<string> customEnumerableX ? customEnumerableX.ToArray() : Array.Empty<string>();

            if (chartStyle.In(ChartStyle.Radar, ChartStyle.RadarWithMarkers, ChartStyle.RadarArea))
                return 0.0;

            Directions dir;
            int maxForBars = 0;

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return 0.0;
                var totalValues = chartStyle != ChartStyle.Waterfall
                    ? seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue))
                    : seriesArray[0].Values.Select(v => v.CompositeValue.PlainValue);
                dir = Utils.GetDirection(totalValues, chartStyle);
                if (Utils.StyleBars(chartStyle))
                {
                    maxForBars = autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Vertical)
                        ? seriesArray.Max(s => s.Values.Count)
                        : linesCountY;
                }
            }
            else if (seriesArray.All(s => s.IsStockSeries()))
            {
                if (!chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return 0.0;

                if (!seriesArray[0].Values.Any())
                    return 0.0;
                if (chartStyle == ChartStyle.HighLowClose && seriesArray[0] is not HighLowCloseSeries)
                    return 0.0;
                else if (chartStyle == ChartStyle.OpenHighLowClose && seriesArray[0] is not OpenHighLowCloseSeries)
                    return 0.0;

                var totalValues = seriesArray[0].Values.Select(v => (v.CompositeValue.HighValue, v.CompositeValue.LowValue));
                dir = Utils.GetDirectionFinancial(totalValues);
            }
            else
                return 0.0;

            string maxString, maxStringY, maxStringX;
            FormattedText fmtX = null;
            var radius = 0.0;
            var centerX = 0.0;
            Point centerPoint;

            switch (dir)
            {
                case Directions.NorthEast:
                case Directions.NorthWest:
                case Directions.SouthEast:
                    centerX = 0;
                    radius = height - Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = 0;
                    radius = height - Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = 0;
                    radius = (height - Utils.AXIS_THICKNESS) / 2;
                    break;
            }
            centerPoint = new Point(centerX, radius);

            height -= 2 * Utils.AXIS_THICKNESS;

            if (!Utils.StyleBars(chartStyle))
            {
                if (!Utils.StyleFullStacked(chartStyle))
                {
                    var fmtY = new FormattedText("AAA", culture, FlowDirection.LeftToRight,
                        new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);

                    var (max, min, _, stepSize, _, _, _) = autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Vertical)
                        ? Utils.GetMeasures(
                           chartStyle,
                           seriesArray,
                           linesCountY,
                           radius,
                           fmtY.Height,
                           centerPoint,
                           dir == Directions.NorthEastSouthEast)
                        : (maxY, -maxY, linesCountY, maxY / linesCountY, radius / linesCountY, radius / maxY, default);

                    // fractional stepSize
                    if (!Utils.IsInteger(stepSize))
                    {
                        var fractions = Utils.GetDecimalPlaces(stepSize);
                        if (formatY.EndsWith("%"))
                            formatY = $"{formatY.Substring(0, formatY.Length - 1)}{culture.NumberFormat.NumberDecimalSeparator}{new string('0', fractions)}%";
                        else
                        {
                            var (numericPart, literalPart) = Utils.GetFormatParts(formatY, culture);
                            formatY = $"0{culture.NumberFormat.NumberDecimalSeparator}{new string('0', fractions)}" + literalPart;
                        }
                    }
                    maxStringY = customValuesY.Any()
                       ? customValuesY.FirstOrDefault(c => c.Length == customValuesY.Max(v => v.Length))
                       : max.ToString(culture).Length >= min.ToString(culture).Length ? max.ToString(formatY, culture) : min.ToString(formatY, culture);
                }
                else
                {
                    maxStringY = "100%";
                }

                // get first horiizontal values formatted text object 
                maxStringX = customValuesX.Any()
                    ? customValuesX.First()
                    : "0";
                fmtX = new FormattedText(maxStringX, culture, FlowDirection.LeftToRight,
                            new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip)
                {
                    TextAlignment = TextAlignment.Right
                };
                var pt = new Point(0, 0);
                var ngm = fmtX.BuildGeometry(pt);
                var trgr = new TransformGroup();
                trgr.Children.Add(new RotateTransform(-45, pt.X + fmtX.Width, pt.Y));
                ngm.Transform = trgr;

                maxString = maxStringY;
            }
            else
            {
                if (customValuesX.Any())
                {
                    var cv = customValuesX.FirstOrDefault(c => c.Length == customValuesX.Max(v => v.Length));
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
            if (fmtX != null)
                width = fmt.Width > fmtX.Width ? fmt.Width : fmtX.Width;
            else
                width = fmt.Width;
            //add some extra space
            return width + 4;
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
