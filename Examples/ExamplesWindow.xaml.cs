using ag.WPF.Chart.Series;
using ag.WPF.Chart.Values;
using Examples.DataSources;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Examples
{
    /// <summary>
    /// Interaction logic for Examples.xaml
    /// </summary>
    public partial class ExamplesWindow : Window
    {
        public ExamplesWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var ds = new EnergyDataSource();

            ULinesStyle.Series.Add(new PlainSeries("Coal", ds.Select(d => d.Coal)));
            ULinesStyle.Series.Add(new PlainSeries("Hydro", ds.Select(d => d.Hydro)));
            ULinesStyle.Series.Add(new PlainSeries("Nuclear", ds.Select(d => d.Nuclear)));
            ULinesStyle.Series.Add(new PlainSeries("Gas", ds.Select(d => d.Gas)));
            ULinesStyle.Series.Add(new PlainSeries("Oil", ds.Select(d => d.Oil)));

            ULinesStyle.CustomXTexts.AddRange(ds.Select(d => d.Country));

            var dsa = new AlcoholDataSource();
            UColumnsBarsStyles.Series.Add(new PlainSeries("Beer", dsa.Select(d => d.Beer)));
            UColumnsBarsStyles.Series.Add(new PlainSeries("Wine", dsa.Select(d => d.Wine)));
            UColumnsBarsStyles.Series.Add(new PlainSeries("Spirit", dsa.Select(d => d.Spirit)));
            UColumnsBarsStyles.CustomXTexts.AddRange(dsa.Select(d => d.Country));

            var dsg = new GasPricesSource();
            foreach (var gp in dsg)
            {
                UAreaStyles.Series.Add(new PlainSeries(gp.Type, gp.Prices));
            }
            UAreaStyles.CustomXTexts.AddRange(dsg.Years.Select(y => y.ToString()));

            UFunnelPipesStyles.Series.Add(new PlainSeries("Total", dsa.Select((d) => new PlainChartValue(d.Total, d.Country))));
            UFunnelPipesStyles.CustomXTexts.AddRange(dsa.Select(d => d.Country));

            var dpg = new PopulationDataSource();
            var pd = dpg.FirstOrDefault(p => p.Country == "Portugal");
            UWaterfallStyle.Series.Add(new PlainSeries("Portugal", pd.Percents));
            UWaterfallStyle.CustomXTexts.AddRange(new[] { "2013", "2014", "2015", "2016", "2017", "2018" });

            var dsnba = new NbaStatsDataSource();
            foreach (var nba in dsnba)
            {
                URadarStyle.Series.Add(new PlainSeries(nba.Name, new[]
                {
                    new PlainChartValue(nba.FreeGoals, "Free goals"),
                    new PlainChartValue(nba.ThreePointShots, "3-point shots"),
                    new PlainChartValue(nba.FreeThrows, "Free throws"),
                    new PlainChartValue(nba.OfRebounds, "Ofensive rebounds"),
                    new PlainChartValue(nba.DefRebounds, "Defensive rebounds"),
                    new PlainChartValue(nba.Rebounds, "Rebounds"),
                    new PlainChartValue(nba.Assists, "Assits"),
                    new PlainChartValue(nba.Blocks, "Blocks")
                }));
            }
            URadarStyle.CustomXTexts.AddRange(new[] { "Free goals", "3-point shots", "Free throws", "Ofensive rebounds", "Defensive rebounds", "Rebounds", "Assits", "Blocks" });

            var dsst = new StocksDataSource();
            UHLCStyle.Series.Add(new HighLowCloseSeries(dsst[0].Name, dsst.Select(st => (st.High, st.Low, st.Close))));
            UHLCStyle.CustomXTexts.AddRange(dsst.Select(st => st.Date.ToString("dd MMM yyyy")));

            UOHLCStyle.Series.Add(new OpenHighLowCloseSeries(dsst[0].Name, dsst.Select(st => (st.Open, st.High, st.Low, st.Close))));
            UOHLCStyle.CustomXTexts.AddRange(dsst.Select(st => st.Date.ToString("dd MMM yyyy")));

            USignals.Series.Add(new PlainSeries(Brushes.Yellow, "Trappist-B", new SpaceSignals()));
            IEnumerable<string> getTimes()
            {
                for (var i = 0; i <= 10; i++)
                {
                    yield return DateTime.Now.AddSeconds(i).ToString("HH:mm:ss.fff");
                }
            }
            USignals.CustomXTexts.AddRange(getTimes());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog { Filter = "PNG files (*.png)|*.png|GIF files (*.gif)|*.gif|JPG files (*.jpg)|*.jpg|BMP files (*.bmp)|*.bmp|TIF files (*.tif)|*.tif|XPS files (*.xps)|*.xps" };
            if (!sfd.ShowDialog(this).Value)
                return;
            switch (TabCharts.SelectedIndex)
            {
                case 0:
                    ULinesStyle.SaveAsImage(sfd.FileName);
                    break;
                case 1:
                    UColumnsBarsStyles.SaveAsImage(sfd.FileName);
                    break;
                case 2:
                    UAreaStyles.SaveAsImage(sfd.FileName);
                    break;
                case 3:
                    UFunnelPipesStyles.SaveAsImage(sfd.FileName);
                    break;
                case 4:
                    UWaterfallStyle.SaveAsImage(sfd.FileName);
                    break;
                case 5:
                    URadarStyle.SaveAsImage(sfd.FileName);
                    break;
                case 6:
                    UHLCStyle.SaveAsImage(sfd.FileName);
                    break;
                case 7:
                    UOHLCStyle.SaveAsImage(sfd.FileName);
                    break;
                case 8:
                    USignals.SaveAsImage(sfd.FileName);
                    break;
                default:
                    return;
            }
            Process fileopener = new Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + sfd.FileName + "\"";
            fileopener.Start();
        }
    }
}
