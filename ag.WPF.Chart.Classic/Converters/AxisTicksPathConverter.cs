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
    /// Prepares geometry for drawing ticks on x- and y- axes
    /// </summary>
    public class AxisTicksPathConverter : IMultiValueConverter
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
                || values[2] is not int linesCountX
                || values[3] is not int linesCountY
                || values[5] is not ChartStyle chartStyle
                || values[6] is not ChartBoundary chartBoundary
                || values[7] is not FontFamily fontFamily
                || values[8] is not double fontSize
                || values[9] is not FontStyle fontStyle
                || values[10] is not FontWeight fontWeight
                || values[11] is not FontStretch fontStretch
                || values[12] is not AutoAdjustmentMode autoAdjust
                || values[13] is not double maxX
                || values[14] is not double maxY
                || values[15] is not AxesVisibility axesVisibility
                || values[16] is not AxesVisibility showTicks)
                return null;

            if (chartStyle.In(ChartStyle.Area, ChartStyle.StackedArea, ChartStyle.SmoothStackedArea, ChartStyle.FullStackedArea, ChartStyle.SmoothFullStackedArea, ChartStyle.Radar, ChartStyle.RadarWithMarkers, ChartStyle.RadarArea, ChartStyle.SmoothArea))
                return null;

            const double size = 4.0;

            double xStep, yStep;

            var seriesEnumerable = values[4] as IEnumerable<ISeries>;
            var chartSeries = values[17] as IEnumerable<ISeries>;

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
                    ? seriesArray[0].Values.Select(v => v.CompositeValue.PlainValue)
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

            var radius = 0.0;
            var centerX = 0.0;
            Point centerPoint;
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
            }
            else
            {
                switch (dir)
                {
                    case Directions.NorthEast:
                    case Directions.NorthWest:
                    case Directions.SouthEast:
                        centerX = Utils.AXIS_THICKNESS;
                        radius = height - Utils.AXIS_THICKNESS;
                        break;
                    case Directions.NorthEastNorthWest:
                        centerX = width / 2;
                        radius = height - Utils.AXIS_THICKNESS;
                        break;
                    case Directions.NorthEastSouthEast:
                        centerX = Utils.AXIS_THICKNESS;
                        radius = (height - Utils.AXIS_THICKNESS) / 2;
                        break;
                }
            }

            centerPoint = new Point(centerX, radius);

            height -= 2 * Utils.AXIS_THICKNESS;
            width -= 2 * Utils.AXIS_THICKNESS;

            var fmtY = new FormattedText("AAA", culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);

            var (max, min, realLinesCount, stepSize, stepLength, units, zeroPoint) = chartStyle != ChartStyle.Funnel
                ? (autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Horizontal) && Utils.StyleBars(chartStyle)) || autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Vertical)
                    ? Utils.GetMeasures(
                       chartStyle,
                       seriesArray,
                       Utils.StyleBars(chartStyle) ? linesCountX : linesCountY,
                       radius,
                       fmtY.Height,
                       centerPoint,
                       !Utils.StyleBars(chartStyle) ? dir == Directions.NorthEastSouthEast : dir == Directions.NorthEastNorthWest)
                    : (maxY, -maxY, linesCountY, maxY / linesCountY, radius / linesCountY, radius / maxY, default)
                : (seriesArray[0].Values.Count, -maxY, seriesArray[0].Values.Count, maxY / seriesArray[0].Values.Count, radius / seriesArray[0].Values.Count, radius / seriesArray[0].Values.Count, default);

            if (Utils.StyleBars(chartStyle) && !autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Horizontal))
            {
                (max, min, realLinesCount, stepSize, stepLength, units, zeroPoint) = (maxX, -maxX, linesCountX, maxX / linesCountX, radius / linesCountX, radius / maxX, default);
            }

            if (!Utils.StyleBars(chartStyle))
            {
                if (Utils.StyleFullStacked(chartStyle))
                    linesCountY = 10;
                else
                    linesCountY = realLinesCount;
            }
            else
            {
                if (chartStyle == ChartStyle.FullStackedBars)
                    linesCountX = 10;
                else
                    linesCountX = realLinesCount;
            }

            var boundOffset = Utils.BoundaryOffset(offsetBoundary, width, ticks);

            var gm = new PathGeometry();
            int limit;

            switch (dir)
            {
                case Directions.NorthEast:
                    {
                        width -= boundOffset;
                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Horizontal) && showTicks.In(AxesVisibility.Both, AxesVisibility.Vertical) && chartStyle != ChartStyle.Funnel)
                        {
                            var x = Utils.AXIS_THICKNESS + boundOffset;
                            if (!Utils.StyleBars(chartStyle))
                            {
                                if (autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Horizontal))
                                {
                                    var (Step, Limit) = Utils.Limits(chartStyle, offsetBoundary, linesCountX, ticks, boundOffset, width);
                                    xStep = Step;
                                    limit = Limit;
                                }
                                else
                                {
                                    xStep = width / linesCountX;
                                    limit = linesCountX;
                                }
                            }
                            else
                            {
                                xStep = chartStyle != ChartStyle.FullStackedBars ? stepLength : width / 10;
                                limit = linesCountX;
                            }
                            for (var i = 0; i < limit; i++)
                            {
                                var start = new Point(x, height);
                                var end = new Point(start.X, start.Y - size);
                                gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                                x += xStep;
                            }
                        }

                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Vertical) && showTicks.In(AxesVisibility.Both, AxesVisibility.Horizontal))
                        {
                            if (Utils.StyleBars(chartStyle))
                            {
                                if (autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Vertical))
                                {
                                    yStep = height / ticks;
                                    limit = ticks;
                                }
                                else
                                {
                                    yStep = height / linesCountY;
                                    limit = linesCountY;
                                }
                            }
                            else
                            {
                                yStep = (Utils.StyleFullStacked(chartStyle))
                                    ? height / 10
                                    : stepLength;
                                limit = linesCountY;
                            }

                            var y = Utils.AXIS_THICKNESS;
                            for (var i = 1; i < limit; i++)
                            {
                                y += yStep;
                                var start = new Point(Utils.AXIS_THICKNESS, y);
                                var end = new Point(start.X + size, y);
                                gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                            }
                        }
                        break;
                    }
                case Directions.NorthEastNorthWest:
                    {
                        if (!Utils.StyleBars(chartStyle))
                            return null;
                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Horizontal) && showTicks.In(AxesVisibility.Both, AxesVisibility.Vertical))
                        {
                            xStep = chartStyle != ChartStyle.FullStackedBars ? stepLength : width / 20;
                            var x = Utils.AXIS_THICKNESS;
                            for (var i = 1; i < linesCountX * 2; i++)
                            {
                                x += xStep;
                                if (i == linesCountX) continue;
                                var start = new Point(x, height + size);
                                var end = new Point(start.X, height - size);
                                gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                            }
                        }

                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Vertical) && showTicks.In(AxesVisibility.Both, AxesVisibility.Horizontal))
                        {
                            if (autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Vertical))
                            {
                                yStep = height / ticks;
                                limit = ticks;
                            }
                            else
                            {
                                yStep = height / linesCountY;
                                limit = linesCountY;
                            }
                            var y = Utils.AXIS_THICKNESS;
                            for (var i = 1; i < limit; i++)
                            {
                                y += yStep;
                                var start = new Point(width / 2 - size, y);
                                var end = new Point(width / 2 + size, y);
                                gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                            }
                        }
                        break;
                    }
                case Directions.NorthEastSouthEast:
                    {
                        if (Utils.StyleBars(chartStyle))
                            return null;
                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Horizontal) && showTicks.In(AxesVisibility.Both, AxesVisibility.Vertical))
                        {
                            var x = Utils.AXIS_THICKNESS + boundOffset;
                            if (!Utils.StyleBars(chartStyle))
                            {
                                if (autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Horizontal))
                                {
                                    var (Step, Limit) = Utils.Limits(chartStyle, offsetBoundary, linesCountX, ticks, boundOffset, width);
                                    xStep = Step;
                                    limit = Limit;
                                }
                                else
                                {
                                    xStep = width / linesCountX;
                                    limit = linesCountX;
                                }
                            }
                            else
                            {
                                xStep = chartStyle != ChartStyle.FullStackedBars ? stepLength : width / 10;
                                limit = linesCountX;
                            }
                            for (var i = 0; i < limit; i++)
                            {
                                var start = new Point(x, height / 2 + size);
                                var end = new Point(start.X, height / 2 - size);
                                gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                                x += xStep;
                            }
                        }

                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Vertical) && showTicks.In(AxesVisibility.Both, AxesVisibility.Horizontal))
                        {
                            yStep = (Utils.StyleFullStacked(chartStyle))
                                 ? height / 20
                                 : stepLength;
                            limit = linesCountY * 2;

                            var y = Utils.AXIS_THICKNESS;
                            for (var i = 1; i < limit; i++)
                            {
                                y += yStep;
                                if (i == linesCountY) continue;
                                var start = new Point(size, y);
                                var end = new Point(-size, y);
                                gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                            }
                        }
                        break;
                    }
                case Directions.NorthWest:
                    {
                        if (!Utils.StyleBars(chartStyle))
                            return null;
                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Horizontal) && showTicks.In(AxesVisibility.Both, AxesVisibility.Vertical))
                        {
                            xStep = chartStyle != ChartStyle.FullStackedBars ? stepLength : width / 10;
                            var x = Utils.AXIS_THICKNESS;
                            for (var i = 1; i < linesCountX; i++)
                            {
                                x += xStep;
                                var start = new Point(x, height - size);
                                var end = new Point(start.X, height + size);
                                gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                            }
                        }

                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Vertical) && showTicks.In(AxesVisibility.Both, AxesVisibility.Horizontal))
                        {
                            if (autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Vertical))
                            {
                                yStep = height / ticks;
                                limit = ticks;
                            }
                            else
                            {
                                yStep = height / linesCountY;
                                limit = linesCountY;
                            }
                            var y = Utils.AXIS_THICKNESS;
                            for (var i = 1; i < limit; i++)
                            {
                                y += yStep;
                                var start = new Point(width - size, y);
                                var end = new Point(width + size, y);
                                gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                            }
                        }
                        break;
                    }
                case Directions.SouthEast:
                    {
                        if (Utils.StyleBars(chartStyle))
                            return null;
                        width -= boundOffset;
                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Horizontal) && showTicks.In(AxesVisibility.Both, AxesVisibility.Vertical))
                        {
                            var x = Utils.AXIS_THICKNESS + boundOffset;

                            if (autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Horizontal))
                            {
                                var (Step, Limit) = Utils.Limits(chartStyle, offsetBoundary, linesCountX, ticks, boundOffset, width);
                                xStep = Step;
                                limit = Limit;
                            }
                            else
                            {
                                xStep = width / linesCountX;
                                limit = linesCountX;
                            }

                            for (var i = 0; i < limit; i++)
                            {
                                var start = new Point(x, size);
                                var end = new Point(start.X, -size);
                                gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                                x += xStep;
                            }
                        }

                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Vertical) && showTicks.In(AxesVisibility.Both, AxesVisibility.Horizontal))
                        {
                            yStep = stepLength;
                            limit = linesCountY;

                            var y = Utils.AXIS_THICKNESS;
                            for (var i = 1; i < limit; i++)
                            {
                                y += yStep;
                                var start = new Point(-size, y);
                                var end = new Point(size, y);
                                gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                            }
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
