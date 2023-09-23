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
    /// Draws radar numeric values
    /// </summary>
    public class RadarAxesValuesConverter : IMultiValueConverter
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
                || !Utils.StyleRadar(chartStyle)
                || values[4] is not FontFamily fontFamily
                || values[5] is not double fontSize
                || values[6] is not FontStyle fontStyle
                || values[7] is not FontWeight fontWeight
                || values[8] is not FontStretch fontStretch
                || values[9] is not FlowDirection flowDir
                || values[12] is not int linesCount
                || linesCount < 2
                || values[13] is not string format
                || values[14] is not AutoAdjustmentMode autoAdjust)
                return null;

            var seriesEnumerable = values[2] as IEnumerable<ISeries>;
            var chartSeries = values[15] as IEnumerable<ISeries>;

            if (seriesEnumerable != null && seriesEnumerable.Any())
                seriesEnumerable = seriesEnumerable.Where(s => s.IsVisible);
            if (chartSeries != null && chartSeries.Any())
                chartSeries = chartSeries.Where(s => s.IsVisible);
            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return null;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            var gm = new PathGeometry();
            if (!seriesArray.All(s => s is PlainSeries))
                return null;

            var customValuesHorizontal = values[10] is IEnumerable<string> customEnumerableX ? customEnumerableX.ToArray() : Array.Empty<string>();
            var customValuesVertical = values[11] is IEnumerable<string> customEnumerableY ? customEnumerableY.ToArray() : Array.Empty<string>();

            var currentDegrees = 0.0;
            var pointsCount = seriesArray.Max(s => s.Values.Count);
            var degreesStep = 360.0 / pointsCount;

            var maxCv = customValuesHorizontal.Any() ? customValuesHorizontal.Max(v => (v, v.Length)) : (v: "", Length: 0);
            var number = maxCv.Length > pointsCount.ToString(culture).Length ? maxCv.v : pointsCount.ToString(culture);
            var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);

            var radius = width > height ? (height - 2 * Utils.AXIS_THICKNESS) / 2 : (width - 2 * Utils.AXIS_THICKNESS) / 2;

            var centerPoint = width > height ? new Point((width - 2 * Utils.AXIS_THICKNESS) / 2, radius) : new Point(radius, (height - 2 * Utils.AXIS_THICKNESS) / 2);
            if (width > height)
                radius -= (2 * fmt.Height + 8);
            else
                radius -= (2 * fmt.Width + 8);

            for (var i = 0; i < pointsCount; i++)
            {
                currentDegrees = 90.0 + i * degreesStep;
                var rads = currentDegrees * Math.PI / 180.0;
                var num = customValuesHorizontal.Length > i ? customValuesHorizontal[i] : (i + 1).ToString(culture);
                fmt = new FormattedText(num, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                var x = centerPoint.X - (radius + 4) * Math.Cos(rads);
                var y = centerPoint.Y - (radius + 4) * Math.Sin(rads);
                if (currentDegrees == 90.0)
                {
                    x -= fmt.Width / 2;
                    y -= (fmt.Height + 4);
                }
                else if (currentDegrees == 180.0)
                {
                    x += 4;
                    y -= fmt.Height / 2;
                }
                else if (currentDegrees == 270.0)
                {
                    x -= fmt.Width / 2;
                    y += 4;
                }
                else if (currentDegrees == 360.0)
                {
                    x -= (fmt.Width + 4);
                    y -= fmt.Height / 2;
                }
                else
                {
                    var quadrant = Utils.GetRadarQuadrant(currentDegrees);

                    switch (quadrant)
                    {
                        case Quadrants.UpRight:
                            y -= (fmt.Height / 2);
                            x += 4;
                            break;
                        case Quadrants.DownRight:
                            x += 4;
                            y -= fmt.Height / 2;
                            break;
                        case Quadrants.DownLeft:
                            x -= (fmt.Width + 4);
                            y -= fmt.Height / 2;
                            break;
                        case Quadrants.UpLeft:
                            x -= (fmt.Width + 4);
                            y -= fmt.Height / 2;
                            break;
                    }
                }
                var pt = new Point(x, y);

                var ngm = fmt.BuildGeometry(pt);
                if (flowDir == FlowDirection.RightToLeft)
                    ngm.Transform = new ScaleTransform { ScaleX = -1, CenterX = x + fmt.Width / 2, CenterY = y + fmt.Height / 2 };
                gm.AddGeometry(ngm);
            }

            // draw y-axis values
            var fmtY = new FormattedText("AAA", culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
            var (max, min, realLinesCount, stepSize, _, _, _) = Utils.GetMeasures(chartStyle, seriesArray, linesCount, radius, fmtY.Height, centerPoint);
            linesCount = realLinesCount;

            // fractional stepSize
            if (!Utils.IsInteger(stepSize))
            {
                var fractions = Utils.GetDecimalPlaces(stepSize);
                if (format.EndsWith("%"))
                    format = $"{format.Substring(0, format.Length - 1)}{culture.NumberFormat.NumberDecimalSeparator}{new string('0', fractions)}%";
                else
                {
                    var (numericPart, literalPart) = Utils.GetFormatParts(format, culture);
                    format = $"0{culture.NumberFormat.NumberDecimalSeparator}{new string('0', fractions)}" + literalPart;
                }
            }

            var stepY = radius / linesCount;
            var xY = centerPoint.X - 4;
            var yY = centerPoint.Y - radius;
            var yNumber = max;
            for (var i = 0; i < linesCount + 1; i++)
            {
                var text = customValuesVertical.Length > i ? customValuesVertical[i] : yNumber.ToString(format);
                fmt = new FormattedText(text, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip)
                {
                    TextAlignment = flowDir == FlowDirection.RightToLeft ? TextAlignment.Left : TextAlignment.Right
                };
                var pt = new Point(xY - 8, yY - fmt.Height / 2);
                var ngm = fmt.BuildGeometry(pt);
                if (flowDir == FlowDirection.RightToLeft)
                    ngm.Transform = new ScaleTransform { ScaleX = -1, CenterX = pt.X - 8 + fmt.Width / 2, CenterY = pt.Y + fmt.Height / 2 };

                gm.AddGeometry(ngm);
                yNumber -= stepSize;
                yY += stepY;
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
