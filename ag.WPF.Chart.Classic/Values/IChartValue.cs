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
        /// Gets or sets current numeric values
        /// </summary>
        IChartCompositeValue CompositeValue { get; set; }
        /// <summary>
        /// Creates copy of current <see cref="IChartValue"/> object
        /// </summary>
        /// <returns>A copy of current <see cref="IChartValue"/> object</returns>
        IChartValue Clone();
    }
}