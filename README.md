# ag.WPF.Chart

![Nuget](https://img.shields.io/nuget/v/ag.WPF.Chart)

## A custom WPF control, containing series, axes, legends, and other hosted content

| <img src="https://am3pap005files.storage.live.com/y4mRpL4FHZcnYnnhp6K9XK_OtiaqY-2nK8RRDT0U3nw2_2NH-1RPF1qscOpxtKuNOuCl4fHFkNs_53vhDw2qkrxximIddK8MPzFADUtlbhbqsC31AfgLEkm_pPjSBCz23AS38QfRNBO7USC3jke4LuCsD8DZ9SnZv27vxf4ktMkT0rTkPo9MxMD2KHfSS_D2o-a?width=972&height=623&cropmode=none" style="zoom:50%;" /> | <img src="https://am3pap005files.storage.live.com/y4mAZbRE7gAwdbJ9mUH5iZ1fgzjxjvrypHOb3DzVGoTFJ4K3fg6vPGzb_6HFz3eK59PhIxVeo9LSZMy1GhnWcUiZc1GOSffJEhPw6jgkLNmBudCoXCoxb0AUEZm466MgNZ9NLWQYcAj4QIPPOMWgZFTts7Kv4SWQb0xeG8VxgU_ndXsO-GjhRBP48PGxWXVfDaO?width=972&height=623&cropmode=none" style="zoom:50%;" /> |
| :----------------------------------------------------------: | :----------------------------------------------------------: |
| <img src="https://am3pap005files.storage.live.com/y4m54k6vUNZPtzUJI02-BvpegKfxp-zHOh84oojJ_3ZgTbDP6ijKC4wgMYO_krfXbhO1DWMEUkssWjfqH4cYnJrBk3WVeocaRfyVFNrTuuVQohZlDgRIi6euSjuUNV4lBwJas-yNUS47Rg5Inzgr6hllSVeH-1au5VwTz_TMHhL4shMXdDTenqIV9p9K5gNqDO_?width=972&height=623&cropmode=none" style="zoom:50%;" /> | <img src="https://am3pap005files.storage.live.com/y4mFDD9m2tK3mMA6b6IEJeZOVh8IzDt_yi8YMt1w1YuVzccxW8KRad27pljtOy2UtvovuMERG-3qElfn5xtYLg4091apKdL1yBHSqQydFELjtB_m-2qBLd6CeCgxso68ZXkDjxOpw-7o6tSkzKqOOvzaUYeVYXIciMwyKJ7nzVWa3OkS3wGKTkC1ioBrmwVPAwj?width=972&height=623&cropmode=none" style="zoom:50%;" /> |

A Chart contains a collection of Series objects, which are in SeriesSource property. There are 3 groups of series:

1. PlainSeries
2. HighLowCloseSeries
3. OpenHighLowCloseSeries

Series type is defined by ChartStyle enumeration below.

Although a series collection can contain an unlimited amount of series, several chart styles will display only one series at a time, i.e. the very first series in the collection. Such styles are SolidPie, SlicedPie, Doughnut, Waterfall, Funnel, HighLowClose, OpenHighLowClose.

The example below shows how to create and use ag.WPF.Chart.

*Add ag.WPF.Chart to XAML:*

```csharp
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
                     SeriesSource="{Binding Series}" 
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

## Enumerations

### ChartStyle

Specifies chart style

Field|Value|Description
------|-----|-----------
Lines|0|The style represented by straight lines
StackedLines|1|The style represented by straight stacked lines
FullStackedLines|2|The style represented by straight 100% stacked lines
SmoothLines|3|The style represented by smooth lines
SmoothStackedLines|4|The style represented by smooth stacked lines
SmoothFullStackedLines|5|The style represented by smooth 100% stacked lines
LinesWithMarkers|6|The style represented by straight lines with markers at control points
StackedLinesWithMarkers|7|The style represented by straight stacked lines with markers at control points
FullStackedLinesWithMarkers|8|The style represented by straight 100% stacked lines with markers at control points
SmoothLinesWithMarkers|9|The style represented by smooth lines with markers at control points
SmoothStackedLinesWithMarkers|10|The style represented by smooth stacked lines with markers at control points
SmoothFullStackedLinesWithMarkers|11|The style represented by smooth 100% stacked lines with markers at control points
Columns|12|The style represented by columns
StackedColumns|13|The style represented by stacked columns
FullStackedColumns|14|The style represented by 100% stacked columns
Bars|15|The style represented by bars
StackedBars|16|The style represented by stacked bars
FullStackedBars|17|The style represented by 100% stacked bars
Area|18|The style represented by areas
StackedArea|19|The style represented by stacked areas
FullStackedArea|20|The style represented by 100% stacked areas
SmoothArea|21|The style represented by smooth areas
SmoothStackedArea|22|The style represented by smooth stacked areas
SmoothFullStackedArea|23|The style represented by 100% smooth stacked areas
Bubbles|24|The style represented by bubbles
SolidPie|25|The style represented by solid sectors
SlicedPie|26|The style represented by sectors divided with thin lines
Doughnut|27|The style represented by solid arcs
Waterfall|28|The style represented by cumulated effect of positive and negative values
Radar|29|The style represented by radar
RadarWithMarkers|30|The style represented by radar with markers at control points
RadarArea|31|The style represented by radar areas
Funnel|32|The style represented by funnel
HighLowClose|33|The stock style represented by high, low, and close prices
OpenHighLowClose|34|The stock style represented by open, high, low, and close prices

### LegendAlignment

Specifies chart legend alignment

Field|Value|Description
------|-----|-----------
Left|0|Legend is left aligned (default)
Top|1|Legend is top aligned
Right|2|Legend is right aligned
Bottom|3|Legend is bottom aligned

### AxesVisibility

Specifies visiblity of numeric/custom values drawn next to axes

Field|Value|Description
------|-----|-----------
None|0|No values are visible
Horizontal|1|Horizontal (X) axis values are visible
Vertical|2|Vertical (Y) axis values are visible
Both|3|Values of both axes are visible (default)

### AutoAdjustmentMode

Specifies auto-adjustment mode of chart control

Field|Value|Description
------|-----|-----------
None|0|No auto-adjustment
Horizontal|1|Horizontal values are auto-adjusted
Vertical|2|Vertical values are auto-adjusted
Both|3|Both horizontal and vertical values are auto-adjusted (default)

### LegendSize

Specifies size of legend
Field|Value|Description
------|-----|-----------
ExtraSmall|16|16x16 size (default)
Small|24|24x24 size
Menium|32|32x32 size
Large|48|48x48 size
ExtraLarge|64|64x64 size

### ShapeStyle

Specifies shape of legend

Field|Value|Description
------|-----|-----------
Rectangle|0|Rectanglular shape (default)
Circle|1|Circular shape
Star5|2|Star shape with five rays
Star6|3|Star shape with six rays
Star8|4|Star shape with eight rays

### ChartBoundary

Specifies chart boundary

Field|Value|Description
------|-----|-----------
OnAxes|0|Chart boundary starts on y-axes
WithOffset|1|Chart boundary starts with offset from y-axes (default)

## Chart class

### Dependency properties

Property|Type|Description|Category
------|-----|-----------|---------
LegendsOpenHighLowClose|IEnumerable&lt;string&gt;|Gets or sets the collection of custom legend text when ChartStyle is set to OpenHighLowClose|ChartLegend
LegendsHighLowClose|IEnumerable&lt;string&gt;|Gets or sets the collection of custom legend text when ChartStyle is set to HighLowClose|ChartLegend
LegendsWaterfall|IEnumerable&lt;string&gt;|Gets or sets the collection of custom legend text when ChartStyle is set to Waterfall|ChartLegend
LegendFontFamily|FontFamily|ets or sets the chart legend's font family|ChartLegend
LegendFontWeight|FontWeight|Gets or sets the chart legend's font wweight|ChartLegend
LegendFontStyle|FontStyle|Gets or sets the chart legend's font style|ChartLegend
LegendFontSize|double|Gets or sets the chart legend's font size|ChartLegend
LegendShape|ShapeStyle|Gets or sets the shape of the chart legend. Can be one of ShapeStyle enumeration members|ChartLegend
LegendSize|LegendSize|Gets or sets the size of the chart legend. Can be one of LegendSize enumeration members|ChartLegend
ShowLegend|bool|Specifies whether chart legends should be shown|ChartLegend
LegendAlignment|LegendAlignment|"Gets or sets the chart legend alignment. Can be one of LegendAlignment enumeration members|ChartLegend
SeriesSource|IEnumerable&lt;ISeries>&gt;|Gets or sets the collection of Series objects associated with chart control|ChartAppearance
ChartBoundary|ChartBoundary|Specifies whether chart boundary is started on y-axes or with offfset from y-axes|ChartAppearance
ShowTicks|AxesVisibility|Specifies whether ticks are drawn on axes. Can be one of AxesValuesVisibility enumeration members|ChartAppearance
AutoAdjustment|AutoAdjustmentMode|Specifies whether control will automatically adjust its max x- and y- values or they should be set explicitly. Can be one of AutoAdjustmentMode enumeration members|ChartAppearance
MarkerShape|ShapeStyle|Gets or sets the shape of chart series markers. Can be one of ShapeStyle enumeration members|ChartAppearance
MaxY|double|Gets or sets max numeric value for vertical axis. The default value is 100|ChartMeasures
MaxX|double|Gets or sets max numeric value for horizontal axis. The default value is 100|ChartMeasures
LineThickness|double|Gets or sets the thickness of the chart lines. The default value is 2.0 px|ChartAppearance
ChartOpacity|double|Gets or set the chart opacity in range of 0.0 (fully transparent) to 1.0 (fully opaque)|ChartAppearance
CustomValuesX|IEnumerable&lt;string&gt;|Gets or sets the custom sequence of strings to be drawn next to x-axis instead of numeric values|ChartAxes
CustomValuesY|IEnumerable&lt;string&gt;|Gets or sets the custom sequence of strings to be drawn next to y-axis instead of numeric values|ChartAxes
AxesValuesVisibility|AxesVisibility|Gets or sets the visibility state of x- and y- axes numeric/custom values. Can be one of AxesValuesVisibility enumeration members|ChartAxes
AxesLinesVisibility|AxesVisibility|Gets or sets the visibility state of x- and y- axes lines. Can be one of AxesValuesVisibility enumeration members|ChartAxes
SecondaryLinesVisibility|AxesVisibility|Gets or sets the visibility state of secondary horizontal and vertical lines. Can be one of AxesValuesVisibility enumeration members|ChartAppearance
ChartStyle|ChartStyle|Gets or sets the chart style. Can be one of ChartStyle enumeration members|ChartAppearance
ShowValuesOnBarsAndColumns|bool|Specifies whether values should be drawn on bars and columns|ChartAppearance
Title|string|Gets or sets the chart's title|ChartTitle
TitleFontSize|double|Gets of sets the font size of the chart's title|ChartTitle
TitleFontStyle|FontStyle|Gets of sets the font style of the chart's title|ChartTitle
TitleFontWeight|FontWeight|Gets of sets the font weight of the chart's title|ChartTitle
TitleFontFamily|FontFamily|Gets of sets the font family of the chart's title|ChartTitle
TitleFontStretch|FontStretch|Gets of sets the font stretch of the chart's title|ChartTitle
AxesFontSize|double|Gets of sets the font size of the chart's axes|ChartAxes
AxesFontStyle|FontStyle|Gets of sets the font style of the chart's axes|ChartAxes
AxesFontWeight|FontWeight|Gets of sets the font weight of the chart's axes|ChartAxes
AxesFontFamily|FontFamily|Gets of sets the font family of the chart's axes|ChartAxes
AxesFontStretch|FontStretch|Gets of sets the font stretch of the chart's axes|ChartAxes
AxisTitleX|string|Gets or sets the text which appears on right/left of the horizontal axis|ChartAxes
AxisTitleY|string|Gets or sets the text which appears on the top/bottom of the vertical axis|ChartAxes
SectionsX|int|Gets or sets the amount of vertical lines|ChartMeasures
SectionsY|int|Gets or sets the amount of horizontal lines|ChartMeasures
HorizontalAxisValuesFormat|string|Gets or sets the format for numeric values drawn next to the horizontal axis|ChartAxes
VerticalAxisValuesFormat|string|Gets or sets the format for numeric values drawn next to the vertical axis|ChartAxes
PiePercentsFormat|string|Gets or sets the numeric format for pie percents|ChartAppearance

### Properties

Property|Type|Description
------|-----|-----------
SeriesItems|ChartItemsCollection&lt;ISeries>&gt;|Gets the collection of ISeries objects used to generate the content of the  Chart  control
