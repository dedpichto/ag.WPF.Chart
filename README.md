# ag.WPF.Chart
## A custom WPF control, containing series, axes, legends, and other hosted content.

| [<img src="https://am3pap005files.storage.live.com/y4mRpL4FHZcnYnnhp6K9XK_OtiaqY-2nK8RRDT0U3nw2_2NH-1RPF1qscOpxtKuNOuCl4fHFkNs_53vhDw2qkrxximIddK8MPzFADUtlbhbqsC31AfgLEkm_pPjSBCz23AS38QfRNBO7USC3jke4LuCsD8DZ9SnZv27vxf4ktMkT0rTkPo9MxMD2KHfSS_D2o-a?width=972&height=623&cropmode=none" style="zoom:50%;" />](https://am3pap005files.storage.live.com/y4mRpL4FHZcnYnnhp6K9XK_OtiaqY-2nK8RRDT0U3nw2_2NH-1RPF1qscOpxtKuNOuCl4fHFkNs_53vhDw2qkrxximIddK8MPzFADUtlbhbqsC31AfgLEkm_pPjSBCz23AS38QfRNBO7USC3jke4LuCsD8DZ9SnZv27vxf4ktMkT0rTkPo9MxMD2KHfSS_D2o-a?width=972&height=623&cropmode=none) | [<img src="https://am3pap005files.storage.live.com/y4mAZbRE7gAwdbJ9mUH5iZ1fgzjxjvrypHOb3DzVGoTFJ4K3fg6vPGzb_6HFz3eK59PhIxVeo9LSZMy1GhnWcUiZc1GOSffJEhPw6jgkLNmBudCoXCoxb0AUEZm466MgNZ9NLWQYcAj4QIPPOMWgZFTts7Kv4SWQb0xeG8VxgU_ndXsO-GjhRBP48PGxWXVfDaO?width=972&height=623&cropmode=none" style="zoom:50%;" />](https://am3pap005files.storage.live.com/y4mAZbRE7gAwdbJ9mUH5iZ1fgzjxjvrypHOb3DzVGoTFJ4K3fg6vPGzb_6HFz3eK59PhIxVeo9LSZMy1GhnWcUiZc1GOSffJEhPw6jgkLNmBudCoXCoxb0AUEZm466MgNZ9NLWQYcAj4QIPPOMWgZFTts7Kv4SWQb0xeG8VxgU_ndXsO-GjhRBP48PGxWXVfDaO?width=972&height=623&cropmode=none) |
| :----------------------------------------------------------: | :----------------------------------------------------------: |
| [<img src="https://am3pap005files.storage.live.com/y4m54k6vUNZPtzUJI02-BvpegKfxp-zHOh84oojJ_3ZgTbDP6ijKC4wgMYO_krfXbhO1DWMEUkssWjfqH4cYnJrBk3WVeocaRfyVFNrTuuVQohZlDgRIi6euSjuUNV4lBwJas-yNUS47Rg5Inzgr6hllSVeH-1au5VwTz_TMHhL4shMXdDTenqIV9p9K5gNqDO_?width=972&height=623&cropmode=none" style="zoom:50%;" />](https://am3pap005files.storage.live.com/y4m54k6vUNZPtzUJI02-BvpegKfxp-zHOh84oojJ_3ZgTbDP6ijKC4wgMYO_krfXbhO1DWMEUkssWjfqH4cYnJrBk3WVeocaRfyVFNrTuuVQohZlDgRIi6euSjuUNV4lBwJas-yNUS47Rg5Inzgr6hllSVeH-1au5VwTz_TMHhL4shMXdDTenqIV9p9K5gNqDO_?width=972&height=623&cropmode=none) | [<img src="https://am3pap005files.storage.live.com/y4mFDD9m2tK3mMA6b6IEJeZOVh8IzDt_yi8YMt1w1YuVzccxW8KRad27pljtOy2UtvovuMERG-3qElfn5xtYLg4091apKdL1yBHSqQydFELjtB_m-2qBLd6CeCgxso68ZXkDjxOpw-7o6tSkzKqOOvzaUYeVYXIciMwyKJ7nzVWa3OkS3wGKTkC1ioBrmwVPAwj?width=972&height=623&cropmode=none" style="zoom:50%;" />](https://am3pap005files.storage.live.com/y4mFDD9m2tK3mMA6b6IEJeZOVh8IzDt_yi8YMt1w1YuVzccxW8KRad27pljtOy2UtvovuMERG-3qElfn5xtYLg4091apKdL1yBHSqQydFELjtB_m-2qBLd6CeCgxso68ZXkDjxOpw-7o6tSkzKqOOvzaUYeVYXIciMwyKJ7nzVWa3OkS3wGKTkC1ioBrmwVPAwj?width=972&height=623&cropmode=none) |

A Chart contains a collection of Series objects, which are in ItemsSource property. The table below describes different series types and chart styles.

| Series type            | Available chart styles            | Description                                                  |
| ---------------------- | --------------------------------- | ------------------------------------------------------------ |
| PlainSeries            | Lines                             | The style represented by straight lines                      |
|                        | StackedLines                      | The style represented by straight stacked lines              |
|                        | FullStackedLines                  | The style represented by straight 100% stacked lines         |
|                        | SmoothLines                       | The style represented by smooth lines                        |
|                        | SmoothStackedLines                | The style represented by smooth stacked lines                |
|                        | SmoothFullStackedLines            | The style represented by smooth 100% stacked lines           |
|                        | LinesWithMarkers                  | The style represented by straight lines with markers at control points |
|                        | StackedLinesWithMarkers           | The style represented by straight stacked lines with markers at control points |
|                        | FullStackedLinesWithMarkers       | The style represented by straight 100% stacked lines with markers at control points |
|                        | SmoothLinesWithMarkers            | The style represented by smooth lines with markers at control points |
|                        | SmoothStackedLinesWithMarkers     | The style represented by smooth stacked lines with markers at control points |
|                        | SmoothFullStackedLinesWithMarkers | The style represented by smooth 100% stacked lines with markers at control points |
|                        | Columns                           | The style represented by columns                             |
|                        | StackedColumns                    | The style represented by stacked columns                     |
|                        | FullStackedColumns                | The style represented by 100% stacked columns                |
|                        | Bars                              | The style represented by bars                                |
|                        | StackedBars                       | The style represented by stacked bars                        |
|                        | FullStackedBars                   | The style represented by 100% stacked bars                   |
|                        | Area                              | The style represented by areas                               |
|                        | StackedArea                       | The style represented by stacked areas                       |
|                        | FullStackedArea                   | The style represented by 100% stacked areas                  |
|                        | SmoothArea                        | The style represented by smooth areas                        |
|                        | SmoothStackedArea                 | The style represented by smooth stacked areas                |
|                        | SmoothFullStackedArea             | The style represented by 100% smooth stacked areas           |
|                        | Bubbles                           | The style represented by bubbles                             |
|                        | SolidPie                          | The style represented by solid sectors                       |
|                        | SlicedPie                         | The style represented by sectors divided with thin lines     |
|                        | Doughnut                          | The style represented by solid arcs                          |
|                        | Waterfall                         | The style represented by cumulated effect of positive and negative values |
|                        | Radar                             | The style represented by radar                               |
|                        | RadarWithMarkers                  | The style represented by radar with markers at control points |
|                        | RadarArea                         | The style represented by radar areas                         |
|                        | Funnel                            | The style represented by funnel                              |
| HighLowCloseSeries     | HighLowClose                      | The stock style represented by high, low, and close prices   |
| OpenHighLowCloseSeries | OpenHighLowClose                  | The stock style represented by open, high, low, and close prices |

Although a series collection can contain an unlimited amount of series, several chart styles will display only one series at a time, i.e. the very first series in the collection. Such styles are SolidPie, SlicedPie, Doughnut, Waterfall, Funnel, HighLowClose, OpenHighLowClose.

The example below shows how to create and use ag.WPF.Chart.

*Add ag.WPF.Chart to XAML:*

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
                     ChartStyle="Columns" 
                     Title="Alcohol per capita in liters" 
                     AxisTitleY="Liters" 
                     AxisTitleX="Countries" 
                     ValuesFormatY="0 L" 
                     ItemsSource="{Binding Series}" 
                     CustomValuesX="{Binding CustomXTexts}"/>
    </Grid>
</Window>
```

*Add some code to window:*

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
```

*As a result you will get the following chart:*

![](https://am3pap005files.storage.live.com/y4mg_xxl-4GzNLmpQjQu-9hRHt6RxjZm9inNhVWMT4LMa7vXwLs6vWb-0xXSe0OFyb63PmZd_TTmGsCEIZ75l_WwEz5ajZqhSjuv7riAAlRKH3pBmH_wChavI62sQ9Qax7qubkD5uP3amORjX79292seOTeXDRicYb8V7_e6ulEESoZjSueOuyzVq4cOiVmvJg6?width=972&height=505&cropmode=none)