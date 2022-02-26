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
    /// Prepares geometry for drawing vertical dotted lines
    /// </summary>
    public class VerticalLinesPathConverter : IMultiValueConverter
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
                || values[1] is not double height
                || values[3] is not ChartStyle chartStyle
                || chartStyle == ChartStyle.Funnel
                || values[4] is not int linesCount
                || values[5] is not ChartBoundary chartBoundary
                || values[6] is not FontFamily fontFamily
                || values[7] is not double fontSize
                || values[8] is not FontStyle fontStyle
                || values[9] is not FontWeight fontWeight
                || values[10] is not FontStretch fontStretch
                || values[11] is not AutoAdjustmentMode autoAdjust
                || values[12] is not double maxX)
                return null;

            if (chartStyle.In(ChartStyle.Area, ChartStyle.StackedArea, ChartStyle.SmoothStackedArea, ChartStyle.FullStackedArea, ChartStyle.SmoothFullStackedArea, ChartStyle.SmoothArea))
                return null;

            var seriesEnumerable = values[2] as IEnumerable<ISeries>;
            var chartSeries = values[13] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return null;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            var offsetBoundary = Utils.OffsetBoundary(chartBoundary, chartStyle);

            Directions dir;
            int ticks;

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return null;
                var totalValues = chartStyle.In(ChartStyle.Waterfall)
                    ? seriesArray.First().Values.Select(v => v.CompositeValue.PlainValue)
                    : seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue));
                dir = Utils.GetDirection(totalValues, chartStyle);
                ticks = seriesArray.Select(s => s.Values.Count).Max();
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
                ticks = seriesArray[0].Values.Count;
            }
            else
                return null;

            double xStep;

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
            height -= 2 * Utils.AXIS_THICKNESS;

            var boundOffset = Utils.BoundaryOffset(offsetBoundary, width, ticks);
            int limit;
            var gm = new PathGeometry();

            var fmtY = new FormattedText("AAA", culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
            var stepLength = 0.0;
            if (Utils.StyleBars(chartStyle))
            {
                var (max, min, realLinesCount, stepSize, stepL, units, zeroPoint) = autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Horizontal)
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
                if (chartStyle == ChartStyle.FullStackedBars)
                {
                    linesCount = 10;
                    stepLength = dir == Directions.NorthEastNorthWest ? width / 20 : width / 10;
                }
                else
                {
                    linesCount = realLinesCount;
                }
            }

            switch (dir)
            {
                case Directions.NorthEast:
                    {
                        width -= boundOffset;
                        var x = Utils.AXIS_THICKNESS + boundOffset;
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
                                limit = linesCount;
                            }
                        }
                        else
                        {
                            xStep = stepLength;
                            limit = linesCount;
                        }
                        for (var i = 0; i <= limit; i++)
                        {
                            var start = new Point(x, 0);
                            var end = new Point(start.X, height);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                            x += xStep;
                        }
                        break;
                    }
                case Directions.NorthEastNorthWest:
                    {
                        if (!Utils.StyleBars(chartStyle))
                            return null;
                        xStep = stepLength;
                        var x = Utils.AXIS_THICKNESS;
                        for (var i = 0; i <= linesCount * 2 + 1; i++)
                        {
                            if (i == linesCount) continue;
                            var start = new Point(x, 0);
                            var end = new Point(start.X, height);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                            x += xStep;
                        }
                        break;
                    }
                case Directions.NorthEastSouthEast:
                    {
                        var x = Utils.AXIS_THICKNESS + boundOffset;
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
                                limit = linesCount;
                            }
                        }
                        else
                        {
                            xStep = stepLength;
                            limit = linesCount;
                        }
                        for (var i = 0; i <= limit; i++)
                        {
                            var start = new Point(x, 0);
                            var end = new Point(start.X, height);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                            x += xStep;
                        }
                        break;
                    }
                case Directions.NorthWest:
                    {
                        if (!Utils.StyleBars(chartStyle))
                            return null;
                        xStep = stepLength;
                        var x = Utils.AXIS_THICKNESS;
                        for (var i = 0; i < linesCount; i++)
                        {
                            var start = new Point(x, 0);
                            var end = new Point(start.X, height);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                            x += xStep;
                        }
                        break;
                    }
                case Directions.SouthEast:
                    {
                        var x = Utils.AXIS_THICKNESS + boundOffset;
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
                                limit = linesCount + 1;
                            }
                        }
                        else
                        {
                            xStep = stepLength;
                            limit = linesCount;
                        }
                        for (var i = 0; i < limit; i++)
                        {
                            var start = new Point(x, 0);
                            var end = new Point(start.X, height);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                            x += xStep;
                        }
                        break;
                    }
                default:
                    return null;
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
