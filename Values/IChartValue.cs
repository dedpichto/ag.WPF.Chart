namespace ag.WPF.Chart.Values
{
    public interface IChartValue
    {
        /// <summary>
        /// Gets or sets custom value (usually string) associated with current value. This custom value will be displayed as chart point tooltip
        /// </summary>
        object CustomValue { get; set; }
        /// <summary>
        /// Gets or sets current numeric value
        /// </summary>
        (double PlainValue, double HighValue, double LowValue, double CloseValue, double VolumeValue, double OpenValue) Value { get; set; }
    }
}