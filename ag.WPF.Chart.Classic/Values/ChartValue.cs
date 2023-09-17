using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ag.WPF.Chart.Values
{
    /// <summary>
    /// Represents the basic abstract class of chart value
    /// </summary>
    public abstract class ChartValue : DependencyObject, IChartValue
    {
#nullable disable
        private bool _isVisible = true;

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
        /// <inheritdoc />
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return; _isVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region INotifyPropertyChanged members
        /// <summary>
        /// Raises the PropertyChanged event when the property value has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
#nullable restore
    }
}
