﻿namespace ag.WPF.Chart.Values
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
            CompositeValue = new ChartCompositeValue();
        }
        /// <summary>
        /// Initializes a new instance of PlainChartValue object, using given numeric value
        /// </summary>
        /// <param name="plainValue">Current value</param>
        public PlainChartValue(double plainValue)
        {
            CompositeValue = new ChartCompositeValue
            {
                PlainValue = plainValue
            };
        }

        /// <summary>
        /// Initializes a new instance of PlainChartValue object, using given numeric value and custom value
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
        public override IChartValue Clone() => (PlainChartValue)MemberwiseClone();
        #endregion
    }
}
