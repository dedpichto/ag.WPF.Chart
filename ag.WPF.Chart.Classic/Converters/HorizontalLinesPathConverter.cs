using ag.WPF.Chart.Series;
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
    /// Prepares geometry for drawing horizontal dotted lines
    /// </summary>
    public class HorizontalLinesPathConverter : IMultiValueConverter
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
                || values[5] is not FontFamily fontFamily
                || values[6] is not double fontSize
                || values[7] is not FontStyle fontStyle
                || values[8] is not FontWeight fontWeight
                || values[9] is not FontStretch fontStretch
                || values[10] is not AutoAdjustmentMode autoAdjust
                || values[11] is not double maxY)
                return null;

            var seriesEnumerable = values[2] as IEnumerable<ISeries>;
            var chartSeries = values[12] as IEnumerable<ISeries>;

            if (seriesEnumerable != null && seriesEnumerable.Any())
                seriesEnumerable = seriesEnumerable.Where(s => s.IsVisible);
            if (chartSeries != null && chartSeries.Any())
                chartSeries = chartSeries.Where(s => s.IsVisible);
            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return null;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();


            Directions dir;

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return null;
                var totalValues = chartStyle != ChartStyle.Waterfall
                   ? seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue))
                   : seriesArray[0].Values.Select(v => v.CompositeValue.PlainValue);
                dir = Utils.GetDirection(totalValues, chartStyle);
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
            }
            else
                return null;

            int limit;
            var gm = new PathGeometry();

            var radius = 0.0;
            var centerX = 0.0;
            Point centerPoint;

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
            centerPoint = new Point(centerX, radius);

            height -= 2 * Utils.AXIS_THICKNESS;
            width -= 2 * Utils.AXIS_THICKNESS;

            var fmtY = new FormattedText("AAA", culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);

            var (max, min, realLinesCount, stepSize, stepLength, units, zeroPoint) = autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Vertical)
                ? Utils.GetMeasures(
                   chartStyle,
                   seriesArray,
                   linesCount,
                   radius,
                   fmtY.Height,
                   centerPoint,
                   dir == Directions.NorthEastSouthEast)
                : (maxY, -maxY, linesCount, maxY / linesCount, radius / linesCount, radius / maxY, default);

            if (Utils.StyleFullStacked(chartStyle))
                realLinesCount = 10;

            switch (dir)
            {
                case Directions.NorthEastSouthEast:
                    if (autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Horizontal) || !Utils.StyleBars(chartStyle))
                    {
                        limit = realLinesCount * 2;
                        if (Utils.StyleFullStacked(chartStyle))
                            stepLength = height / 20;
                    }
                    else
                    {
                        limit = linesCount * 2;
                        stepLength = height / 2 / linesCount;
                    }

                    break;
                case Directions.NorthEast:
                case Directions.NorthEastNorthWest:
                case Directions.NorthWest:
                case Directions.SouthEast:
                    if (autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Horizontal) || !Utils.StyleBars(chartStyle))
                    {
                        limit = realLinesCount;
                        if (Utils.StyleFullStacked(chartStyle))
                            stepLength = height / 10;
                    }
                    else
                    {
                        limit = linesCount;
                        stepLength = height / linesCount;
                    }
                    break;
                default:
                    return null;
            }
            var y = Utils.AXIS_THICKNESS;
            for (var i = 0; i <= limit; i++)
            {
                var start = new Point(Utils.AXIS_THICKNESS, y);
                var end = new Point(width, y);
                gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                y += stepLength;
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
