*************************************************************************
USAGE EXAMPLE
*************************************************************************

<!-- Add ag.WPF.Chart to XAML: -->

<Window x:Class="WpfApp4.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:chart="clr-namespace:ag.WPF.Chart;assembly=ag.WPF.Chart"
        mc:Ignorable="d"
        Title="MainWindow" Height="450" Width="800"
        DataContext="{Binding RelativeSource={RelativeSource Mode=Self}}">
    <Grid>
        <chart:Chart Margin="4" 
                     Background="#FF262626" 
                     Foreground="#D9D9D9" 
                     ChartStyle="Columns" 
                     Title="Alcohol per capita in liters" 
                     AxisTitleY="Liters" 
                     AxisTitleX="Countries" 
                     VerticalAxisValuesFormat="0 L" 
                     SeriesSource="{Binding Series}" 
                     CustomValuesX="{Binding CustomXTexts}"/>
    </Grid>
</Window>

// Add some code to window:

using ag.WPF.Chart.Series;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace WpfApp4
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<ISeries> Series { get; } = new ObservableCollection<ISeries>();
        public List<string> CustomXTexts { get; } = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
     
            // create sample data source
            var dsa = new AlcoholDataSource();
            // add series to collection, providing series' name and collection of values,
            // in our case - the simple IEnumerable<double>
            Series.Add(new PlainSeries("Beer", dsa.Select(d => d.Beer)));
            Series.Add(new PlainSeries("Wine", dsa.Select(d => d.Wine)));
            Series.Add(new PlainSeries("Spirit", dsa.Select(d => d.Spirit)));
            // add countries names as custom values for x-axis
            CustomXTexts.AddRange(dsa.Select(d => d.Country));
        }
    }

    public class AlcoholDrinks
    {
        public string Country { get; set; }
        public double Total { get; set; }
        public double Beer { get; set; }
        public double Wine { get; set; }
        public double Spirit { get; set; }
    }

    public class AlcoholDataSource : List<AlcoholDrinks>
    {
        public AlcoholDataSource()
        {
            // add some sample data
            Add(new AlcoholDrinks { Country = "Australia", Total = 13.4, Beer = 5.3466, Wine = 5.025, Spirit = 3.0284 });
            Add(new AlcoholDrinks { Country = "Canada", Total = 13.8, Beer = 6.2238, Wine = 3.5328, Spirit = 4.0434 });
            Add(new AlcoholDrinks { Country = "United States", Total = 13.7, Beer = 6.439, Wine = 2.4797, Spirit = 4.7813 });
            Add(new AlcoholDrinks { Country = "Russia", Total = 20.1, Beer = 7.839, Wine = 2.5728, Spirit = 9.6882 });
            Add(new AlcoholDrinks { Country = "China", Total = 12.9, Beer = 3.8184, Wine = 0.3999, Spirit = 8.6817 });
            Add(new AlcoholDrinks { Country = "France", Total = 16.7, Beer = 3.1396, Wine = 9.8196, Spirit = 3.7408 });
        }
    }
}