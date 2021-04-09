using ag.WPF.Chart.Annotations;
using ag.WPF.Chart.Values;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ag.WPF.Chart.Series
{
    public abstract class Series : DependencyObject, ISeries
    {
        #region Private fields
        private Brush _mainBrush;
        private Brush _secondaryBrush;
        private string _name;
        private int _index;
        private readonly ChartValues _values = new ChartValues();
        #endregion

        #region Public properties
        /// <inheritdoc />
        public ChartValues Values
        {
            get { return _values; }
        }
        /// <inheritdoc />
        public Brush MainBrush
        {
            get { return _mainBrush; }
            set
            {
                if (Equals(_mainBrush, value)) return;
                _mainBrush = value;
                OnPropertyChanged();
            }
        }
        /// <inheritdoc />
        public Brush SecondaryBrush
        {
            get { return _secondaryBrush; }
            set
            {
                if (Equals(_secondaryBrush, value)) return;
                _secondaryBrush = value;
                OnPropertyChanged();
            }
        }
        /// <inheritdoc />
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.Equals(_name, value, StringComparison.Ordinal)) return;
                _name = value;
                OnPropertyChanged();
            }
        }
        /// <inheritdoc />
        public int Index
        {
            get { return _index; }
            set
            {
                if (_index == value) return; _index = value;
                OnPropertyChanged();
            }
        }

        ///// <inheritdoc />
        //public BrushesCollection PieBrushes { get; private set; }
        /// <inheritdoc />
        public List<Rect> RealRects { get; } = new List<Rect>();
        /// <inheritdoc />
        public List<Rect> RealStockHighRects { get; } = new List<Rect>();
        /// <inheritdoc />
        public List<Rect> RealStockLowRects { get; } = new List<Rect>();
        ///// <inheritdoc />
        //public Path Path { get; private set; }
        ///// <inheritdoc />
        //public Path PositivePath { get; private set; }
        ///// <inheritdoc />
        //public Path NegativePath { get; private set; }
        ///// <inheritdoc />
        //public Path StockPath { get; private set; }
        /// <inheritdoc />
        public List<Point> RealPoints { get; } = new List<Point>();
        /// <inheritdoc />
        public Path[] Paths { get; } = new Path[9];
        #endregion

        #region Event handlers
        private void values_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!(sender is ChartValues chartValues)) return;
            foreach (var p in Paths)
            {
                if (p == null) continue;
                var binding = BindingOperations.GetMultiBindingExpression(p, Path.DataProperty);
                if (binding != null)
                    binding.UpdateTarget();
            }
            OnPropertyChanged("Values");
        }
        #endregion

        #region Internal methods
        internal void InitFields(string name)
        {
            _values.CollectionChanged += values_CollectionChanged;
            //PieBrushes = new BrushesCollection(Statics.PredefinedMainBrushes.Length, this);

            //for (var i = 0; i < PieBrushes.Length(); i++)
            //    PieBrushes[i] = Statics.PredefinedMainBrushes[i].Brush;
            Name = name;
            Paths[0] = new Path
            {
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeDashCap = PenLineCap.Round,
                Tag = this,
                ToolTip = new ToolTip { Placement = PlacementMode.Mouse }
            };
            Paths[1] = new Path
            {
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeDashCap = PenLineCap.Round,
                Tag = this,
                ToolTip = new ToolTip { Placement = PlacementMode.Mouse }
            };
            if (this is StockSeries)
            {
                Paths[2] = new Path
                {
                    StrokeThickness = 2,
                    StrokeLineJoin = PenLineJoin.Miter,
                    StrokeEndLineCap = PenLineCap.Flat,
                    StrokeStartLineCap = PenLineCap.Flat,
                    StrokeDashCap = PenLineCap.Flat,
                    Tag = this,
                    ToolTip = new ToolTip { Placement = PlacementMode.Mouse }
                };
            }
            //Path = new Path
            //{
            //    StrokeThickness = 2,
            //    StrokeLineJoin = PenLineJoin.Round,
            //    StrokeEndLineCap = PenLineCap.Round,
            //    StrokeStartLineCap = PenLineCap.Round,
            //    StrokeDashCap = PenLineCap.Round,
            //    Tag = this,
            //    ToolTip = new ToolTip { Placement = PlacementMode.Mouse }
            //};
            //PositivePath = new Path
            //{
            //    StrokeThickness = 2,
            //    StrokeLineJoin = PenLineJoin.Miter,
            //    StrokeEndLineCap = PenLineCap.Flat,
            //    StrokeStartLineCap = PenLineCap.Flat,
            //    StrokeDashCap = PenLineCap.Flat,
            //    Tag = this,
            //    ToolTip = new ToolTip { Placement = PlacementMode.Mouse }
            //};
            //NegativePath = new Path
            //{
            //    StrokeThickness = 2,
            //    StrokeLineJoin = PenLineJoin.Miter,
            //    StrokeEndLineCap = PenLineCap.Flat,
            //    StrokeStartLineCap = PenLineCap.Flat,
            //    StrokeDashCap = PenLineCap.Flat,
            //    Tag = this,
            //    ToolTip = new ToolTip { Placement = PlacementMode.Mouse }
            //};
            //StockPath = new Path
            //{
            //    StrokeThickness = 2,
            //    StrokeLineJoin = PenLineJoin.Miter,
            //    StrokeEndLineCap = PenLineCap.Flat,
            //    StrokeStartLineCap = PenLineCap.Flat,
            //    StrokeDashCap = PenLineCap.Flat,
            //    Tag = this,
            //    ToolTip = new ToolTip { Placement = PlacementMode.Mouse }
            //};
            //_values.Path = Path;
        }
        #endregion

        #region INotifyPropertyChanged members
        /// <summary>
        /// Raises the PropertyChanged event when the property value has changed
        /// </summary>
        /// <param name="propertyName">Property name</param>
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
