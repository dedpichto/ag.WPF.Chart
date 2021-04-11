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
            StockType = StockType.HLC;
        }

        /// <summary>
        /// Initializes a new instance of StockChartValue object
        /// </summary>
        /// <param name="volumeOrOpenValue">Volume or open value</param>
        /// <param name="highValue">High value</param>
        /// <param name="lowValue">Low value</param>
        /// <param name="closeValue">Close value</param>
        /// <param name="volumeOrOpen">Boolen, specifies whether Volume or Open value should be set. Send True for Volume and False for Open</param>
        public StockChartValue(double volumeOrOpenValue, double highValue, double lowValue, double closeValue, bool volumeOrOpen)
        {
            if (volumeOrOpen)
            {
                Value = (0, highValue, lowValue, closeValue, volumeOrOpenValue, 0);
                StockType = StockType.VHLC;
            }
            else
            {
                Value = (0, highValue, lowValue, closeValue, 0, volumeOrOpenValue);
                StockType = StockType.OHLC;
            }
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
            StockType = StockType.VOHLC;
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
