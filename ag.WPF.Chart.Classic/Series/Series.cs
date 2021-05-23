﻿using ag.WPF.Chart.Values;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// <summary>
    /// Represents the basic abstract class of series.
    /// </summary>
    public abstract class Series : DependencyObject, ISeries
    {
        #region Private fields
        private Brush _mainBrush;
        private Brush _secondaryBrush;
        private string _name;
        private int _index;
        private ChartItemsCollection<IChartValue> _values = new ChartItemsCollection<IChartValue>();
        #endregion

        #region Dependency properties
        /// <summary>
        /// The identifier of the <see cref="ValuesSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValuesSourceProperty = DependencyProperty.Register(nameof(ValuesSource), typeof(IEnumerable<IChartValue>), typeof(Series), new FrameworkPropertyMetadata(null));
        #endregion

        #region Public properties
        /// <inheritdoc />
        public IEnumerable<IChartValue> ValuesSource
        {
            get
            {
                var b = BindingOperations.GetBinding(this, ValuesSourceProperty);
                if (b != null)
                    return (IEnumerable<IChartValue>)GetValue(ValuesSourceProperty);
                else
                    return Values;
            }            
            set { SetValue(ValuesSourceProperty, value); }
        }
        /// <inheritdoc />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [EditorBrowsable(EditorBrowsableState.Never), Bindable(false)]
        public ChartItemsCollection<IChartValue> Values
        {
            get 
            {
                var b = BindingOperations.GetBinding(this, ValuesSourceProperty);
                if (b == null)
                {
                    if (_values == null)
                    {
                        _values = new ChartItemsCollection<IChartValue>();
                        _values.CollectionChanged += values_CollectionChanged;
                    }
                }
                else
                {
                    if (ValuesSource != null)
                        _values = new ChartItemsCollection<IChartValue>(ValuesSource);
                    else
                    {
                        if (_values == null)
                        {
                            _values = new ChartItemsCollection<IChartValue>();
                            _values.CollectionChanged += values_CollectionChanged;
                        }
                    }
                }
                return _values;
            }
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
        /// <inheritdoc />
        public List<Rect> RealRects { get; } = new List<Rect>();
        /// <inheritdoc />
        public List<Point> RealPoints { get; } = new List<Point>();
        /// <inheritdoc />
        public Path[] Paths { get; } = new Path[9];
        #endregion

        #region Event handlers
        private void values_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
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

            Name = name;
            Paths[0] = new Path
            {
                StrokeLineJoin = PenLineJoin.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeDashCap = PenLineCap.Round,
                Tag = this,
                ToolTip = new ToolTip { Placement = PlacementMode.Mouse }
            };
            Paths[1] = new Path
            {
                StrokeLineJoin = PenLineJoin.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeDashCap = PenLineCap.Round,
                Tag = this,
                ToolTip = new ToolTip { Placement = PlacementMode.Mouse }
            };
            if (this.IsStockSeries())
            {
                Paths[2] = new Path
                {
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
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
