using ag.WPF.Chart.Values;
using System.Collections.Generic;
using System.Windows.Media;

namespace ag.WPF.Chart.Series
{
    /// <summary>
    /// Represents stock series object with high, low, and close values for each series point.
    /// </summary>
    public class HighLowCloseSeries : Series
    {
        #region ctor
        /// <summary>
        /// Initializes a new instance of StockSeries object using specified name and sequence of values
        /// </summary>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public HighLowCloseSeries(string name, IEnumerable<(double highValue, double lowValue, double closeValue)> values)
        {
            foreach (var (highValue, lowValue, closeValue) in values)
            {
                Values.Add(new HighLowCloseChartValue(highValue, lowValue, closeValue));
            }

            InitFields(name);
        }


        /// <summary>
        /// Initializes a new instance of StockSeries object using specified brush, name and sequence of values
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public HighLowCloseSeries(Brush mainBrush, string name, IEnumerable<(double highValue, double lowValue, double closeValue)> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified brush, secondary brush, name and sequence of values
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="secondaryBrush">Series secondary background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public HighLowCloseSeries(Brush mainBrush, Brush secondaryBrush, string name, IEnumerable<(double highValue, double lowValue, double closeValue)> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
            SecondaryBrush = secondaryBrush;
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified name and sequence of StockChartValue objects
        /// </summary>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public HighLowCloseSeries(string name, IEnumerable<IChartValue> values)
        {
            foreach (var v in values)
            {
                Values.Add(v.Clone());
            }
            InitFields(name);
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified brush, name and sequence of StockChartValue objects
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public HighLowCloseSeries(Brush mainBrush, string name, IEnumerable<IChartValue> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified brush, secondary brush, name and sequence of StockChartValue objects
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="secondaryBrush">Series secondary background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public HighLowCloseSeries(Brush mainBrush, Brush secondaryBrush, string name, IEnumerable<IChartValue> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
            SecondaryBrush = secondaryBrush;
        }
        #endregion
    }
}
