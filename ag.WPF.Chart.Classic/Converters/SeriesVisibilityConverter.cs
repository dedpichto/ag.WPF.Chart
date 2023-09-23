using ag.WPF.Chart.Series;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ag.WPF.Chart.Converters
{
    /// <summary>
    /// Defines Path visibility accordingly to <see cref="ISeries"/> <see cref="ISeries.IsVisible"/> property.
    /// </summary>
    public class SeriesVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts <see cref="ISeries"/> <see cref="ISeries.IsVisible"/> property to <see cref="Visibility"/>.
        /// </summary>
        /// <param name="value"><see cref="ISeries"/> <see cref="ISeries.IsVisible"/> property.</param>
        /// <param name="targetType"><see cref="Visibility"/>.</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns><see cref="Visibility"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool isVisible)
                return Visibility.Collapsed;
            return isVisible ? Visibility.Visible: Visibility.Collapsed;
        }
        /// <summary>
        /// Converts <see cref="Visibility"/> to <see cref="ISeries"/> <see cref="ISeries.IsVisible"/> property.
        /// </summary>
        /// <param name="value"><see cref="Visibility"/>.</param>
        /// <param name="targetType"><see cref="ISeries"/> <see cref="ISeries.IsVisible"/> property.</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Boolean</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Visibility visibility)
                return false;
            return visibility == Visibility.Visible;
        }
    }
}
