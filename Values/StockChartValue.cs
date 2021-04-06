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
        /// <param name="highValue">High value</param>
        /// <param name="lowValue">Low value</param>
        /// <param name="closeValue">Close value</param>
        public StockChartValue(double highValue, double lowValue, double closeValue)
        {
            Value = (0, highValue, lowValue, closeValue, 0, 0);
        }

        /// <summary>
        /// Initializes a new instance of StockChartValue object
        /// </summary>
        /// <param name="volumeValue">Volume value</param>
        /// <param name="highValue">High value</param>
        /// <param name="lowValue">Low value</param>
        /// <param name="closeValue">Close value</param>
        public StockChartValue(double volumeValue, double highValue, double lowValue, double closeValue)
        {
            Value = (0, highValue, lowValue, closeValue, volumeValue, 0);
        }

        /// <summary>
        /// Initializes a new instance of StockChartValue object
        /// </summary>
        /// <param name="volumeValue">Volume value</param>
        /// <param name="openValue">Open value</param>
        /// <param name="highValue">High value</param>
        /// <param name="lowValue">Low value</param>
        /// <param name="closeValue">Close value</param>
        public StockChartValue(double volumeValue, double openValue, double highValue, double lowValue, double closeValue)
        {
            Value = (0, highValue, lowValue, closeValue, volumeValue, openValue);
        }
        #endregion

        #region Abstraction metods overrides
        public override IChartValue Clone()
        {
            return (StockChartValue)MemberwiseClone();
        }
        #endregion
    }
}
