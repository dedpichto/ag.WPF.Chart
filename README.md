# ag.WPF.Chart
## A custom WPF control, containing series, axes, legends, and other hosted content.

A Chart contains a collection of Series objects, which are in ItemsSource property. 
A Series may be of type PlainSeries, which consists of simple double values, associated with each series point, or stock series, such as HighLowCloseSeries, OpenHighLowCloseSeries and so on. 
Each Series contains a collection of ChartValue objects, which can be of type PlainChartValue, HighLowCloseChartValue, OpenHighLowCloseChartValue. 

The following example shows how to create and use ag.WPF.Chart.

Add ag.WPF.Chart to XAML code:

```xaml
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
                     chart:ChartStyle="Columns" 
                     Title="Alcohol per capita in liters" 
                     AxisTitleY="Liters" 
                     AxisTitleX="Countries" 
                     ValuesFormatY="0 L" 
                     ItemsSource="{Binding Series}" 
                     CustomValuesX="{Binding CustomXTexts}"/>
    </Grid>
</Window>
```

Add some code to window:

```C#
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

            var dsa = new AlcoholDataSource();
            Series.Add(new PlainSeries("Beer", dsa.Select(d => d.Beer)));
            Series.Add(new PlainSeries("Wine", dsa.Select(d => d.Wine)));
            Series.Add(new PlainSeries("Spirit", dsa.Select(d => d.Spirit)));
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
            Add(new AlcoholDrinks { Country = "Australia", Total = 13.4, Beer = 5.3466, Wine = 5.025, Spirit = 3.0284 });
            Add(new AlcoholDrinks { Country = "Canada", Total = 13.8, Beer = 6.2238, Wine = 3.5328, Spirit = 4.0434 });
            Add(new AlcoholDrinks { Country = "United States", Total = 13.7, Beer = 6.439, Wine = 2.4797, Spirit = 4.7813 });
            Add(new AlcoholDrinks { Country = "Russia", Total = 20.1, Beer = 7.839, Wine = 2.5728, Spirit = 9.6882 });
            Add(new AlcoholDrinks { Country = "China", Total = 12.9, Beer = 3.8184, Wine = 0.3999, Spirit = 8.6817 });
            Add(new AlcoholDrinks { Country = "France", Total = 16.7, Beer = 3.1396, Wine = 9.8196, Spirit = 3.7408 });
        }
    }
}
```

As a result you will get the following chart:

![](https://am3pap005files.storage.live.com/y4mg_xxl-4GzNLmpQjQu-9hRHt6RxjZm9inNhVWMT4LMa7vXwLs6vWb-0xXSe0OFyb63PmZd_TTmGsCEIZ75l_WwEz5ajZqhSjuv7riAAlRKH3pBmH_wChavI62sQ9Qax7qubkD5uP3amORjX79292seOTeXDRicYb8V7_e6ulEESoZjSueOuyzVq4cOiVmvJg6?width=972&height=505&cropmode=none)