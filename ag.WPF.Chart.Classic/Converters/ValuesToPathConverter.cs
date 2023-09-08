using ag.WPF.Chart.Series;
using ag.WPF.Chart.Values;
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
    /// Prepares geometry for drawing series
    /// </summary>
    public class ValuesToPathConverter : IMultiValueConverter
    {
        private const double COLUMN_BAR_OFFSET = 4.0;
        private const double RECT_SIZE = 3.0;

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
                || chartStyle.In(ChartStyle.SolidPie, ChartStyle.SlicedPie, ChartStyle.Doughnut)
                || values[4] is not int index
                || chartStyle.In(ChartStyle.Waterfall, ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose) && index > 0
                || values[5] is not AutoAdjustmentMode autoAdjust
                || values[6] is not double maxXConv
                || values[7] is not double maxYConv
                || values[8] is not ChartBoundary chartBoundary
                || values[9] is not FontFamily fontFamily
                || values[10] is not double fontSize
                || values[11] is not FontStyle fontStyle
                || values[12] is not FontWeight fontWeight
                || values[13] is not FontStretch fontStretch
                || values[15] is not int linesCountY
                || values[16] is not int linesCountX
                || values[17] is not bool showValues
                || values[18] is not FlowDirection flowDirection
                || values[19] is not ShapeStyle shapeStyle
                || !(parameter is (int order, ColoredPaths colored)))
                return null;

            var seriesEnumerable = values[2] as IEnumerable<ISeries>;
            var chartSeries = values[20] as IEnumerable<ISeries>;

            if (seriesEnumerable != null && seriesEnumerable.Any())
                seriesEnumerable = seriesEnumerable.Where(s => s.IsVisible);
            if (chartSeries != null && chartSeries.Any())
                chartSeries = chartSeries.Where(s => s.IsVisible);
            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return null;

            if (!chartStyle.In(ChartStyle.Waterfall, ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose) && order > 0)
                return null;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            var offsetBoundary = Utils.OffsetBoundary(chartBoundary, chartStyle);

            int maxX;
            Directions dir;
            List<(List<IChartValue> Values, int Index)> rawValues;

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return null;
                rawValues = Utils.GetPaddedSeries(seriesArray);
                var totalValues = chartStyle.In(ChartStyle.Waterfall)
                    ? seriesArray[0].Values.Select(v => v.CompositeValue.PlainValue).ToArray()
                    : seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue)).ToArray();
                dir = Utils.GetDirection(totalValues, chartStyle);
            }
            else if (seriesArray.All(s => s.IsStockSeries()))
            {
                if (!chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return null;
                if (!seriesArray[0].Values.Any())
                    return null;
                if (chartStyle == ChartStyle.HighLowClose && seriesArray[0] is not HighLowCloseSeries)
                {
                    return null;
                }
                if (chartStyle == ChartStyle.OpenHighLowClose && seriesArray[0] is not OpenHighLowCloseSeries)
                {
                    return null;
                }
                rawValues = Utils.GetPaddedSeriesFinancial(seriesArray[0]);
                var totalValues = seriesArray[0].Values.Select(v => (v.CompositeValue.HighValue, v.CompositeValue.LowValue));
                dir = Utils.GetDirectionFinancial(totalValues);
            }
            else
                return null;

            var customValues = values[14] is IEnumerable<string> customEnumerable ? customEnumerable.ToArray() : Array.Empty<string>();
            var maxPointsCount = chartStyle.In(ChartStyle.Waterfall, ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose)
                ? seriesArray[0].Values.Count
                : seriesArray.Max(s => s.Values.Count);

            var maxCv = customValues.Any() ? customValues.Max(v => (v, v.Length)) : (v: "", Length: 0);
            var number = maxCv.Length > maxPointsCount.ToString(culture).Length ? maxCv.v : maxPointsCount.ToString(culture);
            var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);

            Point centerPoint;
            var boundOffset = chartStyle switch
            {
                ChartStyle.Waterfall => 0.0,
                ChartStyle.HighLowClose => Utils.BoundaryOffset(true, width, maxPointsCount),
                ChartStyle.OpenHighLowClose => Utils.BoundaryOffset(true, width, maxPointsCount),
                _ => Utils.BoundaryOffset(offsetBoundary, width, maxPointsCount)
            };

            width -= boundOffset;
            switch (chartStyle)
            {
                case ChartStyle.OpenHighLowClose:
                    {
                        var units = Utils.GetUnitsForLines(new[] { seriesArray[0] }, chartStyle, dir, width, height, boundOffset, linesCountY, fmt.Height, autoAdjust, maxYConv);
                        return drawOHLC(width, height, seriesArray[0].Values.Count, units, dir, seriesArray[0], colored, boundOffset);
                    }
                case ChartStyle.HighLowClose:
                    {
                        var units = Utils.GetUnitsForLines(new[] { seriesArray[0] }, chartStyle, dir, width, height, boundOffset, linesCountY, fmt.Height, autoAdjust, maxYConv);
                        return drawHLC(width, height, seriesArray[0].Values.Count, units, dir, seriesArray[0], colored, boundOffset);
                    }
                case ChartStyle.Waterfall:
                    {
                        var units = Utils.GetUnitsForLines(new[] { seriesArray[0] }, chartStyle, dir, width, height, boundOffset, linesCountY, fmt.Height, autoAdjust, maxYConv);
                        return drawWaterfall(width, height, dir, seriesArray[0], units, colored, showValues, fontFamily, fontStyle, fontWeight, fontStretch, fontSize, culture, flowDirection);
                    }
                case ChartStyle.Funnel:
                    if (index > 0) return null;
                    return drawFunnel(seriesArray[0], width, height, showValues, fontFamily, fontStyle, fontWeight, fontStretch, fontSize, culture, flowDirection);
                //break;
                case ChartStyle.Radar:
                case ChartStyle.RadarWithMarkers:
                case ChartStyle.RadarArea:
                    {
                        var radius = width > height ? (height - 2 * Utils.AXIS_THICKNESS) / 2 : (width - 2 * Utils.AXIS_THICKNESS) / 2;
                        centerPoint = width > height ? new Point((width - 2 * Utils.AXIS_THICKNESS) / 2, radius) : new Point(radius, (height - 2 * Utils.AXIS_THICKNESS) / 2);
                        if (width > height)
                            radius -= (2 * fmt.Height + 8);
                        else
                            radius -= (2 * fmt.Width + 8);
                        var (max, min, realLinesCount, stepSize, stepLength, units, zeroPoint) = Utils.GetMeasures(chartStyle, seriesArray, linesCountY, radius, fmt.Height, centerPoint);
                        return drawRadar(seriesArray, index, chartStyle, units, stepLength, radius, maxPointsCount, realLinesCount, zeroPoint.Level, centerPoint, shapeStyle);
                    }
                case ChartStyle.Lines:
                case ChartStyle.LinesWithMarkers:
                case ChartStyle.SmoothLines:
                case ChartStyle.SmoothLinesWithMarkers:
                case ChartStyle.Area:
                case ChartStyle.SmoothArea:
                    {
                        var units = Utils.GetUnitsForLines(seriesArray, chartStyle, dir, width, height, boundOffset, linesCountY, fmt.Height, autoAdjust, maxYConv);
                        maxX = seriesArray.Max(s => s.Values.Count);
                        var currentSeries = seriesArray.FirstOrDefault(s => s.Index == index);
                        if (currentSeries == null)
                            return null;
                        return drawLine(width, height, maxX, units, chartStyle, dir, currentSeries, offsetBoundary, boundOffset, shapeStyle);
                    }
                case ChartStyle.Bubbles:
                    {
                        var units = Utils.GetUnitsForLines(seriesArray, chartStyle, dir, width, height, boundOffset, linesCountY, fmt.Height, autoAdjust, maxYConv);
                        maxX = seriesArray.Max(s => s.Values.Count);
                        var currentSeries = seriesArray.FirstOrDefault(s => s.Index == index);
                        if (currentSeries == null)
                            return null;
                        return drawBubbles(width, height, maxX, units, dir, currentSeries, offsetBoundary, boundOffset);
                    }
                case ChartStyle.StackedLines:
                case ChartStyle.StackedLinesWithMarkers:
                case ChartStyle.SmoothStackedLines:
                case ChartStyle.SmoothStackedLinesWithMarkers:
                    {
                        var units = Utils.GetUnitsForLines(seriesArray, chartStyle, dir, width, height, boundOffset, linesCountY, fmt.Height, autoAdjust, maxYConv);
                        maxX = seriesArray.Max(s => s.Values.Count);
                        return drawStackedLine(width, height, maxX, units, chartStyle, dir, seriesArray, index, rawValues, offsetBoundary, boundOffset, shapeStyle);
                    }
                case ChartStyle.StackedArea:
                case ChartStyle.SmoothStackedArea:
                    {
                        var units = Utils.GetUnitsForLines(seriesArray, chartStyle, dir, width, height, boundOffset, linesCountY, fmt.Height, autoAdjust, maxYConv);
                        maxX = seriesArray.Max(s => s.Values.Count);
                        return drawStackedArea(width, height, maxX, units, chartStyle, dir, seriesArray, index, rawValues);
                    }
                case ChartStyle.FullStackedLines:
                case ChartStyle.FullStackedLinesWithMarkers:
                case ChartStyle.SmoothFullStackedLines:
                case ChartStyle.SmoothFullStackedLinesWithMarkers:
                    maxX = seriesArray.Max(s => s.Values.Count);
                    return drawFullStackedLine(width, height, maxX, chartStyle, dir, seriesArray, index, rawValues, offsetBoundary, boundOffset, shapeStyle);
                case ChartStyle.Columns:
                    {
                        var units = Utils.GetUnitsForLines(seriesArray, chartStyle, dir, width, height, boundOffset, linesCountY, fmt.Height, autoAdjust, maxYConv);
                        return drawColumns(width, height, units, dir, seriesArray, index, showValues, fontFamily, fontStyle, fontWeight, fontStretch, fontSize, culture, flowDirection);
                    }
                case ChartStyle.StackedColumns:
                    {
                        var units = Utils.GetUnitsForLines(seriesArray, chartStyle, dir, width, height, boundOffset, linesCountY, fmt.Height, autoAdjust, maxYConv);
                        return drawStackedColumns(width, height, units, dir, seriesArray, index, rawValues, showValues, fontFamily, fontStyle, fontWeight, fontStretch, fontSize, culture, flowDirection);
                    }
                case ChartStyle.FullStackedColumns:
                    return drawFullStackedColumns(width, height, dir, seriesArray, index, rawValues, showValues, fontFamily, fontStyle, fontWeight, fontStretch, fontSize, culture, flowDirection);
                //break;
                case ChartStyle.Bars:
                    {
                        var units = getUnitsForBars(seriesArray, chartStyle, dir, width, linesCountX, fmt.Height, autoAdjust, maxXConv);
                        return drawBars(width, height, units, dir, seriesArray, index, showValues, fontFamily, fontStyle, fontWeight, fontStretch, fontSize, culture, flowDirection);
                    }
                case ChartStyle.StackedBars:
                    {
                        var units = getUnitsForBars(seriesArray, chartStyle, dir, width, linesCountX, fmt.Height, autoAdjust, maxXConv);
                        return drawStackedBars(width, height, units, dir, seriesArray, index, rawValues, showValues, fontFamily, fontStyle, fontWeight, fontStretch, fontSize, culture, flowDirection);
                    }
                case ChartStyle.FullStackedBars:
                    return drawFullStackedBars(width, height, dir, seriesArray, index, rawValues, showValues, fontFamily, fontStyle, fontWeight, fontStretch, fontSize, culture, flowDirection);
                case ChartStyle.FullStackedArea:
                case ChartStyle.SmoothFullStackedArea:
                    maxX = seriesArray.Max(s => s.Values.Count);
                    return drawFullStackedArea(width, height, maxX, dir, seriesArray, index, rawValues, chartStyle);
            }

            return null;
        }

        private PathGeometry drawOHLC(double width, double height, int maxX, double units, Directions dir, ISeries currentSeries, ColoredPaths colored, double boundOffset)
        {
            double stepX;
            double centerX, centerY;
            var gm = new PathGeometry();
            var delimeter = maxX > 1 ? maxX - 1 : 1;

            switch (dir)
            {
                case Directions.NorthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter;
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = height / 2;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter;
                    break;
                case Directions.SouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter;
                    break;
                default:
                    return null;
            }

            var addition = 2 * Utils.RECT_SIZE < boundOffset ? 2 * Utils.RECT_SIZE : boundOffset - 2 * Utils.RECT_SIZE;

            switch (colored)
            {
                case ColoredPaths.Stock:
                    currentSeries.RealRects.Clear();
                    break;
            }

            for (var i = 0; i < currentSeries.Values.Count; i++)
            {
                var x = centerX + i * stepX;

                switch (colored)
                {
                    case ColoredPaths.Stock:
                        {
                            var y1 = centerY - currentSeries.Values[i].CompositeValue.HighValue * units;
                            var y2 = currentSeries.Values[i].CompositeValue.CloseValue > currentSeries.Values[i].CompositeValue.OpenValue
                                ? centerY - currentSeries.Values[i].CompositeValue.CloseValue * units
                                : centerY - currentSeries.Values[i].CompositeValue.OpenValue * units;
                            var line = new LineGeometry(new Point(x, y1), new Point(x, y2));
                            gm.AddGeometry(line);

                            y1 = currentSeries.Values[i].CompositeValue.CloseValue > currentSeries.Values[i].CompositeValue.OpenValue
                                ? centerY - currentSeries.Values[i].CompositeValue.OpenValue * units
                                : centerY - currentSeries.Values[i].CompositeValue.CloseValue * units;
                            y2 = centerY - currentSeries.Values[i].CompositeValue.LowValue * units;
                            line = new LineGeometry(new Point(x, y1), new Point(x, y2));
                            gm.AddGeometry(line);

                            y1 = centerY - currentSeries.Values[i].CompositeValue.CloseValue * units;
                            y2 = centerY - currentSeries.Values[i].CompositeValue.OpenValue * units;
                            var rect = new Rect(new Point(x - addition, y1), new Point(x + addition, y2));
                            currentSeries.RealRects.Add(rect);
                            break;
                        }
                    case ColoredPaths.Up:
                        {
                            if (currentSeries.Values[i].CompositeValue.CloseValue <= currentSeries.Values[i].CompositeValue.OpenValue)
                                break;
                            var y1 = centerY - currentSeries.Values[i].CompositeValue.CloseValue * units;
                            var y2 = centerY - currentSeries.Values[i].CompositeValue.OpenValue * units;
                            var rect = new Rect(new Point(x - addition, y1), new Point(x + addition, y2));
                            gm.AddGeometry(new RectangleGeometry(rect));
                            break;
                        }
                    case ColoredPaths.Down:
                        {
                            if (currentSeries.Values[i].CompositeValue.OpenValue <= currentSeries.Values[i].CompositeValue.CloseValue)
                                break;
                            var y1 = centerY - currentSeries.Values[i].CompositeValue.OpenValue * units;
                            var y2 = centerY - currentSeries.Values[i].CompositeValue.CloseValue * units;
                            var rect = new Rect(new Point(x - addition, y1), new Point(x + addition, y2));
                            gm.AddGeometry(new RectangleGeometry(rect));
                            break;
                        }
                }
            }
            return gm;
        }

        private PathGeometry drawHLC(double width, double height, int maxX, double units, Directions dir, ISeries currentSeries, ColoredPaths colored, double boundOffset)
        {
            double stepX;
            double centerX, centerY;
            var gm = new PathGeometry();
            var delimeter = maxX > 1 ? maxX - 1 : 1;

            switch (dir)
            {
                case Directions.NorthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter;
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = height / 2;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter;
                    break;
                case Directions.SouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter;
                    break;
                default:
                    return null;
            }

            switch (colored)
            {
                case ColoredPaths.Stock:
                    currentSeries.RealRects.Clear();
                    break;
            }

            for (var i = 0; i < currentSeries.Values.Count; i++)
            {
                var x = centerX + i * stepX;

                switch (colored)
                {
                    case ColoredPaths.Stock:
                        {
                            var y1 = centerY - currentSeries.Values[i].CompositeValue.HighValue * units;// + 2 * Utils.RECT_SIZE;
                            var y2 = centerY - currentSeries.Values[i].CompositeValue.LowValue * units;// - 2 * Utils.RECT_SIZE;
                            var y3 = centerY - currentSeries.Values[i].CompositeValue.CloseValue * units;
                            var line = new LineGeometry(new Point(x, y1), new Point(x, y2));
                            gm.AddGeometry(line);
                            line = new LineGeometry(new Point(x - Utils.COLUMN_BAR_OFFSET / 2, y3), new Point(x + Utils.COLUMN_BAR_OFFSET / 2, y3));
                            gm.AddGeometry(line);
                            currentSeries.RealRects.Add(new Rect(new Point(x - 1, y1), new Point(x + 1, y2)));
                            break;
                        }
                    case ColoredPaths.Up:
                        {
                            var y1 = centerY - currentSeries.Values[i].CompositeValue.HighValue * units;
                            var line = new LineGeometry(new Point(x, y1), new Point(x + Utils.COLUMN_BAR_OFFSET / 2, y1));
                            gm.AddGeometry(line);
                            break;
                        }
                    case ColoredPaths.Down:
                        {
                            var y1 = centerY - currentSeries.Values[i].CompositeValue.LowValue * units;
                            var line = new LineGeometry(new Point(x - Utils.COLUMN_BAR_OFFSET / 2, y1), new Point(x, y1));
                            gm.AddGeometry(line);
                            break;
                        }
                }
            }
            return gm;
        }

        private CombinedGeometry drawWaterfall(double width, double height, Directions dir, ISeries currentSeries, double stepLength, ColoredPaths colored, bool showValues,
           FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, double fontSize,
           CultureInfo culture, FlowDirection flowDirection)
        {
            var gm = new PathGeometry();
            var gmValues = new PathGeometry();
            var cgm = new CombinedGeometry
            {
                GeometryCombineMode = GeometryCombineMode.Exclude,
                Geometry1 = gm,
                Geometry2 = gmValues
            };
            var segSize = width / currentSeries.Values.Count;
            var columnWidth = segSize - segSize / 8;
            var startY = Utils.AXIS_THICKNESS;

            switch (dir)
            {
                case Directions.NorthEast:
                    startY = height - Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastNorthWest:
                    startY = height - Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthWest:
                    startY = height - Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastSouthEast:
                    startY = height / 2;
                    break;
                case Directions.SouthEast:
                    startY = Utils.AXIS_THICKNESS;
                    break;
            }

            currentSeries.RealRects.Clear();
            var y = startY;
            for (var i = 0; i < currentSeries.Values.Count; i++)
            {
                //var drawValue = false;
                var value = currentSeries.Values[i].CompositeValue.PlainValue;
                var x = i * segSize + segSize / 16;
                Rect rect;
                if (value >= 0)
                {
                    rect = new Rect(new Point(x, y), new Point(x + columnWidth, y - currentSeries.Values[i].CompositeValue.PlainValue * stepLength));
                    y -= rect.Height;
                    if (colored == ColoredPaths.Up)
                    {
                        gm.AddGeometry(new RectangleGeometry(rect));
                    }
                }
                else
                {
                    rect = new Rect(new Point(x, y), new Point(x + columnWidth, y + Math.Abs(currentSeries.Values[i].CompositeValue.PlainValue) * stepLength));
                    y += rect.Height;
                    if (colored == ColoredPaths.Down)
                    {
                        gm.AddGeometry(new RectangleGeometry(rect));
                    }
                }

                currentSeries.RealRects.Add(rect);

                // add values
                if (currentSeries.Values.Count <= i) continue;
                if (!showValues) continue;
                var number = !string.IsNullOrEmpty(currentSeries.Values[i].CustomValue) ? currentSeries.Values[i].CustomValue : currentSeries.Values[i].CompositeValue.PlainValue.ToString(culture);
                var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                if (fmt.Width > rect.Width || fmt.Height > rect.Height) continue;
                var pt = new Point(x + (rect.Width - fmt.Width) / 2, rect.Top + (rect.Height - fmt.Height) / 2);
                var ngm = fmt.BuildGeometry(pt);
                if (flowDirection == FlowDirection.RightToLeft)
                    ngm.Transform = new ScaleTransform { ScaleX = -1, CenterX = pt.X + fmt.Width / 2, CenterY = pt.Y + fmt.Height / 2 };
                gmValues.AddGeometry(ngm);
            }

            return cgm;
        }

        private CombinedGeometry drawFunnel(ISeries currentSeries, double width, double heigth, bool showValues,
           FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, double fontSize,
           CultureInfo culture, FlowDirection flowDirection)
        {
            if (currentSeries.Values.All(v => v.CompositeValue.PlainValue <= 0))
                return null;
            const double FUNNEL_OFFSET = 2.0;
            var cgm = new CombinedGeometry { GeometryCombineMode = GeometryCombineMode.Exclude };
            var gm = new PathGeometry();
            var gmValues = new PathGeometry();
            var drawingWidth = width - Utils.AXIS_THICKNESS - COLUMN_BAR_OFFSET * 2;
            heigth -= Utils.AXIS_THICKNESS;

            var maxWidth = currentSeries.Values.Where(v => v.CompositeValue.PlainValue > 0).Max(v => v.CompositeValue.PlainValue);
            var units = maxWidth != 0 ? drawingWidth / maxWidth : 0;

            var barHeight = heigth / currentSeries.Values.Count - FUNNEL_OFFSET * 2;
            currentSeries.RealRects.Clear();
            for (int i = 0, j = 1; i < currentSeries.Values.Count; i++, j += 2)
            {
                if (currentSeries.Values[i].CompositeValue.PlainValue <= 0)
                    continue;
                var rectWidth = currentSeries.Values[i].CompositeValue.PlainValue * units;
                var x = COLUMN_BAR_OFFSET + (width - rectWidth) / 2;
                var y = j * FUNNEL_OFFSET + i * barHeight;
                var rect = new Rect(x, y, rectWidth, barHeight);
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
                // add values
                if (!showValues) continue;
                var number = !string.IsNullOrEmpty(currentSeries.Values[i].CustomValue)
                    ? $"{currentSeries.Values[i].CompositeValue.PlainValue.ToString(culture)} {currentSeries.Values[i].CustomValue}"
                    : currentSeries.Values[i].CompositeValue.PlainValue.ToString(culture);
                var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                if (fmt.Width > rectWidth || fmt.Height > barHeight) continue;
                var pt = new Point(x + (rectWidth - fmt.Width) / 2, y + (barHeight - fmt.Height) / 2);
                var ngm = fmt.BuildGeometry(pt);
                if (flowDirection == FlowDirection.RightToLeft)
                    ngm.Transform = new ScaleTransform { ScaleX = -1, CenterX = pt.X + fmt.Width / 2, CenterY = pt.Y + fmt.Height / 2 };
                gmValues.AddGeometry(ngm);
            }
            cgm.Geometry1 = gm;
            cgm.Geometry2 = gmValues;
            return cgm;
        }

        private double getUnitsForBars(ISeries[] series, ChartStyle chartStyle, Directions dir, double width, int linesCountX, double formatHeight, AutoAdjustmentMode autoAdjust, double maxX)
        {
            var centerX = 0.0;
            var radius = 0.0;

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
            if (!autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Horizontal))
                return radius / maxX;
            var centerPoint = new Point(centerX, radius);
            var (_, _, _, _, _, units, _) = Utils.GetMeasures(
               chartStyle,
               series,
               linesCountX,
               radius,
               formatHeight,
               centerPoint,
               dir == Directions.NorthEastNorthWest);
            return units;
        }

        private CombinedGeometry drawBars(double width, double heigth, double units, Directions dir, ISeries[] series, int index, bool showValues,
           FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, double fontSize,
           CultureInfo culture, FlowDirection flowDirection)
        {
            var gm = new PathGeometry();
            var gmValues = new PathGeometry();
            var cgm = new CombinedGeometry
            {
                GeometryCombineMode = GeometryCombineMode.Exclude,
                Geometry1 = gm,
                Geometry2 = gmValues
            };
            var segSize = heigth / series.Max(s => s.Values.Count);
            var barHeight = (segSize - COLUMN_BAR_OFFSET * 2) / series.Length;
            var startX = 0.0;
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    startX = Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastNorthWest:
                    startX = width / 2;
                    break;
                case Directions.NorthEastSouthEast:
                    startX = Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthWest:
                    startX = width - Utils.AXIS_THICKNESS;
                    break;
                case Directions.SouthEast:
                    startX = Utils.AXIS_THICKNESS;
                    break;
            }
            currentSeries.RealRects.Clear();

            for (var i = 0; i < currentSeries.Values.Count; i++)
            {
                var y = heigth - (i * segSize + COLUMN_BAR_OFFSET + index * barHeight);
                var rect = new Rect(new Point(startX, y), new Point(startX + currentSeries.Values[i].CompositeValue.PlainValue * units, y - barHeight));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
                // add values
                if (!showValues) continue;
                var number = !string.IsNullOrEmpty(currentSeries.Values[i].CustomValue) ? currentSeries.Values[i].CustomValue : currentSeries.Values[i].CompositeValue.PlainValue.ToString(culture);
                var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                if (fmt.Width > rect.Width || fmt.Height > rect.Height) continue;
                var pt = new Point(rect.Left + (rect.Width - fmt.Width) / 2, rect.Top + (rect.Height - fmt.Height) / 2);
                var ngm = fmt.BuildGeometry(pt);
                if (flowDirection == FlowDirection.RightToLeft)
                    ngm.Transform = new ScaleTransform { ScaleX = -1, CenterX = pt.X + fmt.Width / 2, CenterY = pt.Y + fmt.Height / 2 };
                gmValues.AddGeometry(ngm);
            }
            return cgm;
        }

        private CombinedGeometry drawStackedBars(double width, double height, double units, Directions dir, ISeries[] series, int index, List<(List<IChartValue> Values, int Index)> tuples, bool showValues,
           FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, double fontSize,
           CultureInfo culture, FlowDirection flowDirection)
        {
            var gm = new PathGeometry();
            var gmValues = new PathGeometry();
            var cgm = new CombinedGeometry
            {
                GeometryCombineMode = GeometryCombineMode.Exclude,
                Geometry1 = gm,
                Geometry2 = gmValues
            };
            var tp = tuples.FirstOrDefault(t => t.Index == index);
            if (tp == default) return null;
            var values = tp.Values;
            var segSize = height / values.Count;
            var barHeight = segSize - COLUMN_BAR_OFFSET * 2;
            var startX = 0.0;
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    startX = Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastNorthWest:
                    startX = width / 2;
                    break;
                case Directions.NorthEastSouthEast:
                    startX = Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthWest:
                    startX = width - Utils.AXIS_THICKNESS;
                    break;
                case Directions.SouthEast:
                    startX = Utils.AXIS_THICKNESS;
                    break;
            }

            currentSeries.RealRects.Clear();
            for (var i = 0; i < values.Count; i++)
            {
                var x = startX;
                if (index > 0)
                {
                    var i1 = i;
                    var prevsTuples =
                        tuples.Where(t => t.Index < index && Math.Sign(t.Values[i1].CompositeValue.PlainValue) == Math.Sign(values[i1].CompositeValue.PlainValue));
                    x += prevsTuples.Sum(sr => sr.Values[i].CompositeValue.PlainValue * units);
                }
                var y = height - (i * segSize + COLUMN_BAR_OFFSET);
                var rect = new Rect(new Point(x, y), new Point(x + values[i].CompositeValue.PlainValue * units, y - barHeight));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
                // add values
                if (currentSeries.Values.Count <= i) continue;
                if (!showValues) continue;
                var number = !string.IsNullOrEmpty(currentSeries.Values[i].CustomValue) ? currentSeries.Values[i].CustomValue : currentSeries.Values[i].CompositeValue.PlainValue.ToString(culture);
                var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                if (fmt.Width > rect.Width || fmt.Height > rect.Height) continue;
                var pt = new Point(rect.Left + (rect.Width - fmt.Width) / 2, rect.Top + (rect.Height - fmt.Height) / 2);
                var ngm = fmt.BuildGeometry(pt);
                if (flowDirection == FlowDirection.RightToLeft)
                    ngm.Transform = new ScaleTransform { ScaleX = -1, CenterX = pt.X + fmt.Width / 2, CenterY = pt.Y + fmt.Height / 2 };
                gmValues.AddGeometry(ngm);
            }
            return cgm;
        }

        private CombinedGeometry drawFullStackedBars(double width, double height, Directions dir, ISeries[] series, int index, List<(List<IChartValue> Values, int Index)> tuples, bool showValues,
           FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, double fontSize,
           CultureInfo culture, FlowDirection flowDirection)
        {
            var gm = new PathGeometry();
            var gmValues = new PathGeometry();
            var cgm = new CombinedGeometry
            {
                GeometryCombineMode = GeometryCombineMode.Exclude,
                Geometry1 = gm,
                Geometry2 = gmValues
            };
            var tp = tuples.FirstOrDefault(t => t.Index == index);
            if (tp == default) return null;
            var values = tp.Values;
            var segSize = height / values.Count;
            var barHeight = segSize - COLUMN_BAR_OFFSET * 2;
            var startX = Utils.AXIS_THICKNESS;
            var stepX = 0.0;
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    startX = Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS);
                    break;
                case Directions.NorthEastNorthWest:
                    startX = width / 2;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / 2;
                    break;
                case Directions.NorthEastSouthEast:
                    startX = Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS);
                    break;
                case Directions.NorthWest:
                    startX = width - Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS);
                    break;
                case Directions.SouthEast:
                    startX = Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS);
                    break;
            }
            currentSeries.RealRects.Clear();
            for (var i = 0; i < values.Count; i++)
            {
                var sign = Math.Sign(values[i].CompositeValue.PlainValue);
                var y = height - (i * segSize + COLUMN_BAR_OFFSET);
                var i1 = i;
                var sum = tuples.Sum(sr => Math.Abs(sr.Values[i1].CompositeValue.PlainValue));
                var percent = Math.Abs(values[i].CompositeValue.PlainValue) / (sum != 0 ? sum : 1) * 100;
                var segWidth = sign * stepX / 100 * percent;

                var prevs = tuples.Where(s => s.Index < index)
                    .Where(s => Math.Sign(s.Values[i1].CompositeValue.PlainValue) == sign);
                var prevSum = prevs.Sum(pvs => Math.Abs(pvs.Values[i1].CompositeValue.PlainValue));
                var prevPerc = prevSum / (sum != 0 ? sum : 1) * 100;
                var prevWidth = stepX / 100 * prevPerc;
                var x = startX + sign * prevWidth;
                var rect = new Rect(new Point(x, y), new Point(x + segWidth, y - barHeight));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
                // add values
                if (currentSeries.Values.Count <= i) continue;
                if (!showValues) continue;
                var number = !string.IsNullOrEmpty(currentSeries.Values[i].CustomValue) ? currentSeries.Values[i].CustomValue : currentSeries.Values[i].CompositeValue.PlainValue.ToString(culture);
                var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                if (fmt.Width > rect.Width || fmt.Height > rect.Height) continue;
                var pt = new Point(rect.Left + (rect.Width - fmt.Width) / 2, rect.Top + (rect.Height - fmt.Height) / 2);
                var ngm = fmt.BuildGeometry(pt);
                if (flowDirection == FlowDirection.RightToLeft)
                    ngm.Transform = new ScaleTransform { ScaleX = -1, CenterX = pt.X + fmt.Width / 2, CenterY = pt.Y + fmt.Height / 2 };
                gmValues.AddGeometry(ngm);
            }
            return cgm;
        }

        private CombinedGeometry drawFullStackedColumns(double width, double height, Directions dir, ISeries[] series, int index, List<(List<IChartValue> Values, int Index)> tuples, bool showValues,
           FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, double fontSize,
           CultureInfo culture, FlowDirection flowDirection)
        {
            var gm = new PathGeometry();
            var gmValues = new PathGeometry();
            var cgm = new CombinedGeometry
            {
                GeometryCombineMode = GeometryCombineMode.Exclude,
                Geometry1 = gm,
                Geometry2 = gmValues
            };
            var tp = tuples.FirstOrDefault(t => t.Index == index);
            if (tp == default) return null;
            var values = tp.Values;
            var segSize = width / values.Count;
            var columnWidth = segSize - COLUMN_BAR_OFFSET * 2;
            var startY = Utils.AXIS_THICKNESS;
            var stepY = 0.0;
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    startY = height - Utils.AXIS_THICKNESS;
                    stepY = (height - 2 * Utils.AXIS_THICKNESS);
                    break;
                case Directions.NorthEastNorthWest:
                    startY = height - Utils.AXIS_THICKNESS;
                    stepY = (height - 2 * Utils.AXIS_THICKNESS);
                    break;
                case Directions.NorthWest:
                    startY = height - Utils.AXIS_THICKNESS;
                    stepY = (height - 2 * Utils.AXIS_THICKNESS);
                    break;
                case Directions.NorthEastSouthEast:
                    startY = height / 2;
                    stepY = (height - 2 * Utils.AXIS_THICKNESS) / 2;
                    break;
                case Directions.SouthEast:
                    startY = Utils.AXIS_THICKNESS;
                    stepY = (height - 2 * Utils.AXIS_THICKNESS);
                    break;
            }

            currentSeries.RealRects.Clear();
            for (var i = 0; i < values.Count; i++)
            {
                var sign = Math.Sign(values[i].CompositeValue.PlainValue);
                var x = i * segSize + COLUMN_BAR_OFFSET;
                var i1 = i;
                var sum = tuples.Sum(sr => Math.Abs(sr.Values[i1].CompositeValue.PlainValue));
                var percent = Math.Abs(values[i].CompositeValue.PlainValue) / (sum != 0 ? sum : 1) * 100;
                var segHeight = sign * stepY / 100 * percent;
                var prevs = tuples.Where(s => s.Index < index)
                    .Where(s => Math.Sign(s.Values[i1].CompositeValue.PlainValue) == sign);
                var prevSum = prevs.Sum(pvs => Math.Abs(pvs.Values[i1].CompositeValue.PlainValue));
                var prevPerc = prevSum / (sum != 0 ? sum : 1) * 100;
                var prevHeight = stepY / 100 * prevPerc;
                var y = startY - sign * prevHeight;
                var rect = new Rect(new Point(x, y), new Point(x + columnWidth, y - segHeight));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
                // add values
                if (currentSeries.Values.Count <= i) continue;
                if (!showValues) continue;
                var number = !string.IsNullOrEmpty(currentSeries.Values[i].CustomValue) ? currentSeries.Values[i].CustomValue : currentSeries.Values[i].CompositeValue.PlainValue.ToString(culture);
                var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                if (fmt.Width > rect.Width || fmt.Height > rect.Height) continue;
                var pt = new Point(x + (rect.Width - fmt.Width) / 2, rect.Top + (rect.Height - fmt.Height) / 2);
                var ngm = fmt.BuildGeometry(pt);
                if (flowDirection == FlowDirection.RightToLeft)
                    ngm.Transform = new ScaleTransform { ScaleX = -1, CenterX = pt.X + fmt.Width / 2, CenterY = pt.Y + fmt.Height / 2 };
                gmValues.AddGeometry(ngm);
            }
            return cgm;
        }

        private CombinedGeometry drawStackedColumns(double width, double height, double units, Directions dir, ISeries[] series, int index, List<(List<IChartValue> Values, int Index)> tuples, bool showValues,
           FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, double fontSize,
           CultureInfo culture, FlowDirection flowDirection)
        {
            var gm = new PathGeometry();
            var gmValues = new PathGeometry();
            var cgm = new CombinedGeometry
            {
                GeometryCombineMode = GeometryCombineMode.Exclude,
                Geometry1 = gm,
                Geometry2 = gmValues
            };

            var tp = tuples.FirstOrDefault(t => t.Index == index);
            if (tp == default) return null;
            var values = tp.Values;
            var segSize = width / values.Count;
            var columnWidth = segSize - COLUMN_BAR_OFFSET * 2;
            var startY = Utils.AXIS_THICKNESS;
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    startY = height - Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastNorthWest:
                    startY = height - Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthWest:
                    startY = height - Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastSouthEast:
                    startY = height / 2;
                    break;
                case Directions.SouthEast:
                    startY = Utils.AXIS_THICKNESS;
                    break;
            }

            currentSeries.RealRects.Clear();
            for (var i = 0; i < values.Count; i++)
            {
                var x = i * segSize + COLUMN_BAR_OFFSET;
                var y = startY;
                if (index > 0)
                {
                    var i1 = i;
                    var prevs =
                        tuples.Where(
                            s => s.Index < index && Math.Sign(s.Values[i1].CompositeValue.PlainValue) == Math.Sign(values[i1].CompositeValue.PlainValue));
                    y -= prevs.Sum(sr => sr.Values[i].CompositeValue.PlainValue * units);
                }
                var rect = new Rect(new Point(x, y), new Point(x + columnWidth, y - values[i].CompositeValue.PlainValue * units));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
                // add values
                if (currentSeries.Values.Count <= i) continue;
                if (!showValues) continue;
                var number = !string.IsNullOrEmpty(currentSeries.Values[i].CustomValue) ? currentSeries.Values[i].CustomValue : currentSeries.Values[i].CompositeValue.PlainValue.ToString(culture);
                var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                if (fmt.Width > rect.Width || fmt.Height > rect.Height) continue;
                var pt = new Point(x + (rect.Width - fmt.Width) / 2, rect.Top + (rect.Height - fmt.Height) / 2);
                var ngm = fmt.BuildGeometry(pt);
                if (flowDirection == FlowDirection.RightToLeft)
                    ngm.Transform = new ScaleTransform { ScaleX = -1, CenterX = pt.X + fmt.Width / 2, CenterY = pt.Y + fmt.Height / 2 };
                gmValues.AddGeometry(ngm);
            }
            return cgm;
        }

        private CombinedGeometry drawColumns(double width, double height, double units, Directions dir, ISeries[] series, int index, bool showValues,
           FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, double fontSize,
           CultureInfo culture, FlowDirection flowDirection)
        {
            var gm = new PathGeometry();
            var gmValues = new PathGeometry();
            var cgm = new CombinedGeometry
            {
                GeometryCombineMode = GeometryCombineMode.Exclude,
                Geometry1 = gm,
                Geometry2 = gmValues
            };

            var startY = Utils.AXIS_THICKNESS;
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            var segSize = width / series.Max(s => s.Values.Count);
            var columnWidth = (segSize - COLUMN_BAR_OFFSET * 2) / series.Length;

            switch (dir)
            {
                case Directions.NorthEast:
                    startY = height - Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastNorthWest:
                    startY = height - Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthWest:
                    startY = height - Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastSouthEast:
                    startY = height / 2;
                    break;
                case Directions.SouthEast:
                    startY = Utils.AXIS_THICKNESS;
                    break;
            }

            currentSeries.RealRects.Clear();
            for (var i = 0; i < currentSeries.Values.Count; i++)
            {
                var x = i * segSize + COLUMN_BAR_OFFSET + index * columnWidth;
                var rect = new Rect(new Point(x, startY), new Point(x + columnWidth, startY - currentSeries.Values[i].CompositeValue.PlainValue * units));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
                // add values
                if (!showValues) continue;
                var number = !string.IsNullOrEmpty(currentSeries.Values[i].CustomValue) ? currentSeries.Values[i].CustomValue : currentSeries.Values[i].CompositeValue.PlainValue.ToString(culture);
                var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                if (fmt.Width > rect.Width || fmt.Height > rect.Height) continue;
                var pt = new Point(x + (rect.Width - fmt.Width) / 2, rect.Top + (rect.Height - fmt.Height) / 2);
                var ngm = fmt.BuildGeometry(pt);
                if (flowDirection == FlowDirection.RightToLeft)
                    ngm.Transform = new ScaleTransform { ScaleX = -1, CenterX = pt.X + fmt.Width / 2, CenterY = pt.Y + fmt.Height / 2 };
                gmValues.AddGeometry(ngm);
            }
            return cgm;
        }

        private PathGeometry drawFullStackedArea(double width, double height, double maxX, Directions dir,
            ISeries[] series, int index, List<(List<IChartValue> Values, int Index)> tuples, ChartStyle chartStyle)
        {
            var tp = tuples.FirstOrDefault(t => t.Index == index);
            if (tp == default) return null;
            var values = tp.Values;
            double stepX, stepY;
            double centerX, centerY;
            var gm = new PathGeometry();

            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            var delimeter = maxX > 1 ? maxX - 1 : 1;
            switch (dir)
            {
                case Directions.NorthEast:
                    centerX = Utils.AXIS_THICKNESS;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    stepY = (height - 2 * Utils.AXIS_THICKNESS);
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = width / 2;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / 2 / delimeter;
                    stepY = (height - 2 * Utils.AXIS_THICKNESS);
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = Utils.AXIS_THICKNESS;
                    centerY = height / 2;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    stepY = (height - 2 * Utils.AXIS_THICKNESS) / 2;
                    break;
                case Directions.SouthEast:
                    centerX = Utils.AXIS_THICKNESS;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    stepY = height - 2 * Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthWest:
                    centerX = width - Utils.AXIS_THICKNESS;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    stepY = height - 2 * Utils.AXIS_THICKNESS;
                    break;
                default:
                    return null;
            }
            currentSeries.RealRects.Clear();
            currentSeries.RealPoints.Clear();

            var start = new Point();
            double x = 0, y;

            for (var i = 0; i < values.Count; i++)
            {
                if (index == 0)
                {
                    y = centerY;
                }
                else
                {
                    var prevSeries = series.FirstOrDefault(s => s.Index == index - 1);
                    if (prevSeries == null) return null;
                    y = prevSeries.RealPoints[i].Y;
                }
                var sum = tuples.Sum(s => Math.Abs(s.Values[i].CompositeValue.PlainValue));
                var sign = Math.Sign(values[i].CompositeValue.PlainValue);
                var perc = Math.Abs(values[i].CompositeValue.PlainValue) / (sum != 0 ? sum : 1) * 100;
                y -= sign * perc * stepY / 100;
                x = stepX * i;
                currentSeries.RealPoints.Add(new Point(x, y));
                if (i == 0)
                    start = new Point(x, y);
            }

            var points = new List<Point>();
            points.AddRange(currentSeries.RealPoints);

            if (index == 0)
            {
                y = centerY;
                points.Add(new Point(x, y));
                points.Add(new Point(start.X, centerY));
            }
            else
            {
                var prevSeries = series.FirstOrDefault(s => s.Index == index - 1);
                if (prevSeries == null) return null;
                for (var i = prevSeries.RealPoints.Count - 1; i >= 0; i--)
                {
                    points.Add(prevSeries.RealPoints[i]);
                }
            }

            if (chartStyle == ChartStyle.SmoothFullStackedArea)
            {
                points.Insert(0, new Point(centerX, centerY));
                points.Add(new Point(points[points.Count - 1].X, centerY));
                var bezierSegments = InterpolatePointsWithBezierCurves(points, false);
                if (bezierSegments == null || bezierSegments.Count == 0)
                {
                    var poly = new PolyLineSegment(points, true);
                    gm.Figures.Add(new PathFigure(points[0], new[] { poly }, true));
                }
                else
                {
                    var segments = bezierSegments.Select(bz => new BezierSegment
                    {
                        Point1 = bz.FirstControlPoint,
                        Point2 = bz.SecondControlPoint,
                        Point3 = bz.EndPoint
                    }).Cast<PathSegment>().ToList();
                    gm.Figures.Add(new PathFigure(points[0], segments, true));
                }
            }
            else
            {
                var poly = new PolyLineSegment(points, true);
                gm.Figures.Add(new PathFigure(start, new[] { poly }, true));
            }

            return gm;
        }

        private Geometry drawRectangle(Rect rect)
        {
            var gm = new PathGeometry();
            while (rect.Width >= 1)
            {
                gm.AddGeometry(new RectangleGeometry(rect));
                rect.Inflate(-2, -2);
            }
            return gm;
        }

        private Geometry drawCircle(Rect rect)
        {
            var gm = new PathGeometry();
            while (rect.Width >= 1)
            {
                gm.AddGeometry(new EllipseGeometry(rect));
                rect.Inflate(-2, -2);
            }
            return gm;
        }

        private void drawMarker(double x, double y, PathGeometry gm, ShapeStyle shapeStyle, List<Rect> realRects)
        {
            var smallRadius = 0.75 * RECT_SIZE;
            var smallRect = new Rect(new Point(x - smallRadius, y - smallRadius), new Point(x + smallRadius, y + smallRadius));
            var rect = new Rect(new Point(x - RECT_SIZE, y - RECT_SIZE), new Point(x + RECT_SIZE, y + RECT_SIZE));
            realRects.Add(rect);
            Geometry g = shapeStyle switch
            {
                ShapeStyle.Rectangle => drawRectangle(rect),
                ShapeStyle.Circle => drawCircle(rect),
                ShapeStyle.Star5 => Utils.DrawStar(5, RECT_SIZE * 1.5, new Point(x, y), true),
                ShapeStyle.Star6 => Utils.DrawStar(6, RECT_SIZE * 1.5, new Point(x, y), true),
                ShapeStyle.Star8 => Utils.DrawStar(8, RECT_SIZE * 1.5, new Point(x, y), true),
                _ => null
            };
            gm.AddGeometry(g);
            if (shapeStyle.In(ShapeStyle.Star5, ShapeStyle.Star6, ShapeStyle.Star8))
                gm.AddGeometry(drawCircle(smallRect));
        }

        private PathGeometry drawFullStackedLine(double width, double height, double maxX, ChartStyle style,
            Directions dir, ISeries[] series, int index, List<(List<IChartValue> Values, int Index)> tuples, bool offsetBoundary, double boundOffset, ShapeStyle shapeStyle)
        {
            var tp = tuples.FirstOrDefault(t => t.Index == index);
            if (tp == default) return null;
            var values = tp.Values;
            double stepX, stepY;
            double centerY;
            var gm = new PathGeometry();

            var delimeter = maxX > 1 ? maxX - 1 : 1;

            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter : (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    stepY = height - 2 * Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastNorthWest:
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / 2 / delimeter;
                    stepY = height - 2 * Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastSouthEast:
                    centerY = height / 2;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter : (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    stepY = (height - 2 * Utils.AXIS_THICKNESS) / 2;
                    break;
                case Directions.SouthEast:
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter : (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    stepY = height - 2 * Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthWest:
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    stepY = height - 2 * Utils.AXIS_THICKNESS;
                    break;
                default:
                    return null;
            }
            currentSeries.RealRects.Clear();
            currentSeries.RealPoints.Clear();

            var start = new Point();
            for (var i = 0; i < values.Count; i++)
            {
                double y;
                if (index == 0)
                {
                    y = centerY;
                }
                else
                {
                    var prevSeries = series.FirstOrDefault(s => s.Index == index - 1);
                    if (prevSeries == null) return null;
                    y = prevSeries.RealPoints[i].Y;
                }
                var sum = tuples.Sum(s => Math.Abs(s.Values[i].CompositeValue.PlainValue));
                var sign = Math.Sign(values[i].CompositeValue.PlainValue);
                var perc = Math.Abs(values[i].CompositeValue.PlainValue) / (sum != 0 ? sum : 1) * 100;

                y -= sign * perc * stepY / 100;
                var x = offsetBoundary ? boundOffset + stepX * i : stepX * i;
                currentSeries.RealPoints.Add(new Point(x, y));
                if (i == 0)
                    start = new Point(x, y);
                if (!style.In(ChartStyle.FullStackedLinesWithMarkers, ChartStyle.SmoothFullStackedLinesWithMarkers)) continue;
                drawMarker(x, y, gm, shapeStyle, currentSeries.RealRects);
            }

            if (style.In(ChartStyle.SmoothFullStackedLines, ChartStyle.SmoothFullStackedLinesWithMarkers))
            {
                var bezierSegments = InterpolatePointsWithBezierCurves(currentSeries.RealPoints, false);
                if (bezierSegments == null || bezierSegments.Count == 0)
                {
                    var poly = new PolyLineSegment(currentSeries.RealPoints, true);
                    gm.Figures.Add(new PathFigure(currentSeries.RealPoints[0], new[] { poly }, false));
                }
                else
                {
                    var segments = bezierSegments.Select(bz => new BezierSegment
                    {
                        Point1 = bz.FirstControlPoint,
                        Point2 = bz.SecondControlPoint,
                        Point3 = bz.EndPoint
                    }).Cast<PathSegment>().ToList();
                    gm.Figures.Add(new PathFigure(currentSeries.RealPoints[0], segments, false));
                }
            }
            else if (style.In(ChartStyle.FullStackedLines, ChartStyle.FullStackedLinesWithMarkers))
            {
                var poly = new PolyLineSegment(currentSeries.RealPoints, true);
                gm.Figures.Add(new PathFigure(start, new[] { poly }, false));
            }
            return gm;
        }

        private PathGeometry drawStackedArea(double width, double height, double maxX, double units, ChartStyle chartStyle, Directions dir,
           ISeries[] series, int index, List<(List<IChartValue> Values, int Index)> tuples)
        {
            var tp = tuples.FirstOrDefault(t => t.Index == index);
            if (tp == default) return null;
            var values = tp.Values;
            double stepX;
            double centerX, centerY;
            var gm = new PathGeometry();
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            var delimeter = maxX > 1 ? maxX - 1 : 1;
            switch (dir)
            {
                case Directions.NorthEast:
                    centerX = Utils.AXIS_THICKNESS;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = width / 2;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / 2 / delimeter;
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = Utils.AXIS_THICKNESS;
                    centerY = height / 2;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    break;
                case Directions.SouthEast:
                    centerX = Utils.AXIS_THICKNESS;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    break;
                case Directions.NorthWest:
                    centerX = width - Utils.AXIS_THICKNESS;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    break;
                default:
                    return null;
            }

            var points = new List<Point>();
            var x = centerX;
            var y = centerY;
            if (index > 0)
            {
                var prevs = tuples.Where(s => s.Index < index);
                var sum = prevs.Sum(sr => sr.Values[0].CompositeValue.PlainValue);
                y -= sum * units;
            }
            var start = new Point(x, y);
            for (var i = 0; i < values.Count; i++)
            {
                x = centerX + i * stepX;
                y = centerY - values[i].CompositeValue.PlainValue * units;
                if (index > 0)
                {
                    var prevs = tuples.Where(s => s.Index < index);
                    var sum = prevs.Sum(sr => sr.Values[i].CompositeValue.PlainValue);
                    y -= sum * units;
                }
                points.Add(new Point(x, y));
            }

            if (index == 0)
            {
                y = centerY;
                var end = new Point(x, y);

                points.Add(end);
            }
            else
            {
                var prevs = tuples.Where(s => s.Index < index).ToList();
                for (var i = values.Count - 1; i >= 0; i--)
                {
                    y = centerY;
                    var sum = prevs.Sum(sr => sr.Values[i].CompositeValue.PlainValue);
                    y -= sum * units;
                    points.Add(new Point(x, y));
                    x -= stepX;
                }
            }

            if (chartStyle == ChartStyle.SmoothStackedArea)
            {
                points.Insert(0, new Point(centerX, centerY));
                points.Add(new Point(points[points.Count - 1].X, centerY));
                var bezierSegments = InterpolatePointsWithBezierCurves(points, false);
                if (bezierSegments == null || bezierSegments.Count == 0)
                {
                    var poly = new PolyLineSegment(points, true);
                    gm.Figures.Add(new PathFigure(points[0], new[] { poly }, true));
                }
                else
                {
                    var segments = bezierSegments.Select(bz => new BezierSegment
                    {
                        Point1 = bz.FirstControlPoint,
                        Point2 = bz.SecondControlPoint,
                        Point3 = bz.EndPoint
                    }).Cast<PathSegment>().ToList();
                    gm.Figures.Add(new PathFigure(points[0], segments, true));
                }
            }
            else
            {
                var polySegment = new PolyLineSegment(points, true);
                gm.Figures.Add(new PathFigure(start, new[] { polySegment }, true));
            }
            return gm;
        }

        private PathGeometry drawStackedLine(double width, double height, double maxX, double units, ChartStyle style,
            Directions dir, ISeries[] series, int index, List<(List<IChartValue> Values, int Index)> tuples, bool offsetBoundary, double boundOffset, ShapeStyle shapeStyle)
        {
            var tp = tuples.FirstOrDefault(t => t.Index == index);
            if (tp == default) return null;
            var values = tp.Values;
            double stepX;
            double centerX, centerY;
            var gm = new PathGeometry();
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            var delimeter = maxX > 1 ? maxX - 1 : 1;

            switch (dir)
            {
                case Directions.NorthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter : (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = width / 2;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / 2 / delimeter;
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = height / 2;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter : (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    break;
                case Directions.SouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter : (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    break;
                case Directions.NorthWest:
                    centerX = width - Utils.AXIS_THICKNESS;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (width - Utils.AXIS_THICKNESS) / delimeter;
                    break;
                default:
                    return null;
            }
            currentSeries.RealRects.Clear();

            var points = new List<Point>();

            for (var i = 0; i < values.Count; i++)
            {
                var x = centerX + i * stepX;
                var y = centerY - values[i].CompositeValue.PlainValue * units;
                if (index > 0)
                {
                    var prevs = tuples.Where(s => s.Index < index);
                    var sum = prevs.Sum(sr => sr.Values[i].CompositeValue.PlainValue);
                    y -= sum * units;
                }
                points.Add(new Point(x, y));
                if (!style.In(ChartStyle.StackedLinesWithMarkers, ChartStyle.SmoothStackedLinesWithMarkers)) continue;
                drawMarker(x, y, gm, shapeStyle, currentSeries.RealRects);
            }
            if (style.In(ChartStyle.SmoothStackedLines, ChartStyle.SmoothStackedLinesWithMarkers))
            {
                var bezierSegments = InterpolatePointsWithBezierCurves(points, false);
                if (bezierSegments == null || bezierSegments.Count == 0)
                {
                    var poly = new PolyLineSegment(points, true);
                    gm.Figures.Add(new PathFigure(points[0], new[] { poly }, false));
                }
                else
                {
                    var segments = bezierSegments.Select(bz => new BezierSegment
                    {
                        Point1 = bz.FirstControlPoint,
                        Point2 = bz.SecondControlPoint,
                        Point3 = bz.EndPoint
                    }).Cast<PathSegment>().ToList();
                    gm.Figures.Add(new PathFigure(points[0], segments, false));
                }
            }
            else if (style.In(ChartStyle.StackedLines, ChartStyle.StackedLinesWithMarkers))
            {
                var poly = new PolyLineSegment(points, true);
                gm.Figures.Add(new PathFigure(points[0], new[] { poly }, false));
            }
            return gm;
        }

        private PathGeometry drawRadar(ISeries[] series, int index, ChartStyle chartStyle, double units, double stepLength, double radius, int pointsCount, int linesCount, int zeroLevel, Point centerPoint, ShapeStyle shapeStyle)
        {
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;
            var maxLength = series.Max(s => s.Values.Count);
            var values = currentSeries.Values.Select(v => v.CompositeValue.PlainValue).ToArray();

            var gm = new PathGeometry();
            currentSeries.RealRects.Clear();
            var points = new List<Point>();
            var xBeg = 0.0;
            var yBeg = 0.0;
            var currentDegrees = 0.0;
            var degreesStep = 360.0 / pointsCount;
            var zeroDistance = (linesCount - zeroLevel) * stepLength;

            var segments = new List<PathSegment>();
            for (var i = 0; i < values.Length; i++)
            {
                var distance = radius - (zeroDistance - values[i] * units);

                currentDegrees = 90.0 + i * degreesStep;
                var rads = currentDegrees * Math.PI / 180.0;
                xBeg = centerPoint.X - distance * Math.Cos(rads);
                yBeg = centerPoint.Y - distance * Math.Sin(rads);
                points.Add(new Point(xBeg, yBeg));
                if (chartStyle == ChartStyle.RadarWithMarkers)
                {
                    drawMarker(xBeg, yBeg, gm, shapeStyle, currentSeries.RealRects);
                }
            }
            for (var i = 0; i < points.Count; i++)
            {
                segments.Add(new LineSegment(points[i], true));
            }
            if (currentSeries.Values.Count == maxLength)
                gm.Figures.Add(new PathFigure(points[0], segments, true));
            else
                gm.Figures.Add(new PathFigure(points[0], segments, false));
            return gm;
        }

        private PathGeometry drawLine(double width, double height, int maxX, double units, ChartStyle style, Directions dir, ISeries currentSeries, bool offsetBoundary, double boundOffset, ShapeStyle shapeStyle)
        {
            double stepX;
            double centerX, centerY;
            var gm = new PathGeometry();

            var delimeter = maxX > 1 ? maxX - 1 : 1;

            switch (dir)
            {
                case Directions.NorthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary
                        ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter
                        : (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = width / 2;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / 2 / delimeter;
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = height / 2;
                    stepX = offsetBoundary
                        ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter
                        : (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    break;
                case Directions.SouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter : (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    break;
                case Directions.NorthWest:
                    centerX = width - Utils.AXIS_THICKNESS;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    break;
                default:
                    return null;
            }
            currentSeries.RealRects.Clear();
            var points = new List<Point>();

            for (var i = 0; i < currentSeries.Values.Count; i++)
            {
                var x = centerX + i * stepX;
                var y = centerY - currentSeries.Values[i].CompositeValue.PlainValue * units;

                points.Add(new Point(x, y));
                if (!style.In(ChartStyle.LinesWithMarkers, ChartStyle.SmoothLinesWithMarkers)) continue;
                drawMarker(x, y, gm, shapeStyle, currentSeries.RealRects);
            }
            if (style.In(ChartStyle.SmoothLines, ChartStyle.SmoothLinesWithMarkers))
            {
                var bezierSegments = InterpolatePointsWithBezierCurves(points, false);
                if (bezierSegments == null || bezierSegments.Count == 0)
                {
                    var poly = new PolyLineSegment(points, true);
                    gm.Figures.Add(new PathFigure(points[0], new[] { poly }, false));
                }
                else
                {
                    var segments = bezierSegments.Select(bz => new BezierSegment
                    {
                        Point1 = bz.FirstControlPoint,
                        Point2 = bz.SecondControlPoint,
                        Point3 = bz.EndPoint
                    }).Cast<PathSegment>().ToList();
                    gm.Figures.Add(new PathFigure(points[0], segments, false));
                }
            }
            else if (style == ChartStyle.SmoothArea)
            {
                points.Insert(0, new Point(centerX, centerY));
                points.Add(new Point(points[points.Count - 1].X, centerY));
                var bezierSegments = InterpolatePointsWithBezierCurves(points, false);
                if (bezierSegments == null || bezierSegments.Count == 0)
                {
                    var poly = new PolyLineSegment(points, true);
                    gm.Figures.Add(new PathFigure(points[0], new[] { poly }, true));
                }
                else
                {
                    var segments = bezierSegments.Select(bz => new BezierSegment
                    {
                        Point1 = bz.FirstControlPoint,
                        Point2 = bz.SecondControlPoint,
                        Point3 = bz.EndPoint
                    }).Cast<PathSegment>().ToList();
                    gm.Figures.Add(new PathFigure(points[0], segments, true));
                }
            }
            else if (style.In(ChartStyle.Lines, ChartStyle.LinesWithMarkers))
            {
                var poly = new PolyLineSegment(points, true);
                gm.Figures.Add(new PathFigure(points[0], new[] { poly }, false));
            }
            else if (style.In(ChartStyle.Area))
            {
                points.Insert(0, new Point(centerX, centerY));
                points.Add(new Point(points[points.Count - 1].X, centerY));
                var poly = new PolyLineSegment(points, true);
                gm.Figures.Add(new PathFigure(points[0], new[] { poly }, true));
            }

            return gm;
        }

        private PathGeometry drawBubbles(double width, double height, int maxX, double units, Directions dir, ISeries currentSeries, bool offsetBoundary, double boundOffset)
        {
            double stepX;
            double centerX, centerY;
            var segSize = height > width ? (width - 2 * Utils.AXIS_THICKNESS) / (maxX + 2) / 2 : (height - 2 * Utils.AXIS_THICKNESS) / (maxX + 2) / 2;

            if (boundOffset > 0)
                segSize = boundOffset;

            var gm = new PathGeometry();

            var delimeter = maxX > 1 ? maxX - 1 : 1;

            switch (dir)
            {
                case Directions.NorthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter : (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = width / 2;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / 2 / delimeter;
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = height / 2;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter : (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    break;
                case Directions.SouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / delimeter : (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    break;
                case Directions.NorthWest:
                    centerX = width - Utils.AXIS_THICKNESS;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / delimeter;
                    break;
                default:
                    return null;
            }
            currentSeries.RealRects.Clear();

            for (var i = 0; i < currentSeries.Values.Count; i++)
            {
                var x = centerX + i * stepX;
                var y = centerY - currentSeries.Values[i].CompositeValue.PlainValue * units;

                var es = new EllipseGeometry(new Point(x, y), segSize, segSize);
                gm.AddGeometry(es);
                var dist = Math.Sqrt(Math.Pow(segSize, 2) / 2);
                currentSeries.RealRects.Add(new Rect(new Point(x - dist, y - dist), new Point(x + dist, y + dist)));
            }
            return gm;
        }

        //by Raul Otaño Hurtado and Maxim Shemanarev
        //http://www.codeproject.com/Articles/769055/Interpolate-D-points-usign-Bezier-curves-in-WPF
        //http://www.antigrain.com/research/bezier_interpolation/index.html#PAGE_BEZIER_INTERPOLATION
        private static List<BeizerCurveSegment> InterpolatePointsWithBezierCurves(List<Point> points, bool isClosedCurve)
        {
            if (points.Count < 3)
                return null;
            var toRet = new List<BeizerCurveSegment>();

            //if is close curve then add the first point at the end
            if (isClosedCurve)
                points.Add(points.First());

            for (var i = 0; i < points.Count - 1; i++)   //iterate for points but the last one
            {
                // Assume we need to calculate the control
                // points between (x1,y1) and (x2,y2).
                // Then x0,y0 - the previous vertex,
                //      x3,y3 - the next one.
                var x1 = points[i].X;
                var y1 = points[i].Y;

                var x2 = points[i + 1].X;
                var y2 = points[i + 1].Y;

                double x0;
                double y0;

                if (i == 0) //if is first point
                {
                    if (isClosedCurve)
                    {
                        var previousPoint = points[points.Count - 2];    //last Point, but one (due inserted the first at the end)
                        x0 = previousPoint.X;
                        y0 = previousPoint.Y;
                    }
                    else    //Get some previouse point
                    {
                        var previousPoint = points[i];  //if is the first point the previous one will be it self
                        x0 = previousPoint.X;
                        y0 = previousPoint.Y;
                    }
                }
                else
                {
                    x0 = points[i - 1].X;   //Previous Point
                    y0 = points[i - 1].Y;
                }

                double x3, y3;

                if (i == points.Count - 2)    //if is the last point
                {
                    if (isClosedCurve)
                    {
                        var nextPoint = points[1];  //second Point(due inserted the first at the end)
                        x3 = nextPoint.X;
                        y3 = nextPoint.Y;
                    }
                    else    //Get some next point
                    {
                        var nextPoint = points[i + 1];  //if is the last point the next point will be the last one
                        x3 = nextPoint.X;
                        y3 = nextPoint.Y;
                    }
                }
                else
                {
                    x3 = points[i + 2].X;   //Next Point
                    y3 = points[i + 2].Y;
                }

                var xc1 = (x0 + x1) / 2.0;
                var yc1 = (y0 + y1) / 2.0;
                var xc2 = (x1 + x2) / 2.0;
                var yc2 = (y1 + y2) / 2.0;
                var xc3 = (x2 + x3) / 2.0;
                var yc3 = (y2 + y3) / 2.0;

                var len1 = Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0));
                var len2 = Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
                var len3 = Math.Sqrt((x3 - x2) * (x3 - x2) + (y3 - y2) * (y3 - y2));

                var k1 = len1 / (len1 + len2);
                var k2 = len2 / (len2 + len3);

                var xm1 = xc1 + (xc2 - xc1) * k1;
                var ym1 = yc1 + (yc2 - yc1) * k1;

                var xm2 = xc2 + (xc3 - xc2) * k2;
                var ym2 = yc2 + (yc3 - yc2) * k2;

                const double smoothValue = 0.8;
                // Resulting control points. Here smooth_value is mentioned
                // above coefficient K whose value should be in range [0...1].
                var ctrl1X = xm1 + (xc2 - xm1) * smoothValue + x1 - xm1;
                var ctrl1Y = ym1 + (yc2 - ym1) * smoothValue + y1 - ym1;

                var ctrl2X = xm2 + (xc2 - xm2) * smoothValue + x2 - xm2;
                var ctrl2Y = ym2 + (yc2 - ym2) * smoothValue + y2 - ym2;
                toRet.Add(new BeizerCurveSegment
                {
                    StartPoint = new Point(x1, y1),
                    EndPoint = new Point(x2, y2),
                    FirstControlPoint = i == 0 && !isClosedCurve ? new Point(x1, y1) : new Point(ctrl1X, ctrl1Y),
                    SecondControlPoint = i == points.Count - 2 && !isClosedCurve ? new Point(x2, y2) : new Point(ctrl2X, ctrl2Y)
                });
            }

            return toRet;
        }

        private class BeizerCurveSegment
        {
            /// <summary>
            /// Start point
            /// </summary>
            public Point StartPoint { get; set; }
            /// <summary>
            /// End point
            /// </summary>
            public Point EndPoint { get; set; }
            /// <summary>
            /// First control point
            /// </summary>
            public Point FirstControlPoint { get; set; }
            /// <summary>
            /// Second control point
            /// </summary>
            public Point SecondControlPoint { get; set; }
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
