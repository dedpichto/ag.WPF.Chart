using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using ag.WPF.Chart;

namespace ChartTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public ObservableCollection<Series> Series { get; } = new ObservableCollection<Series>();
        public List<string> CustomXTexts { get; } = new List<string>(new string[] { "Canada", "Russia", "USA" });
        public List<string> CustomYTexts { get; } = new List<string>(new string[] { "Oil", "Gas", "Electricity" });
        //public Series S1 { get; } = new Series("Series 1", new double[] { -40, -20, -60 });
        //public Series S2 { get; } = new Series("Series 2", new double[] { 30, 115, 175, 93, 37.5 });
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //FlowDirection = System.Windows.FlowDirection.RightToLeft;
            //var values1 = new[] { new ChartValue(-40, "aaa"), new ChartValue(-10, "bbb"), new ChartValue(-63, "ccc"), new ChartValue(150, "ccc") };
            //var values2 = new[] { new ChartValue( 20), new ChartValue( 50), new ChartValue( 0) , new ChartValue( 0)};
            //var values3 = new[] { new ChartValue(-60, "aaa"), new ChartValue(-20, "bbb"), new ChartValue(27, "ccc") , new ChartValue(-45, "ccc")};

            //var values1 = new[] { new ChartValue(40, "Gray marble"), new ChartValue(35, "Blue marble"), new ChartValue(25, "Pink marble") };
            //var values2 = new double[] {20, 50};
            //var values3 = new double[] {-60, -20, -27, -45};
            var values1 = new double[] { 327, 20, 210, 27, 17 ,37,89,56,-1324};
            var values2 = new double[] { 30, 115, 175, 593, 37.5 };
            var values3 = new double[] { 60, -128.3, 37 };
            var values4 = new double[] { 1,8.3,72,90,130,-23,-12,-13,-14,-121};
            var values5 = new double[] { 1,2,3,4,5,6,7};
            var values6 = new double[] { 978,1458,1459,1460,1461,425,172};
            var values7 = new double[] { -1,-2,-3,-4,-5,-6,-7};

            //chTest.ChartStyle = ChartStyle.Columns;
            //chTest.SeriesCollection.Add(S1);
            //chTest.SeriesCollection.Add(S2);

            //Series.Add(new Series("Series 1", values1));
            //Series.Add(new Series("Series 2", values2));
            //Series.Add(new Series("Series 3", values3));
            Series.Add(new Series("Series 4", values4));
            //Series.Add(new Series("Series 5", values5));
            //Series.Add(new Series("Series 6", values6));
            //Series.Add(new Series("Series 7", values7));

            //chTest.SeriesCollection.Add(new Series("Series 3", values3));

            //chTest.SeriesCollection.Add(new Series((Brush) TryFindResource("grayBrush"), "Series 1", values1));
            //chTest.SeriesCollection[0].PieBrushes[0] = (Brush) TryFindResource("grayBrush");
            //chTest.SeriesCollection[0].PieBrushes[1] = (Brush)TryFindResource("blueBrush");
            //chTest.SeriesCollection[0].PieBrushes[2] = (Brush)TryFindResource("pinkBrush");

            //chTest.SeriesCollection.Add(new Series((Brush)TryFindResource("ausBrush"), "Series 2", values2));

            ////chTest.SeriesCollection.Add(new Series((Brush)TryFindResource("gbrBrush"), "Series 3", values3));
            //chTest.ChartStyle = ChartStyle.SolidPie;

            var customs = new List<string> { "one", "two" };//, "three", "four", "five", "six", "seven", "eight", "nine", "ten" };

            //var customs = new List<string> { "1896 Athens", "1900 Paris", "1904 St. Luis" };//,"aaa","bbb","ccc","ddd","eee","fff","ggg", "iiikkk" };

            //chTest.XAxisCustomValues = customs;
            //chTest.ChartStyle = ChartStyle.Bars;
            ////chTest.AutoAdjust = false;
            var points4 = new List<double>();
            var points5 = new List<double>();
            var points6 = new List<double>();
            var points7 = new List<double>();
            var x = -2 * Math.PI;
            while (x <= 2 * Math.PI)
            {
                var y = Math.Sin(x);
                var z = Math.Cos(x);
                var t = Math.Tan(x);
                var c = Math.Atan(x);
                points4.Add(y);
                points5.Add(z);
                points6.Add(t);
                points7.Add(c);
                x += Math.PI / 8;
            }
            for (double i = -2; i <= 2; i += 0.1)
            {
                var t = Math.Tan(i);
                points6.Add(t);
            }
            //Series.Add(new Series("Sin", points4));
            //Series.Add(new Series("Cos", points5));
            //Series.Add(new Series("Tan", points6));
            //Series.Add(new Series("ATan", points7));
            //chTest.ChartStyle = ChartStyle.LinesWithMarkers;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Series.RemoveAt(0);
            //chTest.SeriesCollection[0].Points[1] = new ChartPoint( new Value(chTest.SeriesCollection[0].Points[1].Value.X,
            //    chTest.SeriesCollection[0].Points[1].Value.Y + 17));
            //chTest.SeriesCollection.RemoveAt(0);
        }

        private void Chart_MarkerLeftButtonDoubleClick(object sender, RoutedEventArgs e)
        {
            if (!(e is ChartPointLeftButtonDoubleClickEventArgs cm)) return;
            Console.WriteLine(cm.Value.Value);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //chTest.IsEnabled = !chTest.IsEnabled;
            if (FlowDirection == FlowDirection.RightToLeft)
                FlowDirection = FlowDirection.LeftToRight;
            else
                FlowDirection = FlowDirection.RightToLeft;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog { Filter = "PNG files (*.png)|*.png|XPS files (*.xps)|*.xps" };
            if (!sfd.ShowDialog(this).Value)
                return;
            chTest.SaveAsImage(sfd.FileName);

            Process fileopener = new Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + sfd.FileName + "\"";
            fileopener.Start();
            //chTest.SaveAsImage(@"E:\chart.png");
            //chTest.SaveAsImage(@"E:\chart.jpg");
            //chTest.SaveAsImage(@"E:\chart.jpeg");
            //chTest.SaveAsImage(@"E:\chart.gif");
            //chTest.SaveAsImage(@"E:\chart.bmp");
            //chTest.SaveAsImage(@"E:\chart.xps");
            //chTest.SaveAsImage(@"E:\chart.tif");
            //chTest.SaveAsImage(@"E:\chart.tiff");

        }
    }
}
