namespace ag.WPF.Chart.Values
{
    /// <summary>
    /// Represents IChartCompositeValue interface
    /// </summary>
    public interface IChartCompositeValue
    {
        /// <summary>
        /// Gets or sets PlainValue
        /// </summary>
        double PlainValue { get; set; }
        /// <summary>
        /// Gets or sets OpenValue
        /// </summary>
        double OpenValue { get; set; }
        /// <summary>
        /// Gets or sets CloseValue
        /// </summary>
        double CloseValue { get; set; }
        /// <summary>
        /// Gets or sets HighValue
        /// </summary>
        double HighValue { get; set; }
        /// <summary>
        /// Gets or sets LowValue
        /// </summary>
        double LowValue { get; set; }
        /// <summary>
        /// Gets or sets VolumeValue
        /// </summary>
        double VolumeValue { get; set; }
    }
}