using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ag.WPF.Chart.Series;

namespace ag.WPF.Chart
{
    /// <summary>
    /// Represents static class with number of attached dependency properties.
    /// </summary>
    public static class Statics
    {
        /// <summary>
        /// The identifier of the SectorData attached dependency property.
        /// </summary>
        public static readonly DependencyProperty SectorDataProperty = DependencyProperty.RegisterAttached("SectorData",
            typeof(string), typeof(ISeries), new FrameworkPropertyMetadata("", null));
        /// <summary>
        /// The identifier of the HasCustomMainBrush attached dependency property.
        /// </summary>
        public static readonly DependencyProperty HasCustomMainBrushProperty = DependencyProperty.RegisterAttached("HasCustomMainBrush",
            typeof(bool), typeof(ISeries), new FrameworkPropertyMetadata(false, null));
        /// <summary>
        /// The identifier of the HasCustomSecondaryBrushProperty attached dependency property.
        /// </summary>
        public static readonly DependencyProperty HasCustomSecondaryBrushProperty = DependencyProperty.RegisterAttached("HasCustomSecondaryBrush",
            typeof(bool), typeof(ISeries), new FrameworkPropertyMetadata(false, null));
        /// <summary>
        /// The identifier of the AddedToCanvasProperty attached dependency property.
        /// </summary>
        public static readonly DependencyProperty AddedToCanvasProperty = DependencyProperty.RegisterAttached("AddedToCanvas", 
            typeof(bool), typeof(ISeries), new FrameworkPropertyMetadata(false, null));

        /// <summary>
        /// Gets AddedToCanvas attached dependency property.
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <returns>AddedToCanvas (boolean).</returns>
        public static bool GetAddedToCanvas(DependencyObject obj)
        {
            return (bool)obj.GetValue(AddedToCanvasProperty);
        }
        /// <summary>
        /// Sets AddedToCanvas attached dependency property.
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <param name="value">Boolean</param>
        public static void SetAddedToCanvas(DependencyObject obj, bool value)
        {
            obj.SetValue(AddedToCanvasProperty, value);
        }
        /// <summary>
        /// Gets SectorData attached dependency property.
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <returns>SectorData (string).</returns>
        public static string GetSectorData(DependencyObject obj)
        {
            return (string)obj.GetValue(SectorDataProperty);
        }
        /// <summary>
        /// Sets SectorData attached dependency property.
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <param name="value">String</param>
        public static void SetSectorData(DependencyObject obj, string value)
        {
            obj.SetValue(SectorDataProperty, value);
        }
        /// <summary>
        /// Gets HasCustomMainBrush attached dependency property.
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <returns>HasCustomMainBrush (boolean).</returns>
        public static bool GetHasCustomMainBrush(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasCustomMainBrushProperty);
        }
        /// <summary>
        /// Sets HasCustomMainBrush attached dependency property.
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <param name="value">Boolean</param>
        public static void SetHasCustomMainBrush(DependencyObject obj, bool value)
        {
            obj.SetValue(HasCustomMainBrushProperty, value);
        }
        /// <summary>
        /// Gets HasCustomSecondaryBrush attached dependency property.
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <returns>HasCustomSecondaryBrush (boolean).</returns>
        public static bool GetHasCustomSecondaryBrush(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasCustomSecondaryBrushProperty);
        }
        /// <summary>
        /// Sets HasCustomSecondaryBrush attached dependency property.
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <param name="value">Boolean</param>
        public static void SetHasCustomSecondaryBrush(DependencyObject obj, bool value)
        {
            obj.SetValue(HasCustomSecondaryBrushProperty, value);
        }

        internal static (Brush Brush, int Counter)[] PredefinedMainBrushes { get; } =
        {
            (new SolidColorBrush(Color.FromArgb(255, 91, 155, 213)),0),
            (new SolidColorBrush(Color.FromArgb(255, 237, 125, 49)),0),
            (new SolidColorBrush(Color.FromArgb(255, 165, 165, 165)),0),
            (new SolidColorBrush(Color.FromArgb(255, 255, 192, 0)),0),
            (new SolidColorBrush(Color.FromArgb(255, 68, 114, 196)),0),
            (new SolidColorBrush(Color.FromArgb(255, 112, 173, 71)),0),
            (new SolidColorBrush(Color.FromArgb(255, 37, 94, 145)),0),
            (new SolidColorBrush(Color.FromArgb(255, 158, 72, 14)),0),
            (new SolidColorBrush(Color.FromArgb(255, 99, 99, 99)),0),
            (new SolidColorBrush(Color.FromArgb(255, 153, 115, 0)),0)
        };

        internal static (Brush Brush, int Counter)[] PredefinedSecondaryBrushes { get; } =
        {
            (new SolidColorBrush(Color.FromArgb(255, 237, 125, 49)),0),
            (new SolidColorBrush(Color.FromArgb(255, 165, 165, 165)),0),
            (new SolidColorBrush(Color.FromArgb(255, 255, 192, 0)),0),
            (new SolidColorBrush(Color.FromArgb(255, 68, 114, 196)),0),
            (new SolidColorBrush(Color.FromArgb(255, 112, 173, 71)),0),
            (new SolidColorBrush(Color.FromArgb(255, 37, 94, 145)),0),
            (new SolidColorBrush(Color.FromArgb(255, 158, 72, 14)),0),
            (new SolidColorBrush(Color.FromArgb(255, 99, 99, 99)),0),
            (new SolidColorBrush(Color.FromArgb(255, 153, 115, 0)),0),
            (new SolidColorBrush(Color.FromArgb(255, 91, 155, 213)),0)
        };
    }
}
