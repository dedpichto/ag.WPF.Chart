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
#nullable restore
}
