using System;
using System.Windows.Media;
using ag.WPF.Chart.Series;

namespace ag.WPF.Chart
{
    /// <summary>
    /// Represents indexer for storing brushes used for drawing chart sectors when <see cref="ag.WPF.Chart.Chart.ChartStyle"/> property of control is set to <see cref="ChartStyle.SolidPie"/> or <see cref="ChartStyle.SlicedPie"/> 
    /// </summary>
    public class BrushesCollection
    {
        internal event EventHandler<BrushChangedEventArgs> BrushChanged;

        private readonly Brush[] _Brushes;
        private readonly ISeries _Series;
        internal BrushesCollection(int capacity, ISeries series)
        {
            _Brushes = new Brush[capacity];
            _Series = series;
        }

        /// <summary>
        /// Gets length of brushes array
        /// </summary>
        /// <returns>Length of brushes array</returns>
        public int Length()
        {
            return _Brushes.Length;
        }
        /// <summary>
        /// Gets or sets brush accordingly to specified index
        /// </summary>
        /// <param name="index">Brush index</param>
        /// <returns>Brush for appropriate index</returns>
        public Brush this[int index]
        {
            get { return _Brushes[index]; }
            set
            {
                if (Equals(_Brushes[index], value))
                    return;
                _Brushes[index] = value;
                if (BrushChanged == null) return;
                BrushChanged(this, new BrushChangedEventArgs(_Series, index));
            }
        }
    }

    internal class BrushChangedEventArgs : EventArgs
    {
        internal ISeries Series { get; private set; }
        internal int Index { get; private set; }

        internal BrushChangedEventArgs(ISeries series, int index)
        {
            Series = series;
            Index = index;
        }
    }
}