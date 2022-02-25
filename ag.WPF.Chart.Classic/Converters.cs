using ag.WPF.Chart.Series;
using ag.WPF.Chart.Values;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ag.WPF.Chart
{
#nullable disable
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

        private static double transformSmallFractionalNumber(double number)
        {
            var sign = Math.Sign(number);
            number = Math.Abs(number);
            var delimeter = 1;
            while (number < 1)
            {
                number *= 10;
                delimeter *= 10;
            }
            number = Math.Ceiling(number);
            number /= delimeter;
            return number * sign;
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
                    yield return max / (b * (long)Math.Pow(10, p));
        }

        private static bool isEven(double number) => (number % 2) < double.Epsilon;

        private static bool is5Delimited(double number) => (number % 5) < double.Epsilon;

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

        private static int getMaxFractionPower(IEnumerable<double> numbers) => numbers.Select(n => GetDecimalPlaces(n)).Max();

        internal static (string numericPart, string literalPart) GetFormatParts(string format, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(format))
                return ("", "");
            var index = -1;
            if (char.IsDigit(format[0]) || format[0] == culture.NumberFormat.NumberDecimalSeparator[0] || format[0] == culture.NumberFormat.NumberGroupSeparator[0])
            {
                for (var i = 0; i < format.Length; i++)
                {
                    if (!char.IsDigit(format[i]) && format[i] != culture.NumberFormat.NumberDecimalSeparator[0] && format[i] != culture.NumberFormat.NumberGroupSeparator[0])
                        break;
                    index++;
                }
                return (format.Substring(0, index + 1), format.Substring(index + 1));
            }
            else
            {
                for (var i = 0; i < format.Length; i++)
                {
                    if (char.IsDigit(format[i]) && format[i] == culture.NumberFormat.NumberDecimalSeparator[0] || format[i] == culture.NumberFormat.NumberGroupSeparator[0])
                        break;
                    index++;
                }
                return (format.Substring(index + 1), format.Substring(0, index + 1));
            }
        }

        internal static int GetDecimalPlaces(double number) => BitConverter.GetBytes(decimal.GetBits((decimal)number)[3])[2];

        internal static double GetUnitsForBars(ISeries[] series, ChartStyle chartStyle, Directions dir, double width, int linesCountX, double formatHeight, bool autoAdjust, double maxX)
        {
            var centerX = 0.0;
            var radius = 0.0;

            switch (dir)
            {
                case Directions.NorthEast:
                case Directions.NorthWest:
                case Directions.SouthEast:
                    centerX = AXIS_THICKNESS;
                    radius = width - AXIS_THICKNESS;
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = AXIS_THICKNESS;
                    radius = (width - AXIS_THICKNESS) / 2;
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = AXIS_THICKNESS;
                    radius = width - AXIS_THICKNESS;
                    break;
            }
            if (!autoAdjust)
                return radius / maxX;
            var centerPoint = new Point(centerX, radius);
            var (_, _, _, _, _, units, _) = GetMeasures(
               chartStyle,
               series,
               linesCountX,
               radius,
               formatHeight,
               centerPoint,
               dir == Directions.NorthEastNorthWest);
            return units;
        }

        internal static double GetUnitsForLines(ISeries[] series, ChartStyle chartStyle, Directions dir, double width, double height, double boundOffset, int linesCountY, double formatHeight, AutoAdjustmentMode autoAdjust, double maxY)
        {
            var centerX = 0.0;
            var radius = 0.0;

            switch (dir)
            {
                case Directions.NorthEast:
                case Directions.NorthWest:
                case Directions.SouthEast:
                    centerX = AXIS_THICKNESS + boundOffset;
                    radius = height - AXIS_THICKNESS;
                    break;
                case Directions.NorthEastNorthWest:
                    centerX = width / 2;
                    radius = height - AXIS_THICKNESS;
                    break;
                case Directions.NorthEastSouthEast:
                    centerX = AXIS_THICKNESS + boundOffset;
                    radius = (height - AXIS_THICKNESS) / 2;
                    break;
            }
            if (!autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Vertical))
                return radius / maxY;
            var centerPoint = new Point(centerX, radius);
            var (_, _, _, _, _, units, _) = GetMeasures(
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

        internal static List<(List<IChartValue> Values, int Index)> GetPaddedSeriesFinancial(ISeries series)
        {
            var rawValues = new List<(List<IChartValue> Values, int Index)>
            {
                (series.Values.ToList(), series.Index)
            };
            var maxCount = rawValues.Max(rw => rw.Values.Count);
            foreach (var rw in rawValues.Where(rv => rv.Values.Count < maxCount))
            {
                var diff = maxCount - rw.Values.Count;
                for (var i = 0; i < diff; i++)
                {
                    if (series is HighLowCloseSeries)
                        rw.Values.Add(new HighLowCloseChartValue(0, 0, 0));
                    else if (series is OpenHighLowCloseSeries)
                        rw.Values.Add(new OpenHighLowCloseChartValue(0, 0, 0, 0));
                }
            }
            return rawValues;
        }

        private static (double max, double min, int linesCount, double step, double stepLength, double units, ZeroPoint zeroPoint) getMeasuresForPositive(double max, int linesCount, double radius, double fontHeight, int fractionPower, ZeroPoint centerPoint)
        {
            if (max < double.Epsilon || radius <= 0)
                return (0, 0, 1, 1, 1, 1, default);
            var stepSize = 0.0;
            var stepLength = 0.0;
            var units = 0.0;

            var originalMax = max;

            // round max to next integer
            max = originalMax > 1 ? Math.Ceiling(max) : transformSmallFractionalNumber(max);

            var pm = Math.Abs((long)max).ToString().Length - 1;
            var p = pm >= 3 ? pm - 1 : pm;
            // do not increase max for integers that are equal to 10 power
            if ((max % Math.Pow(10, p)) != 0 && originalMax > 1)
                max = Math.Sign(max) * roundInt((long)Math.Abs(max), (long)Math.Pow(10, pm));

            if (fractionPower > 0 && originalMax <= 1)
            {
                max *= Math.Pow(10, fractionPower);
            }

            var power = Math.Abs((long)max).ToString().Length - 1;

            // difference is always max
            // get all available integer lines counts
            var lines = calculatedSteps(power, max).Where(l => IsInteger(l)).OrderBy(l => l).Distinct().ToArray();
            // calculate real size for each step
            var sizes = lines.Select(s => radius / s).ToArray();

            var items = sizes.Select((size, index) => new { size, index }).Where(a => a.size >= fontHeight + 4).OrderByDescending(a => a.index);
            if (items.Any())
            {
                foreach (var a in items)
                {
                    var itemLines = lines[a.index];
                    var sz = max / itemLines;
                    var t = max;
                    while (t > 0)
                    {
                        t -= sz;
                    }
                    if (t < double.Epsilon)
                    {
                        linesCount = (int)itemLines;
                        break;
                    }
                }
                linesCount = (int)lines[items.First().index];
            }

            if (linesCount > 0)
            {
                // prepare step
                stepSize = max / linesCount;
                stepLength = radius / linesCount;
                // prepare units
                units = Math.Abs(radius / max);
            }
            else
            {
                stepSize = max / linesCount;
                while (!IsInteger(stepSize))
                {
                    max = Math.Sign(max) * roundInt((long)Math.Abs(max), (long)Math.Pow(10, power));
                    stepSize = max / linesCount;
                }
                stepLength = radius / linesCount;
                units = Math.Abs(radius / max);
            }

            // zero point Y-coordinate is stays the same and leve stays 0

            if (fractionPower > 0 && originalMax <= 1)
            {
                var m = Math.Pow(10, fractionPower);
                max /= m;
                units *= m;
                stepSize /= m;
            }
            return (max, 0, linesCount, stepSize, stepLength, units, centerPoint);
        }

        private static (double max, double min, int linesCount, double stepSize, double stepLength, double units, ZeroPoint zeroPoint) getMeasuresForNegative(double min, int linesCount, double radius, double fontHeight, int fractionPower, ZeroPoint zeroPoint)
        {
            if (min < double.Epsilon || radius <= 0)
                return (0, 0, 1, 1, 1, 1, default);
            var stepSize = 0.0;
            var stepLength = 0.0;
            var units = 0.0;

            var originalMin = Math.Abs(min);

            // round min to prevous integer
            min = originalMin > 1 ? Math.Floor(min) : transformSmallFractionalNumber(min);

            var pm = Math.Abs((long)min).ToString().Length - 1;
            var p = pm >= 3 ? pm - 1 : pm;
            // do not increase max for integers that are equal to 10 power
            if ((min % Math.Pow(10, p)) != 0 && originalMin > 1)
                min = Math.Sign(min) * roundInt((long)Math.Abs(min), (long)Math.Pow(10, pm));

            if (fractionPower > 0 && originalMin <= 1)
            {
                min = -Math.Abs(min * Math.Pow(10, fractionPower));
            }

            var power = Math.Abs((long)min).ToString().Length - 1;

            // difference is always equal absolute value of min
            var diff = Math.Abs(min);
            // get all available integer lines counts
            var lines = calculatedSteps(power, Math.Abs(min)).Where(l => IsInteger(l)).OrderBy(l => l).Distinct().ToArray();
            // calculate real size for each step
            var sizes = lines.Select(s => radius / s).ToArray();

            var items = sizes.Select((size, index) => new { size, index }).Where(a => a.size >= fontHeight + 4).OrderByDescending(a => a.index);
            if (items.Any())
            {
                foreach (var a in items)
                {
                    var itemLines = lines[a.index];
                    var sz = diff / itemLines;
                    var t = min;
                    while (t < 0)
                    {
                        t += sz;
                    }
                    if (t < double.Epsilon)
                    {
                        linesCount = (int)itemLines;
                        break;
                    }
                }
                linesCount = (int)lines[items.First().index];
            }

            if (linesCount > 0)
            {
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
                    min = Math.Sign(min) * roundInt((long)Math.Abs(min), (long)Math.Pow(10, power));
                    diff = Math.Abs(min);
                    stepSize = diff / linesCount;
                }
                units = Math.Abs(radius / diff);
                stepLength = radius / linesCount;
            }

            // find zero point
            zeroPoint.Point.Y -= stepSize * linesCount * units;
            zeroPoint.Level = linesCount;

            if (fractionPower > 0 && originalMin <= 1)
            {
                var m = Math.Pow(10, fractionPower);
                min /= m;
                units *= m;
                stepSize /= m;
            }

            return (0, min, linesCount, stepSize, stepLength, units, zeroPoint);
        }

        private static (double max, double min, int linesCount, double stepSize, double stepLength, double units, ZeroPoint zeroPoint) getMeasuresForComplex(double max, double min, int linesCount, double radius, double fontHeight, int fractionPower, ZeroPoint zeroPoint, bool splitSides)
        {

            if (radius <= 0)
                return (0, 0, 1, 1, 1, 1, default);

            int tempLines;

            var originalMax = max;
            var originalMin = Math.Abs(min);

            // round max to next integer
            max = originalMax > 1 ? Math.Ceiling(max) : transformSmallFractionalNumber(max);
            // round min to prevous integer
            min = originalMin > 1 ? Math.Floor(min) : transformSmallFractionalNumber(min);

            var pMax = Math.Abs((long)max).ToString().Length - 1;
            var pMin = Math.Abs((long)min).ToString().Length - 1;

            // do not increase max for integers that are equal to 10 power
            var p = pMax >= 3 ? pMax - 1 : pMax;
            if ((max % Math.Pow(10, p)) != 0 && (originalMax > 1 || originalMin > 1))
                max = Math.Sign(max) * roundInt((long)Math.Abs(max), (long)Math.Pow(10, pMax));
            // do not increase max for integers that are equal to 10 power
            p = pMin >= 3 ? pMin - 1 : pMin;
            if ((min % Math.Pow(10, p)) != 0 && (originalMax > 1 || originalMin > 1))
                min = Math.Sign(min) * roundInt((long)Math.Abs(min), (long)Math.Pow(10, pMin));

            if (fractionPower > 0 && originalMax <= 1 && originalMin <= 1)
            {
                max *= Math.Pow(10, fractionPower);
                min = -Math.Abs(min * Math.Pow(10, fractionPower));
            }

            var powerMax = Math.Abs((long)max).ToString().Length - 1;
            var powerMin = Math.Abs((long)min).ToString().Length - 1;

            // store absolute values of max and min
            var absMax = Math.Abs(max);
            var absMin = Math.Abs(min);
            if (originalMax > 10)
                while (!isEven(absMax) && !is5Delimited(absMax))
                    absMax++;
            if (originalMin > 10)
                while (!isEven(absMin) && !is5Delimited(absMin))
                    absMin++;

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

                if (fractionPower > 0 && originalMax <= 1 && originalMin <= 1)
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
                tempLines = getLineNumbersForComplex(Math.Max(powerMax, powerMin), absoluteMax, radius, fontHeight, diff, min);
                if (tempLines != 0)
                {
                    // change lines count to selected one
                    linesCount = tempLines;
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

                    if (fractionPower > 0 && originalMax <= 1 && originalMin <= 1)
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
            tempLines = getLineNumbersForComplex(Math.Max(powerMax, powerMin), Math.Abs(diff), radius, fontHeight, diff, min);
            if (tempLines > 0)
            {
                // change lines count to selected one
                linesCount = tempLines;
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
                        max = Math.Sign(max) * roundInt((long)Math.Abs(max), (long)Math.Pow(10, powerMax));
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
                        min = Math.Sign(min) * roundInt((long)Math.Abs(min), (long)Math.Pow(10, powerMin));
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

            if (fractionPower > 0 && originalMax <= 1 && originalMin <= 1)
            {
                var m = Math.Pow(10, fractionPower);
                min /= m;
                max /= m;
                units *= m;
                stepSize /= m;
            }

            return (max, min, linesCount, stepSize, stepLength, units, zeroPoint);
        }

        private static (double max, double min, int linesCount, double stepSize, double stepLength, double units, ZeroPoint zeroPoint) getMeasuresForComplexRadar(double max, double min, int linesCount, double radius, double fontHeight, int fractionPower, ZeroPoint zeroPoint)
        {
            if (radius <= 0)
                return (0, 0, 1, 1, 1, 1, default);

            int tempLines;

            var originalMax = max;
            var originalMin = Math.Abs(min);

            // round max to next integer
            max = originalMax > 1 ? Math.Ceiling(max) : transformSmallFractionalNumber(max);
            // round min to prevous integer
            min = originalMin > 1 ? Math.Floor(min) : transformSmallFractionalNumber(min);

            var pMax = Math.Abs((long)max).ToString().Length - 1;
            var pMin = Math.Abs((long)min).ToString().Length - 1;

            // do not increase max for integers that are equal to 10 power
            var p = pMax >= 3 ? pMax - 1 : pMax;
            if ((max % Math.Pow(10, p)) != 0 && (originalMax > 1 || originalMin > 1))
                max = Math.Sign(max) * roundInt((long)Math.Abs(max), (long)Math.Pow(10, pMax));
            // do not increase max for integers that are equal to 10 power
            p = pMin >= 3 ? pMin - 1 : pMin;
            if ((min % Math.Pow(10, p)) != 0 && (originalMax > 1 || originalMin > 1))
                min = Math.Sign(min) * roundInt((long)Math.Abs(min), (long)Math.Pow(10, pMin));

            if (fractionPower > 0 && originalMax <= 1 && originalMin <= 1)
            {
                max *= Math.Pow(10, fractionPower);
                min = -Math.Abs(min * Math.Pow(10, fractionPower));
            }

            var powerMax = Math.Abs((long)max).ToString().Length - 1;
            var powerMin = Math.Abs((long)min).ToString().Length - 1;

            // store absolute values of max and min
            var absMax = Math.Abs(max);
            var absMin = Math.Abs(min);
            if (originalMax > 10)
                while (!isEven(absMax) && !is5Delimited(absMax))
                    absMax++;
            if (originalMin > 10)
                while (!isEven(absMin) && !is5Delimited(absMin))
                    absMin++;

            max = Math.Sign(max) * absMax;
            min = Math.Sign(min) * absMin;

            // get absolute max
            double stepSize;
            double stepLength;
            double units;
            double temp;
            // store max and min difference
            var diff = getDiff(max, min);

            if (absMax > absMin)
                (tempLines, min) = getLineNumbersForComplexRadar(Math.Max(powerMax, powerMin), max, radius, fontHeight, diff, min);
            else
                (tempLines, max) = getLineNumbersForComplexRadar(Math.Max(powerMax, powerMin), min, radius, fontHeight, diff, max);

            diff = getDiff(max, min);
            if (tempLines > 0)
            {
                // change lines count to selected one
                linesCount = tempLines;
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
                        max = Math.Sign(max) * roundInt((long)Math.Abs(max), (long)Math.Pow(10, powerMax));
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
                        min = Math.Sign(min) * roundInt((long)Math.Abs(min), (long)Math.Pow(10, powerMin));
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

            if (fractionPower > 0 && originalMax <= 1 && originalMin <= 1)
            {
                var m = Math.Pow(10, fractionPower);
                min /= m;
                max /= m;
                units *= m;
                stepSize /= m;
            }

            return (max, min, linesCount, stepSize, stepLength, units, zeroPoint);
        }

        private static (int linesCount, double min) getLineNumbersForComplexRadar(int power, double max, double radius, double fontHeight, double diff, double min)
        {
            // store sign of min
            var signMin = Math.Sign(min);
            // get abs values of max (it may be negative)
            max = Math.Abs(max);
            // get units per max
            var u = radius / diff;
            var radiusMax = u * max;

            // get all available integer lines counts for max
            var lines = calculatedSteps(power, max).Where(l => IsInteger(l)).OrderBy(l => l).Distinct().ToArray();
            var sizes = lines.Select(s => radiusMax / s).ToArray();
            // get all lines with real size more/equal font height
            var items = sizes.Select((size, index) => new { size, index }).Where(a => a.size >= fontHeight).OrderByDescending(a => a.index);
            var linesCount = 1;
            foreach (var a in items)
            {
                var itemLines = lines[a.index];
                var sz = max / itemLines;
                var t = max;
                while (t > 0)
                {
                    t -= sz;
                }
                if (t < double.Epsilon)
                {
                    linesCount = (int)itemLines;
                    break;
                }
            }
            // linesCount is now a lines count for max part
            // get lines per unit
            var del = max / linesCount;
            // calculate min
            for (var i = 1; ; i++)
            {
                var ln = del * i;
                if (ln >= Math.Abs(min))
                {
                    linesCount += i;
                    min = signMin * ln;
                    break;
                }
            }
            return (linesCount, min);
        }

        private static int getLineNumbersForComplex(int power, double max, double radius, double fontHeight, double diff, double min)
        {
            var lines = calculatedSteps(power, max).Where(l => IsInteger(l)).OrderBy(l => l).Distinct().ToArray();
            var sizes = lines.Select(s => radius / s).ToArray();

            var items = sizes.Select((size, index) => new { size, index }).Where(a => a.size >= fontHeight).OrderByDescending(a => a.index);
            if (items.Any())
            {
                foreach (var a in items)
                {
                    var itemLines = lines[a.index];
                    var sz = diff / itemLines;
                    var t = min;
                    while (t < 0)
                    {
                        t += sz;
                    }
                    if (t < double.Epsilon)
                    {
                        return (int)itemLines;
                    }
                }
                return (int)lines[items.First().index];
            }
            return 0;
        }

        private static long roundInt(long number, long tense)
        {
            // smaller multiple
            var smaller = number / tense * tense;
            // larger multiple
            var addition = (number % tense) > tense / 2 ? tense : tense / 2;
            var result = smaller + addition;
            return result;
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

        private static int getMaxValueLength(double max, double min, int fractionPower)
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

            var powerMax = Math.Abs((long)max).ToString().Length - 1;
            var powerMin = Math.Abs((long)min).ToString().Length - 1;

            // do not increase max for integers that are equal to 10 power
            var p = powerMax >= 3 ? powerMax - 1 : powerMax;
            if ((max % Math.Pow(10, p)) != 0)
                max = Math.Sign(max) * roundInt((long)Math.Abs(max), (long)Math.Pow(10, powerMax));
            // do not increase max for integers that are equal to 10 power
            p = powerMin >= 3 ? powerMin - 1 : powerMin;
            if ((min % Math.Pow(10, p)) != 0)
                min = Math.Sign(min) * roundInt((long)Math.Abs(min), (long)Math.Pow(10, powerMin));

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

            var values = seriesArray.All(s => s is PlainSeries)
                ? seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue))
                : seriesArray.All(s => s is not PlainSeries)
                    ? seriesArray[0].Values.Select(v => v.CompositeValue.HighValue).Union(seriesArray[0].Values.Select(v => v.CompositeValue.LowValue))
                    : new double[] { 0, 0 };
            max = values.Max();
            min = values.Min();

            if (StyleStackedLines(chartStyle) || chartStyle.In(ChartStyle.StackedColumns, ChartStyle.StackedBars))
            {
                if (seriesArray.Length == 1)
                {
                    if (values.All(v => v >= 0))
                    {
                        minIsZero = true;
                    }
                    else if (values.All(v => v <= 0))
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
                            var sign = Math.Sign(resultValues[index].Items[i].CompositeValue.PlainValue);
                            var v = (decimal)resultValues[index].Items[i].CompositeValue.PlainValue + temps.Where(t => Math.Sign((decimal)t.Items[i].CompositeValue.PlainValue) == sign).Sum(t => (decimal)t.Items[i].CompositeValue.PlainValue);
                            resultValues[index].Items[i] = new PlainChartValue((double)v);
                        }
                    }
                    values = resultValues.SelectMany(a => a.Items).Select(it => it.CompositeValue.PlainValue);
                    max = values.Max();
                    min = values.Min();
                    if (values.All(v => v >= 0))
                    {
                        minIsZero = true;
                    }
                    else if (values.All(v => v <= 0))
                    {
                        maxIsZero = true;
                    }
                }
            }
            else if (chartStyle == ChartStyle.Waterfall)
            {
                values = seriesArray[0].Values.Select(v => v.CompositeValue.PlainValue);
                (max, min) = getMaxMinForWaterfall(values.ToArray());
                if (max >= 0 && min >= 0)
                {
                    minIsZero = true;
                }
                else if (max <= 0 && min <= 0)
                {
                    maxIsZero = true;
                }
            }
            else
            {
                if (values.All(v => v >= 0))
                {
                    minIsZero = true;
                }
                else if (values.All(v => v <= 0))
                {
                    maxIsZero = true;
                }
            }

            if (minIsZero)
                return getMeasuresForPositive(max, linesCount, radius, fontHeight, getMaxFractionPower(values), new ZeroPoint { Point = centerPoint });
            else if (maxIsZero)
                return getMeasuresForNegative(min, linesCount, radius, fontHeight, getMaxFractionPower(values), new ZeroPoint { Point = centerPoint });
            else
            {
                if (StyleRadar(chartStyle))
                    return getMeasuresForComplexRadar(max, min, linesCount, radius, fontHeight, getMaxFractionPower(values), new ZeroPoint { Point = centerPoint });
                else
                    return getMeasuresForComplex(max, min, linesCount, radius, fontHeight, getMaxFractionPower(values), new ZeroPoint { Point = centerPoint }, splitSides);
            }
        }

        internal static PathGeometry DrawStar(int numberOfPoints, double radius, Point centerPoint = default, bool stroked = false)
        {
            var gm = new PathGeometry();

            var degreesStep = 360.0 / numberOfPoints;

            var rSmall = radius / 2;

            if (centerPoint == default)
                centerPoint = new Point(radius, radius);

            var points = new List<Point>();

            for (var i = 0; i < numberOfPoints; i++)
            {
                var currentDegrees = 90.0 + i * degreesStep;
                var rads = currentDegrees * Math.PI / 180.0;
                var x = centerPoint.X - radius * Math.Cos(rads);
                var y = centerPoint.Y - radius * Math.Sin(rads);
                points.Add(new Point(x, y));

                var smalDegrees = currentDegrees + 360.0 / numberOfPoints / 2;// 30.0;
                rads = smalDegrees * Math.PI / 180.0;
                x = centerPoint.X - rSmall * Math.Cos(rads);
                y = centerPoint.Y - rSmall * Math.Sin(rads);
                points.Add(new Point(x, y));
            }

            var poly = new PolyLineSegment(points, stroked);
            gm.Figures.Add(new PathFigure(points[0], new[] { poly }, true));

            return gm;
        }

        internal static bool IsInteger(double step) => Math.Abs(step % 1) < double.Epsilon;

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
            var delimeter = ticks > 1 ? ticks - 1 : 1;
            return offsetBoundary
                ? ((width - 2 * boundOffset) / delimeter, ticks)
                : (width / delimeter, ticks);
        }

        internal static double BoundaryOffset(bool offsetBoundary, double width, int count) => offsetBoundary ? (width - 2 * AXIS_THICKNESS) / (count + 2) / 2 : 0;

        internal static bool StyleRadar(ChartStyle style) => style.In(ChartStyle.Radar, ChartStyle.RadarWithMarkers, ChartStyle.RadarArea);

        internal static bool StyleBars(ChartStyle style) => style.In(ChartStyle.Bars, ChartStyle.StackedBars, ChartStyle.FullStackedBars);

        internal static bool StyleMeasuredBars(ChartStyle style) => style.In(ChartStyle.Bars, ChartStyle.StackedBars);

        internal static bool StyleColumns(ChartStyle style) => style.In(ChartStyle.Columns, ChartStyle.StackedColumns, ChartStyle.FullStackedColumns, ChartStyle.Waterfall);

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
                ChartStyle.StackedArea,
                ChartStyle.SmoothStackedArea);
        }

        internal static bool StyleFullStacked(ChartStyle chartStyle)
        {
            return chartStyle.In(ChartStyle.FullStackedColumns, ChartStyle.FullStackedArea,
                ChartStyle.FullStackedLines, ChartStyle.FullStackedLinesWithMarkers,
                ChartStyle.SmoothFullStackedLines, ChartStyle.SmoothFullStackedLinesWithMarkers, ChartStyle.SmoothFullStackedArea);
        }

        internal static bool OffsetBoundary(ChartBoundary boundary, ChartStyle style)
        {
            return style.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose)
                || (boundary == ChartBoundary.WithOffset &&
                   style.In(ChartStyle.Lines, ChartStyle.StackedLines, ChartStyle.FullStackedLines,
                       ChartStyle.LinesWithMarkers, ChartStyle.StackedLinesWithMarkers,
                       ChartStyle.FullStackedLinesWithMarkers, ChartStyle.SmoothLines,
                       ChartStyle.SmoothStackedLines, ChartStyle.SmoothFullStackedLines,
                       ChartStyle.SmoothLinesWithMarkers, ChartStyle.SmoothStackedLinesWithMarkers,
                       ChartStyle.SmoothFullStackedLinesWithMarkers, ChartStyle.Bubbles));
        }

        internal static int GetMaxValueLength(List<(List<IChartValue> Values, int Index)> tuples, ChartStyle style)
        {
            if (!tuples.Any()) return 0;
            var result = "100.0".Length;
            var values = tuples.SelectMany(t => t.Values).Select(v => v.CompositeValue.PlainValue);
            switch (style)
            {
                case ChartStyle.Waterfall:
                    var maxPlus = 0.0;
                    var maxMinus = 0.0;
                    var value = 0.0;
                    values = tuples[0].Values.Select(v => v.CompositeValue.PlainValue);
                    foreach (var v in values)
                    {
                        value += v;
                        if (value > 0)
                            maxPlus = Math.Max(maxPlus, value);
                        if (value < 0)
                            maxMinus = Math.Min(maxMinus, value);
                    }
                    result = getMaxValueLength(maxPlus, maxMinus, getMaxFractionPower(values));// Math.Max(maxPlus, Math.Abs(maxMinus));
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
                    result = getMaxValueLength(values.Max(), values.Min(), getMaxFractionPower(values));
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
                                if (s.Values[i].CompositeValue.PlainValue < 0)
                                    minusArr[i] += s.Values[i].CompositeValue.PlainValue;
                                else
                                    plusArr[i] += s.Values[i].CompositeValue.PlainValue;
                            }
                        }
                        result = getMaxValueLength(plusArr.Max(), minusArr.Min(), getMaxFractionPower(values));
                        break;
                    }
                case ChartStyle.StackedArea:
                case ChartStyle.SmoothStackedArea:
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
                                arr1[i] += s.Values[i].CompositeValue.PlainValue;
                                if (Math.Abs(arr2[i]) < Math.Abs(s.Values[i].CompositeValue.PlainValue))
                                    arr2[i] = Math.Abs(s.Values[i].CompositeValue.PlainValue);
                            }
                        }
                        result = getMaxValueLength(arr2.Max(), arr1.Min(), getMaxFractionPower(values));
                        break;
                    }
            }
            return result;
        }

        internal static int GetMaxValueLengthFinancial(List<(List<IChartValue> Values, int Index)> tuples)
        {
            if (!tuples.Any()) return 0;
            var values = tuples.SelectMany(t => t.Values).Select(v => v.CompositeValue.HighValue).Union(tuples.SelectMany(t => t.Values).Select(v => v.CompositeValue.LowValue));
            return getMaxValueLength(values.Max(), values.Min(), getMaxFractionPower(values));
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

        internal static Directions GetDirectionFinancial(IEnumerable<(double High, double Low)> totalValues)
        {
            if (totalValues.All(v => v.High >= 0 && v.Low >= 0))
                return Directions.NorthEast;
            else if (totalValues.All(v => v.High < 0 && v.Low < 0))
                return Directions.SouthEast;
            else
                return Directions.NorthEastSouthEast;
        }

        internal static bool IsStockSeries(this ISeries series) => (series is HighLowCloseSeries) || (series is OpenHighLowCloseSeries);

        internal static bool In<T>(this T t, params T[] values) => values.Contains(t);
    }

    /// <summary>
    /// Prepares geometry for shape drawing
    /// </summary>
    public class StarDrawingConverter : IValueConverter
    {
        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ShapeStyle shapeStyle)
                return null;
            return shapeStyle switch
            {
                ShapeStyle.Star5 => Utils.DrawStar(5, 8),
                ShapeStyle.Star6 => Utils.DrawStar(6, 8),
                ShapeStyle.Star8 => Utils.DrawStar(8, 8),
                ShapeStyle.Circle => new EllipseGeometry(new Rect(new Size(16, 16))),
                ShapeStyle.Rectangle => new RectangleGeometry(new Rect(new Size(16, 16))),
                _ => null
            };
        }

        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
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
                || values[0] is not double width
                || values[1] is not double height
                || values[4] is not ChartStyle chartStyle)
                return null;

            var seriesEnumerable = values[2] as IEnumerable<ISeries>;
            var chartSeries = values[3] as IEnumerable<ISeries>;

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
                || values[0] is not double width
                || values[1] is not double height
                || values[4] is not ChartStyle chartStyle)
                return null;

            var seriesEnumerable = values[2] as IEnumerable<ISeries>;
            var chartSeries = values[3] as IEnumerable<ISeries>;

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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
    }

    /// <summary>
    /// Defines pie legends visibility
    /// </summary>
    public class LegendPieVisibilityConverter : IMultiValueConverter
    {
        /// <summary>Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.</summary>
        /// <returns>A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty" />.<see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding" />.<see cref="F:System.Windows.Data.Binding.DoNothing" /> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> or the default value.</returns>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding" /> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var seriesEnumerable = values[0] as IEnumerable<ISeries>;
            var chartSeries = values[1] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.All(s => s is PlainSeries)) && (chartSeries == null || !chartSeries.All(s => s is PlainSeries)))
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    /// <summary>
    /// Defines stock series legend visibility
    /// </summary>
    public class LegendStockVisibilityConverter : IMultiValueConverter
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
                || values[0] is not ChartStyle chartStyle
                || !chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose)
                || values[1] is not int index
                || index > 0
                || parameter is not ColoredPaths colored)
                return Visibility.Collapsed;

            var seriesEnumerable = values[2] as IEnumerable<ISeries>;
            var chartSeries = values[3] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return Visibility.Collapsed;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            if (!seriesArray[0].Values.Any())
                return Visibility.Collapsed;

            if (chartStyle == ChartStyle.HighLowClose && seriesArray[0] is not HighLowCloseSeries)
                return Visibility.Collapsed;
            else if (chartStyle == ChartStyle.OpenHighLowClose && seriesArray[0] is not OpenHighLowCloseSeries)
                return Visibility.Collapsed;
            else if (chartStyle == ChartStyle.OpenHighLowClose && colored == ColoredPaths.Stock)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }
        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    /// <summary>
    /// Defines waterfall legends visibility
    /// </summary>
    public class LegendWaterfallVisibilityConverter : IMultiValueConverter
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
                || values[0] is not ChartStyle chartStyle
                || chartStyle != ChartStyle.Waterfall
                || values[1] is not int index
                || index > 0)
                return Visibility.Collapsed;

            var seriesEnumerable = values[2] as IEnumerable<ISeries>;
            var chartSeries = values[3] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.All(s => s is PlainSeries)) && (chartSeries == null || !chartSeries.All(s => s is PlainSeries)))
                return Visibility.Collapsed;

            return Visibility.Visible;
        }
        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    /// <summary>
    /// Defines legend visibility
    /// </summary>
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
                || values[0] is not ChartStyle chartStyle
                || chartStyle.In(ChartStyle.Waterfall, ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                return Visibility.Collapsed;

            var seriesEnumerable = values[1] as IEnumerable<ISeries>;
            var chartSeries = values[2] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.All(s => s is PlainSeries)) && (chartSeries == null || !chartSeries.All(s => s is PlainSeries)))
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
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

    /// <summary>
    /// Defines size of legend rectangle
    /// </summary>
    public class LegendSizeConverter : IValueConverter
    {
        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not LegendSize legendSize)
                return 16;
            return System.Convert.ToDouble(legendSize);
        }
        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
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
            if (value is not LegendAlignment legendAlignment) return Orientation.Vertical;
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
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
                || values[2] is not ChartStyle chartStyle)
                return HorizontalAlignment.Right;

            var seriesEnumerable = values[0] as IEnumerable<ISeries>;
            var chartSeries = values[1] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return HorizontalAlignment.Right;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            Directions dir;

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return HorizontalAlignment.Right;
                var totalValues = chartStyle.In(ChartStyle.Waterfall)
                    ? seriesArray.First().Values.Select(v => v.CompositeValue.PlainValue)
                    : seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue));
                dir = Utils.GetDirection(totalValues, chartStyle);
            }
            else if (seriesArray.All(s => s.IsStockSeries()))
            {
                if (!chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return HorizontalAlignment.Right;
                var totalValues = seriesArray.First().Values.Select(v => (v.CompositeValue.HighValue, v.CompositeValue.LowValue));
                dir = Utils.GetDirectionFinancial(totalValues);
            }
            else
                return HorizontalAlignment.Right;

            return dir switch
            {
                Directions.NorthEast or Directions.NorthEastSouthEast or Directions.SouthEast or Directions.NorthEastNorthWest => HorizontalAlignment.Right,
                Directions.NorthWest => HorizontalAlignment.Left,
                _ => HorizontalAlignment.Right,
            };
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
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
                || values[2] is not ChartStyle chartStyle)
                return 2;

            var seriesEnumerable = values[0] as IEnumerable<ISeries>;
            var chartSeries = values[1] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return 2;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            Directions dir;

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return 2;
                var totalValues = chartStyle.In(ChartStyle.Waterfall)
                    ? seriesArray.First().Values.Select(v => v.CompositeValue.PlainValue)
                    : seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue));
                dir = Utils.GetDirection(totalValues, chartStyle);
            }
            else if (seriesArray.All(s => s.IsStockSeries()))
            {
                if (!chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return 2;
                var totalValues = seriesArray.First().Values.Select(v => (v.CompositeValue.HighValue, v.CompositeValue.LowValue));
                dir = Utils.GetDirectionFinancial(totalValues);
            }
            else
                return 2;

            return dir switch
            {
                Directions.NorthEast or Directions.NorthEastSouthEast or Directions.SouthEast or Directions.NorthEastNorthWest => 2,
                Directions.NorthWest => 4,
                _ => 2,
            };
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
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
                || values[2] is not ChartStyle chartStyle)
                return VerticalAlignment.Top;

            var seriesEnumerable = values[0] as IEnumerable<ISeries>;
            var chartSeries = values[1] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return VerticalAlignment.Top;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            Directions dir;

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return VerticalAlignment.Top;
                var totalValues = chartStyle.In(ChartStyle.Waterfall)
                    ? seriesArray.First().Values.Select(v => v.CompositeValue.PlainValue)
                    : seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue));
                dir = Utils.GetDirection(totalValues, chartStyle);
            }
            else if (seriesArray.All(s => s.IsStockSeries()))
            {
                if (!chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return VerticalAlignment.Top;
                var totalValues = seriesArray.First().Values.Select(v => (v.CompositeValue.HighValue, v.CompositeValue.LowValue));
                dir = Utils.GetDirectionFinancial(totalValues);
            }
            else
                return VerticalAlignment.Top;

            return dir switch
            {
                Directions.SouthEast => VerticalAlignment.Bottom,
                Directions.NorthWest or Directions.NorthEast or Directions.NorthEastNorthWest or Directions.NorthEastSouthEast => VerticalAlignment.Top,
                _ => VerticalAlignment.Top,
            };
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
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
                 || values[2] is not ChartStyle chartStyle)
                return 5;

            var seriesEnumerable = values[0] as IEnumerable<ISeries>;
            var chartSeries = values[1] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return 5;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            Directions dir;

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return 5;
                var totalValues = chartStyle.In(ChartStyle.Waterfall)
                    ? seriesArray.First().Values.Select(v => v.CompositeValue.PlainValue)
                    : seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue));
                dir = Utils.GetDirection(totalValues, chartStyle);
            }
            else if (seriesArray.All(s => s.IsStockSeries()))
            {
                if (!chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return 5;
                var totalValues = seriesArray.First().Values.Select(v => (v.CompositeValue.HighValue, v.CompositeValue.LowValue));
                dir = Utils.GetDirectionFinancial(totalValues);
            }
            else
                return 5;

            return dir switch
            {
                Directions.SouthEast => 3,
                Directions.NorthWest or Directions.NorthEast or Directions.NorthEastNorthWest or Directions.NorthEastSouthEast => 5,
                _ => 5,
            };
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
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
                || values[2] is not ChartStyle chartStyle)
                return 2;

            var seriesEnumerable = values[0] as IEnumerable<ISeries>;
            var chartSeries = values[1] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return 2;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            Directions dir;

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return 2;
                var totalValues = chartStyle.In(ChartStyle.Waterfall)
                    ? seriesArray.First().Values.Select(v => v.CompositeValue.PlainValue)
                    : seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue));
                dir = Utils.GetDirection(totalValues, chartStyle);
            }
            else if (seriesArray.All(s => s.IsStockSeries()))
            {
                if (!chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return 2;
                var totalValues = seriesArray.First().Values.Select(v => (v.CompositeValue.HighValue, v.CompositeValue.LowValue));
                dir = Utils.GetDirectionFinancial(totalValues);
            }
            else
                return 2;

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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
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
                || values[2] is not ChartStyle chartStyle)
                return 5;

            var seriesEnumerable = values[0] as IEnumerable<ISeries>;
            var chartSeries = values[1] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return 5;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            Directions dir;

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return 5;
                var totalValues = chartStyle.In(ChartStyle.Waterfall)
                    ? seriesArray.First().Values.Select(v => v.CompositeValue.PlainValue)
                    : seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue));
                dir = Utils.GetDirection(totalValues, chartStyle);
            }
            else if (seriesArray.All(s => s.IsStockSeries()))
            {
                if (!chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return 5;
                var totalValues = seriesArray.First().Values.Select(v => (v.CompositeValue.HighValue, v.CompositeValue.LowValue));
                dir = Utils.GetDirectionFinancial(totalValues);
            }
            else
                return 5;

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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
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
                || values[2] is not ChartStyle chartStyle)
                return VerticalAlignment.Bottom;

            var seriesEnumerable = values[0] as IEnumerable<ISeries>;
            var chartSeries = values[1] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return VerticalAlignment.Bottom;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            Directions dir;

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return VerticalAlignment.Bottom;
                var totalValues = chartStyle.In(ChartStyle.Waterfall)
                    ? seriesArray.First().Values.Select(v => v.CompositeValue.PlainValue)
                    : seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue));
                dir = Utils.GetDirection(totalValues, chartStyle);
            }
            else if (seriesArray.All(s => s.IsStockSeries()))
            {
                if (!chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return VerticalAlignment.Bottom;
                var totalValues = seriesArray.First().Values.Select(v => (v.CompositeValue.HighValue, v.CompositeValue.LowValue));
                dir = Utils.GetDirectionFinancial(totalValues);
            }
            else
                return VerticalAlignment.Bottom;

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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
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
                || values[2] is not ChartStyle chartStyle)
                return HorizontalAlignment.Left;

            var seriesEnumerable = values[0] as IEnumerable<ISeries>;
            var chartSeries = values[1] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return HorizontalAlignment.Left;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            Directions dir;

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return HorizontalAlignment.Left;
                var totalValues = chartStyle.In(ChartStyle.Waterfall)
                    ? seriesArray.First().Values.Select(v => v.CompositeValue.PlainValue)
                    : seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue));
                dir = Utils.GetDirection(totalValues, chartStyle);
            }
            else if (seriesArray.All(s => s.IsStockSeries()))
            {
                if (!chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return HorizontalAlignment.Left;
                var totalValues = seriesArray.First().Values.Select(v => (v.CompositeValue.HighValue, v.CompositeValue.LowValue));
                dir = Utils.GetDirectionFinancial(totalValues);
            }
            else
                return HorizontalAlignment.Left;

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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
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
                || values[0] is not ChartStyle chartStyle
                || values[1] is not Brush brush)
                return null;
            return chartStyle switch
            {
                ChartStyle.Lines or ChartStyle.StackedLines or ChartStyle.FullStackedLines or ChartStyle.SmoothLines or ChartStyle.SmoothStackedLines or ChartStyle.SmoothFullStackedLines or ChartStyle.SmoothLinesWithMarkers or ChartStyle.SmoothStackedLinesWithMarkers or ChartStyle.SmoothFullStackedLinesWithMarkers or ChartStyle.LinesWithMarkers or ChartStyle.StackedLinesWithMarkers or ChartStyle.FullStackedLinesWithMarkers or ChartStyle.Radar or ChartStyle.RadarWithMarkers => null,
                _ => brush,
            };
        }
        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
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
                || values[0] is not ChartStyle chartStyle
                || values[1] is not Brush brush)
                return null;
            return chartStyle switch
            {
                ChartStyle.Area or ChartStyle.Bars or ChartStyle.StackedBars or ChartStyle.FullStackedBars or ChartStyle.Columns or ChartStyle.StackedColumns or ChartStyle.FullStackedColumns or ChartStyle.Bubbles or ChartStyle.Waterfall or ChartStyle.RadarArea or ChartStyle.SmoothArea or ChartStyle.Funnel => null,
                _ => brush,
            };
        }
        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
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
            var height = 8.0;

            if (values == null
                || values[1] is not ChartStyle chartStyle
                || chartStyle == ChartStyle.Funnel
                || values[2] is not string formatX
                || values[3] is not FontFamily fontFamily
                || values[4] is not double fontSize
                || values[5] is not FontStyle fontStyle
                || values[6] is not FontWeight fontWeight
                || values[7] is not FontStretch fontStretch
                || values[9] is not AutoAdjustmentMode autoAdjust
                || values[10] is not double maxX
                || values[12] is not int linesCountX
                || values[13] is not string formatY)
                return height;

            var seriesEnumerable = values[0] as IEnumerable<ISeries>;
            var chartSeries = values[14] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return height;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            var customValuesX = values[8] is IEnumerable<string> customEnumerableX ? customEnumerableX.ToArray() : Array.Empty<string>();
            var customValuesY = values[11] is IEnumerable<string> customEnumerableY ? customEnumerableY.ToArray() : Array.Empty<string>();

            Directions dir;
            int maxFromValues = 0, maxForBars = 0;

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return height;
                var totalValues = chartStyle != ChartStyle.Waterfall
                    ? seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue))
                    : seriesArray[0].Values.Select(v => v.CompositeValue.PlainValue);
                dir = Utils.GetDirection(totalValues, chartStyle);
                if (Utils.StyleBars(chartStyle))
                {
                    maxForBars = autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Horizontal) ? seriesArray.Max(s => s.Values.Count) : linesCountX;
                    if (dir.In(Directions.NorthWest, Directions.NorthEastNorthWest))
                        maxForBars *= -1;
                }
                else
                {
                    var maxV = chartStyle != ChartStyle.Waterfall ? seriesArray.Max(s => s.Values.Count).ToString(culture).Length : seriesArray[0].Values.Count.ToString(culture).Length;
                    maxFromValues = autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Horizontal) ? maxV : maxX.ToString(culture).Length;
                }
            }
            else if (seriesArray.All(s => s.IsStockSeries()))
            {
                if (!chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return height;

                if (!seriesArray[0].Values.Any())
                    return height;
                if (chartStyle == ChartStyle.HighLowClose && seriesArray[0] is not HighLowCloseSeries)
                    return height;
                else if (chartStyle == ChartStyle.OpenHighLowClose && seriesArray[0] is not OpenHighLowCloseSeries)
                    return height;

                var totalValues = seriesArray[0].Values.Select(v => (v.CompositeValue.HighValue, v.CompositeValue.LowValue));
                dir = Utils.GetDirectionFinancial(totalValues);
                maxFromValues = autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Vertical) ? seriesArray[0].Values.Count.ToString(culture).Length : maxX.ToString(culture).Length;
            }
            else
                return height;

            string maxString;
            if (!Utils.StyleBars(chartStyle))
            {
                maxString = customValuesX.Any()
                    ? customValuesX.FirstOrDefault(c => c.Length == customValuesX.Max(v => v.Length))
                    : new string('0', maxFromValues);
            }
            else
            {
                if (customValuesY.Any())
                {
                    var cv = customValuesY.FirstOrDefault(c => c.Length == customValuesY.Max(v => v.Length));
                    if (cv.Length > maxForBars.ToString(formatX, culture).Length)
                        maxString = cv;
                    else
                        maxString = maxForBars.ToString(formatX, culture);
                }
                else
                {
                    maxString = maxForBars.ToString(formatX, culture);
                }
            }


            var fmt = new FormattedText(maxString, culture, FlowDirection.LeftToRight,
                            new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip)
            {
                TextAlignment = TextAlignment.Right
            };
            var pt = new Point(0, 0);
            var ngm = fmt.BuildGeometry(pt);

            var trgr = new TransformGroup();
            trgr.Children.Add(new RotateTransform(-45, pt.X + fmt.Width, pt.Y));
            ngm.Transform = trgr;

            height = ngm.Bounds.Height;
            // add some extra space
            return height + 8;
        }
        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
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
                || values[1] is not ChartStyle chartStyle
                || values[2] is not string formatY
                || values[3] is not FontFamily fontFamily
                || values[4] is not double fontSize
                || values[5] is not FontStyle fontStyle
                || values[6] is not FontWeight fontWeight
                || values[7] is not FontStretch fontStretch
                || values[9] is not AutoAdjustmentMode autoAdjust
                || values[10] is not double maxY
                || values[11] is not double height
                || values[12] is not int linesCountY
                || values[14] is not string formatX)
                return 0.0;
            var width = 28.0;

            var seriesEnumerable = values[0] as IEnumerable<ISeries>;
            var chartSeries = values[15] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return 0.0;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            var customValuesY = values[8] is IEnumerable<string> customEnumerableY ? customEnumerableY.ToArray() : Array.Empty<string>();
            var customValuesX = values[13] is IEnumerable<string> customEnumerableX ? customEnumerableX.ToArray() : Array.Empty<string>();

            if (chartStyle.In(ChartStyle.Radar, ChartStyle.RadarWithMarkers, ChartStyle.RadarArea))
                return 0.0;

            Directions dir;
            int maxForBars = 0;

            if (seriesArray.All(s => s is PlainSeries))
            {
                if (chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return 0.0;
                var totalValues = chartStyle != ChartStyle.Waterfall
                    ? seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue))
                    : seriesArray[0].Values.Select(v => v.CompositeValue.PlainValue);
                dir = Utils.GetDirection(totalValues, chartStyle);
                if (Utils.StyleBars(chartStyle))
                {
                    maxForBars = autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Vertical)
                        ? seriesArray.Max(s => s.Values.Count)
                        : linesCountY;
                }
            }
            else if (seriesArray.All(s => s.IsStockSeries()))
            {
                if (!chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return 0.0;

                if (!seriesArray[0].Values.Any())
                    return 0.0;
                if (chartStyle == ChartStyle.HighLowClose && seriesArray[0] is not HighLowCloseSeries)
                    return 0.0;
                else if (chartStyle == ChartStyle.OpenHighLowClose && seriesArray[0] is not OpenHighLowCloseSeries)
                    return 0.0;

                var totalValues = seriesArray[0].Values.Select(v => (v.CompositeValue.HighValue, v.CompositeValue.LowValue));
                dir = Utils.GetDirectionFinancial(totalValues);
            }
            else
                return 0.0;

            string maxString, maxStringY, maxStringX;
            FormattedText fmtX = null;
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

            if (!Utils.StyleBars(chartStyle))
            {
                if (!Utils.StyleFullStacked(chartStyle))
                {
                    var fmtY = new FormattedText("AAA", culture, FlowDirection.LeftToRight,
                        new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);

                    var (max, min, _, stepSize, _, _, _) = autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Vertical)
                        ? Utils.GetMeasures(
                           chartStyle,
                           seriesArray,
                           linesCountY,
                           radius,
                           fmtY.Height,
                           centerPoint,
                           dir == Directions.NorthEastSouthEast)
                        : (maxY, -maxY, linesCountY, maxY / linesCountY, radius / linesCountY, radius / maxY, default);

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
                    maxStringY = customValuesY.Any()
                       ? customValuesY.FirstOrDefault(c => c.Length == customValuesY.Max(v => v.Length))
                       : max.ToString(culture).Length >= min.ToString(culture).Length ? max.ToString(formatY, culture) : min.ToString(formatY, culture);
                }
                else
                {
                    maxStringY = "100%";
                }

                // get first horiizontal values formatted text object 
                maxStringX = customValuesX.Any()
                    ? customValuesX.First()
                    : "0";
                fmtX = new FormattedText(maxStringX, culture, FlowDirection.LeftToRight,
                            new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip)
                {
                    TextAlignment = TextAlignment.Right
                };
                var pt = new Point(0, 0);
                var ngm = fmtX.BuildGeometry(pt);
                var trgr = new TransformGroup();
                trgr.Children.Add(new RotateTransform(-45, pt.X + fmtX.Width, pt.Y));
                ngm.Transform = trgr;

                maxString = maxStringY;
            }
            else
            {
                if (customValuesX.Any())
                {
                    var cv = customValuesX.FirstOrDefault(c => c.Length == customValuesX.Max(v => v.Length));
                    if (cv.Length > maxForBars.ToString(formatX, culture).Length)
                        maxString = cv;
                    else
                        maxString = maxForBars.ToString(formatX, culture);
                }
                else
                {
                    maxString = maxForBars.ToString(formatX, culture);
                }
            }

            var fmt = new FormattedText(maxString, culture, FlowDirection.LeftToRight,
                            new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip)
            {
                TextAlignment = TextAlignment.Right
            };
            if (fmtX != null)
                width = fmt.Width > fmtX.Width ? fmt.Width : fmtX.Width;
            else
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
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
                || values[2] is not ChartStyle chartStyle)
                return null;

            var seriesEnumerable = values[1] as IEnumerable<ISeries>;
            var chartSeries = values[16] as IEnumerable<ISeries>;

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
                if (!chartStyle.In(ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose))
                    return null;

                if (!seriesArray[0].Values.Any())
                    return null;
                if (chartStyle == ChartStyle.HighLowClose && seriesArray[0] is not HighLowCloseSeries)
                    return null;
                else if (chartStyle == ChartStyle.OpenHighLowClose && seriesArray[0] is not OpenHighLowCloseSeries)
                    return null;
            }
            else
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
                || values[0] is not double height
                || values[5] is not FontFamily fontFamily
                || values[6] is not double fontSize
                || values[7] is not FontStyle fontStyle
                || values[8] is not FontWeight fontWeight
                || values[9] is not FontStretch fontStretch
                || values[14] is not FlowDirection flowDir)
                return null;

            var gm = new PathGeometry();

            var seriesEnumerable = values[1] as IEnumerable<ISeries>;
            var chartSeries = values[16] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return null;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            var ticks = seriesArray[0].Values.Count;

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
                || values[0] is not double height
                || values[2] is not ChartStyle chartStyle
                || values[3] is not int linesCount
                || values[15] is not string formatX
                || values[5] is not FontFamily fontFamily
                || values[6] is not double fontSize
                || values[7] is not FontStyle fontStyle
                || values[8] is not FontWeight fontWeight
                || values[9] is not FontStretch fontStretch
                || values[12] is not AutoAdjustmentMode autoAdjust
                || values[14] is not FlowDirection flowDir)
                return null;

            var seriesEnumerable = values[1] as IEnumerable<ISeries>;
            var chartSeries = values[16] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return null;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            var customValuesX = values[10] is IEnumerable<string> customEnumerableX ? customEnumerableX.ToArray() : Array.Empty<string>();

            var gm = new PathGeometry();

            height -= Utils.AXIS_THICKNESS;

            var rawValues = Utils.GetPaddedSeries(seriesArray);

            var ticks = rawValues.Max(rw => rw.Values.Count);

            var totalValues = seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue));

            var dir = Utils.GetDirection(totalValues, chartStyle);

            if (!autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Vertical))
            {
                ticks = linesCount;
            }
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
                            var number = customValuesX.Length > i
                                ? customValuesX[i]
                                : formatX.EndsWith("%") ? num.ToString(formatX.Substring(0, formatX.Length - 1)) + "%" : num.ToString(formatX);

                            var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip)
                            {
                                TextAlignment = flowDir == FlowDirection.LeftToRight
                                    ? dir == Directions.NorthWest ? TextAlignment.Left : TextAlignment.Right
                                    : dir == Directions.NorthWest ? TextAlignment.Right : TextAlignment.Left
                            };

                            var pt = new Point(0, y - (yStep + fmt.Height) / 2);
                            var ngm = fmt.BuildGeometry(pt);

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
                || values[0] is not double height
                || values[2] is not ChartStyle chartStyle
                || values[3] is not int linesCountY
                || values[4] is not string formatY
                || values[5] is not FontFamily fontFamily
                || values[6] is not double fontSize
                || values[7] is not FontStyle fontStyle
                || values[8] is not FontWeight fontWeight
                || values[9] is not FontStretch fontStretch
                || values[12] is not AutoAdjustmentMode autoAdjust
                || values[13] is not double maxY
                || values[14] is not FlowDirection flowDir)
                return null;

            var gm = new PathGeometry();

            var seriesEnumerable = values[1] as IEnumerable<ISeries>;
            var chartSeries = values[16] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return null;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            var customValuesY = values[11] is IEnumerable<string> customEnumerableY ? customEnumerableY.ToArray() : Array.Empty<string>();
            var drawBetween = chartStyle.In(ChartStyle.Bars, ChartStyle.StackedBars, ChartStyle.FullStackedBars);

            double step, offset;

            Directions dir;

            if (seriesArray.All(s => s is PlainSeries))
            {
                var totalValues = chartStyle != ChartStyle.Waterfall
                    ? seriesArray.SelectMany(s => s.Values.Select(v => v.CompositeValue.PlainValue))
                    : seriesArray[0].Values.Select(v => v.CompositeValue.PlainValue);
                dir = Utils.GetDirection(totalValues, chartStyle);
            }
            else if (seriesArray.All(s => s.IsStockSeries()))
            {
                var totalValues = seriesArray[0].Values.Select(v => (v.CompositeValue.HighValue, v.CompositeValue.LowValue));
                dir = Utils.GetDirectionFinancial(totalValues);
            }
            else
                return null;

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

            var (max, min, realLinesCount, stepSize, stepLength, units, _) = autoAdjust.In(AutoAdjustmentMode.Both, AutoAdjustmentMode.Vertical)
                ? Utils.GetMeasures(
                   chartStyle,
                   seriesArray,
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
                if (formatY.EndsWith("%"))
                    formatY = $"{formatY.Substring(0, formatY.Length - 1)}{culture.NumberFormat.NumberDecimalSeparator}{new string('0', fractions)}%";
                else
                {
                    var (numericPart, literalPart) = Utils.GetFormatParts(formatY, culture);
                    formatY = $"0{culture.NumberFormat.NumberDecimalSeparator}{new string('0', fractions)}" + literalPart;
                }
            }

            switch (dir)
            {
                case Directions.NorthEastSouthEast:
                    step = stepLength;
                    var limit = linesCountY * 2;
                    for (int i = 0, index = 0; i <= limit; i++)
                    {
                        if (i == linesCountY) continue;
                        var num = !Utils.StyleFullStacked(chartStyle)
                            ? (linesCountY - i) * maxMin / linesCountY
                            : (linesCountY - i) * 10;
                        var number = Utils.StyleFullStacked(chartStyle)
                            ? num.ToString(culture)
                            : customValuesY.Length > index
                                ? customValuesY[index++]
                                : formatY.EndsWith("%")
                                    ? num.ToString(formatY.Substring(0, formatY.Length - 1)) + "%"
                                    : num.ToString(formatY);
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
                        var number = Utils.StyleFullStacked(chartStyle) ? num.ToString(culture) : customValuesY.Length > index
                            ? customValuesY[index++]
                            : formatY.EndsWith("%") ? num.ToString(formatY.Substring(0, formatY.Length - 1)) + "%" : num.ToString(formatY);
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
                            : (linesCountY - i) * 10;
                        var number = Utils.StyleFullStacked(chartStyle) ? num.ToString(culture) : customValuesY.Length > index
                            ? customValuesY[index++]
                            : formatY.EndsWith("%") ? num.ToString(formatY.Substring(0, formatY.Length - 1)) + "%" : num.ToString(formatY);
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
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
                || values[0] is not double width
                || values[1] is not double height)
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
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
                || values[0] is not double width
                || values[1] is not double height)
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
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
                || values[0] is not IEnumerable<IChartValue> chartValues
                || values[1] is not string format
                || parameter is not IChartValue chartValue)
                return null;
            var sum = chartValues.Sum(p => Math.Abs(p.CompositeValue.PlainValue));
            if (format.EndsWith("%")) format = format.Substring(0, format.Length - 1);
            var perc = Math.Abs(chartValue.CompositeValue.PlainValue) / (sum != 0 ? sum : 1) * 100;
            var sectorData = $"{chartValue.CompositeValue.PlainValue.ToString(culture)} ({perc.ToString(format, culture)}%)";
            if (!string.IsNullOrEmpty(chartValue.CustomValue))
                sectorData += $" {chartValue.CustomValue}";
            return sectorData;
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    /// <summary>
    /// Draws radar lines
    /// </summary>
    public class RadarLinesPathConverter : IMultiValueConverter
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
                || values[10] is not int linesCount
                || values[11] is not AutoAdjustmentMode autoAdjust)
                return null;

            var gm = new PathGeometry();

            var seriesEnumerable = values[2] as IEnumerable<ISeries>;
            var chartSeries = values[12] as IEnumerable<ISeries>;

            if ((seriesEnumerable == null || !seriesEnumerable.Any()) && (chartSeries == null || !chartSeries.Any()))
                return null;

            var seriesArray = seriesEnumerable != null && seriesEnumerable.Any() ? seriesEnumerable.ToArray() : chartSeries.ToArray();

            var customValues = values[9] is IEnumerable<string> customEnumerable ? customEnumerable.ToArray() : Array.Empty<string>();

            var currentDegrees = 0.0;
            var pointsCount = seriesArray.Max(s => s.Values.Count);
            var degreesStep = 360.0 / pointsCount;

            var maxCv = customValues.Any() ? customValues.Max(v => (v, v.Length)) : (v: "", Length: 0);
            var number = maxCv.Length > pointsCount.ToString(culture).Length ? maxCv.v : pointsCount.ToString(culture);
            var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);

            var radius = width > height ? (height - 2 * Utils.AXIS_THICKNESS) / 2 : (width - 2 * Utils.AXIS_THICKNESS) / 2;
            var centerPoint = width > height ? new Point((width - 2 * Utils.AXIS_THICKNESS) / 2, radius) : new Point(radius, (height - 2 * Utils.AXIS_THICKNESS) / 2);
            if (width > height)
                radius -= (2 * fmt.Height + 8);
            else
                radius -= (2 * fmt.Width + 8);
            var xBeg = radius;
            var yBeg = 90.0;

            var fmtY = new FormattedText("AAA", culture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, Brushes.Black, VisualTreeHelper.GetDpi(Utils.Border).PixelsPerDip);
            var (max, min, realLinesCount, stepSize, _, _, _) = Utils.GetMeasures(chartStyle, seriesArray, linesCount, radius, fmtY.Height, centerPoint);
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
        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
    }

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
            var pts = series.Values.ToArray();
            var sum = pts.Sum(p => Math.Abs(p.CompositeValue.PlainValue));
            var currentDegrees = 90.0;
            var startPoint = new Point(radius, radius);
            var xBeg = radius;
            var yBeg = 0.0;

            var dGroup = new DrawingGroup();

            if (format.EndsWith("%")) format = format.Substring(0, format.Length - 1);
            if (pts.Length == 1)
            {
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
            foreach (var pt in pts)
            {
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

                if (brushIndex == Statics.PredefinedMainBrushes.Length) brushIndex = 0;
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
