using ag.WPF.Chart.Values;
using System.Collections.Generic;
using System.Windows.Media;

namespace ag.WPF.Chart.Series
{
    /// <summary>
    /// Represents stock series object with open, high, low, and close values for each series point.
    /// </summary>
    public class OpenHighLowCloseSeries : Series
    {
        #region ctor
        /// <summary>
        /// Initializes a new instance of StockSeries object using specified name and sequence of values
        /// </summary>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public OpenHighLowCloseSeries(string name, IEnumerable<(double openValue, double highValue, double lowValue, double closeValue)> values)
        {
            foreach (var (openValue, highValue, lowValue, closeValue) in values)
            {
                Values.Add(new OpenHighLowCloseChartValue(openValue, highValue, lowValue, closeValue));
            }

            InitFields(name);
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified brush, name and sequence of values
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public OpenHighLowCloseSeries(Brush mainBrush, string name, IEnumerable<(double openValue, double highValue, double lowValue, double closeValue)> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified brush, name and sequence of values
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="secondaryBrush">Series secondary background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public OpenHighLowCloseSeries(Brush mainBrush, Brush secondaryBrush, string name, IEnumerable<(double openValue, double highValue, double lowValue, double closeValue)> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
            SecondaryBrush = secondaryBrush;
        }
        #endregion
    }
}
