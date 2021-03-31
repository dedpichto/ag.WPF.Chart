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
    public class ChartValues : ObservableCollection<ChartValue>
    {
        internal Path Path { get; set; }
    }

    /// <summary>
    /// Represents single chart value
    /// </summary>
    public class ChartValue
    {
        /// <summary>
        /// Gets or sets current numeric value
        /// </summary>
        public (double V1, double V2, double V3, double V4, double V5) Value { get; set; }
        /// <summary>
        /// Gets or sets custom value (usually string) associated with current value. This custom value will be displayed as chart point tooltip
        /// </summary>
        public object CustomValue { get; set; }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="V1">Current value</param>
        public ChartValue(double V1)
        {
            Value = (V1, 0, 0, 0, 0);
        }
        public ChartValue(double V1, double V2)
        {
            Value = (V1, V2, 0, 0, 0);
        }
        public ChartValue(double V1, double V2, double V3)
        {
            Value = (V1, V2, V3, 0, 0);
        }
        public ChartValue(double V1, double V2, double V3, double V4)
        {
            Value = (V1, V2, V3, V4, 0);
        }
        public ChartValue(double V1, double V2, double V3, double V4, double V5)
        {
            Value = (V1, V2, V3, V4, V5);
        }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="value">Current value</param>
        /// <param name="customValue">Current custom value</param>
        public ChartValue(double value, object customValue)
            : this(value)
        {
            CustomValue = customValue;
        }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="values">Current values</param>
        /// <param name="customValue">Current custom value</param>
        public ChartValue((double V1, double V2) values, object customValue)
            : this(values.V1, values.V2)
        {
            CustomValue = customValue;
        }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="values">Current values</param>
        /// <param name="customValue">Current custom value</param>
        public ChartValue((double V1, double V2, double V3) values, object customValue)
            : this(values.V1, values.V2, values.V3)
        {
            CustomValue = customValue;
        }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="values">Current values</param>
        /// <param name="customValue">Current custom value</param>
        public ChartValue((double V1, double V2, double V3, double V4) values, object customValue)
            : this(values.V1, values.V2, values.V3, values.V4)
        {
            CustomValue = customValue;
        }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="values">Current values</param>
        /// <param name="customValue">Current custom value</param>
        public ChartValue((double V1, double V2, double V3, double V4, double V5) values, object customValue)
            : this(values.V1, values.V2, values.V3, values.V4, values.V5)
        {
            CustomValue = customValue;
        }
    }
}
