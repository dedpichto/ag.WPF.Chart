﻿using System;
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
using ag.WPF.Chart.Series;

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

        public List<string> WaterfallLegends { get; } = new List<string> { "Подъем", "Спуск", "Хрен знает что" };
        public ObservableCollection<ISeries> Series { get; } = new ObservableCollection<ISeries>();
        public List<string> CustomXTexts { get; } = new List<string>(new string[] { "Canada", "Russia", "USA" });
        public List<string> CustomYTexts { get; } = new List<string>(new string[] { "Oil", "Gas", "Electricity" });
        //public Series S1 { get; } = new Series("Series 1", new double[] { -40, -20, -60 });
        //public Series S2 { get; } = new Series("Series 2", new double[] { 30, 115, 175, 93, 37.5 });
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //FlowDirection = System.Windows.FlowDirection.RightToLeft;
            //var values1 = new[] { new PlainChartValue(-40, "aaa"), new PlainChartValue(-10, "bbb"), new PlainChartValue(-63, "ccc"), new PlainChartValue(150, "ccc") };
            //var values2 = new[] { new PlainChartValue( 20), new PlainChartValue( 50), new PlainChartValue( 0) , new PlainChartValue( 0)};
            //var values3 = new[] { new PlainChartValue(-60, "aaa"), new PlainChartValue(-20, "bbb"), new PlainChartValue(27, "ccc") , new PlainChartValue(-45, "ccc")};

            //var values1 = new[] { new PlainChartValue(40, "Gray marble"), new PlainChartValue(35, "Blue marble"), new PlainChartValue(25, "Pink marble") };
            //var values2 = new double[] {20, 50};
            //var values3 = new double[] {-60, -20, -27, -45};
            var values1 = new double[] { 327, 20, 210, 27, 17, 37, 89, 56, -1324 };
            var values2 = new double[] { 30, 115, 175, 593, 37.5};
            var values3 = new double[] { 60, -128.3, 37 };
            var values4 = new double[] { 1, 8.3, 72, 90, 130, -23, -12, -13, -14, -121 };
            var values5 = new double[] { 1, 2, 3, 4, 5, 6, 7, 17 };
            var values6 = new double[] { 978, 1458, 1459, 1460, 1461, 425, 172 };
            var values7 = new double[] { -1, -2, -3, -4, -5, -6, -11, -5 };
            var values8 = new double[] { 1017, -1200, -500 };
            var values9 = new double[] { 0.234, 0.172,   0.2117,  0.37875, 1.84689 };
            var values10 = new double[] { -0.234, -0.172,   -0.2117,  -0.37875, -1.84689 };

            //chTest.ChartStyle = ChartStyle.Columns;

            Series.Add(new PlainSeries("Series 1", values1));
            Series.Add(new PlainSeries("Series 2", values2));
            Series.Add(new PlainSeries("Series 3", values3));
            Series.Add(new PlainSeries("Series 4", values4));
            Series.Add(new PlainSeries("Series 5", values5));
            Series.Add(new PlainSeries("Series 6", values6));
            Series.Add(new PlainSeries("Series 7", values7));
            Series.Add(new PlainSeries("Series 8", values8));
            Series.Add(new PlainSeries("Series 9", values9));
            Series.Add(new PlainSeries("Series 10", values10));


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

        private void CmdAdd_Click(object sender, RoutedEventArgs e)
        {
            Series[0].Values.Add(new ag.WPF.Chart.Values.PlainChartValue(1723));
        }
    }
}
