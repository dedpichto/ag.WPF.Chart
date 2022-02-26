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
    /// Prepares geometry for drawing numeric/custom values drawn next to y-axis
    /// </summary>
    public class VerticalValuesConverter : IMultiValueConverter
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
                || values[2] is not ChartStyle chartStyle)
                return null;

            var seriesEnumerable = values[1] as IEnumerable<ISeries>;
            var chartSeries = values[16] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return null;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return null;
            }
            else if (seriesArray.All(s => s.IsStockSeries()))
            {
                if (!chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return null;

                if (!seriesArray[0].Values.Any())
                    return null;
                if (chartStyle == ChartStyle.HighLowClose && seriesArray[0] is not HighLowCloseSeries)
                    return null;
                else if (chartStyle == ChartStyle.OpenHighLowClose && seriesArray[0] is not OpenHighLowCloseSeries)
                    return null;
            }
            else
                return null;

            if (chartStyle.In(ChartStyle.SlicedPie, ChartStyle.SolidPie, ChartStyle.Doughnut, ChartStyle.Radar, ChartStyle.RadarWithMarkers, ChartStyle.RadarArea))
                return null;
            else if (chartStyle == ChartStyle.Funnel)
                return drawVerticalValuesForFunnel(values, culture);
            else if (Utils.StyleBars(chartStyle))
                return drawVerticalValuesForBars(values, culture);
            return drawVerticalValues(values, culture);
        }

        private PathGeometry drawVerticalValuesForFunnel(object[] values, CultureInfo culture)
        {
            if (values == null
                || values[0] is not double height
                || values[5] is not FontFamily fontFamily
                || values[6] is not double fontSize
                || values[7] is not FontStyle fontStyle
                || values[8] is not FontWeight fontWeight
                || values[9] is not FontStretch fontStretch
                || values[14] is not FlowDirection flowDir)
                return null;

            var gm = new PathGeometry();

            var seriesEnumerable = values[1] as IEnumerable<ISeries>;
            var chartSeries = values[16] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return null;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            var ticks = seriesArray[0].Values.Count;

            height -= Utils.AXIS_THICKNESS;

            var yStep = height / ticks;

            var y = height - Utils.AXIS_THICKNESS;

            for (int i = ticks; i > 0; i--)
            {
                var fmt = new FormattedText(i.ToString(culture), culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip)
                {
                    TextAlignment = flowDir == FlowDirection.LeftToRight
                        ? TextAlignment.Right
                        : TextAlignment.Left
                };

                var pt = new Point(0, y - (yStep + fmt.Height) / 2);
                var ngm = fmt.BuildGeometry(pt);

                if (flowDir == FlowDirection.RightToLeft)
                    ngm.Transform = new ScaleTransform { ScaleX = -1 };
                gm.AddGeometry(ngm);
                y -= yStep;
            }

            return gm;
        }

        private PathGeometry drawVerticalValuesForBars(object[] values, CultureInfo culture)
        {
            if (values == null
                || values[0] is not double height
                || values[2] is not ChartStyle chartStyle
                || values[3] is not int linesCount
                || values[15] is not string formatX
                || values[5] is not FontFamily fontFamily
                || values[6] is not double fontSize
                || values[7] is not FontStyle fontStyle
                || values[8] is not FontWeight fontWeight
                || values[9] is not FontStretch fontStretch
                || values[12] is not AutoAdjustmentMode autoAdjust
                || values[14] is not FlowDirection flowDir)
                return null;

            var seriesEnumerable = values[1] as IEnumerable<ISeries>;
            var chartSeries = values[16] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return null;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            var customValuesX = values[10] is IEnumerable<string> customEnumerableX ? customEnumerableX.ToArray() : Array.Empty<string>();

            var gm = new PathGeometry();

            height -= Utils.AXIS_THICKNESS;

            var rawValues = Utils.GetPaddedSeries(seriesArray);

            var ticks = rawValues.Max(rw => rw.Values.Count);

            var totalValues = seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue));

            var dir = Utils.GetDirection(totalValues, chartStyle);

            if (!autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Vertical))
            {
                ticks = linesCount;
            }
            var yStep = height / ticks;

            var y = height - Utils.AXIS_THICKNESS;

            switch (dir)
            {
                case Directions.NorthEast:
                case Directions.NorthEastNorthWest:
                case Directions.NorthWest:
                    {
                        for (int i = 0, j = ticks - 1; i < ticks; i++, j--)
                        {
                            var num = i + 1;
                            var number = customValuesX.Length > i
                                ? customValuesX[i]
                                : formatX.EndsWith("%") ? num.ToString(formatX.Substring(0, formatX.Length - 1)) + "%" : num.ToString(formatX);

                            var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip)
                            {
                                TextAlignment = flowDir == FlowDirection.LeftToRight
                                    ? dir == Directions.NorthWest ? TextAlignment.Left : TextAlignment.Right
                                    : dir == Directions.NorthWest ? TextAlignment.Right : TextAlignment.Left
                            };

                            var pt = new Point(0, y - (yStep + fmt.Height) / 2);
                            var ngm = fmt.BuildGeometry(pt);

                            if (flowDir == FlowDirection.RightToLeft)
                                ngm.Transform = new ScaleTransform { ScaleX = -1 };
                            gm.AddGeometry(ngm);
                            y -= yStep;
                        }
                        break;
                    }
                default:
                    return null;
            }
            return gm;
        }

        private PathGeometry drawVerticalValues(object[] values, CultureInfo culture)
        {
            if (values == null
                || values[0] is not double height
                || values[2] is not ChartStyle chartStyle
                || values[3] is not int linesCountY
                || values[4] is not string formatY
                || values[5] is not FontFamily fontFamily
                || values[6] is not double fontSize
                || values[7] is not FontStyle fontStyle
                || values[8] is not FontWeight fontWeight
                || values[9] is not FontStretch fontStretch
                || values[12] is not AutoAdjustmentMode autoAdjust
                || values[13] is not double maxY
                || values[14] is not FlowDirection flowDir)
                return null;

            var gm = new PathGeometry();

            var seriesEnumerable = values[1] as IEnumerable<ISeries>;
            var chartSeries = values[16] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return null;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            var customValuesY = values[11] is IEnumerable<string> customEnumerableY ? customEnumerableY.ToArray() : Array.Empty<string>();
            var drawBetween = chartStyle.In(ChartStyle.Bars, ChartStyle.StackedBars, ChartStyle.FullStackedBars);

            double step, offset;

            Directions dir;

            if (seriesArray.All(s => s is PlainSeries))
            {
                var totalValues = chartStyle != ChartStyle.Waterfall
                    ? seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue))
                    : seriesArray[0].Values.Select(v => v.CompositeValue.PlainValue);
                dir = Utils.GetDirection(totalValues, chartStyle);
            }
            else if (seriesArray.All(s => s.IsStockSeries()))
            {
                var totalValues = seriesArray[0].Values.Select(v => (v.CompositeValue.HighValue, v.CompositeValue.LowValue));
                dir = Utils.GetDirectionFinancial(totalValues);
            }
            else
                return null;

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

            var fmtY = new FormattedText("AAA", culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);

            var (max, min, realLinesCount, stepSize, stepLength, units, _) = autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Vertical)
                ? Utils.GetMeasures(
                   chartStyle,
                   seriesArray,
                   linesCountY,
                   radius,
                   fmtY.Height,
                   centerPoint,
                   dir == Directions.NorthEastSouthEast)
                : (maxY, -maxY, linesCountY, maxY / linesCountY, radius / linesCountY, radius / maxY, default);
            if (Utils.StyleFullStacked(chartStyle))
            {
                linesCountY = 10;
                stepLength = dir == Directions.NorthEastSouthEast ? height / 20 : height / 10;
            }
            else
            {
                linesCountY = realLinesCount;
            }

            var maxMin = Math.Abs(Math.Max(max, Math.Abs(min)));

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

            switch (dir)
            {
                case Directions.NorthEastSouthEast:
                    step = stepLength;
                    var limit = linesCountY * 2;
                    for (int i = 0, index = 0; i <= limit; i++)
                    {
                        if (i == linesCountY) continue;
                        var num = !Utils.StyleFullStacked(chartStyle)
                            ? (linesCountY - i) * maxMin / linesCountY
                            : (linesCountY - i) * 10;
                        var number = Utils.StyleFullStacked(chartStyle)
                            ? num.ToString(culture)
                            : customValuesY.Length > index
                                ? customValuesY[index++]
                                : formatY.EndsWith("%")
                                    ? num.ToString(formatY.Substring(0, formatY.Length - 1)) + "%"
                                    : num.ToString(formatY);
                        if (Utils.StyleFullStacked(chartStyle))
                            number += "%";
                        var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                            new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                        if (flowDir == FlowDirection.LeftToRight)
                            fmt.TextAlignment = TextAlignment.Right;
                        offset = i < linesCountY
                            ? (drawBetween ? step / 2 - fmt.Height / 2 : 0)
                            : (drawBetween ? -step / 2 - fmt.Height / 2 : -fmt.Height / 2);

                        var pt = new Point(0, i * step + offset);
                        var ngm = fmt.BuildGeometry(pt);
                        if (flowDir == FlowDirection.RightToLeft)
                            ngm.Transform = new ScaleTransform { ScaleX = -1 };
                        gm.AddGeometry(ngm);
                    }
                    break;
                case Directions.SouthEast:
                    step = stepLength;
                    for (int i = 1, index = 0; i <= linesCountY; i++)
                    {
                        var num = (!Utils.StyleFullStacked(chartStyle))
                            ? i * min / linesCountY
                            : i * 10 / linesCountY;
                        var number = Utils.StyleFullStacked(chartStyle) ? num.ToString(culture) : customValuesY.Length > index
                            ? customValuesY[index++]
                            : formatY.EndsWith("%") ? num.ToString(formatY.Substring(0, formatY.Length - 1)) + "%" : num.ToString(formatY);
                        if (Utils.StyleFullStacked(chartStyle))
                            number += "%";
                        var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                            new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                        if (flowDir == FlowDirection.LeftToRight)
                            fmt.TextAlignment = TextAlignment.Right;
                        offset = drawBetween ? -step / 2 - fmt.Height / 2 : -fmt.Height / 2;
                        var pt = new Point(0, i * step + offset);
                        var ngm = fmt.BuildGeometry(pt);
                        if (flowDir == FlowDirection.RightToLeft)
                            ngm.Transform = new ScaleTransform { ScaleX = -1 };
                        gm.AddGeometry(ngm);
                    }
                    break;
                case Directions.NorthEast:
                case Directions.NorthEastNorthWest:
                case Directions.NorthWest:
                    step = stepLength;
                    for (int i = 0, index = 0; i < linesCountY; i++)
                    {
                        var num = (!Utils.StyleFullStacked(chartStyle))
                            ? maxMin - stepSize * i
                            : (linesCountY - i) * 10;
                        var number = Utils.StyleFullStacked(chartStyle) ? num.ToString(culture) : customValuesY.Length > index
                            ? customValuesY[index++]
                            : formatY.EndsWith("%") ? num.ToString(formatY.Substring(0, formatY.Length - 1)) + "%" : num.ToString(formatY);
                        if (Utils.StyleFullStacked(chartStyle))
                            number += "%";
                        var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                            new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                        if (dir.In(Directions.NorthEast, Directions.NorthEastNorthWest) && flowDir == FlowDirection.LeftToRight)
                            fmt.TextAlignment = TextAlignment.Right;
                        offset = drawBetween ? step / 2 - fmt.Height / 2 : 0;
                        var pt = new Point(0, i * step + offset);
                        var ngm = fmt.BuildGeometry(pt);
                        if (flowDir == FlowDirection.RightToLeft)
                            ngm.Transform = new ScaleTransform { ScaleX = -1 };
                        gm.AddGeometry(ngm);
                    }
                    break;
            }
            return gm;
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
