using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ag.WPF.Chart.Values
{
    public class OpenHighLowCloseChartValue : ChartValue
    {
        #region ctor
        /// <summary>
        /// Initializes a new instance of StockChartValue object
        /// </summary>
        /// <param name="volumeOrOpenValue">Volume or open value</param>
        /// <param name="highValue">High value</param>
        /// <param name="lowValue">Low value</param>
        /// <param name="closeValue">Close value</param>
        public OpenHighLowCloseChartValue(double volumeOrOpenValue, double highValue, double lowValue, double closeValue)
        {
            Value = (0, highValue, lowValue, closeValue, 0, volumeOrOpenValue);
        }
        #endregion

        #region Abstraction metods overrides
        public override IChartValue Clone()
        {
            return (OpenHighLowCloseChartValue)MemberwiseClone();
        }
        #endregion
    }
}
