using ag.WPF.Chart.Values;
using ag.WPF.Chart.Series;
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

        private static readonly int[] _bases = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        internal static Quadrants GetQuadrant(double degrees)
        {
            if (degrees >= 0.0 && degrees <= 90.0)
                return Quadrants.UpRight;
            if (degrees <= 360.0 && degrees >= 270.0)
                return Quadrants.DownRight;
            if (degrees <= 270.0 && degrees >= 180.0)
                return Quadrants.DownLeft;
            return Quadrants.UpLeft;
        }

        internal static Quadrants GetRadarQuadrant(double degrees)
        {
            if (degrees >= 0.0 && degrees <= 90.0)
                return Quadrants.UpLeft;
            if (degrees <= 360.0 && degrees >= 270.0)
                return Quadrants.DownLeft;
            if (degrees <= 270.0 && degrees >= 180.0)
                return Quadrants.DownRight;
            if (degrees > 360.0)
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

        private static bool isEven(double number)
        {
            return number % 2 == 0;
        }

        private static bool is5Delimited(double number)
        {
            return number % 5 == 0;
        }

        private static (double max, double min) getMaxMinForWaterfall(double[] values)
        {
            var temp = 0.0;
            var list = new List<double>();

            for (var i = 0; i < values.Length; i++)
            {
                temp += values[i];
                list.Add(temp);
            }
            return (list.Max(), list.Min());
        }

        private static int getMaxFractionPower(IEnumerable<double> numbers)
        {
            return numbers.Select(n => GetDecimalPlaces(n)).Max();
        }

        internal static int GetDecimalPlaces(double number)
        {
            return BitConverter.GetBytes(decimal.GetBits((decimal)number)[3])[2];
        }

        internal static double GetUnitsForBars(ISeries[] series, ChartStyle chartStyle, Directions dir, double width, double height, double boundOffset, int linesCountX, double formatHeight, bool autoAdjust, double maxX)
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
            if (!autoAdjust)
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

        internal static double GetUnitsForLines(ISeries[] series, ChartStyle chartStyle, Directions dir, double width, double height, double boundOffset, int linesCountY, double formatHeight, bool autoAdjust, double maxY)
        {
            var centerX = 0.0;
            var radius = 0.0;

            switch (dir)
            {
                case Directions.NorthEast:
                case Directions.NorthWest:
                case Directions.SouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    radius = height - Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = width / 2;
                    radius = height - Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    radius = (height - Utils.AXIS_THICKNESS) / 2;
                    break;
            }
            if (!autoAdjust)
                return radius / maxY;
            var centerPoint = new Point(centerX, radius);
            var (_, _, _, _, _, units, _) = Utils.GetMeasures(
               chartStyle,
               series,
               linesCountY,
               radius,
               formatHeight,
               centerPoint,
               dir == Directions.NorthEastSouthEast);
            return units;
        }

        internal static List<(List<IChartValue> Values, int Index)> GetPaddedSeries(IEnumerable<ISeries> series)
        {
            var rawValues = series.Select(s => (s.Values.ToList(), s.Index)).ToList();
            var maxCount = rawValues.Max(rw => rw.Item1.Count);
            foreach (var rw in rawValues.Where(rv => rv.Item1.Count < maxCount))
            {
                var diff = maxCount - rw.Item1.Count;
                for (var i = 0; i < diff; i++)
                    rw.Item1.Add(new PlainChartValue(0));
            }
            return rawValues;
        }

        private static (double max, double min, int linesCount, double step, double stepLength, double units, ZeroPoint zeroPoint) getMeasuresForPositive(double diff, int linesCount, double radius, double fontHeight, int fractionPower, ZeroPoint centerPoint)
        {
            var stepSize = 0.0;
            var stepLength = 0.0;
            var units = 0.0;

            // round max to next integer
            diff = Math.Ceiling(diff);

            if (fractionPower > 0)
            {
                diff *= Math.Pow(10, fractionPower);
            }

            var power = Math.Abs((int)diff).ToString().Length - 1;

            var p = power >= 3 ? power - 1 : power;
            // do not increase max for integers that are equal to 10 power
            if (diff % Math.Pow(10, p) != 0)
                diff = Math.Sign(diff) * roundInt((int)Math.Abs(diff), (int)Math.Pow(10, power));
            // difference is alway max
            // get all available integer lines counts
            var lines = calculatedSteps(power, diff).Where(l => IsInteger(l)).OrderBy(l => l).Distinct().ToArray();
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
                while (!IsInteger(stepSize))
                {
                    diff = Math.Sign(diff) * roundInt((int)Math.Abs(diff), (int)Math.Pow(10, power));
                    stepSize = diff / linesCount;
                }
                stepLength = radius / linesCount;
                units = Math.Abs(radius / diff);
            }

            // zero point Y-coordinate is stays the same and leve stays 0

            if (fractionPower > 0)
            {
                var m = Math.Pow(10, fractionPower);
                diff /= m;
                units *= m;
                stepSize /= m;
            }
            return (diff, 0, linesCount, stepSize, stepLength, units, centerPoint);
        }

        private static (double max, double min, int linesCount, double stepSize, double stepLength, double units, ZeroPoint zeroPoint) getMeasuresForNegative(double min, int linesCount, double radius, double fontHeight, int fractionPower, ZeroPoint zeroPoint)
        {
            var stepSize = 0.0;
            var stepLength = 0.0;
            var units = 0.0;
            // round min to prevous integer
            min = Math.Floor(min);

            if (fractionPower > 0)
            {
                min = -Math.Abs(min * Math.Pow(10, fractionPower));
            }

            var power = Math.Abs((int)min).ToString().Length - 1;

            var p = power >= 3 ? power - 1 : power;
            // do not increase max for integers that are equal to 10 power
            if (min % Math.Pow(10, p) != 0)
                min = Math.Sign(min) * roundInt((int)Math.Abs(min), (int)Math.Pow(10, power));
            // difference is always equal absolute value of min
            var diff = Math.Abs(min);
            // get all available integer lines counts
            var lines = calculatedSteps(power, Math.Abs(min)).Where(l => IsInteger(l)).OrderBy(l => l).Distinct().ToArray();
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
                while (!IsInteger(stepSize))
                {
                    min = Math.Sign(min) * roundInt((int)Math.Abs(min), (int)Math.Pow(10, power));
                    diff = Math.Abs(min);
                    stepSize = diff / linesCount;
                }
                units = Math.Abs(radius / diff);
                stepLength = radius / linesCount;
            }

            // find zero point
            zeroPoint.Point.Y -= stepSize * linesCount * units;
            zeroPoint.Level = linesCount;

            if (fractionPower > 0)
            {
                var m = Math.Pow(10, fractionPower);
                min /= m;
                units *= m;
                stepSize /= m;
            }

            return (0, min, linesCount, stepSize, stepLength, units, zeroPoint);
        }

        private static (double max, double min, int linesCount, double stepSize, double stepLength, double units, ZeroPoint zeroPoint) getMeasuresForComplex(ChartStyle chartStyle, double max, double min, int linesCount, double radius, double fontHeight, int fractionPower, ZeroPoint zeroPoint, bool splitSides)
        {
            int tempLines;

            // round max to next integer
            max = Math.Ceiling(max);
            // round min to prevous integer
            min = Math.Floor(min);

            if (fractionPower > 0)
            {
                max *= Math.Pow(10, fractionPower);
                min = -Math.Abs(min * Math.Pow(10, fractionPower));
            }

            var powerMax = Math.Abs((int)max).ToString().Length - 1;
            var powerMin = Math.Abs((int)min).ToString().Length - 1;

            // do not increase max for integers that are equal to 10 power
            var p = powerMax >= 3 ? powerMax - 1 : powerMax;
            if (max % Math.Pow(10, p) != 0)
                max = Math.Sign(max) * roundInt((int)Math.Abs(max), (int)Math.Pow(10, powerMax));
            // do not increase max for integers that are equal to 10 power
            p = powerMin >= 3 ? powerMin - 1 : powerMin;
            if (min % Math.Pow(10, p) != 0)
                min = Math.Sign(min) * roundInt((int)Math.Abs(min), (int)Math.Pow(10, powerMin));

            // store absolute values of max and min
            var absMax = Math.Abs(max);
            var absMin = Math.Abs(min);
            while (!isEven(absMax) && !is5Delimited(absMax))
                absMax++;
            while (!isEven(absMin) && !is5Delimited(absMin))
                absMin++;

            // TODO - both should be either even or 5 dtlimited

            max = Math.Sign(max) * absMax;
            min = Math.Sign(min) * absMin;

            // get absolute max
            var absoluteMax = Math.Max(absMax, absMin);
            double stepSize;
            double stepLength;
            double units;
            double temp;
            // store max and min difference
            var diff = splitSides ? absoluteMax : getDiff(max, min);
            var exit = false;
            if (absMax < 10 && absMax > absMin)
            {
                linesCount = (int)absMax;
                exit = true;
            }
            else if (absMin < 10 && absMin > absMax)
            {
                linesCount = (int)absMin;
                exit = true;
            }
            if (exit)
            {
                // prepare step
                stepSize = diff / linesCount;
                stepLength = radius / linesCount;
                // prepare units
                units = Math.Abs(radius / diff);

                // find zero point
                temp = min;
                while (temp < 0)
                {
                    temp += stepSize;
                    zeroPoint.Point.Y -= stepSize * units;
                    zeroPoint.Level++;
                }

                if (fractionPower > 0)
                {
                    var m = Math.Pow(10, fractionPower);
                    min /= m;
                    max /= m;
                    units *= m;
                    stepSize /= m;
                }

                return (max, min, linesCount, stepSize, stepLength, units, zeroPoint);
            }
            if (splitSides)
            {
                tempLines = Utils.StyleRadar(chartStyle)
                   ? getLineNumbersForComplexRadar(Math.Max(powerMax, powerMin), absoluteMax, radius, fontHeight, diff, min)
                   : getLineNumbersForCoplex(Math.Max(powerMax, powerMin), absoluteMax, radius, fontHeight, diff, min);
                if (tempLines != 0)
                {
                    // change lines count to selected one
                    linesCount = tempLines;
                    //if(splitSides && )
                    // prepare step
                    stepSize = diff / linesCount;
                    stepLength = radius / linesCount;
                    // prepare units
                    units = Math.Abs(radius / diff);

                    // find zero point
                    temp = min;
                    while (temp < 0)
                    {
                        temp += stepSize;
                        zeroPoint.Point.Y -= stepSize * units;
                        zeroPoint.Level++;
                    }

                    if (fractionPower > 0)
                    {
                        var m = Math.Pow(10, fractionPower);
                        min /= m;
                        max /= m;
                        units *= m;
                        stepSize /= m;
                    }

                    return (max, min, linesCount, stepSize, stepLength, units, zeroPoint);
                }
            }
            tempLines = Utils.StyleRadar(chartStyle)
                ? getLineNumbersForComplexRadar(Math.Max(powerMax, powerMin), Math.Abs(diff), radius, fontHeight, diff, min)
                : getLineNumbersForCoplex(Math.Max(powerMax, powerMin), Math.Abs(diff), radius, fontHeight, diff, min);
            if (tempLines > 0)
            {
                // change lines count to selected one
                linesCount = tempLines;
                //if(splitSides && )
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
                    while (!IsInteger(stepSize) || stepSize < prevMin)
                    {
                        max = Math.Sign(max) * roundInt((int)Math.Abs(max), (int)Math.Pow(10, powerMax));
                        diff = getDiff(max, min);
                        stepSize = diff / linesCount;
                    }
                    linesCount++;
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
                    while (!IsInteger(stepSize) || stepSize < prevMax)
                    {
                        min = Math.Sign(min) * roundInt((int)Math.Abs(min), (int)Math.Pow(10, powerMin));
                        diff = getDiff(max, min);
                        stepSize = diff / linesCount;
                    }
                    linesCount++;
                    max = sign * stepSize;
                    stepLength = radius / linesCount;
                    units = Math.Abs(radius / (diff + max));
                }
            }

            // find zero point
            temp = min;
            while (temp < 0)
            {
                temp += stepSize;
                zeroPoint.Point.Y -= stepSize * units;
                zeroPoint.Level++;
            }

            if (fractionPower > 0)
            {
                var m = Math.Pow(10, fractionPower);
                min /= m;
                max /= m;
                units *= m;
                stepSize /= m;
            }

            return (max, min, linesCount, stepSize, stepLength, units, zeroPoint);
        }

        private static int getLineNumbersForComplexRadar(int power, double max, double radius, double fontHeight, double diff, double min)
        {
            // get all available integer lines counts
            var lines = calculatedSteps(power, max).Where(l => IsInteger(l)).OrderBy(l => l).Distinct().ToArray();
            var sizes = lines.Select(s => radius / s).ToArray();
            // get the largest step with real size more/equal font height
            var items = sizes.Select((size, index) => new { size, index }).Where(a => a.size >= fontHeight + 4).OrderByDescending(a => a.index);
            foreach (var a in items)
            {
                var itemLines = lines[a.index];
                var sz = diff / itemLines;
                var t = min;
                while (t < 0)
                {
                    t += sz;
                }
                if (t == 0)
                {
                    return (int)itemLines;
                }
            }
            return 0;
        }

        private static int getLineNumbersForCoplex(int power, double max, double radius, double fontHeight, double diff, double min)
        {
            var lines = calculatedSteps(power, max).Where(l => IsInteger(l)).OrderBy(l => l).Distinct().ToArray();
            var sizes = lines.Select(s => radius / s).ToArray();
            var splitItem = sizes.Select((size, index) => new { size, index }).LastOrDefault(a => a.size >= fontHeight + 4);
            if (splitItem != null)
            {
                return (int)lines[splitItem.index];
            }
            return 0;
        }

        private static int roundInt(int number, int tense)
        {
            // smaller multiple
            int a = number / tense * tense;
            // larger multiple
            int b = a + tense;
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

        internal static int GetMaxValueLength(double max, double min, int fractionPower)
        {
            // round max to next integer
            max = Math.Ceiling(max);
            // round min to prevous integer
            min = Math.Floor(min);

            if (fractionPower > 0)
            {
                max *= Math.Pow(10, fractionPower);
                min = -Math.Abs(min * Math.Pow(10, fractionPower));
            }

            var powerMax = Math.Abs((int)max).ToString().Length - 1;
            var powerMin = Math.Abs((int)min).ToString().Length - 1;

            // do not increase max for integers that are equal to 10 power
            var p = powerMax >= 3 ? powerMax - 1 : powerMax;
            if (max % Math.Pow(10, p) != 0)
                max = Math.Sign(max) * roundInt((int)Math.Abs(max), (int)Math.Pow(10, powerMax));
            // do not increase max for integers that are equal to 10 power
            p = powerMin >= 3 ? powerMin - 1 : powerMin;
            if (min % Math.Pow(10, p) != 0)
                min = Math.Sign(min) * roundInt((int)Math.Abs(min), (int)Math.Pow(10, powerMin));

            if (fractionPower > 0)
            {
                max /= Math.Pow(10, fractionPower);
                min = -Math.Abs(min / Math.Pow(10, fractionPower));
            }

            return Math.Max(max.ToString().Length, min.ToString().Length);
        }

        internal static (double max, double min, int linesCount, double stepSize, double stepLength, double units, ZeroPoint zeroPoint) GetMeasures(ChartStyle chartStyle, ISeries[] seriesArray, int linesCount, double radius, double fontHeight, Point centerPoint, bool splitSides = false)
        {
            var max = 0.0;
            var min = 0.0;
            var minIsZero = false;
            var maxIsZero = false;

            var values = seriesArray.SelectMany(s => s.Values.Select(v => v.Value.PlainValue));
            max = values.Max();
            min = values.Min();

            if (StyleStackedLines(chartStyle) || chartStyle.In(ChartStyle.StackedColumns, ChartStyle.StackedBars))
            {
                if (seriesArray.Length == 1)
                {
                    if (values.All(v => v > 0))
                    {
                        minIsZero = true;
                    }
                    else if (values.All(v => v < 0))
                    {
                        maxIsZero = true;
                    }
                }
                else
                {
                    var resultValues = seriesArray.Select(s => new { Items = s.Values.ToList(), s.Index }).ToArray();
                    var tempValues = seriesArray.Select(s => new { Items = s.Values.ToList(), s.Index }).ToArray();
                    var maxCount = resultValues.Max(rw => rw.Items.Count);
                    for (var i = 0; i < resultValues.Length; i++)
                    {
                        if (resultValues[i].Items.Count == maxCount)
                            continue;
                        var diff = maxCount - resultValues[i].Items.Count;
                        for (var j = 0; j < diff; j++)
                        {
                            resultValues[i].Items.Add(new PlainChartValue(0));
                            tempValues[i].Items.Add(new PlainChartValue(0));
                        }
                    }
                    for (var index = 1; index < resultValues.Length; index++)
                    {
                        var temps = tempValues.Where(rw => rw.Index < index);
                        for (var i = 0; i < resultValues[index].Items.Count; i++)
                        {
                            var v = (decimal)resultValues[index].Items[i].Value.PlainValue + temps.Sum(t => (decimal)t.Items[i].Value.PlainValue);
                            resultValues[index].Items[i] = new PlainChartValue((double)v);
                        }
                    }
                    values = resultValues.SelectMany(a => a.Items).Select(it => it.Value.PlainValue);
                    max = values.Max();
                    min = values.Min();
                    if (values.All(v => v > 0))
                    {
                        minIsZero = true;
                    }
                    else if (values.All(v => v < 0))
                    {
                        maxIsZero = true;
                    }
                }
            }
            else if (chartStyle == ChartStyle.Waterfall)
            {
                (max, min) = getMaxMinForWaterfall(seriesArray[0].Values.Select(v => v.Value.PlainValue).ToArray());
                if (max > 0 && min > 0)
                {
                    minIsZero = true;
                }
                else if (max < 0 && min < 0)
                {
                    maxIsZero = true;
                }
            }
            else
            {
                if (values.All(v => v > 0))
                {
                    minIsZero = true;
                }
                else if (values.All(v => v < 0))
                {
                    maxIsZero = true;
                }
            }

            if (minIsZero)
                return getMeasuresForPositive(max, linesCount, radius, fontHeight, getMaxFractionPower(values), new ZeroPoint { Point = centerPoint });
            else if (maxIsZero)
                return getMeasuresForNegative(min, linesCount, radius, fontHeight, getMaxFractionPower(values), new ZeroPoint { Point = centerPoint });
            else
                return getMeasuresForComplex(chartStyle, max, min, linesCount, radius, fontHeight, getMaxFractionPower(values), new ZeroPoint { Point = centerPoint }, splitSides);
        }

        internal static bool IsInteger(double step)
        {
            return Math.Abs(step % 1) < double.Epsilon;
        }

        internal static (double step, int limit) Limits(ChartStyle style, bool offsetBoundary, int stopsX, int ticks,
            double boundOffset, double width)
        {
            if (StyleColumns(style))
            {
                return (width / ticks, ticks);
            }
            if (StyleBars(style))
            {
                return (width / stopsX, stopsX);
            }
            return offsetBoundary
                ? ((width - 2 * boundOffset) / (ticks - 1), ticks)
                : (width / (ticks - 1), ticks);
        }

        internal static double BoundaryOffset(bool offsetBoundary, double width, int count)
        {
            return offsetBoundary ? (width - 2 * AXIS_THICKNESS) / (count + 2) / 2 : 0;
        }

        internal static bool StyleRadar(ChartStyle style)
        {
            return style.In(ChartStyle.Radar, ChartStyle.RadarWithMarkers, ChartStyle.RadarArea);
        }

        internal static bool StyleBars(ChartStyle style)
        {
            return style.In(ChartStyle.Bars, ChartStyle.StackedBars, ChartStyle.FullStackedBars);
        }

        internal static bool StyleMeasuredBars(ChartStyle style)
        {
            return style.In(ChartStyle.Bars, ChartStyle.StackedBars);
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

        internal static bool StyleStackedLines(ChartStyle chartStyle)
        {
            return chartStyle.In(ChartStyle.StackedLines,
                ChartStyle.StackedLinesWithMarkers,
                ChartStyle.SmoothStackedLines,
                ChartStyle.SmoothStackedLinesWithMarkers,
                ChartStyle.StackedArea);
        }

        internal static bool StyleFullStacked(ChartStyle chartStyle)
        {
            return chartStyle.In(ChartStyle.FullStackedColumns, ChartStyle.FullStackedArea,
                ChartStyle.FullStackedLines, ChartStyle.FullStackedLinesWithMarkers,
                ChartStyle.SmoothFullStackedLines, ChartStyle.SmoothFullStackedLinesWithMarkers);
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

        internal static int GetMaxValueLength(List<(List<IChartValue> Values, int Index)> tuples, ChartStyle style)
        {
            if (!tuples.Any()) return 0;
            var result = "100.0".Length;
            var values = tuples.SelectMany(t => t.Values).Select(v => v.Value.PlainValue);
            switch (style)
            {
                case ChartStyle.Waterfall:
                    var maxPlus = 0.0;
                    var maxMinus = 0.0;
                    var value = 0.0;
                    foreach (var v in tuples[0].Values.Select(v => v.Value.PlainValue))
                    {
                        value += v;
                        if (value > 0)
                            maxPlus = Math.Max(maxPlus, value);
                        if (value < 0)
                            maxMinus = Math.Min(maxMinus, value);
                    }
                    result = GetMaxValueLength(maxPlus, maxMinus, getMaxFractionPower(values));// Math.Max(maxPlus, Math.Abs(maxMinus));
                    break;
                case ChartStyle.Lines:
                case ChartStyle.LinesWithMarkers:
                case ChartStyle.SmoothLines:
                case ChartStyle.SmoothLinesWithMarkers:
                case ChartStyle.Columns:
                case ChartStyle.Bars:
                case ChartStyle.Area:
                case ChartStyle.SmoothArea:
                case ChartStyle.Bubbles:
                    result = GetMaxValueLength(values.Max(), values.Min(), getMaxFractionPower(values));
                    break;
                case ChartStyle.StackedColumns:
                case ChartStyle.StackedBars:
                    {
                        var plusArr = new double[tuples.Max(s => s.Values.Count)];
                        var minusArr = new double[tuples.Max(s => s.Values.Count)];
                        foreach (var s in tuples)
                        {
                            for (var i = 0; i < s.Values.Count; i++)
                            {
                                if (s.Values[i].Value.PlainValue < 0)
                                    minusArr[i] += s.Values[i].Value.PlainValue;
                                else
                                    plusArr[i] += s.Values[i].Value.PlainValue;
                            }
                        }
                        result = GetMaxValueLength(plusArr.Max(), minusArr.Min(), getMaxFractionPower(values));
                        break;
                    }
                case ChartStyle.StackedArea:
                case ChartStyle.StackedLines:
                case ChartStyle.StackedLinesWithMarkers:
                case ChartStyle.SmoothStackedLines:
                case ChartStyle.SmoothStackedLinesWithMarkers:
                    {
                        var arr1 = new double[tuples.Max(s => s.Values.Count)];
                        var arr2 = new double[tuples.Max(s => s.Values.Count)];
                        foreach (var s in tuples)
                        {
                            for (var i = 0; i < s.Values.Count; i++)
                            {
                                arr1[i] += s.Values[i].Value.PlainValue;
                                if (Math.Abs(arr2[i]) < Math.Abs(s.Values[i].Value.PlainValue))
                                    arr2[i] = Math.Abs(s.Values[i].Value.PlainValue);
                            }
                        }
                        result = GetMaxValueLength(arr2.Max(), arr1.Min(), getMaxFractionPower(values));
                        break;
                    }
            }
            return result;
        }

        internal static Directions GetDirection(IEnumerable<double> totalValues, ChartStyle style)
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
            else if (style == ChartStyle.Funnel)
            {
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
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[3] is ChartStyle style))
                return null;

            var gm = new PathGeometry();
            var totalValues = seriesEnumerable.SelectMany(s => s.Values.Select(v => v.Value.PlainValue)).ToArray();
            var dir = Utils.GetDirection(totalValues, style);

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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Prepares geometry for drawing x- and y- axes lines
    /// </summary>
    public class AxesYLinesConverter : IMultiValueConverter
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
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[3] is ChartStyle style))
                return null;

            var gm = new PathGeometry();
            var totalValues = seriesEnumerable.SelectMany(s => s.Values.Select(v => v.Value.PlainValue)).ToArray();
            var dir = Utils.GetDirection(totalValues, style);

            switch (dir)
            {
                case Directions.NorthEast:
                    gm.Figures.Add(new PathFigure(new Point(Utils.AXIS_THICKNESS, Utils.AXIS_THICKNESS), new[] { new LineSegment(new Point(Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), true) }, false));
                    break;
                case Directions.NorthEastNorthWest:
                    gm.Figures.Add(new PathFigure(new Point(width / 2, Utils.AXIS_THICKNESS), new[] { new LineSegment(new Point(width / 2, height - Utils.AXIS_THICKNESS), true) }, false));
                    break;
                case Directions.NorthEastSouthEast:
                    gm.Figures.Add(new PathFigure(new Point(Utils.AXIS_THICKNESS, Utils.AXIS_THICKNESS), new[] { new LineSegment(new Point(Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), true) }, false));
                    break;
                case Directions.SouthEast:
                    gm.Figures.Add(new PathFigure(new Point(Utils.AXIS_THICKNESS, Utils.AXIS_THICKNESS), new[] { new LineSegment(new Point(Utils.AXIS_THICKNESS, height - Utils.AXIS_THICKNESS), true) }, false));
                    break;
                case Directions.NorthWest:
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
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[3] is ChartStyle style))
                return null;

            var gm = new PathGeometry();
            var totalValues = seriesEnumerable.SelectMany(s => s.Values.Select(v => v.Value.PlainValue)).ToArray();
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
                return Visibility.Collapsed;

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
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is IEnumerable<ISeries> seriesEnumerable)
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
                || !(values[14] is int linesCountY)
                || !(values[15] is int linesCountX)
                || !(parameter is bool isPositive)
                || !(values[16] is bool showValues)
                || !(values[17] is FlowDirection flowDirection))
                return null;

            var series = seriesEnumerable.ToArray();

            var totalValues = series[0].Values.Select(v => v.Value.PlainValue).ToArray();

            var tuples = new List<(List<IChartValue> Values, int Index)> { (series[0].Values.ToList(), 0) };
            var dir = Utils.GetDirection(totalValues, ChartStyle.Waterfall);

            var maxPointsCount = series[0].Values.Count;
            var number = maxPointsCount.ToString(culture);
            var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);

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

            var (max, min, realLinesCount, stepSize, stepLength, units, zeroPoint) = autoAdjust
                ? Utils.GetMeasures(
                   chartStyle,
                   series,
                   linesCountY,
                   radius,
                   fmt.Height,
                   centerPoint,
                   dir == Directions.NorthEastSouthEast)
                : (maxYConv, -maxYConv, linesCountY, maxYConv / linesCountY, radius / linesCountY, radius / maxYConv, default);

            return drawWaterfall(width, height, dir, series[0], units, isPositive, showValues, fontFamily, fontStyle, fontWeight, fontStretch, fontSize, culture, flowDirection);
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


        private CombinedGeometry drawWaterfall(double width, double height, Directions dir, ISeries currentSeries, double stepLength, bool isPositive, bool showValues,
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
                var value = currentSeries.Values[i].Value.PlainValue;
                var x = i * segSize + segSize / 16;
                Rect rect;
                if (value >= 0)
                {
                    rect = new Rect(new Point(x, y), new Point(x + columnWidth, y - currentSeries.Values[i].Value.PlainValue * stepLength));
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
                    rect = new Rect(new Point(x, y), new Point(x + columnWidth, y + Math.Abs(currentSeries.Values[i].Value.PlainValue) * stepLength));
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

                // add values
                if (currentSeries.Values.Count <= i) continue;
                if (!showValues) continue;
                var number = !string.IsNullOrEmpty(currentSeries.Values[i].CustomValue) ? currentSeries.Values[i].CustomValue : currentSeries.Values[i].Value.PlainValue.ToString(culture);
                var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Transparent, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
                if (fmt.Width > rect.Width || fmt.Height > rect.Height) continue;
                var pt = new Point(x + (rect.Width - fmt.Width) / 2, rect.Top + (rect.Height - fmt.Height) / 2);
                var ngm = fmt.BuildGeometry(pt);
                if (flowDirection == FlowDirection.RightToLeft)
                    ngm.Transform = new ScaleTransform { ScaleX = -1, CenterX = pt.X + fmt.Width / 2, CenterY = pt.Y + fmt.Height / 2 };
                gmValues.AddGeometry(ngm);

            }

            return cgm;
        }
    }

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
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is IEnumerable<ISeries> seriesEnumerable)
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
                || !(values[15] is int linesCountY)
                || !(values[16] is int linesCountX)
                || !(values[17] is bool showValues)
                || !(values[18] is FlowDirection flowDirection))
                return null;

            var series = seriesEnumerable.ToArray();
            var offsetBoundary = Utils.OffsetBoundary(chartBoundary, chartStyle);

            if (!series.Any()) return null;

            var gm = new PathGeometry();
            int maxX;

            var totalValues = series.SelectMany(s => s.Values.Select(v => v.Value.PlainValue)).ToArray();
            var rawValues = Utils.GetPaddedSeries(series);

            var dir = Utils.GetDirection(totalValues, chartStyle);

            var customValues = values[14] is IEnumerable<string> customEnumerable ? customEnumerable.ToArray() : new string[] { };
            var maxPointsCount = series.Max(s => s.Values.Count);
            var maxCv = customValues.Any() ? customValues.Max(v => (v, v.Length)) : (v: "", Length: 0);
            var number = maxCv.Length > maxPointsCount.ToString(culture).Length ? maxCv.v : maxPointsCount.ToString(culture);
            var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);

            Point centerPoint;
            var boundOffset = Utils.BoundaryOffset(offsetBoundary, width, maxPointsCount);
            switch (chartStyle)
            {
                case ChartStyle.Funnel:
                    if (index > 0) return null;
                    return drawFunnel(series[0], width, height, showValues, fontFamily, fontStyle, fontWeight, fontStretch, fontSize, culture, flowDirection);
                //break;
                case ChartStyle.Radar:
                case ChartStyle.RadarWithMarkers:
                case ChartStyle.RadarArea:
                    {
                        var radius = width > height ? (height - 2 * Utils.AXIS_THICKNESS) / 2 : (width - 2 * Utils.AXIS_THICKNESS) / 2;
                        centerPoint = new Point(radius, radius);
                        if (width > height)
                            radius -= (2 * fmt.Height + 8);
                        else
                            radius -= (2 * fmt.Width + 8);
                        var (max, min, realLinesCount, stepSize, stepLength, units, zeroPoint) = Utils.GetMeasures(chartStyle, series, linesCountY, radius, fmt.Height, centerPoint);
                        gm = drawRadar(series, index, chartStyle, units, stepLength, radius, maxPointsCount, realLinesCount, zeroPoint.Level, centerPoint);
                        break;
                    }
                case ChartStyle.Lines:
                case ChartStyle.LinesWithMarkers:
                case ChartStyle.SmoothLines:
                case ChartStyle.SmoothLinesWithMarkers:
                case ChartStyle.Area:
                case ChartStyle.SmoothArea:
                    {
                        var units = getUnitsForLines(series, chartStyle, dir, width, height, boundOffset, linesCountY, fmt.Height, autoAdjust, maxYConv);
                        maxX = series.Max(s => s.Values.Count);
                        gm = drawLine(width, height, maxX, units, chartStyle, dir, series.FirstOrDefault(s => s.Index == index), offsetBoundary);
                        break;
                    }
                case ChartStyle.Bubbles:
                    {
                        var units = getUnitsForLines(series, chartStyle, dir, width, height, boundOffset, linesCountY, fmt.Height, autoAdjust, maxYConv);
                        maxX = series.Max(s => s.Values.Count);
                        gm = drawBubbles(width, height, maxX, units, dir, series.FirstOrDefault(s => s.Index == index), offsetBoundary);
                        break;
                    }
                case ChartStyle.StackedLines:
                case ChartStyle.StackedLinesWithMarkers:
                case ChartStyle.SmoothStackedLines:
                case ChartStyle.SmoothStackedLinesWithMarkers:
                case ChartStyle.StackedArea:
                    {
                        var units = getUnitsForLines(series, chartStyle, dir, width, height, boundOffset, linesCountY, fmt.Height, autoAdjust, maxYConv);
                        maxX = series.Max(s => s.Values.Count);
                        gm = drawStackedLine(width, height, maxX, units, chartStyle, dir, series, index, rawValues, offsetBoundary);
                        break;
                    }
                case ChartStyle.FullStackedLines:
                case ChartStyle.FullStackedLinesWithMarkers:
                case ChartStyle.SmoothFullStackedLines:
                case ChartStyle.SmoothFullStackedLinesWithMarkers:
                    maxX = series.Max(s => s.Values.Count);
                    gm = drawFullStackedLine(width, height, maxX, chartStyle, dir, series, index, rawValues, offsetBoundary);
                    break;
                case ChartStyle.Columns:
                    {
                        var units = getUnitsForLines(series, chartStyle, dir, width, height, boundOffset, linesCountY, fmt.Height, autoAdjust, maxYConv);
                        return drawColumns(width, height, units, dir, series, index, showValues, fontFamily, fontStyle, fontWeight, fontStretch, fontSize, culture, flowDirection);
                        //break;
                    }
                case ChartStyle.StackedColumns:
                    {
                        var units = getUnitsForLines(series, chartStyle, dir, width, height, boundOffset, linesCountY, fmt.Height, autoAdjust, maxYConv);
                        return drawStackedColumns(width, height, units, dir, series, index, rawValues, showValues, fontFamily, fontStyle, fontWeight, fontStretch, fontSize, culture, flowDirection);
                        //break;
                    }
                case ChartStyle.FullStackedColumns:
                    return drawFullStackedColumns(width, height, dir, series, index, rawValues, showValues, fontFamily, fontStyle, fontWeight, fontStretch, fontSize, culture, flowDirection);
                //break;
                case ChartStyle.Bars:
                    {
                        var units = getUnitsForBars(series, chartStyle, dir, width, height, boundOffset, linesCountX, fmt.Height, autoAdjust, maxXConv);
                        return drawBars(width, height, units, dir, series, index, showValues, fontFamily, fontStyle, fontWeight, fontStretch, fontSize, culture, flowDirection);
                        //break;
                    }
                case ChartStyle.StackedBars:
                    {
                        var units = getUnitsForBars(series, chartStyle, dir, width, height, boundOffset, linesCountX, fmt.Height, autoAdjust, maxXConv);
                        return drawStackedBars(width, height, units, dir, series, index, rawValues, showValues, fontFamily, fontStyle, fontWeight, fontStretch, fontSize, culture, flowDirection);
                        //break;
                    }
                case ChartStyle.FullStackedBars:
                    return drawFullStackedBars(width, height, dir, series, index, rawValues, showValues, fontFamily, fontStyle, fontWeight, fontStretch, fontSize, culture, flowDirection);
                //break;
                case ChartStyle.FullStackedArea:
                    maxX = series.Max(s => s.Values.Count);
                    gm = drawFullStackedArea(width, height, maxX, dir, series, index, rawValues);
                    break;
            }

            return gm;
        }

        private CombinedGeometry drawFunnel(ISeries currentSeries, double width, double heigth, bool showValues,
           FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, double fontSize,
           CultureInfo culture, FlowDirection flowDirection)
        {
            const double FUNNEL_OFFSET = 2.0;
            var cgm = new CombinedGeometry { GeometryCombineMode = GeometryCombineMode.Exclude };
            var gm = new PathGeometry();
            var gmValues = new PathGeometry();
            var drawingWidth = width - Utils.AXIS_THICKNESS - COLUMN_BAR_OFFSET * 2;
            heigth -= Utils.AXIS_THICKNESS;

            var maxWidth = currentSeries.Values.Where(v => v.Value.PlainValue > 0).Max(v => v.Value.PlainValue);
            var units = maxWidth != 0 ? drawingWidth / maxWidth : 0;

            var barHeight = heigth / currentSeries.Values.Count - FUNNEL_OFFSET * 2;
            currentSeries.RealRects.Clear();
            for (int i = 0, j = 1; i < currentSeries.Values.Count; i++, j += 2)
            {
                if (currentSeries.Values[i].Value.PlainValue <= 0)
                    continue;
                var rectWidth = currentSeries.Values[i].Value.PlainValue * units;
                var x = COLUMN_BAR_OFFSET + (width - rectWidth) / 2;
                var y = j * FUNNEL_OFFSET + i * barHeight;
                var rect = new Rect(x, y, rectWidth, barHeight);
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
                // add values
                if (!showValues) continue;
                var number = !string.IsNullOrEmpty(currentSeries.Values[i].CustomValue) ? currentSeries.Values[i].CustomValue : currentSeries.Values[i].Value.PlainValue.ToString(culture);
                var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Transparent, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
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

        private double getUnitsForBars(ISeries[] series, ChartStyle chartStyle, Directions dir, double width, double height, double boundOffset, int linesCountX, double formatHeight, bool autoAdjust, double maxX)
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
            if (!autoAdjust)
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

        private double getUnitsForLines(ISeries[] series, ChartStyle chartStyle, Directions dir, double width, double height, double boundOffset, int linesCountY, double formatHeight, bool autoAdjust, double maxY)
        {
            var centerX = 0.0;
            var radius = 0.0;

            switch (dir)
            {
                case Directions.NorthEast:
                case Directions.NorthWest:
                case Directions.SouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    radius = height - Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = width / 2;
                    radius = height - Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    radius = (height - Utils.AXIS_THICKNESS) / 2;
                    break;
            }
            if (!autoAdjust)
                return radius / maxY;
            var centerPoint = new Point(centerX, radius);
            var (_, _, _, _, _, units, _) = Utils.GetMeasures(
               chartStyle,
               series,
               linesCountY,
               radius,
               formatHeight,
               centerPoint,
               dir == Directions.NorthEastSouthEast);
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
                var rect = new Rect(new Point(startX, y), new Point(startX + currentSeries.Values[i].Value.PlainValue * units, y - barHeight));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
                // add values
                if (!showValues) continue;
                var number = !string.IsNullOrEmpty(currentSeries.Values[i].CustomValue) ? currentSeries.Values[i].CustomValue : currentSeries.Values[i].Value.PlainValue.ToString(culture);
                var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Transparent, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
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
                        tuples.Where(t => t.Index < index && Math.Sign(t.Values[i1].Value.PlainValue) == Math.Sign(values[i1].Value.PlainValue));
                    x += prevsTuples.Sum(sr => sr.Values[i].Value.PlainValue * units);
                }
                var y = height - (i * segSize + COLUMN_BAR_OFFSET);
                var rect = new Rect(new Point(x, y), new Point(x + values[i].Value.PlainValue * units, y - barHeight));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
                // add values
                if (currentSeries.Values.Count <= i) continue;
                if (!showValues) continue;
                var number = !string.IsNullOrEmpty(currentSeries.Values[i].CustomValue) ? currentSeries.Values[i].CustomValue : currentSeries.Values[i].Value.PlainValue.ToString(culture);
                var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Transparent, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
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
                var sign = Math.Sign(values[i].Value.PlainValue);
                var y = height - (i * segSize + COLUMN_BAR_OFFSET);
                var i1 = i;
                var sumTotal = tuples.Sum(sr => Math.Abs(sr.Values[i1].Value.PlainValue));
                var percent = Math.Abs(values[i].Value.PlainValue) / sumTotal * 100;
                var segWidth = sign * stepX / 100 * percent;

                var prevs = tuples.Where(s => s.Index < index)
                    .Where(s => Math.Sign(s.Values[i1].Value.PlainValue) == sign);
                var prevSum = prevs.Sum(pvs => Math.Abs(pvs.Values[i1].Value.PlainValue));
                var prevPerc = prevSum / sumTotal * 100;
                var prevWidth = stepX / 100 * prevPerc;
                var x = startX + sign * prevWidth;
                var rect = new Rect(new Point(x, y), new Point(x + segWidth, y - barHeight));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
                // add values
                if (currentSeries.Values.Count <= i) continue;
                if (!showValues) continue;
                var number = !string.IsNullOrEmpty(currentSeries.Values[i].CustomValue) ? currentSeries.Values[i].CustomValue : currentSeries.Values[i].Value.PlainValue.ToString(culture);
                var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Transparent, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
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
                var sign = Math.Sign(values[i].Value.PlainValue);
                var x = i * segSize + COLUMN_BAR_OFFSET;
                var i1 = i;
                var sumTotal = tuples.Sum(sr => Math.Abs(sr.Values[i1].Value.PlainValue));
                var percent = Math.Abs(values[i].Value.PlainValue) / sumTotal * 100;
                var segHeight = sign * stepY / 100 * percent;
                var prevs = tuples.Where(s => s.Index < index)
                    .Where(s => Math.Sign(s.Values[i1].Value.PlainValue) == sign);
                var prevSum = prevs.Sum(pvs => Math.Abs(pvs.Values[i1].Value.PlainValue));
                var prevPerc = prevSum / sumTotal * 100;
                var prevHeight = stepY / 100 * prevPerc;
                var y = startY - sign * prevHeight;
                var rect = new Rect(new Point(x, y), new Point(x + columnWidth, y - segHeight));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
                // add values
                if (currentSeries.Values.Count <= i) continue;
                if (!showValues) continue;
                var number = !string.IsNullOrEmpty(currentSeries.Values[i].CustomValue) ? currentSeries.Values[i].CustomValue : currentSeries.Values[i].Value.PlainValue.ToString(culture);
                var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Transparent, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
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
                            s => s.Index < index && Math.Sign(s.Values[i1].Value.PlainValue) == Math.Sign(values[i1].Value.PlainValue));
                    y -= prevs.Sum(sr => sr.Values[i].Value.PlainValue * units);
                }
                var rect = new Rect(new Point(x, y), new Point(x + columnWidth, y - values[i].Value.PlainValue * units));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
                // add values
                if (currentSeries.Values.Count <= i) continue;
                if (!showValues) continue;
                var number = !string.IsNullOrEmpty(currentSeries.Values[i].CustomValue) ? currentSeries.Values[i].CustomValue : currentSeries.Values[i].Value.PlainValue.ToString(culture);
                var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Transparent, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
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
            var segSize = width / series.Max(s => s.Values.Count);
            var columnWidth = (segSize - COLUMN_BAR_OFFSET * 2) / series.Length;

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
            for (var i = 0; i < currentSeries.Values.Count; i++)
            {
                var x = i * segSize + COLUMN_BAR_OFFSET + index * columnWidth;
                var rect = new Rect(new Point(x, startY), new Point(x + columnWidth, startY - currentSeries.Values[i].Value.PlainValue * units));
                var rg = new RectangleGeometry(rect);
                currentSeries.RealRects.Add(rect);
                gm.AddGeometry(rg);
                // add values
                if (!showValues) continue;
                var number = !string.IsNullOrEmpty(currentSeries.Values[i].CustomValue) ? currentSeries.Values[i].CustomValue : currentSeries.Values[i].Value.PlainValue.ToString(culture);
                var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Transparent, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
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
            ISeries[] series, int index, List<(List<IChartValue> Values, int Index)> tuples)
        {
            var tp = tuples.FirstOrDefault(t => t.Index == index);
            if (tp == default) return null;
            var values = tp.Values;
            double stepX, stepY;
            double centerY;
            var gm = new PathGeometry();

            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (height - 2 * Utils.AXIS_THICKNESS);
                    break;
                case Directions.NorthEastNorthWest:
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / 2 / (maxX - 1);
                    stepY = (height - 2 * Utils.AXIS_THICKNESS);
                    break;
                case Directions.NorthEastSouthEast:
                    centerY = height / 2;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (height - 2 * Utils.AXIS_THICKNESS) / 2;
                    break;
                case Directions.SouthEast:
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = height - 2 * Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthWest:
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
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
                var sum = tuples.Sum(s => Math.Abs(s.Values[i].Value.PlainValue));
                var sign = Math.Sign(values[i].Value.PlainValue);
                var perc = Math.Abs(values[i].Value.PlainValue) / sum * 100;
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

        private PathGeometry drawFullStackedLine(double width, double height, double maxX, ChartStyle style,
            Directions dir, ISeries[] series, int index, List<(List<IChartValue> Values, int Index)> tuples, bool offsetBoundary)
        {
            var tp = tuples.FirstOrDefault(t => t.Index == index);
            if (tp == default) return null;
            var values = tp.Values;
            double stepX, stepY;
            double centerY;
            var gm = new PathGeometry();

            var boundOffset = Utils.BoundaryOffset(offsetBoundary, width, values.Count);

            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            switch (dir)
            {
                case Directions.NorthEast:
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = height - 2 * Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastNorthWest:
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / 2 / (maxX - 1);
                    stepY = height - 2 * Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthEastSouthEast:
                    centerY = height / 2;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = (height - 2 * Utils.AXIS_THICKNESS) / 2;
                    break;
                case Directions.SouthEast:
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    stepY = height - 2 * Utils.AXIS_THICKNESS;
                    break;
                case Directions.NorthWest:
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
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
                var sum = tuples.Sum(s => Math.Abs(s.Values[i].Value.PlainValue));
                var sign = Math.Sign(values[i].Value.PlainValue);
                var perc = Math.Abs(values[i].Value.PlainValue) / sum * 100;

                y -= sign * perc * stepY / 100;
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
            else if (style.In(ChartStyle.FullStackedLines, ChartStyle.FullStackedLinesWithMarkers))
            {
                var poly = new PolyLineSegment(currentSeries.RealPoints, true);
                gm.Figures.Add(new PathFigure(start, new[] { poly }, false));
            }
            return gm;
        }

        private PathGeometry drawStackedLine(double width, double height, double maxX, double units, ChartStyle style,
            Directions dir, ISeries[] series, int index, List<(List<IChartValue> Values, int Index)> tuples, bool offsetBoundary)
        {
            var tp = tuples.FirstOrDefault(t => t.Index == index);
            if (tp == default) return null;
            var values = tp.Values;
            double stepX;
            double centerX, centerY;
            var gm = new PathGeometry();
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;

            var boundOffset = Utils.BoundaryOffset(offsetBoundary, width, values.Count);

            switch (dir)
            {
                case Directions.NorthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = width / 2;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / 2 / (maxX - 1);
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = height / 2;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    break;
                case Directions.SouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    break;
                case Directions.NorthWest:
                    centerX = width - Utils.AXIS_THICKNESS;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (width - Utils.AXIS_THICKNESS) / (maxX - 1);
                    break;
                default:
                    return null;
            }
            currentSeries.RealRects.Clear();

            var points = new List<Point>();

            for (var i = 0; i < values.Count; i++)
            {
                var x = centerX + i * stepX;
                var y = centerY - values[i].Value.PlainValue * units;
                if (index > 0)
                {
                    var prevs = tuples.Where(s => s.Index < index);
                    var sum = prevs.Sum(sr => sr.Values[i].Value.PlainValue);
                    y -= sum * units;
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
            else if (style.In(ChartStyle.StackedLines, ChartStyle.StackedLinesWithMarkers))
            {
                var poly = new PolyLineSegment(points, true);
                gm.Figures.Add(new PathFigure(points[0], new[] { poly }, false));
            }
            else if (style.In(ChartStyle.StackedArea))
            {
                points.Insert(0, new Point(centerX, centerY));
                points.Add(new Point(points[points.Count - 1].X, centerY));
                var poly = new PolyLineSegment(points, true);
                gm.Figures.Add(new PathFigure(points[0], new[] { poly }, true));
            }
            return gm;
        }

        private PathGeometry drawRadar(ISeries[] series, int index, ChartStyle chartStyle, double units, double stepLength, double radius, int pointsCount, int linesCount, int zeroLevel, Point centerPoint)
        {
            //var values = tp.Values.Select(v => v.Value.PlainValue).ToArray();
            var currentSeries = series.FirstOrDefault(s => s.Index == index);
            if (currentSeries == null) return null;
            var maxLength = series.Max(s => s.Values.Count);
            var values = currentSeries.Values.Select(v => v.Value.PlainValue).ToArray();

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
                    drawMarker(xBeg, yBeg, gm, currentSeries.RealRects);
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

        private PathGeometry drawLine(double width, double height, int maxX, double units, ChartStyle style, Directions dir, ISeries currentSeries, bool offsetBoundary)
        {
            double stepX;
            double centerX, centerY;
            var gm = new PathGeometry();

            var boundOffset = Utils.BoundaryOffset(offsetBoundary, width, maxX);

            switch (dir)
            {
                case Directions.NorthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = width / 2;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / 2 / (maxX - 1);
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = height / 2;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    break;
                case Directions.SouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    break;
                case Directions.NorthWest:
                    centerX = width - Utils.AXIS_THICKNESS;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    break;
                default:
                    return null;
            }
            currentSeries.RealRects.Clear();
            var points = new List<Point>();

            for (var i = 0; i < currentSeries.Values.Count; i++)
            {
                var x = centerX + i * stepX;
                var y = centerY - currentSeries.Values[i].Value.PlainValue * units;

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
            else if (style == ChartStyle.SmoothArea)
            {
                points.Insert(0, new Point(centerX, centerY));
                points.Add(new Point(points[points.Count - 1].X, centerY));
                var bezierSegments = interpolatePointsWithBezierCurves(points, false);
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

        private PathGeometry drawBubbles(double width, double height, int maxX, double units, Directions dir, ISeries currentSeries, bool offsetBoundary)
        {
            double stepX;
            double centerX, centerY;
            var segSize = height > width ? (width - 2 * Utils.AXIS_THICKNESS) / (maxX + 2) / 2 : (height - 2 * Utils.AXIS_THICKNESS) / (maxX + 2) / 2;

            var boundOffset = Utils.BoundaryOffset(offsetBoundary, width, maxX);
            if (boundOffset > 0)
                segSize = boundOffset;

            var gm = new PathGeometry();

            switch (dir)
            {
                case Directions.NorthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = width / 2;
                    centerY = height - Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / 2 / (maxX - 1);
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = height / 2;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    break;
                case Directions.SouthEast:
                    centerX = Utils.AXIS_THICKNESS + boundOffset;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = offsetBoundary ? (width - 2 * Utils.AXIS_THICKNESS - 2 * boundOffset) / (maxX - 1) : (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    break;
                case Directions.NorthWest:
                    centerX = width - Utils.AXIS_THICKNESS;
                    centerY = Utils.AXIS_THICKNESS;
                    stepX = (width - 2 * Utils.AXIS_THICKNESS) / (maxX - 1);
                    break;
                default:
                    return null;
            }
            currentSeries.RealRects.Clear();

            for (var i = 0; i < currentSeries.Values.Count; i++)
            {
                var x = centerX + i * stepX;
                var y = centerY - currentSeries.Values[i].Value.PlainValue * units;

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
            if (!(value is LegendAlignment legendAlignment)) return Orientation.Vertical;
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
                || !(values[0] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle))
                return HorizontalAlignment.Right;

            var totalValues = seriesEnumerable.SelectMany(s => s.Values.Select(v => v.Value.PlainValue)).ToArray();
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
                    return HorizontalAlignment.Right;
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
                || !(values[0] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle))
                return 2;

            var totalValues = seriesEnumerable.SelectMany(s => s.Values.Select(v => v.Value.PlainValue)).ToArray();
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
                || !(values[0] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle))
                return VerticalAlignment.Top;

            var totalValues = seriesEnumerable.SelectMany(s => s.Values.Select(v => v.Value.PlainValue)).ToArray();
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
                || !(values[0] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle))
                return 5;

            var totalValues = seriesEnumerable.SelectMany(s => s.Values.Select(v => v.Value.PlainValue).ToArray());
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
                || !(values[0] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle))
                return 2;

            var totalValues = seriesEnumerable.SelectMany(s => s.Values.Select(v => v.Value.PlainValue).ToArray());
            var dir = Utils.GetDirection(totalValues, chartStyle);

            return dir switch
            {
                Directions.NorthEast => 2,
                Directions.NorthEastNorthWest => 2,
                Directions.NorthEastSouthEast => 2,
                Directions.SouthEast => 6,
                Directions.NorthWest => 2,
                _ => 2,
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
                || !(values[0] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle))
                return 5;

            var totalValues = seriesEnumerable.SelectMany(s => s.Values.Select(v => v.Value.PlainValue).ToArray());
            var dir = Utils.GetDirection(totalValues, chartStyle);

            return dir switch
            {
                Directions.NorthEast => 5,
                Directions.NorthEastNorthWest => 5,
                Directions.NorthEastSouthEast => 5,
                Directions.SouthEast => 5,
                Directions.NorthWest => 1,
                _ => 5,
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
                || !(values[0] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle))
                return VerticalAlignment.Bottom;

            var totalValues = seriesEnumerable.SelectMany(s => s.Values.Select(v => v.Value.PlainValue).ToArray());
            var dir = Utils.GetDirection(totalValues, chartStyle);

            return dir switch
            {
                Directions.NorthEast => VerticalAlignment.Bottom,
                Directions.NorthEastNorthWest => VerticalAlignment.Bottom,
                Directions.NorthEastSouthEast => VerticalAlignment.Center,
                Directions.SouthEast => VerticalAlignment.Top,
                Directions.NorthWest => VerticalAlignment.Bottom,
                _ => VerticalAlignment.Bottom,
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
                || !(values[0] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle))
                return HorizontalAlignment.Left;

            var totalValues = seriesEnumerable.SelectMany(s => s.Values.Select(v => v.Value.PlainValue).ToArray());
            var dir = Utils.GetDirection(totalValues, chartStyle);

            return dir switch
            {
                Directions.NorthEast => HorizontalAlignment.Left,
                Directions.NorthEastNorthWest => HorizontalAlignment.Center,
                Directions.NorthEastSouthEast => HorizontalAlignment.Left,
                Directions.SouthEast => HorizontalAlignment.Left,
                Directions.NorthWest => HorizontalAlignment.Right,
                _ => HorizontalAlignment.Left,
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
                case ChartStyle.SmoothArea:
                case ChartStyle.Funnel:
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
                || !(values[0] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle)
                || chartStyle == ChartStyle.Funnel
                || !(values[2] is string format)
                || !(values[3] is FontFamily fontFamily)
                || !(values[4] is double fontSize)
                || !(values[5] is FontStyle fontStyle)
                || !(values[6] is FontWeight fontWeight)
                || !(values[7] is FontStretch fontStretch)
                || !(values[9] is bool autoAdjust)
                || !(values[10] is double maxX))
                return 0.0;

            var height = 8.0;
            var series = seriesEnumerable.ToArray();
            var customValues = values[8] is IEnumerable<string> customEnumerable ? customEnumerable.ToArray() : new string[] { };

            if (!series.Any()) return height;

            var rawValues = chartStyle != ChartStyle.Waterfall
                ? Utils.GetPaddedSeries(series)
                : new List<(List<IChartValue> Values, int Index)> { (series[0].Values.ToList(), series[0].Index) };

            var totalValues = chartStyle != ChartStyle.Waterfall
                ? series.SelectMany(s => s.Values.Select(v => v.Value.PlainValue))
                : series[0].Values.Select(v => v.Value.PlainValue);

            var dir = Utils.GetDirection(totalValues, chartStyle);
            var maxFromValues = autoAdjust ? Utils.GetMaxValueLength(rawValues, chartStyle) : maxX.ToString(culture).Length;
            var maxFromCustom = customValues.Any() ? customValues.Max(v => v.Length) : 0;
            var maxString = new string('W', Math.Max(maxFromValues, maxFromCustom));

            var fmt = new FormattedText(maxString, culture, FlowDirection.LeftToRight,
                            new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip)
            {
                TextAlignment = TextAlignment.Right
            };
            height = fmt.Width;
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
                || !(values[0] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[1] is ChartStyle chartStyle)
                || !(values[2] is string format)
                || !(values[3] is FontFamily fontFamily)
                || !(values[4] is double fontSize)
                || !(values[5] is FontStyle fontStyle)
                || !(values[6] is FontWeight fontWeight)
                || !(values[7] is FontStretch fontStretch)
                || !(values[9] is bool autoAdjust)
                || !(values[10] is double maxY))
                return 0.0;
            var width = 28.0;

            var series = seriesEnumerable.ToArray();
            var customValues = values[8] is IEnumerable<string> customEnumerable ? customEnumerable.ToArray() : new string[] { };

            if (!series.Any()) return width;


            if (chartStyle.In(ChartStyle.Bars, ChartStyle.StackedBars, ChartStyle.FullStackedBars, ChartStyle.Radar, ChartStyle.RadarWithMarkers, ChartStyle.RadarArea))
                return 0.0;

            var rawValues = chartStyle != ChartStyle.Waterfall
                ? Utils.GetPaddedSeries(series)
                : new List<(List<IChartValue> Values, int Index)> { (series[0].Values.ToList(), series[0].Index) };

            var totalValues = chartStyle != ChartStyle.Waterfall
                ? series.SelectMany(s => s.Values.Select(v => v.Value.PlainValue))
                : series[0].Values.Select(v => v.Value.PlainValue);

            var dir = Utils.GetDirection(totalValues, chartStyle);
            var maxFromValues = chartStyle != ChartStyle.Funnel
                ? autoAdjust
                    ? Utils.GetMaxValueLength(rawValues, chartStyle)
                    : maxY.ToString(culture).Length
                : series[0].Values.Count.ToString(culture).Length;
            var maxFromCustom = customValues.Any() ? customValues.Max(v => v.Length) : 0;
            var maxString = new string('W', Math.Max(maxFromValues, maxFromCustom));

            var fmt = new FormattedText(maxString, culture, FlowDirection.LeftToRight,
                            new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip)
            {
                TextAlignment = TextAlignment.Right
            };
            width = fmt.Width;
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
                || !(values[2] is ChartStyle chartStyle))
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
                || !(values[0] is double height)
                || !(values[1] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[5] is FontFamily fontFamily)
                || !(values[6] is double fontSize)
                || !(values[7] is FontStyle fontStyle)
                || !(values[8] is FontWeight fontWeight)
                || !(values[9] is FontStretch fontStretch)
                || !(values[14] is FlowDirection flowDir))
                return null;

            var gm = new PathGeometry();

            if (!seriesEnumerable.Any()) return null;

            var series = seriesEnumerable.ToArray();

            var ticks = series[0].Values.Count;

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

                //var trgr = new TransformGroup();
                //trgr.Children.Add(flowDir == FlowDirection.LeftToRight
                //    ? new RotateTransform(45, pt.X+fmt.Width/2, pt.Y+ fmt.Height/2)
                //    : new RotateTransform(-45, pt.X + fmt.Width, pt.Y+ fmt.Height/2));
                //if (flowDir == FlowDirection.RightToLeft)
                //    trgr.Children.Add(new ScaleTransform { ScaleX = -1 });
                //ngm.Transform = trgr;
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
                || !(values[0] is double height)
                || !(values[1] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[2] is ChartStyle chartStyle)
                || !(values[4] is string format)
                || !(values[5] is FontFamily fontFamily)
                || !(values[6] is double fontSize)
                || !(values[7] is FontStyle fontStyle)
                || !(values[8] is FontWeight fontWeight)
                || !(values[9] is FontStretch fontStretch)
                || !(values[14] is FlowDirection flowDir))
                return null;

            if (!seriesEnumerable.Any()) return null;

            var series = seriesEnumerable.ToArray();

            var customValues = values[10] is IEnumerable<string> customEnumerable ? customEnumerable.ToArray() : new string[] { };

            var gm = new PathGeometry();

            var rawValues = Utils.GetPaddedSeries(series);

            var ticks = rawValues.Max(rw => rw.Values.Count);

            var totalValues = series.SelectMany(s => s.Values.Select(v => v.Value.PlainValue));

            var dir = Utils.GetDirection(totalValues, chartStyle);

            height -= Utils.AXIS_THICKNESS;

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
                            var number = customValues.Length > i
                                ? customValues[i]
                                : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);

                            var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip)
                            {
                                TextAlignment = flowDir == FlowDirection.LeftToRight
                                    ? dir == Directions.NorthWest ? TextAlignment.Left : TextAlignment.Right
                                    : dir == Directions.NorthWest ? TextAlignment.Right : TextAlignment.Left
                            };

                            var pt = new Point(0, y - (yStep + fmt.Height) / 2);
                            var ngm = fmt.BuildGeometry(pt);

                            //var trgr = new TransformGroup();
                            //trgr.Children.Add(flowDir == FlowDirection.LeftToRight
                            //    ? new RotateTransform(45, pt.X+fmt.Width/2, pt.Y+ fmt.Height/2)
                            //    : new RotateTransform(-45, pt.X + fmt.Width, pt.Y+ fmt.Height/2));
                            //if (flowDir == FlowDirection.RightToLeft)
                            //    trgr.Children.Add(new ScaleTransform { ScaleX = -1 });
                            //ngm.Transform = trgr;
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
                || !(values[0] is double height)
                || !(values[1] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[2] is ChartStyle chartStyle)
                || !(values[3] is int linesCountY)
                || !(values[4] is string format)
                || !(values[5] is FontFamily fontFamily)
                || !(values[6] is double fontSize)
                || !(values[7] is FontStyle fontStyle)
                || !(values[8] is FontWeight fontWeight)
                || !(values[9] is FontStretch fontStretch)
                || !(values[12] is bool autoAdjust)
                || !(values[13] is double maxY)
                || !(values[14] is FlowDirection flowDir))
                return null;

            if (!seriesEnumerable.Any())
                return null;

            var gm = new PathGeometry();

            var series = seriesEnumerable.ToArray();
            var customValues = values[11] is IEnumerable<string> customEnumerable ? customEnumerable.ToArray() : new string[] { };
            var drawBetween = chartStyle.In(ChartStyle.Bars, ChartStyle.StackedBars, ChartStyle.FullStackedBars);

            double step, offset;

            var rawValues = chartStyle != ChartStyle.Waterfall
                ? Utils.GetPaddedSeries(series)
                : new List<(List<IChartValue> Values, int Index)> { (series[0].Values.ToList(), series[0].Index) };

            var maxCount = rawValues.Max(rw => rw.Values.Count);

            var totalValues = chartStyle != ChartStyle.Waterfall
                ? series.SelectMany(s => s.Values.Select(v => v.Value.PlainValue))
                : series[0].Values.Select(v => v.Value.PlainValue);

            var dir = Utils.GetDirection(totalValues, chartStyle);

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

            var (max, min, realLinesCount, stepSize, stepLength, units, _) = autoAdjust
                ? Utils.GetMeasures(
                   chartStyle,
                   series,
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
                if (format.EndsWith("%"))
                    format = $"{format.Substring(0, format.Length - 1)}{culture.NumberFormat.NumberDecimalSeparator}{new string('0', fractions)}%";
                else
                    format += $"{culture.NumberFormat.NumberDecimalSeparator}{new string('0', fractions)}";
            }

            switch (dir)
            {
                case Directions.NorthEastSouthEast:
                    step = stepLength;// height / 2 / linesCount;
                    var limit = linesCountY * 2;
                    for (int i = 0, index = 0; i <= limit; i++)
                    {
                        if (i == linesCountY) continue;
                        var num = !Utils.StyleFullStacked(chartStyle)
                            ? (linesCountY - i) * maxMin / linesCountY
                            : (linesCountY - i) * 10;
                        var number = customValues.Length > index
                            ? customValues[index++]
                            : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);
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
                        var number = customValues.Length > index
                            ? customValues[index++]
                            : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);
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
                            : 100 - stepSize * i;
                        var number = customValues.Length > index
                            ? customValues[index++]
                            : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);
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
                || !(values[0] is double width)
                || !(values[1] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[2] is ChartStyle chartStyle)
                || chartStyle == ChartStyle.Funnel
                || !(values[3] is int linesCount)
                || !(values[4] is string format)
                || !(values[5] is FontFamily fontFamily)
                || !(values[6] is double fontSize)
                || !(values[7] is FontStyle fontStyle)
                || !(values[8] is FontWeight fontWeight)
                || !(values[9] is FontStretch fontStretch)
                || !(values[11] is bool autoAdjust)
                || !(values[12] is double maxX)
                || !(values[13] is FlowDirection flowDir)
                || !(values[14] is ChartBoundary chartBoundary))
                return null;

            if (chartStyle.In(ChartStyle.Area, ChartStyle.StackedArea, ChartStyle.FullStackedArea, ChartStyle.SlicedPie, ChartStyle.SolidPie, ChartStyle.Doughnut, ChartStyle.Radar, ChartStyle.RadarWithMarkers, ChartStyle.RadarArea, ChartStyle.SmoothArea))
                return null;
            if (!seriesEnumerable.Any()) return null;

            var series = seriesEnumerable.ToArray();
            var drawBetween = Utils.StyleColumns(chartStyle);
            var customValues = values[10] is IEnumerable<string> customEnumerable ? customEnumerable.ToArray() : new string[] { };

            double xStep;

            var gm = new PathGeometry();

            var rawValues = chartStyle != ChartStyle.Waterfall
                ? Utils.GetPaddedSeries(series)
                : new List<(List<IChartValue> Values, int Index)> { (series[0].Values.ToList(), series[0].Index) };

            var ticks = rawValues.Max(rw => rw.Values.Count);

            var totalValues = chartStyle != ChartStyle.Waterfall
                ? series.SelectMany(s => s.Values.Select(v => v.Value.PlainValue))
                : series[0].Values.Select(v => v.Value.PlainValue);

            var dir = Utils.GetDirection(totalValues, chartStyle);

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

                var (max, min, realLinesCount, stepS, stepL, units, zeroPoint) = autoAdjust
                    ? Utils.GetMeasures(
                       chartStyle,
                       series,
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
                    if (format.EndsWith("%"))
                        format = $"{format.Substring(0, format.Length - 1)}{culture.NumberFormat.NumberDecimalSeparator}{new string('0', fractions)}%";
                    else
                        format += $"{culture.NumberFormat.NumberDecimalSeparator}{new string('0', fractions)}";
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
                //DONE
                case Directions.NorthEast:
                case Directions.NorthEastSouthEast:
                    {
                        var (Step, Limit) = Utils.Limits(chartStyle, offsetBoundary, linesCount, ticks, boundOffset, width);
                        if (!Utils.StyleBars(chartStyle))
                        {
                            xStep = Step;
                            limit = Limit;
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
                                ? Utils.StyleColumns(chartStyle) ? flowDir == FlowDirection.LeftToRight ? i + 1 : j + 1 : flowDir == FlowDirection.LeftToRight ? i : j
                                : Utils.StyleMeasuredBars(chartStyle)
                                    ? flowDir == FlowDirection.LeftToRight ? maxMin - stepSize * j : maxMin - stepSize * i
                                    : flowDir == FlowDirection.LeftToRight ? 10 * (i + 1) : 10 * (j + 1);
                            var index = flowDir == FlowDirection.LeftToRight ? i : j;
                            var number = customValues.Length > index && !Utils.StyleBars(chartStyle)
                                ? customValues[index]
                                : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);
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
                                    : drawBetween ? boundOffset + indexStep * xStep + xStep / 2 : boundOffset + indexStep * xStep;
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
                //DONE
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
                            var number = customValues.Length > index && !Utils.StyleBars(chartStyle)
                                ? customValues[index++]
                                : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);

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
                //DONE
                case Directions.SouthEast:
                    {
                        var (Step, Limit) = Utils.Limits(chartStyle, offsetBoundary, linesCount, ticks, boundOffset, width);
                        xStep = Step;
                        limit = Limit;
                        for (int i = 0, j = limit - 1; i < limit; i++, j--)
                        {
                            var num = Utils.StyleColumns(chartStyle) ? flowDir == FlowDirection.LeftToRight ? i + 1 : j + 1 : flowDir == FlowDirection.LeftToRight
                                ? i
                                : j;
                            var index = flowDir == FlowDirection.LeftToRight ? i : j;
                            var number = customValues.Length > index && !Utils.StyleBars(chartStyle)
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
                        xStep = Utils.StyleMeasuredBars(chartStyle) ? stepLength : width / 10;
                        for (int i = 0, j = linesCount; i < linesCount; i++, j--)
                        {
                            var indexStep = flowDir == FlowDirection.LeftToRight ? i : i + 1;
                            var index = flowDir == FlowDirection.LeftToRight ? j : i;
                            var num = Utils.StyleMeasuredBars(chartStyle)
                                    ? flowDir == FlowDirection.LeftToRight ? -j * maxMin / linesCount : -(i + 1) * maxMin / linesCount
                                    : flowDir == FlowDirection.LeftToRight ? -j * 10 : -indexStep * 10;
                            var number = customValues.Length > index && !Utils.StyleBars(chartStyle)
                                ? customValues[index]
                                : format.EndsWith("%") ? num.ToString(format.Substring(0, format.Length - 1)) + "%" : num.ToString(format);
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
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is int linesCountX)
                || !(values[3] is int linesCountY)
                || !(values[4] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[5] is ChartStyle chartStyle)
                || !(values[6] is ChartBoundary chartBoundary)
                || !(values[7] is FontFamily fontFamily)
                || !(values[8] is double fontSize)
                || !(values[9] is FontStyle fontStyle)
                || !(values[10] is FontWeight fontWeight)
                || !(values[11] is FontStretch fontStretch)
                || !(values[12] is bool autoAdjust)
                || !(values[13] is double maxX)
                || !(values[14] is double maxY)
                || !(values[15] is AxesVisibility axesVisibility))
                return null;

            if (chartStyle.In(ChartStyle.Area, ChartStyle.StackedArea, ChartStyle.FullStackedArea, ChartStyle.Radar, ChartStyle.RadarWithMarkers, ChartStyle.RadarArea, ChartStyle.SmoothArea))
                return null;
            if (!seriesEnumerable.Any())
                return null;

            const double size = 4.0;

            double xStep, yStep;

            var series = seriesEnumerable.ToArray();
            var offsetBoundary = Utils.OffsetBoundary(chartBoundary, chartStyle);
            var totalValues = series.SelectMany(s => s.Values.Select(v => v.Value.PlainValue).ToArray());
            var dir = Utils.GetDirection(totalValues, chartStyle);
            var ticks = series.Select(s => s.Values.Count).Max();

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
                ? autoAdjust
                    ? Utils.GetMeasures(
                       chartStyle,
                       series,
                       Utils.StyleBars(chartStyle) ? linesCountX : linesCountY,
                       radius,
                       fmtY.Height,
                       centerPoint,
                       !Utils.StyleBars(chartStyle) ? dir == Directions.NorthEastSouthEast : dir == Directions.NorthEastNorthWest)
                    : (maxY, -maxY, linesCountY, maxY / linesCountY, radius / linesCountY, radius / maxY, default)
                : (series[0].Values.Count, -maxY, series[0].Values.Count, maxY / series[0].Values.Count, radius / series[0].Values.Count, radius / series[0].Values.Count, default);

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
                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Horizontal) && chartStyle != ChartStyle.Funnel)
                        {
                            var x = Utils.AXIS_THICKNESS + boundOffset;
                            if (!Utils.StyleBars(chartStyle))
                            {
                                var (Step, Limit) = Utils.Limits(chartStyle, offsetBoundary, linesCountX, ticks, boundOffset, width);
                                xStep = Step;
                                limit = Limit;
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

                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Vertical))
                        {
                            if (Utils.StyleBars(chartStyle))
                                yStep = height / ticks;
                            else
                                yStep = (Utils.StyleFullStacked(chartStyle))
                                    ? height / 10
                                    : stepLength;

                            limit = Utils.StyleBars(chartStyle) ? ticks : linesCountY;
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

                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Horizontal))
                        {
                            if (!Utils.StyleBars(chartStyle))
                                xStep = width / 2 / linesCountX;
                            else
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

                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Vertical))
                        {
                            if (Utils.StyleBars(chartStyle))
                                yStep = height / ticks;
                            else
                                yStep = (Utils.StyleFullStacked(chartStyle))
                                    ? height / 10
                                    : stepLength;
                            var y = Utils.AXIS_THICKNESS;
                            limit = Utils.StyleBars(chartStyle) ? ticks : linesCountY;
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
                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Horizontal))
                        {
                            var x = Utils.AXIS_THICKNESS + boundOffset;
                            if (!Utils.StyleBars(chartStyle))
                            {
                                var (Step, Limit) = Utils.Limits(chartStyle, offsetBoundary, linesCountX, ticks, boundOffset, width);
                                xStep = Step;
                                limit = Limit;
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

                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Vertical))
                        {
                            if (Utils.StyleBars(chartStyle))
                                yStep = height / 2 / ticks;
                            else
                                yStep = (Utils.StyleFullStacked(chartStyle))
                                     ? height / 20
                                     : stepLength;
                            var y = Utils.AXIS_THICKNESS;
                            limit = Utils.StyleBars(chartStyle) ? ticks * 2 : linesCountY * 2;
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

                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Horizontal))
                        {
                            if (!Utils.StyleBars(chartStyle))
                                xStep = width / linesCountX;
                            else
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

                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Vertical))
                        {
                            if (Utils.StyleBars(chartStyle))
                                yStep = height / ticks;
                            else
                                yStep = (Utils.StyleFullStacked(chartStyle))
                                     ? height / 10
                                     : stepLength;
                            var y = Utils.AXIS_THICKNESS;
                            limit = Utils.StyleBars(chartStyle) ? ticks : linesCountY;
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
                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Horizontal))
                        {
                            var x = Utils.AXIS_THICKNESS + boundOffset;
                            if (!Utils.StyleBars(chartStyle))
                            {
                                var (Step, Limit) = Utils.Limits(chartStyle, offsetBoundary, linesCountX, ticks, boundOffset, width);
                                xStep = Step;
                                limit = Limit;
                            }
                            else
                            {
                                xStep = stepLength;
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

                        if (axesVisibility.In(AxesVisibility.Both, AxesVisibility.Vertical))
                        {
                            if (chartStyle.In(ChartStyle.Bars, ChartStyle.StackedBars, ChartStyle.FullStackedBars))
                                yStep = height / ticks;
                            else
                                yStep = stepLength;
                            var y = Utils.AXIS_THICKNESS;
                            limit = Utils.StyleBars(chartStyle) ? ticks : linesCountY;
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
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[3] is ChartStyle chartStyle)
                || chartStyle == ChartStyle.Funnel
                || !(values[4] is int linesCount)
                || !(values[5] is ChartBoundary chartBoundary)
                || !(values[6] is FontFamily fontFamily)
                || !(values[7] is double fontSize)
                || !(values[8] is FontStyle fontStyle)
                || !(values[9] is FontWeight fontWeight)
                || !(values[10] is FontStretch fontStretch)
                || !(values[11] is bool autoAdjust)
                || !(values[12] is double maxX))
                return null;

            if (chartStyle.In(ChartStyle.Area, ChartStyle.StackedArea, ChartStyle.FullStackedArea, ChartStyle.SmoothArea))
                return null;
            if (!seriesEnumerable.Any())
                return null;

            var series = seriesEnumerable.ToArray();
            var offsetBoundary = Utils.OffsetBoundary(chartBoundary, chartStyle);
            var totalValues = series.SelectMany(s => s.Values.Select(v => v.Value.PlainValue).ToArray());
            var dir = Utils.GetDirection(totalValues, chartStyle);
            var ticks = series.Select(s => s.Values.Count).Max();
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
                var (max, min, realLinesCount, stepSize, stepL, units, zeroPoint) = autoAdjust
                    ? Utils.GetMeasures(
                       chartStyle,
                       series,
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
                        var x = Utils.AXIS_THICKNESS + boundOffset;
                        if (!Utils.StyleBars(chartStyle))
                        {
                            var (Step, Limit) = Utils.Limits(chartStyle, offsetBoundary, linesCount, ticks, boundOffset, width);
                            xStep = Step;
                            limit = Limit;
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
                        if (!Utils.StyleBars(chartStyle))
                            xStep = width / 2 / linesCount;
                        else
                            xStep = stepLength;
                        var x = Utils.AXIS_THICKNESS;
                        for (var i = 1; i < linesCount * 2; i++)
                        {
                            x += xStep;
                            if (i == linesCount) continue;
                            var start = new Point(x, 0);
                            var end = new Point(start.X, height);
                            gm.Figures.Add(new PathFigure(start, new[] { new LineSegment(end, true) }, false));
                        }
                        break;
                    }
                case Directions.NorthEastSouthEast:
                    {
                        var x = Utils.AXIS_THICKNESS + boundOffset;
                        if (!Utils.StyleBars(chartStyle))
                        {
                            var (Step, Limit) = Utils.Limits(chartStyle, offsetBoundary, linesCount, ticks, boundOffset, width);
                            xStep = Step;
                            limit = Limit;
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
                case Directions.NorthWest:
                    {
                        if (!Utils.StyleBars(chartStyle))
                            return null;
                        if (!Utils.StyleBars(chartStyle))
                            xStep = width / linesCount;
                        else
                            xStep = stepLength;
                        var x = Utils.AXIS_THICKNESS;
                        for (var i = 1; i < linesCount; i++)
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
                        if (!Utils.StyleBars(chartStyle))
                        {
                            var (Step, Limit) = Utils.Limits(chartStyle, offsetBoundary, linesCount, ticks, boundOffset, width);
                            xStep = Step;
                            limit = Limit;
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
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is IEnumerable<ISeries> seriesEnumerable)
                || !(values[3] is ChartStyle chartStyle)
                || chartStyle == ChartStyle.Funnel
                || !(values[4] is int linesCount)
                || !(values[5] is FontFamily fontFamily)
                || !(values[6] is double fontSize)
                || !(values[7] is FontStyle fontStyle)
                || !(values[8] is FontWeight fontWeight)
                || !(values[9] is FontStretch fontStretch)
                || !(values[10] is bool autoAdjust)
                || !(values[11] is double maxY))
                return null;

            if (!seriesEnumerable.Any())
                return null;

            var series = seriesEnumerable.ToArray();

            var totalValues = chartStyle != ChartStyle.Waterfall
               ? series.SelectMany(s => s.Values.Select(v => v.Value.PlainValue))
               : series[0].Values.Select(v => v.Value.PlainValue);

            var dir = Utils.GetDirection(totalValues, chartStyle);

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

            var (max, min, realLinesCount, stepSize, stepLength, units, zeroPoint) = autoAdjust
                ? Utils.GetMeasures(
                   chartStyle,
                   series,
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
                    limit = realLinesCount * 2;
                    if (Utils.StyleFullStacked(chartStyle))
                        stepLength = height / 20;
                    break;
                case Directions.NorthEast:
                case Directions.NorthEastNorthWest:
                case Directions.NorthWest:
                case Directions.SouthEast:
                    limit = realLinesCount;
                    if (Utils.StyleFullStacked(chartStyle))
                        stepLength = height / 10;
                    break;
                default:
                    return null;
            }
            //var yStep = height / limit;
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
                || !(values[0] is double width)
                || !(values[1] is double height))
                return 0.0;
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
                || !(values[0] is double width)
                || !(values[1] is double height))
                return 0.0;
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
                || !(values[0] is ChartValues chartValues)
                || !(values[1] is string format)
                || !(parameter is IChartValue chartValue))
                return null;
            var sum = chartValues.Sum(p => Math.Abs(p.Value.PlainValue));
            if (format.EndsWith("%")) format = format.Substring(0, format.Length - 1);
            var perc = Math.Abs(chartValue.Value.PlainValue) / sum * 100;
            var sectorData = chartValue.CustomValue
                ?? chartValue.Value.PlainValue + " (" + perc.ToString(format) + "%)";
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
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is IEnumerable<ISeries> seriesEnumerable)
                || !seriesEnumerable.Any()
                || !(values[3] is ChartStyle chartStyle)
                || !Utils.StyleRadar(chartStyle)
                //|| !(values[6] is string format)
                || !(values[4] is FontFamily fontFamily)
                || !(values[5] is double fontSize)
                || !(values[6] is FontStyle fontStyle)
                || !(values[7] is FontWeight fontWeight)
                || !(values[8] is FontStretch fontStretch)
                //|| !(values[9] is IEnumerable<string> customEnumerable)
                || !(values[10] is int linesCount)
                || !(values[11] is bool autoAdjust))
                return null;

            var gm = new PathGeometry();
            var customValues = values[9] is IEnumerable<string> customEnumerable ? customEnumerable.ToArray() : new string[] { };

            var series = seriesEnumerable.ToArray();
            var currentDegrees = 0.0;
            var pointsCount = series.Max(s => s.Values.Count);
            var degreesStep = 360.0 / pointsCount;

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
            var (max, min, realLinesCount, stepSize, _, _, _) = Utils.GetMeasures(chartStyle, series, linesCount, radius, fmtY.Height, centerPoint);
            linesCount = realLinesCount;

            var distanceStep = radius / linesCount;

            for (var step = 0; step < linesCount; step++)
            {
                var distance = radius - distanceStep * step;
                var points = new List<Point>();
                for (var i = 0; i < pointsCount; i++)
                {
                    currentDegrees = 90.0 + i * degreesStep;
                    var rads = currentDegrees * Math.PI / 180.0;
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
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is IEnumerable<ISeries> seriesEnumerable)
                || !seriesEnumerable.Any()
                || !(values[3] is ChartStyle chartStyle)
                || !Utils.StyleRadar(chartStyle)
                || !(values[4] is FontFamily fontFamily)
                || !(values[5] is double fontSize)
                || !(values[6] is FontStyle fontStyle)
                || !(values[7] is FontWeight fontWeight)
                || !(values[8] is FontStretch fontStretch)
                || !(values[9] is FlowDirection flowDir)
                || !(values[12] is int linesCount)
                || linesCount < 2
                || !(values[13] is string format)
                || !(values[14] is bool autoAdjust))
                return null;

            var gm = new PathGeometry();
            var customValuesHorizontal = values[10] is IEnumerable<string> customEnumerableX ? customEnumerableX.ToArray() : new string[] { };
            var customValuesVertical = values[11] is IEnumerable<string> customEnumerableY ? customEnumerableY.ToArray() : new string[] { };

            var series = seriesEnumerable.ToArray();
            var currentDegrees = 0.0;
            var pointsCount = series.Max(s => s.Values.Count);
            var degreesStep = 360.0 / pointsCount;

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
            var (max, min, realLinesCount, stepSize, _, _, _) = Utils.GetMeasures(chartStyle, series, linesCount, radius, fmtY.Height, centerPoint);
            linesCount = realLinesCount;

            // fractional stepSize
            if (!Utils.IsInteger(stepSize))
            {
                var fractions = Utils.GetDecimalPlaces(stepSize);
                if (format.EndsWith("%"))
                    format = $"{format.Substring(0, format.Length - 1)}{culture.NumberFormat.NumberDecimalSeparator}{new string('0', fractions)}%";
                else
                    format += $"{culture.NumberFormat.NumberDecimalSeparator}{new string('0', fractions)}";
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
                || !(values[0] is double width)
                || !(values[1] is double height)
                || !(values[2] is IEnumerable<ISeries> seriesEnumerable)
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
            var sum = pts.Sum(p => Math.Abs(p.Value.PlainValue));
            var currentDegrees = 90.0;
            var startPoint = new Point(radius, radius);
            var xBeg = radius;
            var yBeg = 0.0;

            var dGroup = new DrawingGroup();

            if (format.EndsWith("%")) format = format.Substring(0, format.Length - 1);
            if (pts.Length == 1)
            {
                var sectorData = pts[0].CustomValue
                    ?? pts[0].Value.PlainValue + " (" + 100.ToString(format) + "%)";
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
                var addition = Math.Abs(pt.Value.PlainValue) / sum * 360.0;
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
                //else if (chartStyle == ChartStyle.SlicedPie)
                //{
                //    var rectGeometry = new RectangleGeometry(new Rect(startPoint.X-2, 0, 4, radius), 4, 4, new RotateTransform(currentDegrees, startPoint.X-2, startPoint.Y));
                //    //var seg1 = new PolyLineSegment(new[] { new Point(xBeg, yBeg), new Point(xBeg + 12, yBeg + 12), new Point(radius + 12, radius + 12) }, false);
                //    //var pathFigure = new PathFigure(startPoint, new[] { seg1 }, true);
                //    //var pathLineGeometry = new PathGeometry(new[] { pathFigure });
                //    combinedGeometry.Geometry2 = rectGeometry;
                //}

                var perc = Math.Abs(pt.Value.PlainValue) / sum * 100;
                var sectorData = pt.CustomValue
                    ?? pt.Value.PlainValue + " (" + perc.ToString(format) + "%)";
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
