using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ag.WPF.Chart.Values
{
    /// <summary>
    /// Represents plain series value object
    /// </summary>
    public class PlainChartValue : ChartValue
    {
        #region ctor
        /// <summary>
        /// Initializes a new instance of PlainChartValue object
        /// </summary>
        public PlainChartValue()
        {
            CompositeValue = new ValuesStruct();
        }
        /// <summary>
        /// Initializes a new instance of PlainChartValue object
        /// </summary>
        /// <param name="plainValue">Current value</param>
        public PlainChartValue(double plainValue)
        {
            CompositeValue = new ValuesStruct();
            CompositeValue.PlainValue = plainValue;
        }

        /// <summary>
        /// Initializes a new instance of PlainChartValue object
        /// </summary>
        /// <param name="plainValue">Current value</param>
        /// <param name="customValue">Current custom value</param>
        public PlainChartValue(double plainValue, string customValue)
            : this(plainValue)
        {
            CustomValue = customValue;
        }
        #endregion

        #region Abstraction metods overrides
        /// <inheritdoc />
        public override IChartValue Clone()
        {
            return (PlainChartValue)MemberwiseClone();
        } 
        #endregion
    }
}
