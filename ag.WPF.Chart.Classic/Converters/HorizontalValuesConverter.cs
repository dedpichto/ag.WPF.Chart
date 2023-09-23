using ag.WPF.Chart.Series;
using ag.WPF.Chart.Values;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ag.WPF.Chart.Converters
{
#nullable disable
    /// <summary>
    /// Prepares geometry for drawing numeric/custom values drawn next to x-axis
    /// </summary>
    public class HorizontalValuesConverter : IMultiValueConverter
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
                || values[0] is not double width
                || values[2] is not ChartStyle chartStyle
                || chartStyle == ChartStyle.Funnel
                || values[3] is not int linesCount
                || values[4] is not string formatX
                || values[5] is not FontFamily fontFamily
                || values[6] is not double fontSize
                || values[7] is not FontStyle fontStyle
                || values[8] is not FontWeight fontWeight
                || values[9] is not FontStretch fontStretch
                || values[11] is not AutoAdjustmentMode autoAdjust
                || values[12] is not double maxX
                || values[13] is not FlowDirection flowDir
                || values[14] is not ChartBoundary chartBoundary
                || values[16] is not string formatY)
                return null;

            if (chartStyle.In(ChartStyle.SlicedPie, ChartStyle.SolidPie, ChartStyle.Doughnut, ChartStyle.Radar, ChartStyle.RadarWithMarkers, ChartStyle.RadarArea))
                return null;

            var seriesEnumerable = values[1] as IEnumerable<ISeries>;
            var chartSeries = values[17] as IEnumerable<ISeries>;

            if (seriesEnumerable != null && seriesEnumerable.Any())
                seriesEnumerable = seriesEnumerable.Where(s => s.IsVisible);
            if (chartSeries != null && chartSeries.Any())
                chartSeries = chartSeries.Where(s => s.IsVisible);
            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return null;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            var drawBetween = Utils.StyleColumns(chartStyle);
            var customValuesX = values[10] is IEnumerable<string> customEnumerableX ? customEnumerableX.ToArray() : Array.Empty<string>();
            var customValuesY = values[15] is IEnumerable<string> customEnumerableY ? customEnumerableY.ToArray() : Array.Empty<string>();

            double xStep;

            var gm = new PathGeometry();

            Directions dir;
            int ticks;

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return null;
                var totalValues = chartStyle != ChartStyle.Waterfall
                    ? seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue))
                    : seriesArray[0].Values.Select(v => v.CompositeValue.PlainValue);
                dir = Utils.GetDirection(totalValues, chartStyle);
                var rawValues = chartStyle != ChartStyle.Waterfall
                    ? Utils.GetPaddedSeries(seriesArray)
                    : new List<(List<IChartValue> Values, int Index)> { (seriesArray[0].Values.ToList(), seriesArray[0].Index) };
                ticks = rawValues.Max(rw => rw.Values.Count);
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

                var totalValues = seriesArray[0].Values.Select(v => (v.CompositeValue.HighValue, v.CompositeValue.LowValue));
                dir = Utils.GetDirectionFinancial(totalValues);
                var rawValues = Utils.GetPaddedSeriesFinancial(seriesArray[0]);
                ticks = rawValues.Max(rw => rw.Values.Count);
            }
            else
                return null;

            var radius = 0.0;
            var centerX = 0.0;
            Point centerPoint = default;

            if (Utils.StyleBars(chartStyle))
            {
                switch (dir)
                {
                    case Directions.NorthEast:
                    case Directions.NorthWest:
                    case Directions.SouthEast:
                        centerX = Utils.AXIS_THICKNESS;
                        radius = width - Utils.AXIS_THICKNESS;
                        break;
                    case Directions.NorthEastNorthWest:
                        centerX = Utils.AXIS_THICKNESS;
                        radius = (width - Utils.AXIS_THICKNESS) / 2;
                        break;
                    case Directions.NorthEastSouthEast:
                        centerX = Utils.AXIS_THICKNESS;
                        radius = width - Utils.AXIS_THICKNESS;
                        break;
                }
                centerPoint = new Point(centerX, radius);
            }

            width -= 2 * Utils.AXIS_THICKNESS;
            var stepLength = 0.0;
            var stepSize = 0.0;
            var maxMin = 0.0;


            if (Utils.StyleBars(chartStyle))
            {
                var fmtY = new FormattedText("AAA", culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);

                var (max, min, realLinesCount, stepS, stepL, units, zeroPoint) = autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Horizontal)
                    ? Utils.GetMeasures(
                       chartStyle,
                       seriesArray,
                       linesCount,
                       radius,
                       fmtY.Height,
                       centerPoint,
                       dir == Directions.NorthEastNorthWest)
                    : (maxX, -maxX, linesCount, maxX / linesCount, radius / linesCount, radius / maxX, default);

                stepLength = stepL;
                stepSize = stepS;
                maxMin = Math.Abs(Math.Max(max, Math.Abs(min)));

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

                if (chartStyle == ChartStyle.FullStackedBars)
                {
                    linesCount = 10;
                    stepLength = dir == Directions.NorthEastSouthEast ? width / 20 : width / 10;
                }
                else
                {
                    linesCount = realLinesCount;
                }
            }

            var offsetBoundary = Utils.OffsetBoundary(chartBoundary, chartStyle);
            var boundOffset = Utils.BoundaryOffset(offsetBoundary, width, ticks);

            int limit;

            switch (dir)
            {
                case Directions.NorthEast:
                case Directions.NorthEastSouthEast:
                    {
                        if (dir == Directions.NorthEast)
                            width -= boundOffset;
                        if (!Utils.StyleBars(chartStyle))
                        {
                            if (autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Horizontal))
                            {
                                var (Step, Limit) = Utils.Limits(chartStyle, offsetBoundary, linesCount, ticks, boundOffset, width);
                                xStep = Step;
                                limit = Limit;
                            }
                            else
                            {
                                xStep = width / linesCount;
                                limit = drawBetween ? linesCount : linesCount + 1;
                            }
                        }
                        else
                        {
                            xStep = stepLength;
                            limit = linesCount;
                        }

                        for (int i = 0, j = limit - 1; i < limit; i++, j--)
                        {
                            var indexStep = flowDir == FlowDirection.LeftToRight ? i + 1 : i;
                            var num = !Utils.StyleBars(chartStyle)
                                ? Utils.StyleColumns(chartStyle) || chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose)
                                    ? flowDir == FlowDirection.LeftToRight
                                        ? i + 1
                                        : j + 1
                                    : flowDir == FlowDirection.LeftToRight
                                        ? i
                                        : j
                                : Utils.StyleMeasuredBars(chartStyle)
                                    ? flowDir == FlowDirection.LeftToRight
                                        ? maxMin - stepSize * j
                                        : maxMin - stepSize * i
                                    : flowDir == FlowDirection.LeftToRight
                                        ? 10 * (i + 1)
                                        : 10 * (j + 1);
                            var index = flowDir == FlowDirection.LeftToRight ? i : j;
                            var number = chartStyle == ChartStyle.FullStackedBars
                                ? num.ToString(culture)
                                : customValuesX.Length > index && !Utils.StyleBars(chartStyle)
                                    ? customValuesX[index]
                                    : customValuesY.Length > index && Utils.StyleBars(chartStyle)
                                        ? customValuesY[index]
                                        : !Utils.StyleBars(chartStyle)
                                            ? formatX.EndsWith("%") ? num.ToString(formatX.Substring(0, formatX.Length - 1)) + "%" : num.ToString(formatX)
                                            : formatY.EndsWith("%") ? num.ToString(formatY.Substring(0, formatY.Length - 1)) + "%" : num.ToString(formatY);
                            if (chartStyle == ChartStyle.FullStackedBars)
                                number += "%";
                            var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                            double x;
                            if (Utils.StyleBars(chartStyle))
                            {
                                x = flowDir == FlowDirection.LeftToRight
                                    ? drawBetween
                                        ? boundOffset + indexStep * xStep - fmt.Width + xStep / 2
                                        : boundOffset + indexStep * xStep - fmt.Width
                                    : drawBetween ? boundOffset + indexStep * xStep + xStep / 2 : boundOffset + indexStep * xStep;
                            }
                            else
                            {
                                x = flowDir == FlowDirection.LeftToRight
                                    ? drawBetween
                                        ? boundOffset + i * xStep - fmt.Width + xStep / 2
                                        : boundOffset + i * xStep - fmt.Width
                                    : drawBetween
                                        ? boundOffset + indexStep * xStep + xStep / 2
                                        : boundOffset + indexStep * xStep;
                            }
                            var pt = new Point(x, 0);
                            var ngm = fmt.BuildGeometry(pt);

                            var trgr = new TransformGroup();
                            trgr.Children.Add(flowDir == FlowDirection.LeftToRight
                                ? new RotateTransform(-45, pt.X + fmt.Width, pt.Y)
                                : new RotateTransform(45, pt.X, pt.Y));
                            if (flowDir == FlowDirection.RightToLeft)
                                trgr.Children.Add(new ScaleTransform { ScaleX = -1, CenterX = width / 2 });
                            ngm.Transform = trgr;
                            gm.AddGeometry(ngm);
                        }
                        break;
                    }
                case Directions.NorthEastNorthWest:
                    {
                        if (!Utils.StyleBars(chartStyle))
                            return null;

                        xStep = Utils.StyleMeasuredBars(chartStyle) ? stepLength : width / 20;
                        for (int i = 0, index = 0; i <= linesCount * 2; i++)
                        {
                            if (i == linesCount) continue;

                            var num = Utils.StyleMeasuredBars(chartStyle)
                                    ? -(linesCount - i) * maxMin / linesCount
                                    : -(linesCount - i) * 10;
                            if (flowDir == FlowDirection.RightToLeft)
                                num *= -1;
                            var number = chartStyle == ChartStyle.FullStackedBars ? num.ToString(culture) : customValuesY.Length > index
                                ? customValuesY[index++]
                                : formatY.EndsWith("%") ? num.ToString(formatY.Substring(0, formatY.Length - 1)) + "%" : num.ToString(formatY);

                            if (chartStyle == ChartStyle.FullStackedBars)
                                number += "%";

                            var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                            var x = i < linesCount
                                ? i * xStep + 1 + fmt.Width / number.Length
                                : flowDir == FlowDirection.LeftToRight ? i * xStep - fmt.Width : i * xStep - fmt.Width - fmt.Width / number.Length;
                            var pt = new Point(x, 0);
                            var ngm = fmt.BuildGeometry(pt);

                            var trgr = new TransformGroup();
                            if (i < linesCount)
                                trgr.Children.Add(new RotateTransform(45, pt.X, pt.Y));
                            else if (i > linesCount)
                                trgr.Children.Add(new RotateTransform(-45, pt.X + fmt.Width, pt.Y));

                            if (flowDir == FlowDirection.RightToLeft)
                                trgr.Children.Add(new ScaleTransform { ScaleX = -1, CenterX = width / 2 });
                            ngm.Transform = trgr;
                            gm.AddGeometry(ngm);
                        }
                        break;
                    }
                case Directions.SouthEast:
                    {
                        width -= boundOffset;
                        if (autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Horizontal))
                        {
                            var (Step, Limit) = Utils.Limits(chartStyle, offsetBoundary, linesCount, ticks, boundOffset, width);
                            xStep = Step;
                            limit = Limit;
                        }
                        else
                        {
                            xStep = width / linesCount;
                            limit = drawBetween ? linesCount : linesCount + 1;
                        }
                        for (int i = 0, j = limit - 1; i < limit; i++, j--)
                        {
                            var num = Utils.StyleColumns(chartStyle) || chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose)
                                ? flowDir == FlowDirection.LeftToRight
                                    ? i + 1
                                    : j + 1
                                : flowDir == FlowDirection.LeftToRight
                                    ? i
                                    : j;
                            var index = flowDir == FlowDirection.LeftToRight ? i : j;
                            var number = chartStyle == ChartStyle.FullStackedBars
                                ? num.ToString(culture)
                                : customValuesX.Length > index && !Utils.StyleBars(chartStyle)
                                    ? customValuesX[index]
                                    : customValuesY.Length > index && Utils.StyleBars(chartStyle)
                                        ? customValuesY[index]
                                        : !Utils.StyleBars(chartStyle)
                                            ? formatX.EndsWith("%") ? num.ToString(formatX.Substring(0, formatX.Length - 1)) + "%" : num.ToString(formatX)
                                            : formatY.EndsWith("%") ? num.ToString(formatY.Substring(0, formatY.Length - 1)) + "%" : num.ToString(formatY);
                            if (chartStyle == ChartStyle.FullStackedBars)
                                number += "%";
                            var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                            var x = flowDir == FlowDirection.LeftToRight
                                ? drawBetween
                                    ? boundOffset + i * xStep - fmt.Width + xStep / 2
                                    : boundOffset + i * xStep - fmt.Width
                                : drawBetween ? boundOffset + i * xStep + xStep / 2 : boundOffset + i * xStep;
                            var pt = new Point(x, 0);
                            var ngm = fmt.BuildGeometry(pt);

                            var trgr = new TransformGroup();
                            trgr.Children.Add(flowDir == FlowDirection.LeftToRight
                                ? new RotateTransform(45, pt.X + fmt.Width, pt.Y + fmt.Height)
                                : new RotateTransform(-45, pt.X, pt.Y + fmt.Height));
                            if (flowDir == FlowDirection.RightToLeft)
                                trgr.Children.Add(new ScaleTransform { ScaleX = -1, CenterX = width / 2 });
                            ngm.Transform = trgr;
                            gm.AddGeometry(ngm);
                        }
                        break;
                    }
                case Directions.NorthWest:
                    {
                        if (!Utils.StyleBars(chartStyle))
                            return null;
                        xStep = Utils.StyleMeasuredBars(chartStyle) ? stepLength : width / 10;
                        for (int i = 0, j = linesCount; i < linesCount; i++, j--)
                        {
                            var indexStep = flowDir == FlowDirection.LeftToRight ? i : i + 1;
                            var index = flowDir == FlowDirection.LeftToRight ? j : i;
                            var num = Utils.StyleMeasuredBars(chartStyle)
                                    ? flowDir == FlowDirection.LeftToRight ? -j * maxMin / linesCount : -(i + 1) * maxMin / linesCount
                                    : flowDir == FlowDirection.LeftToRight ? -j * 10 : -indexStep * 10;
                            var number = chartStyle == ChartStyle.FullStackedBars ? num.ToString(culture) : customValuesY.Length > index
                                ? customValuesY[index]
                                : formatY.EndsWith("%") ? num.ToString(formatY.Substring(0, formatY.Length - 1)) + "%" : num.ToString(formatY);
                            if (chartStyle == ChartStyle.FullStackedBars)
                                number += "%";
                            var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                            var x = flowDir == FlowDirection.LeftToRight
                                ? indexStep * xStep
                                : indexStep * xStep - fmt.Width;
                            var y = fmt.Height / 2;
                            var pt = new Point(x, y);
                            var ngm = fmt.BuildGeometry(pt);

                            var trgr = new TransformGroup();
                            if (flowDir == FlowDirection.LeftToRight && i != linesCount)
                                trgr.Children.Add(new RotateTransform(45, pt.X, pt.Y + fmt.Height));
                            else if (flowDir == FlowDirection.RightToLeft)
                                trgr.Children.Add(new RotateTransform(-45, pt.X + fmt.Width, pt.Y + fmt.Height));
                            if (flowDir == FlowDirection.RightToLeft)
                                trgr.Children.Add(new ScaleTransform { ScaleX = -1, CenterX = width / 2 });
                            ngm.Transform = trgr;
                            gm.AddGeometry(ngm);
                        }
                        break;
                    }
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
