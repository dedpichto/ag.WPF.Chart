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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            chrt.SeriesCollection.Add(new WPFChart.Series("CPU usage", new double[100]));

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
                var values = new double[100];
                values[0] = next;
                for (var i = 0; i < chrt.SeriesCollection[0].Values.Count-1; i++)
                    values[i+1] = chrt.SeriesCollection[0].Values[i].Value;
                chrt.SeriesCollection.Insert(0, new WPFChart.Series("CPU usage", values));
                chrt.SeriesCollection.RemoveAt(1);

            }
        }
    }
}
