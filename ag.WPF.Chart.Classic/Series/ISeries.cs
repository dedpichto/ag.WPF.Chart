using ag.WPF.Chart.Values;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ag.WPF.Chart.Series
{
    /// <summary>
    /// Represents ISeries interface.
    /// </summary>
    public interface ISeries : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets series index.
        /// </summary>
        int Index { get; set; }
        /// <summary>
        /// Gets or sets series name.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Gets or sets series background.
        /// </summary>
        Brush MainBrush { get; set; }
        /// <summary>
        /// Gets or sets series seconday background.
        /// </summary>
        Brush SecondaryBrush { get; set; }
        /// <summary>
        /// Gets the collection of <see cref="IChartValue"/> objects associated with current series.
        /// </summary>
        ChartItemsCollection<IChartValue> Values { get; }
        /// <summary>
        /// Gets or sets the collection of <see cref="IChartValue"/> objects associated with chart control.
        /// </summary>
        IEnumerable<IChartValue> ValuesSource { get; set; }
        /// <summary>
        /// Gets real coordinates of series rectangles.
        /// </summary>
        List<Rect> RealRects { get; }
        /// <summary>
        /// Gets real coordinates of series points.
        /// </summary>
        List<Point> RealPoints { get; }
        /// <summary>
        /// Gets array of series drawing paths.
        /// </summary>
        Path[] Paths { get; }
    }
}