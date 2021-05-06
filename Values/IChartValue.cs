namespace ag.WPF.Chart.Values
{
    /// <summary>
    /// Represents IChartValue interface
    /// </summary>
    public interface IChartValue
    {
        /// <summary>
        /// Gets or sets custom value to be displayed as chart point tooltip
        /// </summary>
        string CustomValue { get; set; }
        /// <summary>
        /// Gets or sets current numeric value
        /// </summary>
        (double PlainValue, double HighValue, double LowValue, double CloseValue, double VolumeValue, double OpenValue) Value { get; set; }
        /// <summary>
        /// Creates copy of current <see cref="IChartValue"/> object
        /// </summary>
        /// <returns>A copy of current <see cref="IChartValue"/> object</returns>
        IChartValue Clone();
    }
}