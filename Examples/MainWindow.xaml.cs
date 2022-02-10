using ag.WPF.Chart.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Examples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> LegendsWaterfall { get; } = new List<string> { "Подъем", "Спуск", "Хрен знает что" };
        public ObservableCollection<ISeries> Series { get; } = new ObservableCollection<ISeries>();
        public List<string> CustomXTexts { get; } = new List<string>();
        public List<string> CustomYTexts { get; } = new List<string>(new string[] { "Oil", "Gas", "Electricity" });

        public List<double[]> AllSeries = new List<double[]>
        {
            new double [] { 327, 20, 210, 27, 17, 37, 89, 56, -1324 },
            new double []{ 30, 115, 175, 593, 37 },
            new double[]{ 60, 128.3, 37 },
            new double[]{ 1, 8.3, 72, 90, 130, -23, -12, -13, -14, -121  },
            new double[]{ 1, 2, 3, 4, 5, 6, 7, 17  },
            new double[]{ 978, 1458, 1459, 1460, 1461, 425, 172 },
            new double[]{ -1, -2, -3, -4, -5, -6, -11, -5 },
            new double[]{ 1017, -1200, -500 },
            new double[]{  0.234, 0.172, 0.2117, 0.37875, 0.84689},
            new double[]{ -0.234, 0.172, -0.2117, -0.37875, -.84689 },
            new double[]{ 13.4, 5.3466, 5.025, 3.0284,-8.217 },
            new double[]{ 0, 0, 0, 0,0 },
            new double[]{ 1.03466, 1, 2.0284,.0217 },

        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Series.Add(new HighLowCloseSeries("Stocks 1", new[] {
            //    (50.0,35.0,45.0),
            //    (50.0,30.0,35.0),
            //    (45.0,30.0,40.0),
            //    (50.0,35.0,45.0),
            //    (50.0,30.0,35.0),
            //    (45.0,30.0,40.0),
            //    (50.0,35.0,45.0),
            //    (50.0,30.0,35.0),
            //    (45.0,30.0,40.0),
            //    (50.0,35.0,45.0)
            //}));

            //chTest.SeriesItems.Add(new OpenHighLowCloseSeries("Stocks 1", new[] {
            //    (40.0,50.0,35.0,45.0),
            //    (45.0,50.0,30.0,35.0),
            //    (35.0,45.0,30.0,40.0),
            //    (40.0, 50.0,35.0,45.0),
            //    (45.0,50.0,30.0,35.0),
            //    (35.0, 45.0,30.0,40.0),
            //    (40.0, 50.0,35.0,45.0),
            //    (45.0,50.0,30.0,35.0),
            //    (35.0,45.0,30.0,40.0),
            //    (40.0, 50.0,35.0,45.0)
            //}));

            //var ds = new EnergyDataSource();

            //Series.Add(new PlainSeries("Coal", ds.Select(d => d.Coal)));
            //Series.Add(new PlainSeries("Hydro", ds.Select(d => d.Hydro)));
            //Series.Add(new PlainSeries("Nuclear", ds.Select(d => d.Nuclear)));
            //Series.Add(new PlainSeries("Gas", ds.Select(d => d.Gas)));
            //Series.Add(new PlainSeries("Oil", ds.Select(d => d.Oil)));

            //var ds = new AlcoholDataSource();
            //Series.Add(new PlainSeries("Beer", ds.Select(d => d.Beer)));
            //Series.Add(new PlainSeries("Wine", ds.Select(d => d.Wine)));
            //Series.Add(new PlainSeries("Spirit", ds.Select(d => d.Spirit)));
            //CustomXTexts.AddRange(ds.Select(d => d.Country));

            for (var i = 0; i < AllSeries.Count; i++)
            {
                //chTest.SeriesItems.Add(new PlainSeries($"Series {i + 1}", AllSeries[i]));
                Series.Add(new PlainSeries($"Series {i + 1}", AllSeries[i]));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) =>
            //for (var i = 0; i < AllSeries.Count; i++)
            //{
            //    chTest.Series.Add(new PlainSeries($"Series {i + 1}", AllSeries[i]));
            //    //Series.Add(new PlainSeries($"Series {i + 1}", AllSeries[i]));
            //}
            //Series.Insert(1, new PlainSeries("kuku", new[] { 3.5, 7.3, 8.1, 7.64, 2.8 }));
            Series[1] = new PlainSeries("kuku", new[] { 3.5, 7.3, 8.1, 7.64, 2.8 });
    }
}
