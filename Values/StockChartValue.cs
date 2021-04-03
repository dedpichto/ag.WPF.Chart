using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ag.WPF.Chart.Values
{
    public class StockChartValue : ChartValue
    {
        #region ctor
        /// <summary>
        /// Initializes a new instance of StockChartValue object
        /// </summary>
        /// <param name="highValue">HighValue</param>
        /// <param name="lowValue">LowValue</param>
        /// <param name="closeValue">CloseValue</param>
        public StockChartValue(double highValue, double lowValue, double closeValue)
        {
            Value = (0, highValue, lowValue, closeValue, 0, 0);
        }

        /// <summary>
        /// Initializes a new instance of StockChartValue object
        /// </summary>
        /// <param name="volumeValue">VolumeValue</param>
        /// <param name="highValue">HighValue</param>
        /// <param name="lowValue">LowValue</param>
        /// <param name="closeValue">CloseValue</param>
        public StockChartValue(double volumeValue, double highValue, double lowValue, double closeValue)
        {
            Value = (0, highValue, lowValue, closeValue, volumeValue, 0);
        }

        /// <summary>
        /// Initializes a new instance of StockChartValue object
        /// </summary>
        /// <param name="volumeValue">VolumeValue</param>
        /// <param name="openValue">OpenValue</param>
        /// <param name="highValue">HighValue</param>
        /// <param name="lowValue">LowValue</param>
        /// <param name="closeValue">CloseValue</param>
        public StockChartValue(double volumeValue, double openValue, double highValue, double lowValue, double closeValue)
        {
            Value = (0, highValue, lowValue, closeValue, volumeValue, openValue);
        }

        /// <summary>
        /// Initializes a new instance of StockChartValue object
        /// </summary>
        /// <param name="values">Current values</param>
        /// <param name="customValue">Current custom value</param>
        public StockChartValue((double HighValue, double LowValue, double CloseValue) values, object customValue)
            : this(values.HighValue, values.LowValue, values.CloseValue)
        {
            CustomValue = customValue;
        }

        /// <summary>
        /// Initializes a new instance of StockChartValue object
        /// </summary>
        /// <param name="values">Current values</param>
        /// <param name="customValue">Current custom value</param>
        public StockChartValue((double VolumeValue, double HighValue, double LowValue, double CloseValue) values, object customValue)
            : this(values.VolumeValue, values.HighValue, values.LowValue, values.CloseValue)
        {
            CustomValue = customValue;
        }

        /// <summary>
        /// Initializes a new instance of StockChartValue object
        /// </summary>
        /// <param name="values">Current values</param>
        /// <param name="customValue">Current custom value</param>
        public StockChartValue((double VolumeValue, double OpenValue, double HighValue, double LowValue, double CloseValue) values, object customValue)
            : this(values.VolumeValue, values.OpenValue, values.HighValue, values.LowValue, values.CloseValue)
        {
            CustomValue = customValue;
        } 
        #endregion
    }
}
