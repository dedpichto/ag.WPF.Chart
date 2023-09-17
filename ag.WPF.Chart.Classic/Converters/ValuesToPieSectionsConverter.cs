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
    /// Prepares <see cref="DrawingGroup"/> to be used for drawing chart, when chart style is set to <see cref="ChartStyle.SolidPie"/> or <see cref="ChartStyle.SlicedPie"/> or <see cref="ag.WPF.Chart.ChartStyle.Doughnut"/>
    /// </summary>
    public class ValuesToPieSectionsConverter : IMultiValueConverter
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
                || values[3] is not Brush backgroundBrush
                || values[4] is not ChartStyle chartStyle
                || !chartStyle.In(ChartStyle.SlicedPie, ChartStyle.SolidPie, ChartStyle.Doughnut)
                || values[5] is not string format)
                return null;

            var seriesEnumerable = values[2] as IEnumerable<ISeries>;
            var chartSeries = values[6] as IEnumerable<ISeries>;

            if (seriesEnumerable != null && seriesEnumerable.Any())
                seriesEnumerable = seriesEnumerable.Where(s => s.IsVisible);
            if (chartSeries != null && chartSeries.Any())
                chartSeries = chartSeries.Where(s => s.IsVisible);
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
                return null;
            }

            var brushIndex = 0;
            var radius = width > height ? (height - 2 * Utils.AXIS_THICKNESS) / 2 : (width - 2 * Utils.AXIS_THICKNESS) / 2;
            var series = seriesArray.First();
            var realPoints = series.Values.ToArray();
            var pts = series.Values.Where(v=>v.IsVisible).ToArray();
            var sum = pts.Sum(p => Math.Abs(p.CompositeValue.PlainValue));
            var currentDegrees = 90.0;
            var startPoint = new Point(radius, radius);
            var xBeg = radius;
            var yBeg = 0.0;

            var dGroup = new DrawingGroup();

            if (format.EndsWith("%")) format = format.Substring(0, format.Length - 1);
            if (pts.Length == 1)
            {
                for(var i = 0; i < realPoints.Length; i++)
                {
                    if (realPoints[i].IsVisible)
                    {
                        brushIndex = i;
                        break;
                    }
                }
                if (brushIndex == Statics.PredefinedMainBrushes.Length) brushIndex = 0;
                var sectorData = $"{pts[0].CompositeValue.PlainValue} ({100.ToString(format, culture)}%)";
                if (!string.IsNullOrEmpty(pts[0].CustomValue))
                    sectorData += $"\n{pts[0].CustomValue}";
                var ellipseGeometry = new EllipseGeometry(new Rect(new Point(0, 0), new Point(radius * 2, radius * 2)));
                var combinedGeometry = new CombinedGeometry
                {
                    GeometryCombineMode = GeometryCombineMode.Exclude,
                    Geometry1 = ellipseGeometry
                };
                if (chartStyle == ChartStyle.Doughnut)
                    combinedGeometry.Geometry2 = new EllipseGeometry(startPoint, radius / 2, radius / 2);

                combinedGeometry.SetValue(Statics.SectorDataProperty, sectorData);

                var gmDrawing = new GeometryDrawing
                {
                    Brush = Statics.PredefinedMainBrushes[brushIndex].Brush,
                    Pen = new Pen(Statics.PredefinedMainBrushes[brushIndex].Brush, 1),
                    Geometry = combinedGeometry
                };
                dGroup.Children.Add(gmDrawing);
                return dGroup;
            }

            var lines = new List<LineGeometry>();
            foreach (var pt in realPoints)
            {
                if (brushIndex == Statics.PredefinedMainBrushes.Length) brushIndex = 0;
                if(!pt.IsVisible)
                {
                    brushIndex++;
                    continue;
                }
                var addition = Math.Abs(pt.CompositeValue.PlainValue) / (sum != 0 ? sum : 1) * 360.0;
                if (currentDegrees <= 90.0 && currentDegrees > 0)
                {
                    if (currentDegrees - addition >= 0)
                    {
                        currentDegrees -= addition;
                    }
                    else
                    {
                        currentDegrees = 360.0 - Math.Abs(currentDegrees - addition);
                    }
                }
                else if (Math.Abs(currentDegrees) < double.Epsilon)
                {
                    currentDegrees = 360.0 - addition;
                }
                else
                {
                    currentDegrees -= addition;
                }

                var gmDrawing = new GeometryDrawing
                {
                    Brush = Statics.PredefinedMainBrushes[brushIndex].Brush,
                    Pen = new Pen(Statics.PredefinedMainBrushes[brushIndex].Brush, 1)
                };
                brushIndex++;

                var pathGeometry = new PathGeometry();
                var segments = new List<PathSegment>();

                var start = new Point(xBeg, yBeg);
                segments.Add(new LineSegment(start, true));

                var rads = currentDegrees * Math.PI / 180.0;
                var quadrant = Utils.GetQuadrant(currentDegrees);
                switch (quadrant)
                {
                    case Quadrants.UpRight:
                        xBeg = radius + radius * Math.Abs(Math.Cos(rads));
                        yBeg = radius - radius * Math.Abs(Math.Sin(rads));
                        break;
                    case Quadrants.DownRight:
                        xBeg = radius + radius * Math.Abs(Math.Cos(rads));
                        yBeg = radius + radius * Math.Abs(Math.Sin(rads));
                        break;
                    case Quadrants.DownLeft:
                        xBeg = radius - radius * Math.Abs(Math.Cos(rads));
                        yBeg = radius + radius * Math.Abs(Math.Sin(rads));
                        break;
                    case Quadrants.UpLeft:
                        xBeg = radius - radius * Math.Abs(Math.Cos(rads));
                        yBeg = radius - radius * Math.Abs(Math.Sin(rads));
                        break;
                }

                var arcSeg = new ArcSegment
                {
                    Point = new Point(xBeg, yBeg),
                    Size = new Size(radius, radius),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = addition >= 180.0
                };
                segments.Add(arcSeg);


                segments.Add(new LineSegment(startPoint, true));

                var figure = new PathFigure(startPoint, segments, true);
                pathGeometry.Figures.Add(figure);

                var combinedGeometry = new CombinedGeometry
                {
                    GeometryCombineMode = GeometryCombineMode.Exclude,
                    Geometry1 = pathGeometry
                };
                if (chartStyle == ChartStyle.Doughnut)
                    combinedGeometry.Geometry2 = new EllipseGeometry(startPoint, radius / 2, radius / 2);

                var perc = Math.Abs(pt.CompositeValue.PlainValue) / (sum != 0 ? sum : 1) * 100;
                var sectorData = $"{pt.CompositeValue.PlainValue} ({perc.ToString(format, culture)}%)";
                if (!string.IsNullOrEmpty(pt.CustomValue))
                    sectorData += $"\n{pt.CustomValue}";
                combinedGeometry.SetValue(Statics.SectorDataProperty, sectorData);
                gmDrawing.Geometry = combinedGeometry;
                dGroup.Children.Add(gmDrawing);

                if (chartStyle == ChartStyle.SlicedPie)
                    lines.Add(new LineGeometry(startPoint, new Point(xBeg, yBeg)));
            }

            if (chartStyle != ChartStyle.SlicedPie) return dGroup;
            foreach (var lg in lines)
                dGroup.Children.Add(new GeometryDrawing
                {
                    Brush = backgroundBrush,
                    Pen = new Pen(backgroundBrush, 4),
                    Geometry = lg
                });

            return dGroup;
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
