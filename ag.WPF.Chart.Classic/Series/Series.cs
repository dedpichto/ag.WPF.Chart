﻿using ag.WPF.Chart.Values;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
#nullable disable
        #region Private fields
        private Brush _mainBrush;
        private Brush _secondaryBrush;
        private string _name;
        private int _index;
        private ChartItemsCollection<IChartValue> _values = new();
        private bool _isVisible = true;
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
                return b != null ? (IEnumerable<IChartValue>)GetValue(ValuesSourceProperty) : Values;
            }
            set => SetValue(ValuesSourceProperty, value);
        }
        /// <inheritdoc />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [EditorBrowsable(EditorBrowsableState.Never), Bindable(false)]
        public ChartItemsCollection<IChartValue> Values
        {
            get
            {
                var b = BindingOperations.GetBinding(this, ValuesSourceProperty);
                if (b != null)
                {
                    if (ValuesSource != null)
                    {
                        _values.CollectionChanged -= values_CollectionChanged;
                        _values = new ChartItemsCollection<IChartValue>(ValuesSource);
                        _values.CollectionChanged += values_CollectionChanged;
                    }
                }
                return _values;
            }
        }
        /// <inheritdoc />
        public Brush MainBrush
        {
            get => _mainBrush;
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
            get => _secondaryBrush;
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
            get => _name;
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
            get => _index;
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
        public Path[] Paths { get; } = new Path[3];
        /// <inheritdoc />
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return; _isVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Event handlers
        private void values_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var p in Paths)
            {
                if (p == null) continue;
                var binding = BindingOperations.GetMultiBindingExpression(p, Path.DataProperty);
                binding?.UpdateTarget();
            }
            OnPropertyChanged(nameof(Values));
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
        }
        #endregion

        #region INotifyPropertyChanged members
        /// <summary>
        /// Raises the PropertyChanged event when the property value has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
#nullable restore
    }
}
