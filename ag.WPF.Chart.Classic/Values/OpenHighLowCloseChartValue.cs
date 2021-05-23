using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ag.WPF.Chart.Values
{
    /// <summary>
    /// Represents stock value object (with open, high, low, and close values).
    /// </summary>
    public class OpenHighLowCloseChartValue : ChartValue
    {
        #region ctor
        /// <summary>
        /// Initializes a new instance of OpenHighLowCloseChartValue object
        /// </summary>
        public OpenHighLowCloseChartValue()
        {
            CompositeValue = new ValuesStruct();
        }
        /// <summary>
        /// Initializes a new instance of OpenHighLowCloseChartValue object
        /// </summary>
        /// <param name="openValue">Volume or open value</param>
        /// <param name="highValue">High value</param>
        /// <param name="lowValue">Low value</param>
        /// <param name="closeValue">Close value</param>
        public OpenHighLowCloseChartValue(double openValue, double highValue, double lowValue, double closeValue)
        {
            CompositeValue = new ValuesStruct();
            CompositeValue.OpenValue = openValue;
            CompositeValue.HighValue = highValue;
            CompositeValue.LowValue = lowValue;
            CompositeValue.CloseValue = closeValue;
        }
        #endregion

        #region Abstraction metods overrides
        /// <inheritdoc />
        public override IChartValue Clone()
        {
            return (OpenHighLowCloseChartValue)MemberwiseClone();
        }
        #endregion
    }
}
