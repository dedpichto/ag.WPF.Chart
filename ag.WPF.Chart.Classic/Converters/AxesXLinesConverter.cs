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
    /// Prepares geometry for drawing x- and y- axes lines
    /// </summary>
    public class AxesXLinesConverter : IMultiValueConverter
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
                || values[4] is not ChartStyle chartStyle)
                return null;

            var seriesEnumerable = values[2] as IEnumerable<ISeries>;
            var chartSeries = values[3] as IEnumerable<ISeries>;

            if (seriesEnumerable != null && seriesEnumerable.Any())
                seriesEnumerable = seriesEnumerable.Where(s => s.IsVisible);
            if (chartSeries != null && chartSeries.Any())
                chartSeries = chartSeries.Where(s => s.IsVisible);
            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return null;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            var gm = new PathGeometry();
            Directions dir;

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return null;
                var totalValues = chartStyle.In(ChartStyle.Waterfall)
                    ? seriesArray.First().Values.Select(v => v.CompositeValue.PlainValue)
                    : seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue));
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

                var totalValues = seriesArray.First().Values.Select(v => (v.CompositeValue.HighValue, v.CompositeValue.LowValue));
                dir = Utils.GetDirectionFinancial(totalValues);
            }
            else
                return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    gm.Figures.Add(new PathFigure(new Point(Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), new[] { new LineSegment(new Point(width - Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), true) }, false));
                    break;
                case Directions.NorthEastNorthWest:
                    gm.Figures.Add(new PathFigure(new Point(Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), new[] { new LineSegment(new Point(width - Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), true) }, false));
                    break;
                case Directions.NorthEastSouthEast:
                    gm.Figures.Add(new PathFigure(new Point(Utils.AXIS_THICKNESS, height / 2), new[] { new LineSegment(new Point(width - Utils.AXIS_THICKNESS, height / 2), true) }, false));
                    break;
                case Directions.SouthEast:
                    gm.Figures.Add(new PathFigure(new Point(Utils.AXIS_THICKNESS, Utils.AXIS_THICKNESS), new[] { new LineSegment(new Point(width - Utils.AXIS_THICKNESS, Utils.AXIS_THICKNESS), true) }, false));
                    break;
                case Directions.NorthWest:
                    gm.Figures.Add(new PathFigure(new Point(Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), new[] { new LineSegment(new Point(width - Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), true) }, false));
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
