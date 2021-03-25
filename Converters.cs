// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

using ag.WPF.Chart.Values;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ag.WPF.Chart
{
    internal enum Directions
    {
        None,
        NorthEast,
        NorthEastNorthWest,
        NorthEastSouthEast,
        SouthEast,
        NorthWest
    }

    internal enum Quadrants
    {
        UpRight,
        DownRight,
        DownLeft,
        UpLeft
    }

    internal struct ZeroPoint
    {
        internal Point Point;
        internal int Level;
    }

    internal static class Utils
    {
        internal const double AXIS_THICKNESS = 0.5;
        internal const double COLUMN_BAR_OFFSET = 16.0;
        internal const double RECT_SIZE = 3.0;

        internal static Border Border { get; } = new Border();

        private static int[] _bases = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        internal static Quadrants GetQuadrant(double degrees)
        {
            if (degrees >= 0 && degrees <= 90)
                return Quadrants.UpRight;
            if (degrees <= 360 && degrees >= 270)
                return Quadrants.DownRight;
            if (degrees <= 270 && degrees >= 180)
                return Quadrants.DownLeft;
            return Quadrants.UpLeft;
        }

        internal static Quadrants GetRadarQuadrant(double degrees)
        {
            if (degrees >= 0 && degrees <= 90)
                return Quadrants.UpLeft;
            if (degrees <= 360 && degrees >= 270)
                return Quadrants.DownLeft;
            if (degrees <= 270 && degrees >= 180)
                return Quadrants.DownRight;
            if (degrees > 360)
                return Quadrants.UpLeft;
            return Quadrants.UpRight;
        }

        private static IEnumerable<double> calculatedSteps(int power, double max)
        {
            var powers = new List<int>();
            for (var i = 0; i <= power; i++)
            {
                powers.Add(i);
            }
            foreach (var b in _bases)
                foreach (var p in powers)
                    yield return max / (b * (int)Math.Pow(10, p));
        }

        internal static (double max, double min, int linesCount, double step, double stepLength, double units, ZeroPoint zeroPoint) GetMeasuresForPositive(double max, double min, int linesCount, double radius, double fontHeight, bool autoAdjust, ZeroPoint centerPoint)
        {
            var stepSize = 0.0;
            var stepLength = 0.0;
            var units = 0.0;
            // round max to next integer
            max = Math.Ceiling(max);
            var power = Math.Abs((int)max).ToString().Length - 1;

            if (autoAdjust)
            {
                // do not increase max for integers that are equal to 10 power
                if (max % Math.Pow(10, power) != 0)
                    max = Math.Sign(max) * roundInt((int)Math.Abs(max), (int)Math.Pow(10, power));
                // store max and min difference
                var diff = getDiff(max, min);
                // get all available integer lines counts
                var lines = calculatedSteps(power, max).Where(l => isInteger(l)).OrderBy(l => l).Distinct().ToArray();
                // calculate real size for each step
                var sizes = lines.Select(s => radius / s).ToArray();
                // get the largest step with real size more/equal font height
                var item = sizes.Select((size, index) => new { size, index }).LastOrDefault(a => a.size >= fontHeight + 4);
                if (item != null)
                {
                    // change lines count to selected one
                    linesCount = (int)lines[item.index];
                    // prepare step
                    stepSize = diff / linesCount;
                    stepLength = radius / linesCount;
                    // prepare units
                    units = Math.Abs(radius / diff);
                }
                else
                {
                    stepSize = diff / linesCount;
                    while (!isInteger(stepSize))
                    {
                        max = Math.Sign(max) * roundInt((int)Math.Abs(max), (int)Math.Pow(10, power));
                        diff = getDiff(max, min);
                        stepSize = diff / linesCount;
                    }
                    stepLength = radius / linesCount;
                    units = Math.Abs(radius / diff);
                }
            }
            else
            {
                // store max and min difference
                var diff = getDiff(max, min);
                stepSize = diff / linesCount;
                while (!isInteger(stepSize))
                {
                    max = Math.Sign(max) * roundInt((int)Math.Abs(max), (int)Math.Pow(10, power));
                    diff = getDiff(max, min);
                    stepSize = diff / linesCount;
                }
                stepLength = radius / linesCount;
                units = Math.Abs(radius / diff);
            }

            // zero point Y-coordinate is stays the same and leve stays 0

            return (max, min, linesCount, stepSize, stepLength, units, centerPoint);
        }

        internal static (double max, double min, int linesCount, double stepSize, double stepLength, double units, ZeroPoint zeroPoint) GetMeasuresForNegative(double max, double min, int linesCount, double radius, double fontHeight, bool autoAdjust, ZeroPoint zeroPoint)
        {
            var stepSize = 0.0;
            var stepLength = 0.0;
            var units = 0.0;
            // round min to prevous integer
            min = Math.Floor(min);
            var power = Math.Abs((int)min).ToString().Length - 1;

            if (autoAdjust)
            {
                // do not increase max for integers that are equal to 10 power
                if (min % Math.Pow(10, power) != 0)
                    min = Math.Sign(min) * roundInt((int)Math.Abs(min), (int)Math.Pow(10, power));
                // store max and min difference
                var diff = getDiff(max, min);
                // get all available integer lines counts
                var lines = calculatedSteps(power, Math.Abs(min)).Where(l => isInteger(l)).OrderBy(l => l).Distinct().ToArray();
                // calculate real size for each step
                var sizes = lines.Select(s => radius / s).ToArray();
                // get the largest step with real size more/equal font height
                var item = sizes.Select((size, index) => new { size, index }).LastOrDefault(a => a.size >= fontHeight + 4);
                if (item != null)
                {
                    // change lines count to selected one
                    linesCount = (int)lines[item.index];
                    // prepare step
                    stepSize = diff / linesCount;
                    stepLength = radius / linesCount;
                    // prepare units
                    units = Math.Abs(radius / diff);
                }
                else
                {
                    stepSize = diff / linesCount;
                    while (!isInteger(stepSize))
                    {
                        min = Math.Sign(min) * roundInt((int)Math.Abs(min), (int)Math.Pow(10, power));
                        diff = getDiff(max, min);
                        stepSize = diff / linesCount;
                    }
                    units = Math.Abs(radius / diff);
                    stepLength = radius / linesCount;
                }
            }
            else
            {
                // store max and min difference
                var diff = getDiff(max, min);
                stepSize = diff / linesCount;
                while (!isInteger(stepSize))
                {
                    min = Math.Sign(min) * roundInt((int)Math.Abs(min), (int)Math.Pow(10, power));
                    diff = getDiff(max, min);
                    stepSize = diff / linesCount;
                }
                units = Math.Abs(radius / diff);
                stepLength = radius / linesCount;
            }
            // find zero point
            zeroPoint.Point.Y -= stepSize * linesCount * units;
            zeroPoint.Level = linesCount;

            return (max, min, linesCount, stepSize, stepLength, units, zeroPoint);
        }

        internal static (double max, double min, int linesCount, double stepSize, double stepLength, double units, ZeroPoint zeroPoint) GetMeasuresForComplex(double max, double min, int linesCount, double radius, double fontHeight, bool autoAdjust, ZeroPoint zeroPoint)
        {
            var stepSize = 0.0;
            var stepLength = 0.0;
            var units = 0.0;

            // round max to next integer
            max = Math.Ceiling(max);
            var powerMax = Math.Abs((int)max).ToString().Length - 1;

            // round min to prevous integer
            min = Math.Floor(min);
            var powerMin = Math.Abs((int)min).ToString().Length - 1;

            // do not increase max for integers that are equal to 10 power
            if (max % Math.Pow(10, powerMax) != 0)
                max = Math.Sign(max) * roundInt((int)Math.Abs(max), (int)Math.Pow(10, powerMax));
            // do not increase max for integers that are equal to 10 power
            if (min % Math.Pow(10, powerMin) != 0)
                min = Math.Sign(min) * roundInt((int)Math.Abs(min), (int)Math.Pow(10, powerMin));

            if (autoAdjust)
            {
                // store max and min difference
                var diff = getDiff(max, min);
                // get all available integer lines counts
                var lines = calculatedSteps(Math.Max(powerMax, powerMin), Math.Abs(diff)).Where(l => isInteger(l)).OrderBy(l => l).Distinct().ToArray();
                // calculate real size for each step
                var sizes = lines.Select(s => radius / s).ToArray();
                // get the largest step with real size more/equal font height
                var item = sizes.Select((size, index) => new { size, index }).LastOrDefault(a => a.size >= fontHeight + 4);
                if (item != null)
                {
                    // change lines count to selected one
                    linesCount = (int)lines[item.index];
                    // prepare step
                    stepSize = diff / linesCount;
                    stepLength = radius / linesCount;
                    // prepare units
                    units = Math.Abs(radius / diff);
                }
                else
                {
                    if (Math.Abs(max) > Math.Abs(min))
                    {
                        var sign = Math.Sign(min);
                        var prevMin = Math.Abs(min);
                        min = 0;
                        linesCount--;
                        diff = getDiff(max, min);
                        stepSize = diff / linesCount;
                        while (!isInteger(stepSize) || stepSize < prevMin)
                        {
                            max = Math.Sign(max) * roundInt((int)Math.Abs(max), (int)Math.Pow(10, powerMax));
                            diff = getDiff(max, min);
                            stepSize = diff / linesCount;
                        }
                        min = sign * stepSize;
                        stepLength = radius / linesCount;
                        units = Math.Abs(radius / (diff + Math.Abs(min)));
                    }
                    else
                    {
                        var sign = Math.Sign(max);
                        var prevMax = Math.Abs(max);
                        max = 0;
                        linesCount--;
                        diff = getDiff(max, min);
                        stepSize = diff / linesCount;
                        while (!isInteger(stepSize) || stepSize < prevMax)
                        {
                            min = Math.Sign(min) * roundInt((int)Math.Abs(min), (int)Math.Pow(10, powerMin));
                            diff = getDiff(max, min);
                            stepSize = diff / linesCount;
                        }
                        max = sign * stepSize;
                        stepLength = radius / linesCount;
                        units = Math.Abs(radius / (diff + max));
                    }
                }
            }
            else
            {
                // store max and min difference
                var diff = getDiff(max, min);
                if (Math.Abs(max) > Math.Abs(min))
                {
                    var sign = Math.Sign(min);
                    var prevMin = Math.Abs(min);
                    min = 0;
                    linesCount--;
                    diff = getDiff(max, min);
                    stepSize = diff / linesCount;
                    while (!isInteger(stepSize) || stepSize < prevMin)
                    {
                        max = Math.Sign(max) * roundInt((int)Math.Abs(max), (int)Math.Pow(10, powerMax));
                        diff = getDiff(max, min);
                        stepSize = diff / linesCount;
                    }
                    min = sign * stepSize;
                    stepLength = radius / linesCount;
                    units = Math.Abs(radius / (diff + Math.Abs(min)));
                }
                else
                {
                    var sign = Math.Sign(max);
                    var prevMax = Math.Abs(max);
                    max = 0;
                    linesCount--;
                    diff = getDiff(max, min);
                    stepSize = diff / linesCount;
                    while (!isInteger(stepSize) || stepSize < prevMax)
                    {
                        min = Math.Sign(min) * roundInt((int)Math.Abs(min), (int)Math.Pow(10, powerMin));
                        diff = getDiff(max, min);
                        stepSize = diff / linesCount;
                    }
                    max = sign * stepSize;
                    stepLength = radius / linesCount;
                    units = Math.Abs(radius / (diff + max));
                }
            }

            // find zero point
            var temp = min;
            while (temp < 0)
            {
                temp += stepSize;
                zeroPoint.Point.Y -= stepSize * units;
                zeroPoint.Level++;
            }

            return (max, min, linesCount, stepSize, stepLength, units, zeroPoint);
        }

        internal static (double max, double min, int linesCount, double stepSize, double stepLength, double units, ZeroPoint zeroPoint) GetMeasures(Series[] seriesArray, int linesCount, double radius, double fontHeight, bool autoAdjust, Point centerPoint)
        {
            var values = seriesArray.SelectMany(s => s.Values.Select(v => v.Value.V1));
            var max = values.Max();
            var min = values.Min();

            if (values.All(v => v > 0))
                return GetMeasuresForPositive(max, 0, linesCount, radius, fontHeight, autoAdjust, new ZeroPoint { Point = centerPoint });
            else if (values.All(v => v < 0))
                return GetMeasuresForNegative(0, min, linesCount, radius, fontHeight, autoAdjust, new ZeroPoint { Point = centerPoint });
            else
            {
                return GetMeasuresForComplex(max, min, linesCount, radius, fontHeight, autoAdjust, new ZeroPoint { Point = centerPoint });
            }
        }

        private static bool isInteger(double step)
        {
            return Math.Abs(step % 1) <= (double.Epsilon * 100);
        }

        private static int roundInt(int number, int tense)
        {
            // Smaller multiple
            int a = number / tense * tense;

            // Larger multiple
            int b = a + tense;

            // Return of closest of two
            //return (number - a < b - number) ? a : b;
            return b;
        }

        private static double getDiff(double max, double min)
        {
            if (max >= 0 && min >= 0)
                return Math.Abs(max - min);
            else if (max < 0 && min > 0)
                return Math.Abs(max) + min;
            else if (max > 0 && min < 0)
                return max + Math.Abs(min);
            else
                return Math.Abs(max + min);
        }

        internal static Tuple<double, int> Limits(ChartStyle style, bool offsetBoundary, int stopsX, int ticks,
            double boundOffset, double width)
        {
            if (StyleColumns(style))
            {
                return Tuple.Create(width / ticks, ticks);
            }
            if (StyleBars(style))
            {
                return Tuple.Create(width / stopsX, stopsX);
            }
            return offsetBoundary
                ? Tuple.Create((width - 2 * boundOffset) / (ticks - 1), ticks)
                : Tuple.Create(width / (ticks - 1), ticks);
        }

        internal static double BoundaryOffset(bool offsetBoundary, double width, int count)
        {
            return offsetBoundary ? (width - 2 * AXIS_THICKNESS) / (count + 2) / 2 : 0;
        }

        internal static bool StyleBars(ChartStyle style)
        {
            return style.In(ChartStyle.Bars, ChartStyle.StackedBars, ChartStyle.FullStackedBars);
        }

        internal static bool StyleColumns(ChartStyle style)
        {
            return style.In(ChartStyle.Columns, ChartStyle.StackedColumns, ChartStyle.FullStackedColumns, ChartStyle.Waterfall);
        }

        internal static bool StyleLines(ChartStyle style)
        {
            return style.In(ChartStyle.Lines, ChartStyle.StackedLines, ChartStyle.FullStackedLines,
                       ChartStyle.LinesWithMarkers, ChartStyle.StackedLinesWithMarkers,
                       ChartStyle.FullStackedLinesWithMarkers, ChartStyle.SmoothLines,
                       ChartStyle.SmoothStackedLines, ChartStyle.SmoothFullStackedLines,
                       ChartStyle.SmoothLinesWithMarkers, ChartStyle.SmoothStackedLinesWithMarkers,
                       ChartStyle.SmoothFullStackedLinesWithMarkers);
        }

        internal static bool OffsetBoundary(ChartBoundary boundary, ChartStyle style)
        {
            return boundary == ChartBoundary.WithOffset &&
                   style.In(ChartStyle.Lines, ChartStyle.StackedLines, ChartStyle.FullStackedLines,
                       ChartStyle.LinesWithMarkers, ChartStyle.StackedLinesWithMarkers,
                       ChartStyle.FullStackedLinesWithMarkers, ChartStyle.SmoothLines,
                       ChartStyle.SmoothStackedLines, ChartStyle.SmoothFullStackedLines,
                       ChartStyle.SmoothLinesWithMarkers, ChartStyle.SmoothStackedLinesWithMarkers,
                       ChartStyle.SmoothFullStackedLinesWithMarkers, ChartStyle.Bubbles);
        }

        internal static double GetMaxY(List<Tuple<List<ChartValue>, int>> tuples, ChartStyle style)
        {
            if (!tuples.Any()) return 0.0;
            var result = 0.0;
            switch (style)
            {
                case ChartStyle.Waterfall:
                    var maxPlus = 0.0;
                    var maxMinus = 0.0;
                    var value = 0.0;
                    foreach (var v in tuples[0].Item1.Select(v => v.Value.V1))
                    {
                        value += v;
                        if (value > 0)
                            maxPlus = Math.Max(maxPlus, value);
                        if (value < 0)
                            maxMinus = Math.Min(maxMinus, value);
                    }
                    result = Math.Max(maxPlus, Math.Abs(maxMinus));
                    break;
                case ChartStyle.Lines:
                case ChartStyle.LinesWithMarkers:
                case ChartStyle.SmoothLines:
                case ChartStyle.SmoothLinesWithMarkers:
                case ChartStyle.Columns:
                case ChartStyle.Bars:
                case ChartStyle.Area:
                case ChartStyle.Bubbles:
                    result = (from s in tuples from v in s.Item1 select Math.Abs(v.Value.V1)).Concat(new[] { result }).Max();
                    break;
                case ChartStyle.StackedColumns:
                case ChartStyle.StackedBars:
                    {
                        var plusArr = new double[tuples.Max(s => s.Item1.Count)];
                        var minusArr = new double[tuples.Max(s => s.Item1.Count)];
                        foreach (var s in tuples)
                        {
                            for (var i = 0; i < s.Item1.Count; i++)
                            {
                                if (s.Item1[i].Value.V1 < 0)
                                    minusArr[i] += s.Item1[i].Value.V1;
                                else
                                    plusArr[i] += s.Item1[i].Value.V1;
                            }
                        }
                        result = Math.Max(Math.Abs(minusArr.Min()), plusArr.Max());
                        break;
                    }
                case ChartStyle.StackedArea:
                case ChartStyle.StackedLines:
                case ChartStyle.StackedLinesWithMarkers:
                case ChartStyle.SmoothStackedLines:
                case ChartStyle.SmoothStackedLinesWithMarkers:
                    {
                        var arr1 = new double[tuples.Max(s => s.Item1.Count)];
                        var arr2 = new double[tuples.Max(s => s.Item1.Count)];
                        foreach (var s in tuples)
                        {
                            for (var i = 0; i < s.Item1.Count; i++)
                            {
                                arr1[i] += s.Item1[i].Value.V1;
                                if (Math.Abs(arr2[i]) < Math.Abs(s.Item1[i].Value.V1))
                                    arr2[i] = Math.Abs(s.Item1[i].Value.V1);
                            }
                        }
                        result = Math.Max(arr2.Max(), arr1.Max(d => Math.Abs(d)));
                        break;
                    }
                case ChartStyle.FullStackedColumns:
                case ChartStyle.FullStackedBars:
                    {
                        var arr = new double[tuples.Max(s => s.Item1.Count)];
                        foreach (var s in tuples)
                        {
                            for (var i = 0; i < s.Item1.Count; i++)
                            {
                                var i1 = i;
                                var sumTotal =
                                    tuples.Where(sr => sr.Item1.Count > i1).Sum(sr => Math.Abs(sr.Item1[i1].Value.V1));
                                var percent = Math.Abs(s.Item1[i].Value.V1) / sumTotal * 100;
                                if (arr[i] < percent)
                                    arr[i] = percent;
                            }
                        }
                        result = arr.Max();
                        break;
                    }
                case ChartStyle.FullStackedArea:
                case ChartStyle.FullStackedLines:
                case ChartStyle.FullStackedLinesWithMarkers:
                case ChartStyle.SmoothFullStackedLines:
                case ChartStyle.SmoothFullStackedLinesWithMarkers:
                    {
                        return 100;
                        //var arr = new double[tuples.Max(s => s.Item1.Count)];
                        //for (var i = 0; i < arr.Length; i++)
                        //{
                        //    var sum = tuples.Where(sr => sr.Item1.Count > i).Sum(sr => Math.Abs(sr.Item1[i].Value));
                        //    foreach (var s in tuples)
                        //    {
                        //        var percent = Math.Abs(s.Item1[i].Value) / sum * 100;
                        //        if (arr[i] < percent)
                        //            arr[i] = percent;
                        //    }
                        //}
                        //result = arr.Max();
                        //break;
                    }
            }
            var upper = (int)Math.Ceiling(result);
            if (result > 10)
            {
                while (upper % 10 != 0)
                    upper++;
            }
            result = upper;
            return result;
        }

        internal static Directions GetDirection(double[] totalValues, ChartStyle style)
        {
            if (style == ChartStyle.Waterfall)
            {
                var sum = 0.0;
                var positive = false;
                var negative = false;
                foreach (var v in totalValues)
                {
                    sum += v;
                    if (sum > 0)
                        positive = true;
                    else if (sum < 0)
                        negative = true;
                }
                if (positive && negative)
                    return Directions.NorthEastSouthEast;
                if (positive)
                    return Directions.NorthEast;
                if (negative)
                    return Directions.SouthEast;
                return Directions.NorthEast;
            }
            if (totalValues.All(v => v >= 0))
                return Directions.NorthEast;
            if (totalValues.All(v => v < 0))
                return style.In(ChartStyle.Bars, ChartStyle.StackedBars, ChartStyle.FullStackedBars)
                    ? Directions.NorthWest
                    : Directions.SouthEast;
            return style.In(ChartStyle.Bars, ChartStyle.StackedBars, ChartStyle.FullStackedBars)
                ? Directions.NorthEastNorthWest
                : Directions.NorthEastSouthEast;
        }

        internal static bool In<T>(this T t, params T[] values)
        {
            return values.Contains(t);
        }
    }

    /// <summary>
    /// Prepares geometry for drawing x- and y- axes lines
    /// </summary>
    public class AxesLinesConverter : IMultiValueConverter
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
                || values.Length != 5
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is IEnumerable<Series> seriesEnumerable)
                || !(values[3] is ChartStyle style))
                return null;

            var series = seriesEnumerable.ToArray();
            var gm = new PathGeometry();
            var totalValues = (from s in series from v in s.Values select v.Value.V1).ToArray();
            var dir = Utils.GetDirection(totalValues, style);

            switch (dir)
            {
                case Directions.NorthEast:
                    gm.Figures.Add(new PathFigure(new Point(Utils.AXIS_THICKNESS, Utils.AXIS_THICKNESS), new[] { new LineSegment(new Point(Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), true) }, false));
                    gm.Figures.Add(new PathFigure(new Point(Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), new[] { new LineSegment(new Point(width - Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), true) }, false));
                    break;
                case Directions.NorthEastNorthWest:
                    gm.Figures.Add(new PathFigure(new Point(width / 2, Utils.AXIS_THICKNESS), new[] { new LineSegment(new Point(width / 2, height - Utils.AXIS_THICKNESS), true) }, false));
                    gm.Figures.Add(new PathFigure(new Point(Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), new[] { new LineSegment(new Point(width - Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), true) }, false));
                    break;
                case Directions.NorthEastSouthEast:
                    gm.Figures.Add(new PathFigure(new Point(Utils.AXIS_THICKNESS, Utils.AXIS_THICKNESS), new[] { new LineSegment(new Point(Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), true) }, false));
                    gm.Figures.Add(new PathFigure(new Point(Utils.AXIS_THICKNESS, height / 2), new[] { new LineSegment(new Point(width - Utils.AXIS_THICKNESS, height / 2), true) }, false));
                    break;
                case Directions.SouthEast:
                    gm.Figures.Add(new PathFigure(new Point(Utils.AXIS_THICKNESS, Utils.AXIS_THICKNESS), new[] { new LineSegment(new Point(width - Utils.AXIS_THICKNESS, Utils.AXIS_THICKNESS), true) }, false));
                    gm.Figures.Add(new PathFigure(new Point(Utils.AXIS_THICKNESS, Utils.AXIS_THICKNESS), new[] { new LineSegment(new Point(Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), true) }, false));
                    break;
                case Directions.NorthWest:
                    gm.Figures.Add(new PathFigure(new Point(Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), new[] { new LineSegment(new Point(width - Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), true) }, false));
                    gm.Figures.Add(new PathFigure(new Point(width - Utils.AXIS_THICKNESS, Utils.AXIS_THICKNESS), new[] { new LineSegment(new Point(width - Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), true) }, false));
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class LegendVisibilityConverter : IMultiValueConverter
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
                || !(values[0] is ChartStyle chartStyle)
                || !(values[1] is int index)
                || !(parameter is bool isMain))
                return null;

            if (chartStyle != ChartStyle.Waterfall)
                return isMain
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            else
                return isMain
                    ? Visibility.Collapsed
                    : index == 0
                        ? Visibility.Visible
                        : Visibility.Collapsed;
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Prepares image for drawing waterfall series
    /// </summary>
    public class ValuesToWaterfallConverter : IMultiValueConverter
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
                //|| values.Length != 8
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is IEnumerable<Series> seriesEnumerable)
                || !seriesEnumerable.Any()
                || !(values[3] is ChartStyle chartStyle)
                || chartStyle != ChartStyle.Waterfall
                || !(values[4] is int index)
                || index > 0
                || !(values[5] is bool autoAdjust)
                || !(values[6] is double maxXConv)
                || !(values[7] is double maxYConv)
                || !(values[8] is FontFamily fontFamily)
                || !(values[9] is double fontSize)
                || !(values[10] is FontStyle fontStyle)
                || !(values[11] is FontWeight fontWeight)
                || !(values[12] is FontStretch fontStretch)
                || !(parameter is bool isPositive))
                return null;

            var series = seriesEnumerable.ToArray();

            var totalValues = series[0].Values.Select(v => v.Value.V1).ToArray();

            var tuple = Tuple.Create(series[0].Values.ToList(), 0);
            var dir = Utils.GetDirection(totalValues, ChartStyle.Waterfall);

            var maxY = autoAdjust ? Utils.GetMaxY(new[] { tuple }.ToList(), ChartStyle.Waterfall) : maxYConv;
            return drawWaterfall(width, height, maxY, dir, series[0], isPositive);
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }


        private PathGeometry drawWaterfall(double width,
            double height,
            double maxLength,
            Directions dir,
            Series currentSeries,
            bool isPositive)
        {
            var gm = new PathGeometry();
            var segSize = width / currentSeries.Values.Count;
            var columnWidth = segSize - segSize / 8;
            var startY = Utils.AXIS_THICKNESS;
            var step = 0.0;

            switch (dir)
            {
                case Directions.NorthEast:
                    startY = height - Utils.AXIS_THICKNESS;
                    step = (height - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
                case Directions.NorthEastNorthWest:
                    startY = height - Utils.AXIS_THICKNESS;
                    step = (height - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
                case Directions.NorthWest:
                    startY = height - Utils.AXIS_THICKNESS;
                    step = (height - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
                case Directions.NorthEastSouthEast:
                    startY = height / 2;
                    step = (height - 2 * Utils.AXIS_THICKNESS) / 2 / maxLength;
                    break;
                case Directions.SouthEast:
                    startY = Utils.AXIS_THICKNESS;
                    step = (height - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
            }

            currentSeries.RealRects.Clear();
            var y = startY;
            for (var i = 0; i < currentSeries.Values.Count; i++)
            {
                var value = currentSeries.Values[i].Value.V1;
                var x = i * segSize + segSize / 16;
                Rect rect;
                if (value >= 0)
                {
                    rect = new Rect(new Point(x, y), new Point(x + columnWidth, y - currentSeries.Values[i].Value.V1 * step));
                    y -= rect.Height;
                    if (isPositive)
                    {
                        gm.AddGeometry(new RectangleGeometry(rect));
                    }
                    else
                    {
                        gm.AddGeometry(new RectangleGeometry(new Rect(new Point(x, y), new Point(x + columnWidth, y))));
                    }
                }
                else
                {
                    rect = new Rect(new Point(x, y), new Point(x + columnWidth, y + Math.Abs(currentSeries.Values[i].Value.V1) * step));
                    y += rect.Height;
                    if (!isPositive)
                    {
                        gm.AddGeometry(new RectangleGeometry(rect));
                    }
                    else
                    {
                        gm.AddGeometry(new RectangleGeometry(new Rect(new Point(x, y), new Point(x + columnWidth, y))));
                    }
                }

                currentSeries.RealRects.Add(rect);

            }

            return gm;
        }
    }

    /// <summary>
    /// Prepares geometry for drawing series
    /// </summary>
    public class ValuesToPathConverter : IMultiValueConverter
    {
        private const double COLUMN_BAR_OFFSET = 16.0;
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
                //|| values.Length != 9
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is IEnumerable<Series> seriesEnumerable)
                || !(values[3] is ChartStyle chartStyle)
                || chartStyle.In(ChartStyle.SolidPie, ChartStyle.SlicedPie, ChartStyle.Doughnut, ChartStyle.Waterfall)
                || !(values[4] is int index)
                || !(values[5] is bool autoAdjust)
                || !(values[6] is double maxXConv)
                || !(values[7] is double maxYConv)
                || !(values[8] is ChartBoundary chartBoundary)
                || !(values[9] is FontFamily fontFamily)
                || !(values[10] is double fontSize)
                || !(values[11] is FontStyle fontStyle)
                || !(values[12] is FontWeight fontWeight)
                || !(values[13] is FontStretch fontStretch)
                || !(values[14] is IEnumerable<string> customEnumerable)
                || !(values[15] is int linesCount))
                return null;

            var series = seriesEnumerable.ToArray();
            var offsetBoundary = Utils.OffsetBoundary(chartBoundary, chartStyle);

            if (!series.Any()) return null;

            var gm = new PathGeometry();
            double maxX;
            double maxY;

            var totalValues = (from s in series from v in s.Values select v.Value.V1).ToArray();

            var rawValues = series.Select(s => Tuple.Create(s.Values.ToList(), s.Index)).ToList();
            var maxCount = rawValues.Max(rw => rw.Item1.Count);
            foreach (var rw in rawValues.Where(rv => rv.Item1.Count < maxCount))
            {
                var diff = maxCount - rw.Item1.Count;
                for (var i = 0; i < diff; i++)
                    rw.Item1.Add(new ChartValue(0));
            }

            var dir = Utils.GetDirection(totalValues, chartStyle);
            var tuple = rawValues.FirstOrDefault(rw => rw.Item2 == index);
            if (tuple == null) return null;

            switch (chartStyle)
            {
                case ChartStyle.Radar:
                case ChartStyle.RadarWithMarkers:
                case ChartStyle.RadarArea:
                    var customValues = customEnumerable.ToArray();
                    var pointsCount = series.Max(s => s.Values.Count);
                    var maxCv = customValues.Any() ? customValues.Max(v => (v, v.Length)) : (v: "", Length: 0);
                    var number = maxCv.Length > pointsCount.ToString(culture).Length ? maxCv.v : pointsCount.ToString(culture);
                    var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                        new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);

                    var radius = width > height ? (height - 2 * Utils.AXIS_THICKNESS) / 2 : (width - 2 * Utils.AXIS_THICKNESS) / 2;
                    var centerPoint = new Point(radius, radius);
                    if (width > height)
                        radius -= (2 * fmt.Height + 8);
                    else
                        radius -= (2 * fmt.Width + 8);
                    var (max, min, realLinesCount, stepSize, stepLength, units, zeroPoint) = Utils.GetMeasures(series, linesCount, radius, fmt.Height, autoAdjust, centerPoint);
                    gm = drawRadar(series.FirstOrDefault(s => s.Index == index), chartStyle, units, stepSize, stepLength, radius, pointsCount, realLinesCount, zeroPoint, centerPoint);
                    break;
                case ChartStyle.Lines:
                case ChartStyle.LinesWithMarkers:
                case ChartStyle.SmoothLines:
                case ChartStyle.SmoothLinesWithMarkers:
                    maxX = series.Max(s => s.Values.Count);
                    maxY = autoAdjust ? Utils.GetMaxY(rawValues, chartStyle) : maxYConv;
                    gm = drawLine(width, height, maxX, maxY, chartStyle, dir, series.FirstOrDefault(s => s.Index == index), tuple.Item1, offsetBoundary);
                    break;
                case ChartStyle.Bubbles:
                    maxX = series.Max(s => s.Values.Count);
                    maxY = autoAdjust ? Utils.GetMaxY(rawValues, chartStyle) : maxYConv;
                    gm = drawBubbles(width, height, maxX, maxY, dir, series.FirstOrDefault(s => s.Index == index), tuple.Item1, offsetBoundary);
                    break;
                case ChartStyle.StackedLines:
                case ChartStyle.StackedLinesWithMarkers:
                case ChartStyle.SmoothStackedLines:
                case ChartStyle.SmoothStackedLinesWithMarkers:
                    maxX = series.Max(s => s.Values.Count);
                    maxY = autoAdjust ? Utils.GetMaxY(rawValues, chartStyle) : maxYConv;
                    gm = drawStackedLine(width, height, maxX, maxY, chartStyle, dir, series, index, rawValues, offsetBoundary);
                    break;
                case ChartStyle.FullStackedLines:
                case ChartStyle.FullStackedLinesWithMarkers:
                case ChartStyle.SmoothFullStackedLines:
                case ChartStyle.SmoothFullStackedLinesWithMarkers:
                    maxX = series.Max(s => s.Values.Count);
                    maxY = autoAdjust ? Utils.GetMaxY(rawValues, chartStyle) : maxYConv;
                    gm = drawFullStackedLine(width, height, maxX, maxY, chartStyle, dir, series, index, rawValues, offsetBoundary);
                    break;
                case ChartStyle.Columns:
                    maxY = autoAdjust ? Utils.GetMaxY(rawValues, chartStyle) : maxYConv;
                    gm = drawColumns(width, height, maxY, dir, series, index, tuple.Item1);
                    break;
                case ChartStyle.StackedColumns:
                    maxY = autoAdjust ? Utils.GetMaxY(rawValues, chartStyle) : maxYConv;
                    gm = drawStackedColumns(width, height, maxY, dir, series, index, rawValues);
                    break;
                case ChartStyle.FullStackedColumns:
                    maxY = autoAdjust ? Utils.GetMaxY(rawValues, chartStyle) : maxYConv;
                    gm = drawFullStackedColumns(width, height, maxY, dir, series, index, rawValues);
                    break;
                case ChartStyle.Bars:
                    maxY = autoAdjust ? Utils.GetMaxY(rawValues, chartStyle) : maxXConv;
                    gm = drawBars(width, height, maxY, dir, series, index, tuple.Item1);
                    break;
                case ChartStyle.StackedBars:
                    maxY = autoAdjust ? Utils.GetMaxY(rawValues, chartStyle) : maxXConv;
                    gm = drawStackedBars(width, height, maxY, dir, series, index, rawValues);
                    break;
                case ChartStyle.FullStackedBars:
                    maxY = autoAdjust ? Utils.GetMaxY(rawValues, chartStyle) : maxXConv;
                    gm = drawFullStackedBars(width, height, maxY, dir, series, index, rawValues);
                    break;
                case ChartStyle.Area:
                    maxX = series.Max(s => s.Values.Count);
                    maxY = autoAdjust ? Utils.GetMaxY(rawValues, chartStyle) : maxYConv;
                    gm = drawArea(width, height, maxX, maxY, dir, tuple.Item1);
                    break;
                case ChartStyle.StackedArea:
                    maxX = series.Max(s => s.Values.Count);
                    maxY = autoAdjust ? Utils.GetMaxY(rawValues, chartStyle) : maxYConv;
                    gm = drawStackedArea(width, height, maxX, maxY, dir, series, index, rawValues);
                    break;
                case ChartStyle.FullStackedArea:
                    maxX = series.Max(s => s.Values.Count);
                    maxY = autoAdjust ? Utils.GetMaxY(rawValues, chartStyle) : maxYConv;
                    gm = drawFullStackedArea(width, height, maxX, maxY, dir, series, index, rawValues);
                    break;
            }

            return gm;
        }

        private PathGeometry drawBars(double w, double h, double maxLength, Directions dir, Series[] series, int index, List<ChartValue> values)
        {
            var gm = new PathGeometry();
            var segSize = h / values.Count;
            var barHeight = (segSize - COLUMN_BAR_OFFSET * 2) / series.Length;
            var step = Utils.AXIS_THICKNESS;
            var startX = 0.0;
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    startX = Utils.AXIS_THICKNESS;
                    step = (w - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
                case Directions.NorthEastNorthWest:
                    startX = w / 2;
                    step = (w - 2 * Utils.AXIS_THICKNESS) / 2 / maxLength;
                    break;
                case Directions.NorthEastSouthEast:
                    startX = Utils.AXIS_THICKNESS;
                    step = (w - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
                case Directions.NorthWest:
                    startX = w - Utils.AXIS_THICKNESS;
                    step = (w - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
                case Directions.SouthEast:
                    startX = Utils.AXIS_THICKNESS;
                    step = (w - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
            }
            currentSeries.RealRects.Clear();

            for (var i = 0; i < values.Count; i++)
            {
                var x = startX;
                var y = h - (i * segSize + COLUMN_BAR_OFFSET + index * barHeight);
                var rect = new Rect(new Point(x, y), new Point(x + values[i].Value.V1 * step, y - barHeight));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
            }
            return gm;
        }

        private PathGeometry drawStackedBars(double w, double h, double maxLength, Directions dir, Series[] series, int index, List<Tuple<List<ChartValue>, int>> tuples)
        {
            var tp = tuples.FirstOrDefault(t => t.Item2 == index);
            if (tp == null) return null;
            var values = tp.Item1;
            var gm = new PathGeometry();
            var segSize = h / values.Count;
            var barHeight = segSize - COLUMN_BAR_OFFSET * 2;
            var step = Utils.AXIS_THICKNESS;
            var startX = 0.0;
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    startX = Utils.AXIS_THICKNESS;
                    step = (w - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
                case Directions.NorthEastNorthWest:
                    startX = w / 2;
                    step = (w - 2 * Utils.AXIS_THICKNESS) / 2 / maxLength;
                    break;
                case Directions.NorthEastSouthEast:
                    startX = Utils.AXIS_THICKNESS;
                    step = (w - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
                case Directions.NorthWest:
                    startX = w - Utils.AXIS_THICKNESS;
                    step = (w - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
                case Directions.SouthEast:
                    startX = Utils.AXIS_THICKNESS;
                    step = (w - 2 * Utils.AXIS_THICKNESS) / maxLength;
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
                        tuples.Where(t => t.Item2 < index && Math.Sign(t.Item1[i1].Value.V1) == Math.Sign(values[i1].Value.V1));
                    x += prevsTuples.Sum(sr => sr.Item1[i].Value.V1 * step);
                }
                var y = h - (i * segSize + COLUMN_BAR_OFFSET);
                var rect = new Rect(new Point(x, y), new Point(x + values[i].Value.V1 * step, y - barHeight));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
            }
            return gm;
        }

        private PathGeometry drawFullStackedBars(double w, double h, double maxLength, Directions dir, Series[] series, int index, List<Tuple<List<ChartValue>, int>> tuples)
        {
            var tp = tuples.FirstOrDefault(t => t.Item2 == index);
            if (tp == null) return null;
            var values = tp.Item1;
            var gm = new PathGeometry();
            var segSize = h / values.Count;
            var barHeight = segSize - COLUMN_BAR_OFFSET * 2;
            var startX = Utils.AXIS_THICKNESS;
            var width = 0.0;
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    startX = Utils.AXIS_THICKNESS;
                    width = (w - 2 * Utils.AXIS_THICKNESS) * 100 / maxLength;
                    break;
                case Directions.NorthEastNorthWest:
                    startX = w / 2;
                    width = (w - 2 * Utils.AXIS_THICKNESS) / 2 * 100 / maxLength;
                    break;
                case Directions.NorthEastSouthEast:
                    startX = Utils.AXIS_THICKNESS;
                    width = (w - 2 * Utils.AXIS_THICKNESS) * 100 / maxLength;
                    break;
                case Directions.NorthWest:
                    startX = w - Utils.AXIS_THICKNESS;
                    width = (w - 2 * Utils.AXIS_THICKNESS) * 100 / maxLength;
                    break;
                case Directions.SouthEast:
                    startX = Utils.AXIS_THICKNESS;
                    width = (w - 2 * Utils.AXIS_THICKNESS) * 100 / maxLength;
                    break;
            }
            currentSeries.RealRects.Clear();
            for (var i = 0; i < values.Count; i++)
            {
                var sign = Math.Sign(values[i].Value.V1);
                var y = h - (i * segSize + COLUMN_BAR_OFFSET);
                var i1 = i;
                var sumTotal = tuples.Sum(sr => Math.Abs(sr.Item1[i1].Value.V1));
                var percent = Math.Abs(values[i].Value.V1) / sumTotal * 100;
                var segWidth = sign * width / 100 * percent;

                var prevs = tuples.Where(s => s.Item2 < index)
                    .Where(s => Math.Sign(s.Item1[i1].Value.V1) == sign);
                var prevSum = prevs.Sum(pvs => Math.Abs(pvs.Item1[i1].Value.V1));
                var prevPerc = prevSum / sumTotal * 100;
                var prevWidth = width / 100 * prevPerc;
                var x = startX + sign * prevWidth;
                var rect = new Rect(new Point(x, y), new Point(x + segWidth, y - barHeight));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
            }
            return gm;
        }

        private PathGeometry drawFullStackedColumns(double w, double h, double maxLength, Directions dir, Series[] series, int index, List<Tuple<List<ChartValue>, int>> tuples)
        {
            var tp = tuples.FirstOrDefault(t => t.Item2 == index);
            if (tp == null) return null;
            var values = tp.Item1;
            var gm = new PathGeometry();
            var segSize = w / values.Count;
            var columnWidth = segSize - COLUMN_BAR_OFFSET * 2;
            var startY = Utils.AXIS_THICKNESS;
            var height = 0.0;
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    startY = h - Utils.AXIS_THICKNESS;
                    height = (h - 2 * Utils.AXIS_THICKNESS) * 100 / maxLength;
                    break;
                case Directions.NorthEastNorthWest:
                    startY = h - Utils.AXIS_THICKNESS;
                    height = (h - 2 * Utils.AXIS_THICKNESS) * 100 / maxLength;
                    break;
                case Directions.NorthWest:
                    startY = h - Utils.AXIS_THICKNESS;
                    height = (h - 2 * Utils.AXIS_THICKNESS) * 100 / maxLength;
                    break;
                case Directions.NorthEastSouthEast:
                    startY = h / 2;
                    height = (h - 2 * Utils.AXIS_THICKNESS) / 2 * 100 / maxLength;
                    break;
                case Directions.SouthEast:
                    startY = Utils.AXIS_THICKNESS;
                    height = (h - 2 * Utils.AXIS_THICKNESS) * 100 / maxLength;
                    break;
            }

            currentSeries.RealRects.Clear();
            for (var i = 0; i < values.Count; i++)
            {
                var sign = Math.Sign(values[i].Value.V1);
                var x = i * segSize + COLUMN_BAR_OFFSET;
                var i1 = i;
                var sumTotal = tuples.Sum(sr => Math.Abs(sr.Item1[i1].Value.V1));
                var percent = Math.Abs(values[i].Value.V1) / sumTotal * 100;
                var segHeight = sign * height / 100 * percent;
                var prevs = tuples.Where(s => s.Item2 < index)
                    .Where(s => Math.Sign(s.Item1[i1].Value.V1) == sign);
                var prevSum = prevs.Sum(pvs => Math.Abs(pvs.Item1[i1].Value.V1));
                var prevPerc = prevSum / sumTotal * 100;
                var prevHeight = height / 100 * prevPerc;
                var y = startY - sign * prevHeight;
                var rect = new Rect(new Point(x, y), new Point(x + columnWidth, y - segHeight));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
            }
            return gm;
        }

        private PathGeometry drawStackedColumns(double w, double h, double maxLength, Directions dir, Series[] series, int index, List<Tuple<List<ChartValue>, int>> tuples)
        {
            var tp = tuples.FirstOrDefault(t => t.Item2 == index);
            if (tp == null) return null;
            var values = tp.Item1;
            var gm = new PathGeometry();
            var segSize = w / values.Count;
            var columnWidth = segSize - COLUMN_BAR_OFFSET * 2;
            var startY = Utils.AXIS_THICKNESS;
            var step = 0.0;
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    startY = h - Utils.AXIS_THICKNESS;
                    step = (h - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
                case Directions.NorthEastNorthWest:
                    startY = h - Utils.AXIS_THICKNESS;
                    step = (h - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
                case Directions.NorthWest:
                    startY = h - Utils.AXIS_THICKNESS;
                    step = (h - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
                case Directions.NorthEastSouthEast:
                    startY = h / 2;
                    step = (h - 2 * Utils.AXIS_THICKNESS) / 2 / maxLength;
                    break;
                case Directions.SouthEast:
                    startY = Utils.AXIS_THICKNESS;
                    step = (h - 2 * Utils.AXIS_THICKNESS) / maxLength;
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
                            s => s.Item2 < index && Math.Sign(s.Item1[i1].Value.V1) == Math.Sign(values[i1].Value.V1));
                    y -= prevs.Sum(sr => sr.Item1[i].Value.V1 * step);
                }
                var rect = new Rect(new Point(x, y), new Point(x + columnWidth, y - values[i].Value.V1 * step));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
            }
            return gm;
        }

        private PathGeometry drawColumns(double w, double h, double maxLength, Directions dir, Series[] series, int index, List<ChartValue> values)
        {
            var gm = new PathGeometry();
            var segSize = w / values.Count;
            var columnWidth = (segSize - COLUMN_BAR_OFFSET * 2) / series.Length;
            var startY = Utils.AXIS_THICKNESS;
            var step = 0.0;
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    startY = h - Utils.AXIS_THICKNESS;
                    step = (h - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
                case Directions.NorthEastNorthWest:
                    startY = h - Utils.AXIS_THICKNESS;
                    step = (h - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
                case Directions.NorthWest:
                    startY = h - Utils.AXIS_THICKNESS;
                    step = (h - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
                case Directions.NorthEastSouthEast:
                    startY = h / 2;
                    step = (h - 2 * Utils.AXIS_THICKNESS) / 2 / maxLength;
                    break;
                case Directions.SouthEast:
                    startY = Utils.AXIS_THICKNESS;
                    step = (h - 2 * Utils.AXIS_THICKNESS) / maxLength;
                    break;
            }

            currentSeries.RealRects.Clear();
            for (var i = 0; i < values.Count; i++)
            {
                var x = i * segSize + COLUMN_BAR_OFFSET + index * columnWidth;
                var y = startY;
                var rect = new Rect(new Point(x, y), new Point(x + columnWidth, y - values[i].Value.V1 * step));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
            }
            return gm;
        }

        private PathGeometry drawArea(double w, double h, double maxX, double maxY, Directions dir, List<ChartValue> values)
        {
            double stepX, stepY;
            double centerX, centerY;
            var gm = new PathGeometry();

            switch (dir)
            {
                case Directions.NorthEast:
                    centerX = Utils.AXIS_THICKNESS;
                    centerY = h - Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = w / 2;
                    centerY = h - Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / 2 / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = Utils.AXIS_THICKNESS;
                    centerY = h / 2;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / 2 / maxY;
                    break;
                case Directions.SouthEast:
                    centerX = Utils.AXIS_THICKNESS;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                case Directions.NorthWest:
                    centerX = w - Utils.AXIS_THICKNESS;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                default:
                    return null;
            }

            var segments = new List<PathSegment>();
            var x = centerX;
            var y = centerY;
            var start = new Point(x, y);

            for (var i = 0; i < values.Count; i++)
            {
                x = centerX + i * stepX;
                y = centerY - values[i].Value.V1 * stepY;
                segments.Add(new LineSegment(new Point(x, y), true));
            }

            y = centerY;
            var end = new Point(x, y);

            segments.Add(new LineSegment(end, true));
            gm.Figures.Add(new PathFigure(start, segments, true));
            return gm;
        }

        private PathGeometry drawStackedArea(double w, double h, double maxX, double maxY, Directions dir,
            Series[] series, int index, List<Tuple<List<ChartValue>, int>> tuples)
        {
            var tp = tuples.FirstOrDefault(t => t.Item2 == index);
            if (tp == null) return null;
            var values = tp.Item1;
            double stepX, stepY;
            double centerX, centerY;
            var gm = new PathGeometry();
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    centerX = Utils.AXIS_THICKNESS;
                    centerY = h - Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = w / 2;
                    centerY = h - Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / 2 / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = Utils.AXIS_THICKNESS;
                    centerY = h / 2;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / 2 / maxY;
                    break;
                case Directions.SouthEast:
                    centerX = Utils.AXIS_THICKNESS;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                case Directions.NorthWest:
                    centerX = w - Utils.AXIS_THICKNESS;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                default:
                    return null;
            }

            var points = new List<Point>();
            var x = centerX;
            var y = centerY;
            if (index > 0)
            {
                var prevs = tuples.Where(s => s.Item2 < index);
                var sum = prevs.Sum(sr => sr.Item1[0].Value.V1);
                y -= sum * stepY;
            }
            var start = new Point(x, y);
            for (var i = 0; i < values.Count; i++)
            {
                x = centerX + i * stepX;
                y = centerY - values[i].Value.V1 * stepY;
                if (index > 0)
                {
                    var prevs = tuples.Where(s => s.Item2 < index);
                    var sum = prevs.Sum(sr => sr.Item1[i].Value.V1);
                    y -= sum * stepY;
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
                var prevs = tuples.Where(s => s.Item2 < index).ToList();
                for (var i = values.Count - 1; i >= 0; i--)
                {
                    y = centerY;
                    var sum = prevs.Sum(sr => sr.Item1[i].Value.V1);
                    y -= sum * stepY;
                    points.Add(new Point(x, y));
                    x -= stepX;
                }
            }
            var polySegment = new PolyLineSegment(points, true);
            gm.Figures.Add(new PathFigure(start, new[] { polySegment }, true));
            return gm;
        }

        private PathGeometry drawFullStackedArea(double w, double h, double maxX, double maxY, Directions dir,
            Series[] series, int index, List<Tuple<List<ChartValue>, int>> tuples)
        {
            var tp = tuples.FirstOrDefault(t => t.Item2 == index);
            if (tp == null) return null;
            var values = tp.Item1;
            double stepX, height;
            double centerY;
            var gm = new PathGeometry();

            var percent = maxY / 100;
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    centerY = h - Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    height = (h - 2 * Utils.AXIS_THICKNESS);
                    break;
                case Directions.NorthEastNorthWest:
                    centerY = h - Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / 2 / (maxX - 1);
                    height = (h - 2 * Utils.AXIS_THICKNESS);
                    break;
                case Directions.NorthEastSouthEast:
                    centerY = h / 2;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    height = (h - 2 * Utils.AXIS_THICKNESS) / 2;
                    break;
                case Directions.SouthEast:
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    height = h - 2 * Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthWest:
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    height = h - 2 * Utils.AXIS_THICKNESS;
                    break;
                default:
                    return null;
            }
            currentSeries.RealRects.Clear();
            currentSeries.RealPoints.Clear();

            height /= percent;

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
                var sum = tuples.Sum(s => Math.Abs(s.Item1[i].Value.V1));
                var sign = Math.Sign(values[i].Value.V1);
                var perc = Math.Abs(values[i].Value.V1) / sum * 100;
                y -= sign * perc * height / 100;
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
            var poly = new PolyLineSegment(points, true);
            gm.Figures.Add(new PathFigure(start, new[] { poly }, true));
            return gm;
        }

        private void drawMarker(double x, double y, PathGeometry gm, List<Rect> realRects)
        {
            var rect = new Rect(new Point(x - RECT_SIZE, y - RECT_SIZE), new Point(x + RECT_SIZE, y + RECT_SIZE));
            realRects.Add(rect);
            for (var r = RECT_SIZE; r > 0; r--)
            {
                gm.AddGeometry(new RectangleGeometry(new Rect(new Point(x - r, y - r), new Point(x + r, y + r))));
            }
        }

        private PathGeometry drawFullStackedLine(double w, double h, double maxX, double maxY, ChartStyle style,
            Directions dir, Series[] series, int index, List<Tuple<List<ChartValue>, int>> tuples, bool offsetBoundary)
        {
            var tp = tuples.FirstOrDefault(t => t.Item2 == index);
            if (tp == null) return null;
            var values = tp.Item1;
            double stepX, height;
            double centerY;
            var gm = new PathGeometry();

            var boundOffset = Utils.BoundaryOffset(offsetBoundary, maxX, values.Count);

            var percent = maxY / 100;
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    centerY = h - Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (w - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    height = h - 2 * Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastNorthWest:
                    centerY = h - Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / 2 / (maxX - 1);
                    height = h - 2 * Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastSouthEast:
                    centerY = h / 2;
                    stepX = offsetBoundary ? (w - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    height = (h - 2 * Utils.AXIS_THICKNESS) / 2;
                    break;
                case Directions.SouthEast:
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (w - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    height = h - 2 * Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthWest:
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    height = h - 2 * Utils.AXIS_THICKNESS;
                    break;
                default:
                    return null;
            }
            currentSeries.RealRects.Clear();
            currentSeries.RealPoints.Clear();

            height /= percent;

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
                var sum = tuples.Sum(s => Math.Abs(s.Item1[i].Value.V1));
                var sign = Math.Sign(values[i].Value.V1);
                var perc = Math.Abs(values[i].Value.V1) / sum * 100;

                y -= sign * perc * height / 100;
                var x = offsetBoundary ? boundOffset + stepX * i : stepX * i;
                currentSeries.RealPoints.Add(new Point(x, y));
                if (i == 0)
                    start = new Point(x, y);
                if (!style.In(ChartStyle.FullStackedLinesWithMarkers, ChartStyle.SmoothFullStackedLinesWithMarkers)) continue;
                drawMarker(x, y, gm, currentSeries.RealRects);
            }

            if (style.In(ChartStyle.SmoothFullStackedLines, ChartStyle.SmoothFullStackedLinesWithMarkers))
            {
                var bezierSegments = interpolatePointsWithBezierCurves(currentSeries.RealPoints, false);
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
            else
            {
                var poly = new PolyLineSegment(currentSeries.RealPoints, true);
                gm.Figures.Add(new PathFigure(start, new[] { poly }, false));
            }
            return gm;
        }

        private PathGeometry drawStackedLine(double w, double h, double maxX, double maxY, ChartStyle style,
            Directions dir, Series[] series, int index, List<Tuple<List<ChartValue>, int>> tuples, bool offsetBoundary)
        {
            var tp = tuples.FirstOrDefault(t => t.Item2 == index);
            if (tp == null) return null;
            var values = tp.Item1;
            double stepX, stepY;
            double centerX, centerY;
            var gm = new PathGeometry();
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            var boundOffset = Utils.BoundaryOffset(offsetBoundary, w, values.Count);

            switch (dir)
            {
                case Directions.NorthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = h - Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (w - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = w / 2;
                    centerY = h - Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / 2 / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = h / 2;
                    stepX = offsetBoundary ? (w - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / 2 / maxY;
                    break;
                case Directions.SouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (w - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                case Directions.NorthWest:
                    centerX = w - Utils.AXIS_THICKNESS;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (w - Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - Utils.AXIS_THICKNESS) / maxY;
                    break;
                default:
                    return null;
            }
            currentSeries.RealRects.Clear();

            var points = new List<Point>();

            for (var i = 0; i < values.Count; i++)
            {
                var x = centerX + i * stepX;
                var y = centerY - values[i].Value.V1 * stepY;
                if (index > 0)
                {
                    var prevs = tuples.Where(s => s.Item2 < index);
                    var sum = prevs.Sum(sr => sr.Item1[i].Value.V1);
                    y -= sum * stepY;
                }
                points.Add(new Point(x, y));
                if (!style.In(ChartStyle.StackedLinesWithMarkers, ChartStyle.SmoothStackedLinesWithMarkers)) continue;
                drawMarker(x, y, gm, currentSeries.RealRects);
            }
            if (style.In(ChartStyle.SmoothStackedLines, ChartStyle.SmoothStackedLinesWithMarkers))
            {
                var bezierSegments = interpolatePointsWithBezierCurves(points, false);
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
            else
            {
                var poly = new PolyLineSegment(points, true);
                gm.Figures.Add(new PathFigure(points[0], new[] { poly }, false));
            }
            return gm;
        }

        private PathGeometry drawRadar(Series currentSeries, ChartStyle chartStyle, double units, double stepNum, double stepLength, double radius, int pointsCount, int linesCount, ZeroPoint zeroPoint, Point centerPoint)
        {
            var gm = new PathGeometry();
            currentSeries.RealRects.Clear();
            var points = new List<Point>();
            var values = currentSeries.Values.Select(v => v.Value.V1).ToArray();
            var xBeg = zeroPoint.Point.X;
            var yBeg = 90.0;
            var currentDegrees = 0.0;
            var degreesStep = 360 / pointsCount;
            var zeroDistance =(linesCount- zeroPoint.Level) * stepLength;

            if (chartStyle.In(ChartStyle.Radar,ChartStyle.RadarWithMarkers))
            {
                var segments = new List<PathSegment>();
                for (var i = 0; i < values.Length; i++)
                {
                    //if (values.Length <= i)
                    //    break;
                    var distance = radius - (zeroDistance - values[i] * units);

                    currentDegrees = 90 + i * degreesStep;
                    var rads = currentDegrees * Math.PI / 180;
                    xBeg = centerPoint.X - distance * Math.Cos(rads);
                    yBeg = centerPoint.Y - distance * Math.Sin(rads);
                    points.Add(new Point(xBeg, yBeg));
                }
                for (var i = 0; i < points.Count; i++)
                {
                    segments.Add(new LineSegment(points[i], true));
                }
                //gm.AddGeometry(new LineGeometry(points[points.Count - 1], points[0]));
                gm.Figures.Add(new PathFigure(points[0], segments, true));
                //for (var i = 0; i < points.Count - 1; i++)
                //{
                //    gm.AddGeometry(new LineGeometry(points[i], points[i + 1]));
                //}
                //gm.AddGeometry(new LineGeometry(points[points.Count - 1], points[0]));
            }
            else
            {
                var segments = new List<PathSegment>();
                for (var i = 0; i < values.Length; i++)
                {
                    //if (values.Length <= i)
                    //    break;
                    var distance = radius - (zeroDistance - values[i] * units);

                    currentDegrees = 90 + i * degreesStep;
                    var rads = currentDegrees * Math.PI / 180;
                    xBeg = centerPoint.X - distance * Math.Cos(rads);
                    yBeg = centerPoint.Y - distance * Math.Sin(rads);
                    points.Add(new Point(xBeg, yBeg));
                }
                for (var i = 0; i < points.Count; i++)
                {
                    segments.Add(new LineSegment(points[i], true));
                }
                //gm.AddGeometry(new LineGeometry(points[points.Count - 1], points[0]));
                gm.Figures.Add(new PathFigure(points[0], segments, true));
            }
            return gm;
        }

        private PathGeometry drawLine(double w, double h, double maxX, double maxY, ChartStyle style, Directions dir, Series currentSeries, List<ChartValue> values, bool offsetBoundary)
        {
            double stepX, stepY;
            double centerX, centerY;
            var gm = new PathGeometry();

            var boundOffset = Utils.BoundaryOffset(offsetBoundary, w, values.Count);

            switch (dir)
            {
                case Directions.NorthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = h - Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (w - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = w / 2;
                    centerY = h - Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / 2 / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = h / 2;
                    stepX = offsetBoundary ? (w - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / 2 / maxY;
                    break;
                case Directions.SouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (w - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                case Directions.NorthWest:
                    centerX = w - Utils.AXIS_THICKNESS;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                default:
                    return null;
            }
            currentSeries.RealRects.Clear();
            var points = new List<Point>();

            for (var i = 0; i < values.Count; i++)
            {
                var x = centerX + i * stepX;
                var y = centerY - values[i].Value.V1 * stepY;

                points.Add(new Point(x, y));
                if (!style.In(ChartStyle.LinesWithMarkers, ChartStyle.SmoothLinesWithMarkers)) continue;
                drawMarker(x, y, gm, currentSeries.RealRects);
            }
            if (style.In(ChartStyle.SmoothLines, ChartStyle.SmoothLinesWithMarkers))
            {
                var bezierSegments = interpolatePointsWithBezierCurves(points, false);
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
            else
            {
                var poly = new PolyLineSegment(points, true);
                gm.Figures.Add(new PathFigure(points[0], new[] { poly }, false));
            }

            return gm;
        }

        private PathGeometry drawBubbles(double w, double h, double maxX, double maxY, Directions dir, Series currentSeries, List<ChartValue> values, bool offsetBoundary)
        {
            double stepX, stepY;
            double centerX, centerY;
            var segSize = h > w ? (w - 2 * Utils.AXIS_THICKNESS) / (values.Count + 2) / 2 : (h - 2 * Utils.AXIS_THICKNESS) / (values.Count + 2) / 2;

            var boundOffset = Utils.BoundaryOffset(offsetBoundary, w, values.Count);
            if (boundOffset > 0)
                segSize = boundOffset;

            var gm = new PathGeometry();

            switch (dir)
            {
                case Directions.NorthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = h - Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (w - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = w / 2;
                    centerY = h - Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / 2 / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = h / 2;
                    stepX = offsetBoundary ? (w - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / 2 / maxY;
                    break;
                case Directions.SouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (w - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                case Directions.NorthWest:
                    centerX = w - Utils.AXIS_THICKNESS;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    break;
                default:
                    return null;
                    //case Directions.NorthEast:
                    //    centerX = Utils.AXIS_THICKNESS;
                    //    centerY = h - Utils.AXIS_THICKNESS;
                    //    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    //    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    //    break;
                    //case Directions.NorthEastNorthWest:
                    //    centerX = w / 2;
                    //    centerY = h - Utils.AXIS_THICKNESS;
                    //    stepX = (w - 2 * Utils.AXIS_THICKNESS) / 2 / (maxX - 1);
                    //    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    //    break;
                    //case Directions.NorthEastSouthEast:
                    //    centerX = Utils.AXIS_THICKNESS;
                    //    centerY = h / 2;
                    //    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    //    stepY = (h - 2 * Utils.AXIS_THICKNESS) / 2 / maxY;
                    //    break;
                    //case Directions.SouthEast:
                    //    centerX = Utils.AXIS_THICKNESS;
                    //    centerY = Utils.AXIS_THICKNESS;
                    //    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    //    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    //    break;
                    //case Directions.NorthWest:
                    //    centerX = w - Utils.AXIS_THICKNESS;
                    //    centerY = Utils.AXIS_THICKNESS;
                    //    stepX = (w - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    //    stepY = (h - 2 * Utils.AXIS_THICKNESS) / maxY;
                    //    break;
                    //default:
                    //    return null;
            }
            currentSeries.RealRects.Clear();

            for (var i = 0; i < values.Count; i++)
            {
                var x = centerX + i * stepX;
                var y = centerY - values[i].Value.V1 * stepY;

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
        private List<BeizerCurveSegment> interpolatePointsWithBezierCurves(List<Point> points, bool isClosedCurve)
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Defines pie legend brush
    /// </summary>
    public class PieLegendBrushConverter : IMultiValueConverter
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
                || values.Length != 2
                || !(values[0] is bool isEnabled)
                || !(values[1] is Brush disabledBrush)
                || !(parameter is Brush enabledBrush))
                return null;
            return isEnabled ? enabledBrush : disabledBrush;
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Defines chart path brush
    /// </summary>
    public class PathBrushConverter : IMultiValueConverter
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
                || values.Length != 3
                || !(values[0] is bool isEnabled)
                || !(values[1] is Brush enabledBrush)
                || !(values[2] is Brush disabledBrush))
                return null;
            return isEnabled ? enabledBrush : disabledBrush;
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Defines size of legend rectangle
    /// </summary>
    public class LegendSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is LegendSize legendSize))
                return 16;
            return System.Convert.ToDouble(legendSize);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Defines series legends container orientation
    /// </summary>
    public class LegendOrientationConverter : IValueConverter
    {
        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is LegendAlignment legendAlignment)) return null;
            return legendAlignment switch
            {
                LegendAlignment.Top => Orientation.Horizontal,
                LegendAlignment.Bottom => Orientation.Horizontal,
                _ => Orientation.Vertical
            };
        }

        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    /// <summary>
    /// Defines horizontal alignment of numeric/custom values drawn next to y-axis
    /// </summary>
    public class VerticalValuesAlignmentConverter : IMultiValueConverter
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
                || values.Length != 3
                || !(values[0] is IEnumerable<Series> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle))
                return null;

            var series = seriesEnumerable.ToArray();
            var totalValues = (from s in series from v in s.Values select v.Value.V1).ToArray();
            var dir = Utils.GetDirection(totalValues, chartStyle);
            switch (dir)
            {
                case Directions.NorthEast:
                case Directions.NorthEastSouthEast:
                case Directions.SouthEast:
                case Directions.NorthEastNorthWest:
                    return HorizontalAlignment.Right;
                case Directions.NorthWest:
                    return HorizontalAlignment.Left;
                default:
                    return null;
            }
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    ///  Defines column of numeric/custom values draw next to y-axis
    /// </summary>
    public class VerticalValuesColumnConverter : IMultiValueConverter
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
                || values.Length != 3
                || !(values[0] is IEnumerable<Series> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle))
                return 2;

            var series = seriesEnumerable.ToArray();
            var totalValues = (from s in series from v in s.Values select v.Value.V1).ToArray();
            var dir = Utils.GetDirection(totalValues, chartStyle);
            switch (dir)
            {
                case Directions.NorthEast:
                case Directions.NorthEastSouthEast:
                case Directions.SouthEast:
                case Directions.NorthEastNorthWest:
                    return 2;
                case Directions.NorthWest:
                    return 4;
                default:
                    return 2;
            }
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    ///  Defines vertical alignment of numeric/custom values drawn next to x-axis
    /// </summary>
    public class HorizontalValuesAlignmentConverter : IMultiValueConverter
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
                || values.Length != 3
                || !(values[0] is IEnumerable<Series> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle))
                return VerticalAlignment.Top;

            var series = seriesEnumerable.ToArray();
            var totalValues = (from s in series from v in s.Values select v.Value.V1).ToArray();
            var dir = Utils.GetDirection(totalValues, chartStyle);
            switch (dir)
            {
                case Directions.SouthEast:
                    return VerticalAlignment.Bottom;
                case Directions.NorthWest:
                case Directions.NorthEast:
                case Directions.NorthEastNorthWest:
                case Directions.NorthEastSouthEast:
                    return VerticalAlignment.Top;
                default:
                    return VerticalAlignment.Top;
            }
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Defines row of numeric/custom values draw next to x-axis
    /// </summary>
    public class HorizontalValuesRowConverter : IMultiValueConverter
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
                || values.Length != 3
                || !(values[0] is IEnumerable<Series> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle))
                return 5;

            var series = seriesEnumerable.ToArray();
            var totalValues = (from s in series from v in s.Values select v.Value.V1).ToArray();
            var dir = Utils.GetDirection(totalValues, chartStyle);
            switch (dir)
            {
                case Directions.SouthEast:
                    return 3;
                case Directions.NorthWest:
                case Directions.NorthEast:
                case Directions.NorthEastNorthWest:
                case Directions.NorthEastSouthEast:
                    return 5;
                default:
                    return 5;
            }
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Defines row of y-axis text
    /// </summary>
    public class AxisYGridRowConverter : IMultiValueConverter
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
                || !(values[0] is IEnumerable<Series> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle))
                return null;

            var series = seriesEnumerable.ToArray();
            var totalValues = (from s in series from v in s.Values select v.Value.V1).ToArray();
            var dir = Utils.GetDirection(totalValues, chartStyle);

            return dir switch
            {
                Directions.NorthEast => 2,
                Directions.NorthEastNorthWest => 2,
                Directions.NorthEastSouthEast => 2,
                Directions.SouthEast => 6,
                Directions.NorthWest => 2,
                _ => null,
            };
        }
        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Defines column of x-axis text
    /// </summary>
    public class AxisXGridColumnConverter : IMultiValueConverter
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
                || !(values[0] is IEnumerable<Series> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle))
                return null;

            var series = seriesEnumerable.ToArray();
            var totalValues = (from s in series from v in s.Values select v.Value.V1).ToArray();
            var dir = Utils.GetDirection(totalValues, chartStyle);

            return dir switch
            {
                Directions.NorthEast => 5,
                Directions.NorthEastNorthWest => 5,
                Directions.NorthEastSouthEast => 5,
                Directions.SouthEast => 5,
                Directions.NorthWest => 1,
                _ => null,
            };
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Defines vertical alignment of x-axis text
    /// </summary>
    public class AxisXTextAlignmentConverter : IMultiValueConverter
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
                || !(values[0] is IEnumerable<Series> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle))
                return null;

            var series = seriesEnumerable.ToArray();
            var totalValues = (from s in series from v in s.Values select v.Value.V1).ToArray();
            var dir = Utils.GetDirection(totalValues, chartStyle);

            return dir switch
            {
                Directions.NorthEast => VerticalAlignment.Bottom,
                Directions.NorthEastNorthWest => VerticalAlignment.Bottom,
                Directions.NorthEastSouthEast => VerticalAlignment.Center,
                Directions.SouthEast => VerticalAlignment.Top,
                Directions.NorthWest => VerticalAlignment.Bottom,
                _ => null,
            };
        }
        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Defines horizontal alignment of y-axis text
    /// </summary>
    public class AxisYTextAlignmentConverter : IMultiValueConverter
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
                || !(values[0] is IEnumerable<Series> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle))
                return null;

            var series = seriesEnumerable.ToArray();
            var totalValues = (from s in series from v in s.Values select v.Value.V1).ToArray();
            var dir = Utils.GetDirection(totalValues, chartStyle);

            return dir switch
            {
                Directions.NorthEast => HorizontalAlignment.Left,
                Directions.NorthEastNorthWest => HorizontalAlignment.Center,
                Directions.NorthEastSouthEast => HorizontalAlignment.Left,
                Directions.SouthEast => HorizontalAlignment.Left,
                Directions.NorthWest => HorizontalAlignment.Right,
                _ => null,
            };
        }
        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Defines chart path fill brush
    /// </summary>
    public class PathFillConverter : IMultiValueConverter
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
                || values.Length != 4
                || !(values[0] is ChartStyle chartStyle)
                || !(values[1] is Brush enabledBrush)
                || !(values[2] is Brush disabledBrush)
                || !(values[3] is bool isEnabled))
                return null;
            switch (chartStyle)
            {
                case ChartStyle.Lines:
                case ChartStyle.StackedLines:
                case ChartStyle.FullStackedLines:
                case ChartStyle.SmoothLines:
                case ChartStyle.SmoothStackedLines:
                case ChartStyle.SmoothFullStackedLines:
                case ChartStyle.SmoothLinesWithMarkers:
                case ChartStyle.SmoothStackedLinesWithMarkers:
                case ChartStyle.SmoothFullStackedLinesWithMarkers:
                case ChartStyle.LinesWithMarkers:
                case ChartStyle.StackedLinesWithMarkers:
                case ChartStyle.FullStackedLinesWithMarkers:
                case ChartStyle.Radar:
                case ChartStyle.RadarWithMarkers:
                    return null;
            }
            return isEnabled ? enabledBrush : disabledBrush;
        }
        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Defines chart path stroke brush
    /// </summary>
    public class PathStrokeConverter : IMultiValueConverter
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
                || values.Length != 4
                || !(values[0] is ChartStyle chartStyle)
                || !(values[1] is Brush enabledBrush)
                || !(values[2] is Brush disabledBrush)
                || !(values[3] is bool isEnabled))
                return null;
            switch (chartStyle)
            {
                case ChartStyle.Area:
                case ChartStyle.Bars:
                case ChartStyle.StackedBars:
                case ChartStyle.FullStackedBars:
                case ChartStyle.Columns:
                case ChartStyle.StackedColumns:
                case ChartStyle.FullStackedColumns:
                case ChartStyle.Bubbles:
                case ChartStyle.Waterfall:
                case ChartStyle.RadarArea:
                    return null;
            }
            return isEnabled ? enabledBrush : disabledBrush;
        }
        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Defines height of row of numeric/custom values drawn next to x-axis
    /// </summary>
    public class HorizontalPlaceholderHeightConverter : IMultiValueConverter
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
                || values.Length != 15
                || !(values[0] is IEnumerable<Series> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle)
                || !(values[2] is int stopsX)
                || !(values[3] is string format)
                || !(values[4] is FontFamily fontFamily)
                || !(values[5] is double fontSize)
                || !(values[6] is FontStyle fontStyle)
                || !(values[7] is FontWeight fontWeight)
                || !(values[8] is FontStretch fontStretch)
                || !(values[9] is IEnumerable<string> customEnumerable)
                || !(values[10] is bool autoAdjust)
                || !(values[11] is double maxY)
                || !(values[12] is ChartBoundary chartBoundary)
                || !(values[13] is double width))
                return null;

            var height = 8.0;
            var series = seriesEnumerable.ToArray();
            var customValues = customEnumerable.ToArray();

            if (!series.Any()) return height;

            width -= 2 * Utils.AXIS_THICKNESS;

            var rawValues = chartStyle != ChartStyle.Waterfall
                ? series.Select(s => Tuple.Create(s.Values.ToList(), s.Index)).ToList()
                : new[] { Tuple.Create(series[0].Values.ToList(), series[0].Index) }.ToList();

            var ticks = rawValues.Max(rw => rw.Item1.Count);
            foreach (var rw in rawValues.Where(rv => rv.Item1.Count < ticks))
            {
                var diff = ticks - rw.Item1.Count;
                for (var i = 0; i < diff; i++)
                    rw.Item1.Add(new ChartValue(0));
            }

            var totalValues = chartStyle != ChartStyle.Waterfall
                ? (from s in series from v in s.Values select v.Value.V1).ToArray()
                : series[0].Values.Select(v => v.Value.V1).ToArray();

            var dir = Utils.GetDirection(totalValues, chartStyle);
            var max = autoAdjust ? Utils.GetMaxY(rawValues, chartStyle) : maxY;
            var offsetBoundary = Utils.OffsetBoundary(chartBoundary, chartStyle);
            var boundOffset = Utils.BoundaryOffset(offsetBoundary, width, ticks);

            var maxString = "";
            FormattedText fmt;
            switch (dir)
            {
                //DONE
                case Directions.NorthEastNorthWest:
                    for (int i = 0, index = 0; i <= stopsX * 2; i++)
                    {
                        if (i == stopsX) continue;
                        var num = (i - stopsX) * max / stopsX;
                        var number = customValues.Length > index
                            ? customValues[index++]
                            : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);
                        if (chartStyle == ChartStyle.FullStackedBars)
                            number += "%";
                        if (maxString.Length < number.Length) maxString = number;
                    }
                    fmt = new FormattedText(maxString, culture, FlowDirection.LeftToRight,
                        new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip)
                    {
                        TextAlignment = TextAlignment.Right
                    };
                    height = fmt.Width;
                    break;
                //DONE
                case Directions.NorthEast:
                case Directions.NorthEastSouthEast:
                case Directions.SouthEast:
                    {
                        var tpl = Utils.Limits(chartStyle, offsetBoundary, stopsX, ticks, boundOffset, width);
                        var limit = tpl.Item2;
                        for (var i = 0; i < limit; i++)
                        {
                            var num = i;
                            var number = customValues.Length > i
                                ? customValues[i]
                                : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);
                            if (chartStyle == ChartStyle.FullStackedBars)
                                number += "%";
                            if (maxString.Length < number.Length) maxString = number;
                        }
                        fmt = new FormattedText(maxString, culture, FlowDirection.LeftToRight,
                            new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                        height = fmt.Width;
                        break;
                    }
                //DONE
                case Directions.NorthWest:
                    for (int i = 0, index = 0; i < stopsX; i++)
                    {
                        var num = (i - stopsX) * max / stopsX;
                        var number = customValues.Length > index
                            ? customValues[index++]
                            : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);
                        if (chartStyle == ChartStyle.FullStackedBars)
                            number += "%";
                        if (maxString.Length < number.Length) maxString = number;
                    }
                    fmt = new FormattedText(maxString, culture, FlowDirection.LeftToRight,
                            new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                    height = fmt.Width;
                    break;
            }
            return height;
        }
        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

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
                || values.Length != 13
                || !(values[0] is IEnumerable<Series> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle)
                || !(values[2] is int stops)
                || !(values[3] is string format)
                || !(values[4] is FontFamily fontFamily)
                || !(values[5] is double fontSize)
                || !(values[6] is FontStyle fontStyle)
                || !(values[7] is FontWeight fontWeight)
                || !(values[8] is FontStretch fontStretch)
                || !(values[9] is IEnumerable<string> customEnumerable)
                || !(values[10] is bool autoAdjust)
                || !(values[11] is double maxX))
                return null;
            var width = 28.0;

            var series = seriesEnumerable.ToArray();
            var customValues = customEnumerable.ToArray();

            if (!series.Any()) return width;


            if (chartStyle.In(ChartStyle.Bars, ChartStyle.StackedBars, ChartStyle.FullStackedBars, ChartStyle.Radar, ChartStyle.RadarWithMarkers))
                return 0.0;

            var rawValues = chartStyle != ChartStyle.Waterfall
                ? series.Select(s => Tuple.Create(s.Values.ToList(), s.Index)).ToList()
                : new[] { Tuple.Create(series[0].Values.ToList(), series[0].Index) }.ToList();

            var maxCount = rawValues.Max(rw => rw.Item1.Count);
            foreach (var rw in rawValues.Where(rv => rv.Item1.Count < maxCount))
            {
                var diff = maxCount - rw.Item1.Count;
                for (var i = 0; i < diff; i++)
                    rw.Item1.Add(new ChartValue(0));
            }

            var totalValues = chartStyle != ChartStyle.Waterfall
                ? (from s in series from v in s.Values select v.Value.V1).ToArray()
                : series[0].Values.Select(v => v.Value.V1).ToArray();

            var dir = Utils.GetDirection(totalValues, chartStyle);
            var max = autoAdjust ? Utils.GetMaxY(rawValues, chartStyle) : maxX;
            var maxString = "";
            FormattedText fmt;
            switch (dir)
            {
                case Directions.NorthEastSouthEast:
                    for (int i = 0, index = 0; i <= stops * 2; i++)
                    {
                        if (i == stops) continue;
                        var num = (stops - i) * max / stops;
                        var number = customValues.Length > index
                            ? customValues[index++]
                            : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);
                        if (chartStyle.In(ChartStyle.FullStackedColumns, ChartStyle.FullStackedArea,
                            ChartStyle.FullStackedLines, ChartStyle.FullStackedLinesWithMarkers,
                            ChartStyle.SmoothFullStackedLines, ChartStyle.SmoothFullStackedLinesWithMarkers))
                            number += "%";
                        if (maxString.Length < number.Length) maxString = number;

                    }
                    fmt = new FormattedText(maxString, culture, FlowDirection.LeftToRight,
                            new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip)
                    {
                        TextAlignment = TextAlignment.Right
                    };
                    width = fmt.Width;
                    break;
                case Directions.SouthEast:
                    for (int i = 1, index = 0; i <= stops; i++)
                    {
                        var num = -i * max / stops;
                        var number = customValues.Length > index
                            ? customValues[index++]
                            : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);
                        if (chartStyle.In(ChartStyle.FullStackedColumns, ChartStyle.FullStackedArea,
                            ChartStyle.FullStackedLines, ChartStyle.FullStackedLinesWithMarkers,
                            ChartStyle.SmoothFullStackedLines, ChartStyle.SmoothFullStackedLinesWithMarkers))
                            number += "%";
                        if (maxString.Length < number.Length) maxString = number;
                    }
                    fmt = new FormattedText(maxString, culture, FlowDirection.LeftToRight,
                            new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip)
                    {
                        TextAlignment = TextAlignment.Right
                    };
                    width = fmt.Width;
                    break;
                case Directions.NorthEast:
                case Directions.NorthEastNorthWest:
                case Directions.NorthWest:
                    for (int i = 0, index = 0; i < stops; i++)
                    {
                        var num = (stops - i) * max / stops;
                        var number = customValues.Length > index
                            ? customValues[index++]
                            : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);
                        if (chartStyle.In(ChartStyle.FullStackedColumns, ChartStyle.FullStackedArea,
                            ChartStyle.FullStackedLines, ChartStyle.FullStackedLinesWithMarkers,
                            ChartStyle.SmoothFullStackedLines, ChartStyle.SmoothFullStackedLinesWithMarkers))
                            number += "%";
                        if (maxString.Length < number.Length) maxString = number;
                    }
                    fmt = new FormattedText(maxString, culture, FlowDirection.LeftToRight,
                            new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                    if (dir.In(Directions.NorthEast, Directions.NorthEastNorthWest))
                        fmt.TextAlignment = TextAlignment.Right;
                    width = fmt.Width;
                    break;
            }
            //add some extra space
            return width + 4;
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

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
                || values.Length != 15
                || !(values[0] is double height)
                || !(values[1] is IEnumerable<Series> seriesEnumerable)
                || !(values[2] is ChartStyle chartStyle)
                || !(values[3] is int stops)
                || !(values[4] is string format)
                || !(values[5] is FontFamily fontFamily)
                || !(values[6] is double fontSize)
                || !(values[7] is FontStyle fontStyle)
                || !(values[8] is FontWeight fontWeight)
                || !(values[9] is FontStretch fontStretch)
                || !(values[10] is IEnumerable<string> customEnumerable)
                || !(values[11] is bool autoAdjust)
                || !(values[12] is double maxY)
                || !(values[13] is FlowDirection flowDir))
                return null;

            if (chartStyle.In(ChartStyle.Bars, ChartStyle.StackedBars, ChartStyle.FullStackedBars, ChartStyle.SlicedPie, ChartStyle.SolidPie, ChartStyle.Doughnut, ChartStyle.Radar, ChartStyle.RadarWithMarkers))
                return null;
            if (!seriesEnumerable.Any())
                return null;

            var series = seriesEnumerable.ToArray();
            var customValues = customEnumerable.ToArray();
            var drawBetween = chartStyle.In(ChartStyle.Bars, ChartStyle.StackedBars, ChartStyle.FullStackedBars);

            double step, offset;

            height -= 2 * Utils.AXIS_THICKNESS;

            var gm = new PathGeometry();

            var rawValues = chartStyle != ChartStyle.Waterfall
                ? series.Select(s => Tuple.Create(s.Values.ToList(), s.Index)).ToList()
                : new[] { Tuple.Create(series[0].Values.ToList(), series[0].Index) }.ToList();

            var maxCount = rawValues.Max(rw => rw.Item1.Count);
            foreach (var rw in rawValues.Where(rv => rv.Item1.Count < maxCount))
            {
                var diff = maxCount - rw.Item1.Count;
                for (var i = 0; i < diff; i++)
                    rw.Item1.Add(new ChartValue(0));
            }

            var totalValues = chartStyle != ChartStyle.Waterfall
                ? (from s in series from v in s.Values select v.Value.V1).ToArray()
                : series[0].Values.Select(v => v.Value.V1).ToArray();

            var dir = Utils.GetDirection(totalValues, chartStyle);
            var max = autoAdjust ? Utils.GetMaxY(rawValues, chartStyle) : maxY;

            switch (dir)
            {
                case Directions.NorthEastSouthEast:
                    step = height / 2 / stops;
                    for (int i = 0, index = 0; i <= stops * 2; i++)
                    {
                        if (i == stops) continue;
                        var num = (stops - i) * max / stops;
                        var number = customValues.Length > index
                            ? customValues[index++]
                            : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);
                        if (chartStyle.In(ChartStyle.FullStackedColumns, ChartStyle.FullStackedArea,
                            ChartStyle.FullStackedLines, ChartStyle.FullStackedLinesWithMarkers,
                            ChartStyle.SmoothFullStackedLines, ChartStyle.SmoothFullStackedLinesWithMarkers))
                            number += "%";
                        var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                            new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                        if (flowDir == FlowDirection.LeftToRight)
                            fmt.TextAlignment = TextAlignment.Right;
                        offset = i < stops
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
                    step = height / stops;
                    for (int i = 1, index = 0; i <= stops; i++)
                    {
                        var num = -i * max / stops;
                        var number = customValues.Length > index
                            ? customValues[index++]
                            : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);
                        if (chartStyle.In(ChartStyle.FullStackedColumns, ChartStyle.FullStackedArea,
                            ChartStyle.FullStackedLines, ChartStyle.FullStackedLinesWithMarkers,
                            ChartStyle.SmoothFullStackedLines, ChartStyle.SmoothFullStackedLinesWithMarkers))
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
                    step = height / stops;
                    for (int i = 0, index = 0; i < stops; i++)
                    {
                        var num = (stops - i) * max / stops;
                        var number = customValues.Length > index
                            ? customValues[index++]
                            : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);
                        if (chartStyle.In(ChartStyle.FullStackedColumns, ChartStyle.FullStackedArea,
                            ChartStyle.FullStackedLines, ChartStyle.FullStackedLinesWithMarkers,
                            ChartStyle.SmoothFullStackedLines, ChartStyle.SmoothFullStackedLinesWithMarkers))
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

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
                || values.Length != 16
                || !(values[0] is double width)
                || !(values[1] is IEnumerable<Series> seriesEnumerable)
                || !(values[2] is ChartStyle chartStyle)
                || !(values[3] is int stopsX)
                || !(values[4] is string format)
                || !(values[5] is FontFamily fontFamily)
                || !(values[6] is double fontSize)
                || !(values[7] is FontStyle fontStyle)
                || !(values[8] is FontWeight fontWeight)
                || !(values[9] is FontStretch fontStretch)
                || !(values[10] is IEnumerable<string> customEnumerable)
                || !(values[11] is bool autoAdjust)
                || !(values[12] is double maxY)
                || !(values[13] is FlowDirection flowDir)
                || !(values[14] is ChartBoundary chartBoundary))
                return null;

            if (chartStyle.In(ChartStyle.Area, ChartStyle.StackedArea, ChartStyle.FullStackedArea, ChartStyle.SlicedPie, ChartStyle.SolidPie, ChartStyle.Doughnut, ChartStyle.Radar, ChartStyle.RadarWithMarkers))
                return null;
            if (!seriesEnumerable.Any()) return null;

            var series = seriesEnumerable.ToArray();
            var drawBetween = Utils.StyleColumns(chartStyle);
            var customValues = customEnumerable.ToArray();

            double xStep;

            width -= 2 * Utils.AXIS_THICKNESS;

            var gm = new PathGeometry();

            var rawValues = chartStyle != ChartStyle.Waterfall
                ? series.Select(s => Tuple.Create(s.Values.ToList(), s.Index)).ToList()
                : new[] { Tuple.Create(series[0].Values.ToList(), series[0].Index) }.ToList();

            var ticks = rawValues.Max(rw => rw.Item1.Count);
            foreach (var rw in rawValues.Where(rv => rv.Item1.Count < ticks))
            {
                var diff = ticks - rw.Item1.Count;
                for (var i = 0; i < diff; i++)
                    rw.Item1.Add(new ChartValue(0));
            }

            var totalValues = chartStyle != ChartStyle.Waterfall
                ? (from s in series from v in s.Values select v.Value.V1).ToArray()
                : series[0].Values.Select(v => v.Value.V1).ToArray();

            var dir = Utils.GetDirection(totalValues, chartStyle);
            var max = autoAdjust ? Utils.GetMaxY(rawValues, chartStyle) : maxY;

            var offsetBoundary = Utils.OffsetBoundary(chartBoundary, chartStyle);
            var boundOffset = Utils.BoundaryOffset(offsetBoundary, width, ticks);

            int limit;

            switch (dir)
            {
                //DONE
                case Directions.NorthEast:
                case Directions.NorthEastSouthEast:
                    {
                        var tpl = Utils.Limits(chartStyle, offsetBoundary, stopsX, ticks, boundOffset, width);
                        xStep = tpl.Item1;
                        limit = tpl.Item2;

                        for (int i = 0, j = limit - 1; i < limit; i++, j--)
                        {
                            var num = flowDir == FlowDirection.LeftToRight ? i : j;
                            var index = flowDir == FlowDirection.LeftToRight ? i : j;
                            var number = customValues.Length > index
                                ? customValues[index]
                                : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);
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
                                ? new RotateTransform(-45, pt.X + fmt.Width, pt.Y)
                                : new RotateTransform(45, pt.X, pt.Y));
                            if (flowDir == FlowDirection.RightToLeft)
                                trgr.Children.Add(new ScaleTransform { ScaleX = -1, CenterX = width / 2 });
                            ngm.Transform = trgr;
                            gm.AddGeometry(ngm);
                        }
                        break;
                    }
                //DONE
                case Directions.NorthEastNorthWest:
                    {
                        if (!Utils.StyleBars(chartStyle))
                            return null;
                        xStep = width / 2 / stopsX;
                        for (int i = 0, index = 0; i <= stopsX * 2; i++)
                        {
                            if (i == stopsX) continue;
                            var num = (i - stopsX) * max / stopsX;
                            if (flowDir == FlowDirection.RightToLeft)
                                num *= -1;
                            var number = customValues.Length > index
                                ? customValues[index++]
                                : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);

                            if (chartStyle == ChartStyle.FullStackedBars)
                                number += "%";

                            var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                            var x = i < stopsX
                                ? i * xStep + 1 + fmt.Width / number.Length
                                : i * xStep - fmt.Width;
                            var pt = new Point(x, 0);
                            var ngm = fmt.BuildGeometry(pt);

                            var trgr = new TransformGroup();
                            if (i < stopsX)
                                trgr.Children.Add(new RotateTransform(45, pt.X, pt.Y));
                            else if (i > stopsX)
                                trgr.Children.Add(new RotateTransform(-45, pt.X + fmt.Width, pt.Y));

                            if (flowDir == FlowDirection.RightToLeft)
                                trgr.Children.Add(new ScaleTransform { ScaleX = -1, CenterX = width / 2 });
                            ngm.Transform = trgr;
                            gm.AddGeometry(ngm);
                        }
                        break;
                    }
                //DONE
                case Directions.SouthEast:
                    {
                        var tpl = Utils.Limits(chartStyle, offsetBoundary, stopsX, ticks, boundOffset, width);
                        xStep = tpl.Item1;
                        limit = tpl.Item2;
                        for (int i = 0, j = limit - 1; i < limit; i++, j--)
                        {
                            var num = flowDir == FlowDirection.LeftToRight
                                ? i
                                : j;
                            var index = flowDir == FlowDirection.LeftToRight ? i : j;
                            var number = customValues.Length > index
                                    ? customValues[index]
                                    : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);

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
                //DONE
                case Directions.NorthWest:
                    {
                        if (!Utils.StyleBars(chartStyle))
                            return null;
                        xStep = width / stopsX;
                        for (int i = 0, j = stopsX; i <= stopsX; i++, j--)
                        {
                            var index = flowDir == FlowDirection.LeftToRight ? j : i;
                            var num = flowDir == FlowDirection.LeftToRight
                                ? (i - stopsX) * max / stopsX
                                : (j - stopsX) * max / stopsX;
                            var number = customValues.Length > index
                                ? customValues[index]
                                : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);
                            if (chartStyle == ChartStyle.FullStackedBars)
                                number += "%";
                            var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                            var x = flowDir == FlowDirection.LeftToRight
                                ? (i != stopsX ? i * xStep : i * xStep - fmt.Width)
                                : j != stopsX ? i * xStep - fmt.Width : i * xStep;
                            var y = (flowDir == FlowDirection.LeftToRight && i != stopsX) ||
                                    (flowDir == FlowDirection.RightToLeft && j != stopsX)
                                ? 0
                                : fmt.Height / 2;
                            var pt = new Point(x, y);
                            var ngm = fmt.BuildGeometry(pt);

                            var trgr = new TransformGroup();
                            if (flowDir == FlowDirection.LeftToRight && i != stopsX)
                                trgr.Children.Add(new RotateTransform(45, pt.X, pt.Y + fmt.Height));
                            else if (flowDir == FlowDirection.RightToLeft && j != stopsX)
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

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
                //|| values.Length != 8
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is int stopsX)
                || !(values[3] is int stopsY)
                || !(values[4] is IEnumerable<Series> seriesEnumerable)
                || !(values[5] is ChartStyle chartStyle)
                || !(values[6] is ChartBoundary chartBoundary)
                || !(values[7] is FontFamily fontFamily)
                || !(values[8] is double fontSize)
                || !(values[9] is FontStyle fontStyle)
                || !(values[10] is FontWeight fontWeight)
                || !(values[11] is FontStretch fontStretch))
                return null;

            if (chartStyle.In(ChartStyle.Area, ChartStyle.StackedArea, ChartStyle.FullStackedArea, ChartStyle.Radar, ChartStyle.RadarWithMarkers))
                return null;
            if (!seriesEnumerable.Any())
                return null;

            const double size = 4.0;

            double xStep, yStep;

            var series = seriesEnumerable.ToArray();
            var offsetBoundary = Utils.OffsetBoundary(chartBoundary, chartStyle);
            var totalValues = (from s in series from v in s.Values select v.Value.V1).ToArray();
            var dir = Utils.GetDirection(totalValues, chartStyle);
            var ticks = series.Select(s => s.Values.Count).Max();

            width -= 2 * Utils.AXIS_THICKNESS;
            height -= 2 * Utils.AXIS_THICKNESS;

            var boundOffset = Utils.BoundaryOffset(offsetBoundary, width, ticks);

            var gm = new PathGeometry();
            int limit;

            switch (dir)
            {
                case Directions.NorthEast:
                    {
                        var x = Utils.AXIS_THICKNESS + boundOffset;
                        var tpl = Utils.Limits(chartStyle, offsetBoundary, stopsX, ticks, boundOffset, width);
                        xStep = tpl.Item1;
                        limit = tpl.Item2;
                        for (var i = 0; i < limit; i++)
                        {
                            var start = new Point(x, height);
                            var end = new Point(start.X, start.Y - size);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                            x += xStep;
                        }

                        if (Utils.StyleBars(chartStyle))
                            yStep = height / ticks;
                        else
                            yStep = height / stopsY;
                        var y = Utils.AXIS_THICKNESS;
                        for (var i = 1; i < stopsY; i++)
                        {
                            y += yStep;
                            var start = new Point(Utils.AXIS_THICKNESS, y);
                            var end = new Point(start.X + size, start.Y);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                        }
                        break;
                    }
                case Directions.NorthEastNorthWest:
                    {
                        if (!Utils.StyleBars(chartStyle))
                            return null;
                        xStep = width / 2 / stopsX;
                        var x = Utils.AXIS_THICKNESS;
                        for (var i = 1; i < stopsX * 2; i++)
                        {
                            x += xStep;
                            if (i == stopsX) continue;
                            var start = new Point(x, height + size);
                            var end = new Point(start.X, height - size);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                        }
                        if (Utils.StyleBars(chartStyle))
                            yStep = height / ticks;
                        else
                            yStep = height / stopsY;
                        var y = Utils.AXIS_THICKNESS;
                        for (var i = 1; i < stopsY; i++)
                        {
                            y += yStep;
                            var start = new Point(width / 2 - size, y);
                            var end = new Point(width / 2 + size, start.Y);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                        }
                        break;
                    }
                case Directions.NorthEastSouthEast:
                    {
                        var x = Utils.AXIS_THICKNESS + boundOffset;
                        var tpl = Utils.Limits(chartStyle, offsetBoundary, stopsX, ticks, boundOffset, width);
                        xStep = tpl.Item1;
                        limit = tpl.Item2;
                        for (var i = 0; i < limit; i++)
                        {
                            var start = new Point(x, height / 2 + size);
                            var end = new Point(start.X, height / 2 - size);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                            x += xStep;
                        }
                        if (Utils.StyleBars(chartStyle))
                            yStep = height / 2 / ticks;
                        else
                            yStep = height / 2 / stopsY;
                        var y = Utils.AXIS_THICKNESS;
                        for (var i = 1; i < stopsY * 2; i++)
                        {
                            y += yStep;
                            if (i == stopsY) continue;
                            var start = new Point(size, y);
                            var end = new Point(-size, start.Y);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                        }
                        break;
                    }
                case Directions.NorthWest:
                    {
                        if (!Utils.StyleBars(chartStyle))
                            return null;
                        xStep = width / stopsX;
                        var x = Utils.AXIS_THICKNESS;
                        for (var i = 1; i < stopsX; i++)
                        {
                            x += xStep;
                            var start = new Point(x, height - size);
                            var end = new Point(start.X, height + size);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                        }
                        if (Utils.StyleBars(chartStyle))
                            yStep = height / ticks;
                        else
                            yStep = height / stopsY;
                        var y = Utils.AXIS_THICKNESS;
                        for (var i = 1; i < stopsY; i++)
                        {
                            y += yStep;
                            var start = new Point(width - size, y);
                            var end = new Point(width + size, start.Y);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                        }
                        break;
                    }
                case Directions.SouthEast:
                    {
                        var x = Utils.AXIS_THICKNESS + boundOffset;
                        var tpl = Utils.Limits(chartStyle, offsetBoundary, stopsX, ticks, boundOffset, width);
                        xStep = tpl.Item1;
                        limit = tpl.Item2;
                        for (var i = 0; i < limit; i++)
                        {
                            var start = new Point(x, size);
                            var end = new Point(start.X, -size);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                            x += xStep;
                        }
                        if (chartStyle.In(ChartStyle.Bars, ChartStyle.StackedBars, ChartStyle.FullStackedBars))
                            yStep = height / ticks;
                        else
                            yStep = height / stopsY;
                        var y = Utils.AXIS_THICKNESS;
                        for (var i = 1; i < stopsY; i++)
                        {
                            y += yStep;
                            var start = new Point(size, y);
                            var end = new Point(-size, start.Y);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

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
                //|| values.Length != 6
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is IEnumerable<Series> seriesEnumerable)
                || !(values[3] is ChartStyle chartStyle)
                || !(values[4] is int stopsX)
                || !(values[5] is ChartBoundary chartBoundary)
                || !(values[6] is FontFamily fontFamily)
                || !(values[7] is double fontSize)
                || !(values[8] is FontStyle fontStyle)
                || !(values[9] is FontWeight fontWeight)
                || !(values[10] is FontStretch fontStretch))
                return null;

            if (chartStyle.In(ChartStyle.Area, ChartStyle.StackedArea, ChartStyle.FullStackedArea))
                return null;
            if (!seriesEnumerable.Any())
                return null;

            var series = seriesEnumerable.ToArray();
            var offsetBoundary = Utils.OffsetBoundary(chartBoundary, chartStyle);
            var totalValues = (from s in series from v in s.Values select v.Value.V1).ToArray();
            var dir = Utils.GetDirection(totalValues, chartStyle);
            var ticks = series.Select(s => s.Values.Count).Max();
            double xStep;

            width -= 2 * Utils.AXIS_THICKNESS;
            height -= 2 * Utils.AXIS_THICKNESS;

            var boundOffset = Utils.BoundaryOffset(offsetBoundary, width, ticks);
            int limit;
            var gm = new PathGeometry();
            switch (dir)
            {
                case Directions.NorthEast:
                    {
                        var x = Utils.AXIS_THICKNESS + boundOffset;
                        var tpl = Utils.Limits(chartStyle, offsetBoundary, stopsX, ticks, boundOffset, width);
                        xStep = tpl.Item1;
                        limit = tpl.Item2;
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
                        xStep = width / 2 / stopsX;
                        var x = Utils.AXIS_THICKNESS;
                        for (var i = 1; i < stopsX * 2; i++)
                        {
                            x += xStep;
                            if (i == stopsX) continue;
                            var start = new Point(x, 0);
                            var end = new Point(start.X, height);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                        }
                        break;
                    }
                case Directions.NorthEastSouthEast:
                    {
                        var x = Utils.AXIS_THICKNESS + boundOffset;
                        var tpl = Utils.Limits(chartStyle, offsetBoundary, stopsX, ticks, boundOffset, width);
                        xStep = tpl.Item1;
                        limit = tpl.Item2;
                        for (var i = 0; i < limit; i++)
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
                        xStep = width / stopsX;
                        var x = Utils.AXIS_THICKNESS;
                        for (var i = 1; i < stopsX; i++)
                        {
                            x += xStep;
                            var start = new Point(x, 0);
                            var end = new Point(start.X, height);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                        }
                        break;
                    }
                case Directions.SouthEast:
                    {
                        var x = Utils.AXIS_THICKNESS + boundOffset;
                        var tpl = Utils.Limits(chartStyle, offsetBoundary, stopsX, ticks, boundOffset, width);
                        xStep = tpl.Item1;
                        limit = tpl.Item2;
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

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
                //|| values.Length != 5
                || !(values[0] is double)
                || !(values[1] is double)
                || !(values[2] is IEnumerable<Series> seriesEnumerable)
                || !(values[3] is ChartStyle chartStyle)
                || !(values[4] is int stepsY)
                || !(values[5] is FontFamily fontFamily)
                || !(values[6] is double fontSize)
                || !(values[7] is FontStyle fontStyle)
                || !(values[8] is FontWeight fontWeight)
                || !(values[9] is FontStretch fontStretch))
                return null;

            if (!seriesEnumerable.Any())
                return null;

            var width = (double)values[0];
            var height = (double)values[1];
            var series = seriesEnumerable.ToArray();

            var totalValues = chartStyle != ChartStyle.Waterfall
               ? (from s in series from v in s.Values select v.Value.V1).ToArray()
               : series[0].Values.Select(v => v.Value.V1).ToArray();

            var dir = Utils.GetDirection(totalValues, chartStyle);

            int limit;
            var gm = new PathGeometry();

            height -= 2 * Utils.AXIS_THICKNESS;
            width -= 2 * Utils.AXIS_THICKNESS;

            switch (dir)
            {
                case Directions.NorthEastSouthEast:
                    limit = stepsY * 2;
                    break;
                case Directions.NorthEast:
                case Directions.NorthEastNorthWest:
                case Directions.NorthWest:
                case Directions.SouthEast:
                    limit = stepsY;
                    break;
                default:
                    return null;
            }
            var yStep = height / limit;
            var y = Utils.AXIS_THICKNESS;
            for (var i = 0; i <= limit; i++)
            {
                var start = new Point(Utils.AXIS_THICKNESS, y);
                var end = new Point(width, start.Y);
                gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                y += yStep;
            }
            return gm;
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Defines top coordinate of chart image when chart style is set to <see cref="ChartStyle.SolidPie"/> or <see cref="ChartStyle.SlicedPie"/> or <see cref="ag.WPF.Chart.ChartStyle.Doughnut"/>
    /// </summary>
    public class PieTopConverter : IMultiValueConverter
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
                || values.Length != 2
                || !(values[0] is double width)
                || !(values[1] is double height))
                return null;
            var cw = width > height ? height : width;
            return (height - cw) / 2;
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Defines left coordinate of chart image when chart style is set to <see cref="ChartStyle.SolidPie"/> or <see cref="ChartStyle.SlicedPie"/> or <see cref="ag.WPF.Chart.ChartStyle.Doughnut"/>
    /// </summary>
    public class PieLeftConverter : IMultiValueConverter
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
                || values.Length != 2
                || !(values[0] is double width)
                || !(values[1] is double height))
                return null;
            var cw = width > height ? height : width;
            return (width - cw) / 2;
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Defines text to be shown as pie sector tooltip, when chart style is set to <see cref="ChartStyle.SolidPie"/> or <see cref="ChartStyle.SlicedPie"/> or <see cref="ag.WPF.Chart.ChartStyle.Doughnut"/>
    /// </summary>
    public class PieSectionTextConverter : IMultiValueConverter
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
                || values.Length != 2
                || !(values[0] is ChartValues chartValues)
                || !(values[1] is string format)
                || !(parameter is ChartValue chartValue))
                return null;
            var sum = chartValues.Sum(p => Math.Abs(p.Value.V1));
            if (format.EndsWith("%")) format = format.Substring(0, format.Length - 1);
            var perc = Math.Abs(chartValue.Value.V1) / sum * 100;
            var sectorData = chartValue.CustomValue != null
                    ? chartValue.CustomValue + " (" + perc.ToString(format) + "%)"
                    : chartValue.Value.V1 + " (" + perc.ToString(format) + "%)";
            return sectorData;
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RadarLinesPathConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null
                //|| values.Length != 8
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is IEnumerable<Series> seriesEnumerable)
                || !seriesEnumerable.Any()
                || !(values[3] is ChartStyle chartStyle)
                || !chartStyle.In(ChartStyle.Radar, ChartStyle.RadarWithMarkers, ChartStyle.RadarArea)
                //|| !(values[6] is string format)
                || !(values[4] is FontFamily fontFamily)
                || !(values[5] is double fontSize)
                || !(values[6] is FontStyle fontStyle)
                || !(values[7] is FontWeight fontWeight)
                || !(values[8] is FontStretch fontStretch)
                || !(values[9] is IEnumerable<string> customEnumerable)
                || !(values[10] is int linesCount)
                || !(values[11] is bool autoAdjust))
                return null;

            var gm = new PathGeometry();
            var customValues = customEnumerable.ToArray();

            var series = seriesEnumerable.ToArray();
            var currentDegrees = 0.0;
            var pointsCount = series.Max(s => s.Values.Count);
            var degreesStep = 360 / pointsCount;

            var maxCv = customValues.Any() ? customValues.Max(v => (v, v.Length)) : (v: "", Length: 0);
            var number = maxCv.Length > pointsCount.ToString(culture).Length ? maxCv.v : pointsCount.ToString(culture);
            var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);

            var radius = width > height ? (height - 2 * Utils.AXIS_THICKNESS) / 2 : (width - 2 * Utils.AXIS_THICKNESS) / 2;
            var centerPoint = new Point(radius, radius);
            if (width > height)
                radius -= (2 * fmt.Height + 8);
            else
                radius -= (2 * fmt.Width + 8);
            var xBeg = radius;
            var yBeg = 90.0;

            var fmtY = new FormattedText("AAA", culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
            var (max, min, realLinesCount, stepSize, _, _, _) = Utils.GetMeasures(series, linesCount, radius, fmtY.Height, autoAdjust, centerPoint);
            linesCount = realLinesCount;

            var distanceStep = radius / linesCount;

            for (var step = 0; step < linesCount; step++)
            {
                var distance = radius - distanceStep * step;
                var points = new List<Point>();
                for (var i = 0; i < pointsCount; i++)
                {
                    currentDegrees = 90 + i * degreesStep;
                    var rads = currentDegrees * Math.PI / 180;
                    xBeg = centerPoint.X - distance * Math.Cos(rads);
                    yBeg = centerPoint.Y - distance * Math.Sin(rads);
                    points.Add(new Point(xBeg, yBeg));
                }

                for (var i = 0; i < points.Count - 1; i++)
                {
                    gm.AddGeometry(new LineGeometry(points[i], points[i + 1]));
                }
                gm.AddGeometry(new LineGeometry(points[points.Count - 1], points[0]));
            }

            return gm;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class RadarAxesValuesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null
                //|| values.Length != 8
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is IEnumerable<Series> seriesEnumerable)
                || !seriesEnumerable.Any()
                || !(values[3] is ChartStyle chartStyle)
                || !chartStyle.In(ChartStyle.Radar, ChartStyle.RadarWithMarkers,ChartStyle.RadarArea)
                || !(values[4] is FontFamily fontFamily)
                || !(values[5] is double fontSize)
                || !(values[6] is FontStyle fontStyle)
                || !(values[7] is FontWeight fontWeight)
                || !(values[8] is FontStretch fontStretch)
                || !(values[9] is FlowDirection flowDir)
                || !(values[10] is IEnumerable<string> customEnumerableX)
                || !(values[11] is IEnumerable<string> customEnumerableY)
                || !(values[12] is int linesCount)
                || linesCount < 2
                || !(values[13] is string format)
                || !(values[14] is bool autoAdjust))
                return null;

            var gm = new PathGeometry();
            var customValuesHorizontal = customEnumerableX.ToArray();
            var customValuesVertical = customEnumerableY.ToArray();

            var series = seriesEnumerable.ToArray();
            var currentDegrees = 0.0;
            var pointsCount = series.Max(s => s.Values.Count);
            var degreesStep = 360 / pointsCount;

            var maxCv = customValuesHorizontal.Any() ? customValuesHorizontal.Max(v => (v, v.Length)) : (v: "", Length: 0);
            var number = maxCv.Length > pointsCount.ToString(culture).Length ? maxCv.v : pointsCount.ToString(culture);
            var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);

            var radius = width > height ? (height - 2 * Utils.AXIS_THICKNESS) / 2 : (width - 2 * Utils.AXIS_THICKNESS) / 2;

            var centerPoint = new Point(radius, radius);
            if (width > height)
                radius -= (2 * fmt.Height + 8);
            else
                radius -= (2 * fmt.Width + 8);

            for (var i = 0; i < pointsCount; i++)
            {
                currentDegrees = 90 + i * degreesStep;
                var rads = currentDegrees * Math.PI / 180;
                var num = customValuesHorizontal.Length > i ? customValuesHorizontal[i] : (i + 1).ToString(culture);
                fmt = new FormattedText(num, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                var x = centerPoint.X - (radius + 4) * Math.Cos(rads);
                var y = centerPoint.Y - (radius + 4) * Math.Sin(rads);
                if (currentDegrees == 90)
                {
                    x -= fmt.Width / 2;
                    y -= (fmt.Height + 4);
                }
                else if (currentDegrees == 180)
                {
                    x += 4;
                    y -= fmt.Height / 2;
                }
                else if (currentDegrees == 270)
                {
                    x -= fmt.Width / 2;
                    y += 4;
                }
                else if (currentDegrees == 360)
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
            var (max, min, realLinesCount, stepSize, _, _, _) = Utils.GetMeasures(series, linesCount, radius, fmtY.Height, autoAdjust, centerPoint);
            linesCount = realLinesCount;

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

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

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
                || values.Length != 8
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is IEnumerable<Series> seriesEnumerable)
                || !seriesEnumerable.Any()
                || !(values[3] is bool isEnabled)
                || !(values[4] is Brush disabledBrush)
                || !(values[5] is Brush backgroundBrush)
                || !(values[6] is ChartStyle chartStyle)
                || !chartStyle.In(ChartStyle.SlicedPie, ChartStyle.SolidPie, ChartStyle.Doughnut)
                || !(values[7] is string format))
                return null;

            var brushIndex = 0;
            var radius = width > height ? (height - 2 * Utils.AXIS_THICKNESS) / 2 : (width - 2 * Utils.AXIS_THICKNESS) / 2;
            var series = seriesEnumerable.First();
            var pts = series.Values.ToArray();
            var sum = pts.Sum(p => Math.Abs(p.Value.V1));
            var currentDegrees = 90.0;
            var startPoint = new Point(radius, radius);
            var xBeg = radius;
            var yBeg = 0.0;

            var dGroup = new DrawingGroup();

            if (format.EndsWith("%")) format = format.Substring(0, format.Length - 1);
            if (pts.Length == 1)
            {
                var sectorData = pts[0].CustomValue != null
                    ? pts[0].CustomValue + " (" + 100.ToString(format) + "%)"
                    : pts[0].Value.V1 + " (" + 100.ToString(format) + "%)";
                var ellipseGeometry = new EllipseGeometry(new Rect(new Point(0, 0), new Point(radius * 2, radius * 2)));
                var combinedGeometry = new CombinedGeometry
                {
                    GeometryCombineMode = GeometryCombineMode.Exclude,
                    Geometry1 = ellipseGeometry
                };
                if (chartStyle == ChartStyle.Doughnut)
                    combinedGeometry.Geometry2 = new EllipseGeometry(startPoint, radius / 2, radius / 2);

                combinedGeometry.SetValue(Series.SectorDataProperty, sectorData);

                var gmDrawing = new GeometryDrawing
                {
                    Brush = isEnabled ? series.PieBrushes[brushIndex] : disabledBrush,
                    Pen = new Pen(isEnabled ? series.PieBrushes[brushIndex] : disabledBrush, 1),
                    Geometry = combinedGeometry
                };
                dGroup.Children.Add(gmDrawing);
                return dGroup;
            }

            var lines = new List<LineGeometry>();
            foreach (var pt in pts)
            {
                var addition = Math.Abs(pt.Value.V1) / sum * 360;
                if (currentDegrees <= 90 && currentDegrees > 0)
                {
                    if (currentDegrees - addition >= 0)
                    {
                        currentDegrees -= addition;
                    }
                    else
                    {
                        currentDegrees = 360 - Math.Abs(currentDegrees - addition);
                    }
                }
                else if (Math.Abs(currentDegrees) < double.Epsilon)
                {
                    currentDegrees = 360 - addition;
                }
                else
                {
                    currentDegrees -= addition;
                }

                if (brushIndex == series.PieBrushes.Length()) brushIndex = 0;
                var gmDrawing = new GeometryDrawing
                {
                    Brush = isEnabled ? series.PieBrushes[brushIndex] : disabledBrush,
                    Pen = new Pen(isEnabled ? series.PieBrushes[brushIndex] : disabledBrush, 1)
                };
                brushIndex++;

                var pathGeometry = new PathGeometry();
                var segments = new List<PathSegment>();

                var start = new Point(xBeg, yBeg);
                segments.Add(new LineSegment(start, true));

                var rads = currentDegrees * Math.PI / 180;
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
                    IsLargeArc = addition >= 180
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
                //else if (chartStyle == ChartStyle.SlicedPie)
                //{
                //    var rectGeometry = new RectangleGeometry(new Rect(startPoint.X-2, 0, 4, radius), 4, 4, new RotateTransform(currentDegrees, startPoint.X-2, startPoint.Y));
                //    //var seg1 = new PolyLineSegment(new[] { new Point(xBeg, yBeg), new Point(xBeg + 12, yBeg + 12), new Point(radius + 12, radius + 12) }, false);
                //    //var pathFigure = new PathFigure(startPoint, new[] { seg1 }, true);
                //    //var pathLineGeometry = new PathGeometry(new[] { pathFigure });
                //    combinedGeometry.Geometry2 = rectGeometry;
                //}

                var perc = Math.Abs(pt.Value.V1) / sum * 100;
                var sectorData = pt.CustomValue != null
                    ? pt.CustomValue + " (" + perc.ToString(format) + "%)"
                    : pt.Value.V1 + " (" + perc.ToString(format) + "%)";
                combinedGeometry.SetValue(Series.SectorDataProperty, sectorData);
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
