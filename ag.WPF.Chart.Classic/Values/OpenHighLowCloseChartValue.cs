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
            CompositeValue = new ChartCompositeValue();
        }
        /// <summary>
        /// Initializes a new instance of OpenHighLowCloseChartValue object, using given open, high, low and close values
        /// </summary>
        /// <param name="openValue">Volume or open value</param>
        /// <param name="highValue">High value</param>
        /// <param name="lowValue">Low value</param>
        /// <param name="closeValue">Close value</param>
        public OpenHighLowCloseChartValue(double openValue, double highValue, double lowValue, double closeValue)
        {
            CompositeValue = new ChartCompositeValue
            {
                OpenValue = openValue,
                HighValue = highValue,
                LowValue = lowValue,
                CloseValue = closeValue
            };
        }
        #endregion

        #region Abstraction metods overrides
        /// <inheritdoc />
        public override IChartValue Clone() => (OpenHighLowCloseChartValue)MemberwiseClone();
        #endregion
    }
}
