using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ag.WPF.Chart
{
    public static class Statics
    {
        public static readonly DependencyProperty SectorDataProperty = DependencyProperty.RegisterAttached("SectorData",
            typeof(string), typeof(ISeries), new FrameworkPropertyMetadata("", null));

        public static string GetSectorData(DependencyObject obj)
        {
            return (string)obj.GetValue(SectorDataProperty);
        }

        public static void SetSectorData(DependencyObject obj, string value)
        {
            obj.SetValue(SectorDataProperty, value);
        }

        internal static Brush[] PredefinedPieBrushes { get; } =
{
            new SolidColorBrush(Color.FromArgb(255, 91, 155, 213)),
            new SolidColorBrush(Color.FromArgb(255, 237, 125, 49)),
            new SolidColorBrush(Color.FromArgb(255, 165, 165, 165)),
            new SolidColorBrush(Color.FromArgb(255, 255, 192, 0)),
            new SolidColorBrush(Color.FromArgb(255, 68, 114, 196)),
            new SolidColorBrush(Color.FromArgb(255, 112, 173, 71)),
            new SolidColorBrush(Color.FromArgb(255, 37, 94, 145)),
            new SolidColorBrush(Color.FromArgb(255, 158, 72, 14)),
            new SolidColorBrush(Color.FromArgb(255, 99, 99, 99)),
            new SolidColorBrush(Color.FromArgb(255, 153, 115, 0))
        };
    }
}
