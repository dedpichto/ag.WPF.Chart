using ag.WPF.Chart.Values;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ag.WPF.Chart.Series
{
    /// <summary>
    /// Represents single chart series
    /// </summary>
    public interface ISeries : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets series index
        /// </summary>
        int Index { get; set; }
        /// <summary>
        /// Gets or sets series name
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Gets or sets series background
        /// </summary>
        Brush MainBrush { get; set; }
        /// <summary>
        /// Gets or sets series seconday background
        /// </summary>
        Brush SecondaryBrush { get; set; }

        /// <summary>
        /// Gets array of 10 brushes used for drawing chart sectors when <see cref="ChartStyle"/> property of control is set to <see cref="ChartStyle.SolidPie"/> or <see cref="ChartStyle.SlicedPie"/> or <see cref="ag.WPF.Chart.ChartStyle.Doughnut"/>
        /// </summary>
        /// <remarks>
        /// Each series has its own copy of brushes array. By default all brushes are set to solid brushes equals with colors from Office 2013 chart color scheme, but you can replace them with your own brushes.
        /// </remarks>
        BrushesCollection PieBrushes { get; }
        /// <summary>
        /// Gets the collection of <see cref="IChartValue"/> objects associated with current series
        /// </summary>
        ChartValues Values { get; }
        /// <summary>
        /// Gets series drawing path
        /// </summary>
        Path Path { get; }
        /// <summary>
        /// Gets drawing path for <see cref="ChartStyle.Waterfall"/> positive values
        /// </summary>
        Path PositivePath { get; }
        /// <summary>
        /// Gets drawing path for <see cref="ChartStyle.Waterfall"/> negative values
        /// </summary>
        Path NegativePath { get; }
        /// <summary>
        /// Gets real coordinates of series rectangles
        /// </summary>
        List<Rect> RealRects { get; }
        /// <summary>
        /// Gets real coordinates of series points
        /// </summary>
        List<Point> RealPoints { get; }
        /// <summary>
        /// Gets real coordinates of stock series high value rectangles
        /// </summary>
        List<Rect> RealStockHighRects { get; }
        /// <summary>
        /// Gets real coordinates of stock series low value rectangles
        /// </summary>
        List<Rect> RealStockLowRects { get; }
    }
}