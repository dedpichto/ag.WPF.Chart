using System.ComponentModel;

namespace ag.WPF.Chart.Values
{
    /// <summary>
    /// Represents IChartValue interface
    /// </summary>
    public interface IChartValue : INotifyPropertyChanged
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
        /// <summary>
        /// Gets or sets value visibility.
        /// </summary>
        /// <remarks>This property is affected only <see cref="ChartStyle.SolidPie"/>,<see cref="ChartStyle.SlicedPie"/>,<see cref="ChartStyle.Doughnut"/> styles.</remarks>
        bool IsVisible { get; set; }
    }
}