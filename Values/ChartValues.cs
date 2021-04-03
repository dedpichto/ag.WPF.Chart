using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace ag.WPF.Chart.Values
{
    /// <summary>
    /// Represents collection of <see cref="ChartValue"/>
    /// </summary>
    public class ChartValues : ObservableCollection<IChartValue>
    {
        internal Path Path { get; set; }
    }

    /// <summary>
    /// Represents single chart value
    /// </summary>
    public class ChartValue : IChartValue
    {
        /// <summary>
        /// Gets or sets current numeric value
        /// </summary>
        public (double PlainValue, double HighValue, double LowValue, double CloseValue, double VolumeValue, double OpenValue) Value { get; set; }
        /// <summary>
        /// Gets or sets custom value (usually string) associated with current value. This custom value will be displayed as chart point tooltip
        /// </summary>
        public object CustomValue { get; set; }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="plainValue">Current value</param>
        public ChartValue(double plainValue)
        {
            Value = (plainValue, 0, 0, 0, 0, 0);
        }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="highValue">HighValue</param>
        /// <param name="lowValue">LowValue</param>
        /// <param name="closeValue">CloseValue</param>
        public ChartValue(double highValue, double lowValue, double closeValue)
        {
            Value = (0, highValue, lowValue, closeValue, 0, 0);
        }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="volumeValue">VolumeValue</param>
        /// <param name="highValue">HighValue</param>
        /// <param name="lowValue">LowValue</param>
        /// <param name="closeValue">CloseValue</param>
        public ChartValue(double volumeValue, double highValue, double lowValue, double closeValue)
        {
            Value = (0, highValue, lowValue, closeValue, volumeValue, 0);
        }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="volumeValue">VolumeValue</param>
        /// <param name="openValue">OpenValue</param>
        /// <param name="highValue">HighValue</param>
        /// <param name="lowValue">LowValue</param>
        /// <param name="closeValue">CloseValue</param>
        public ChartValue(double volumeValue, double openValue, double highValue, double lowValue, double closeValue)
        {
            Value = (0, highValue, lowValue, closeValue, volumeValue, openValue);
        }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="plainValue">PlainValue</param>
        /// <param name="volumeValue">VolumeValue</param>
        /// <param name="openValue">OpenValue</param>
        /// <param name="highValue">HighValue</param>
        /// <param name="lowValue">LowValue</param>
        /// <param name="closeValue">CloseValue</param>
        public ChartValue(double plainValue, double volumeValue, double openValue, double highValue, double lowValue, double closeValue)
        {
            Value = (plainValue, highValue, lowValue, closeValue, volumeValue, openValue);
        }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="plainValue">Current value</param>
        /// <param name="customValue">Current custom value</param>
        public ChartValue(double plainValue, object customValue)
            : this(plainValue)
        {
            CustomValue = customValue;
        }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="values">Current values</param>
        /// <param name="customValue">Current custom value</param>
        public ChartValue((double HighValue, double LowValue, double CloseValue) values, object customValue)
            : this(values.HighValue, values.LowValue, values.CloseValue)
        {
            CustomValue = customValue;
        }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="values">Current values</param>
        /// <param name="customValue">Current custom value</param>
        public ChartValue((double VolumeValue, double HighValue, double LowValue, double CloseValue) values, object customValue)
            : this(values.VolumeValue, values.HighValue, values.LowValue, values.CloseValue)
        {
            CustomValue = customValue;
        }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="values">Current values</param>
        /// <param name="customValue">Current custom value</param>
        public ChartValue((double VolumeValue, double OpenValue, double HighValue, double LowValue, double CloseValue) values, object customValue)
            : this(values.VolumeValue, values.OpenValue, values.HighValue, values.LowValue, values.CloseValue)
        {
            CustomValue = customValue;
        }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="values">Current values</param>
        /// <param name="customValue">Current custom value</param>
        public ChartValue((double PlainValue, double VolumeValue, double OpenValue, double HighValue, double LowValue, double CloseValue) values, object customValue)
            : this(values.PlainValue, values.VolumeValue, values.OpenValue, values.HighValue, values.LowValue, values.CloseValue)
        {
            CustomValue = customValue;
        }
    }
}
