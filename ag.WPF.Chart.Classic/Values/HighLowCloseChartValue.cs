using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ag.WPF.Chart.Values
{
    /// <summary>
    /// Represents stock value object (with high, low, and close values).
    /// </summary>
    public class HighLowCloseChartValue : ChartValue
    {
        #region ctor
        /// <summary>
        /// Initializes a new instance of HighLowCloseChartValue object
        /// </summary>
        public HighLowCloseChartValue()
        {
            CompositeValue = new ChartCompositeValue();
        }
        /// <summary>
        /// Initializes a new instance of HighLowCloseChartValue object
        /// </summary>
        /// <param name="highValue">High value</param>
        /// <param name="lowValue">Low value</param>
        /// <param name="closeValue">Close value</param>
        public HighLowCloseChartValue(double highValue, double lowValue, double closeValue)
        {
            CompositeValue = new ChartCompositeValue
            {
                HighValue = highValue,
                LowValue = lowValue,
                CloseValue = closeValue
            };
        }
        #endregion

        #region Abstraction metods overrides
        /// <inheritdoc />
        public override IChartValue Clone()
        {
            return (HighLowCloseChartValue)MemberwiseClone();
        }
        #endregion
    }
}
