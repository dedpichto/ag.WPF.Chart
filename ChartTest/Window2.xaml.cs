using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChartTest
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
            FlowDirection = System.Windows.FlowDirection.RightToLeft;
        }
    }

    public class HorizontalValuesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is double))
                return null;
            var w = (double)value;
            var gm = new PathGeometry();
            var step = w / 10;
            for (var i = 1; i <= 10; i++)
            {
                var num = i * 10;
                var number = num.ToString();
                var fmt = new FormattedText(number, culture, FlowDirection.LeftToRight,
                    new Typeface("Segoe UI"),12, Brushes.Black);
                var offset = -fmt.Width;
                var pt = new Point(i*step + offset, 0);
                var ngm = fmt.BuildGeometry(pt);

                var trgr = new TransformGroup();
                trgr.Children.Add(i < 6
                    ? new RotateTransform(-45, pt.X + fmt.Width, pt.Y)
                    : new RotateTransform(45, pt.X, pt.Y));
                trgr.Children.Add(new ScaleTransform{ScaleX = -1, CenterX = w/2});
                //ngm.Transform = i < 6 ? new RotateTransform(-45, pt.X + fmt.Width, pt.Y) : new RotateTransform(45, pt.X, pt.Y);
                ngm.Transform = trgr;
                gm.AddGeometry(ngm);
            }
            return gm;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
