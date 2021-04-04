using ag.WPF.Chart.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private readonly PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private readonly Timer _Timer = new Timer(1000);

        public ObservableCollection<ISeries> Series { get; } = new ObservableCollection<ISeries>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //chrt.SeriesCollection.Add(new ag.WPF.Chart.Series("CPU usage", new double[100]));
            Series.Add(new PlainSeries("Series 1", new double[60]));
            _Timer.Elapsed += _Timer_Elapsed;
            _Timer.Start();
        }

        delegate void timerDelegate(object sender, ElapsedEventArgs e);
        void _Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new timerDelegate(_Timer_Elapsed), sender, e);
            }
            else
            {
                var next = cpuCounter.NextValue();
                //if (Series[0].Values.Count == chrt.MaxX)
                    Series[0].Values.RemoveAt(Series[0].Values.Count - 1);
                Series[0].Values.Insert(0,new ag.WPF.Chart.Values.PlainChartValue(next));
                var values = new double[100];
                values[0] = next;
                //for (var i = 0; i < chrt.SeriesCollection[0].Values.Count-1; i++)
                //    values[i+1] = chrt.SeriesCollection[0].Values[i].Value.V1;
                //chrt.SeriesCollection.Insert(0, new ag.WPF.Chart.Series("CPU usage", values));
                //chrt.SeriesCollection.RemoveAt(1);

            }
        }
    }
}
