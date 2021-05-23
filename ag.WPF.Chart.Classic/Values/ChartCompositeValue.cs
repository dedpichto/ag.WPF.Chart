using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ag.WPF.Chart.Values
{
    /// <summary>
    /// Represnts values for various chart types
    /// </summary>
    public class ChartCompositeValue : DependencyObject, IChartCompositeValue
    {
        /// <summary>
        /// The identifier of the <see cref="PlainValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlainValueProperty = DependencyProperty.Register(nameof(PlainValue), typeof(double), typeof(ChartCompositeValue));
        /// <summary>
        /// The identifier of the <see cref="OpenValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OpenValueProperty = DependencyProperty.Register(nameof(OpenValue), typeof(double), typeof(ChartCompositeValue));
        /// <summary>
        /// The identifier of the <see cref="CloseValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CloseValueProperty = DependencyProperty.Register(nameof(CloseValue), typeof(double), typeof(ChartCompositeValue));
        /// <summary>
        /// The identifier of the <see cref="HighValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HighValueProperty = DependencyProperty.Register(nameof(HighValue), typeof(double), typeof(ChartCompositeValue));
        /// <summary>
        /// The identifier of the <see cref="LowValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LowValueProperty = DependencyProperty.Register(nameof(LowValue), typeof(double), typeof(ChartCompositeValue));
        /// <summary>
        /// The identifier of the <see cref="VolumeValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VolumeValueProperty = DependencyProperty.Register(nameof(VolumeValue), typeof(double), typeof(ChartCompositeValue));

        /// <inheritdoc />
        public double PlainValue 
        {
            get { return (double)GetValue(PlainValueProperty); }
            set { SetValue(PlainValueProperty, value); }
        }
        /// <inheritdoc />
        public double OpenValue
        {
            get { return (double)GetValue(OpenValueProperty); }
            set { SetValue(OpenValueProperty, value); }
        }
        /// <inheritdoc />
        public double CloseValue
        {
            get { return (double)GetValue(CloseValueProperty); }
            set { SetValue(CloseValueProperty, value); }
        }
        /// <inheritdoc />
        public double HighValue
        {
            get { return (double)GetValue(HighValueProperty); }
            set { SetValue(HighValueProperty, value); }
        }
        /// <inheritdoc />
        public double LowValue
        {
            get { return (double)GetValue(LowValueProperty); }
            set { SetValue(LowValueProperty, value); }
        }
        /// <inheritdoc />
        public double VolumeValue
        {
            get { return (double)GetValue(VolumeValueProperty); }
            set { SetValue(VolumeValueProperty, value); }
        }
    }
}
