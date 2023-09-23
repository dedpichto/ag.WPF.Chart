using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ag.WPF.Chart.Converters
{
#nullable disable
    /// <summary>
    /// Prepares geometry for shape drawing
    /// </summary>
    public class StarDrawingConverter : IValueConverter
    {
        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ShapeStyle shapeStyle)
                return null;
            return shapeStyle switch
            {
                ShapeStyle.Star5 => Utils.DrawStar(5, 8),
                ShapeStyle.Star6 => Utils.DrawStar(6, 8),
                ShapeStyle.Star8 => Utils.DrawStar(8, 8),
                ShapeStyle.Circle => new EllipseGeometry(new Rect(new Size(16, 16))),
                ShapeStyle.Rectangle => new RectangleGeometry(new Rect(new Size(16, 16))),
                _ => null
            };
        }

        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
#nullable restore
}
