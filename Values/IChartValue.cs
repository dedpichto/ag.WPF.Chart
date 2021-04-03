namespace ag.WPF.Chart.Values
{
    public interface IChartValue
    {
        object CustomValue { get; set; }
        (double PlainValue, double HighValue, double LowValue, double CloseValue, double VolumeValue, double OpenValue) Value { get; set; }
    }
}