using System.Windows;

namespace ag.WPF.Chart.Values
{
    /// <summary>
    /// Represents the basic abstract class of chart value
    /// </summary>
    public abstract class ChartValue : DependencyObject, IChartValue
    {
        #region Dependency properties
        /// <summary>
        /// The identifier of the <see cref="CompositeValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CompositeValueProperty = DependencyProperty.Register(nameof(CompositeValue), typeof(IChartCompositeValue), typeof(ChartValue), new FrameworkPropertyMetadata(null));
        #endregion

        #region Public properties
        /// <inheritdoc />
        public IChartCompositeValue CompositeValue
        {
            get => (IChartCompositeValue)GetValue(CompositeValueProperty);
            set => SetValue(CompositeValueProperty, value);
        }
        /// <inheritdoc />
        public string CustomValue { get; set; }
        /// <inheritdoc />
        public abstract IChartValue Clone();
        #endregion
    }
}
