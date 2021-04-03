namespace ag.WPF.Chart.Values
{
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
        /// Creates copy of current object
        /// </summary>
        /// <returns></returns>
        IChartValue Clone();
    }
}