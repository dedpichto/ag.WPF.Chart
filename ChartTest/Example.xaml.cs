using System;
using System.Collections.Generic;
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
using ag.WPF.Chart;

namespace ChartTest
{
    /// <summary>
    /// Interaction logic for Example.xaml
    /// </summary>
    public partial class Example : Window
    {
        public Example()
        {
            InitializeComponent();
        }

        private void oplympicChart()
        {
            var cities = new[]
            {
                "1896 Athens", "1900 Paris", "1904 St. Luis", "1906 Athens", "1908 London", "1912 Stockholm",
                "1920 Antwerpen", "1924 Paris", "1928 Amsterdam", "1932 Los Angeles", "1936 Berlin", "1948 London",
                "1952 Helsinki", "1956 Melbourne", "1960 Rome", "1964 Tokyo", "1968 Mexico", "1972 Munchen",
                "1976 Montreal", "1980 Moscow", "1984 Los Angeles", "1988 Seoul", "1992 Barcelona", "1996 Atlanta",
                "2000 Syndney", "2004 Athens", "2008 Beijing","2012 London","2016 Rio de Janeiro"
            };

            var usa = new double[]
            {
                19, 48, 234, 23, 46, 63, 94, 99, 56, 110, 57, 84, 74, 72, 71, 90, 107, 95, 94, 0, 173, 94, 108, 102, 93,
                103, 110,103,121
            };
            var gbr = new double[]
            {
                46, 30, 28, 15, 20, 24, 37, 21, 13, 17, 13, 18, 20, 24, 11, 26, 14, 17, 22, 36, 43, 41, 142, 24, 2, 35, 7,65,67
            };
            var frn = new double[]
            {
                40, 33, 38, 37, 29, 16, 28, 14, 9, 13, 15, 15, 5, 14, 18, 32, 18, 20, 25, 40, 41, 16, 19, 40, 0, 110, 11,34,42
            };
            //chOlympic.XAxisCustomValuesBetweenTicks = true;
            chOlympic.XAxisCustomValues = cities.ToList();
            chOlympic.SeriesCollection.Add(new Series("USA", usa));
            chOlympic.SeriesCollection.Add(new Series("Great Britain", gbr));
            chOlympic.SeriesCollection.Add(new Series("France", frn));

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //FlowDirection = System.Windows.FlowDirection.RightToLeft;
            oplympicChart();
        }
    }
}
