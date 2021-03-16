﻿// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFChart.Annotations;

namespace WPFChart
{
    /// <summary>
    /// Represents single chart series
    /// </summary>
    public class Series : DependencyObject, INotifyPropertyChanged
    {
        internal static readonly DependencyProperty SectorDataProperty = DependencyProperty.RegisterAttached("SectorData",
            typeof(string), typeof(Series), new FrameworkPropertyMetadata("", null));

        internal static readonly Brush[] PredefinedPieBrushes =
        {
            new SolidColorBrush(Color.FromArgb(255, 91, 155, 213)),
            new SolidColorBrush(Color.FromArgb(255, 237, 125, 49)),
            new SolidColorBrush(Color.FromArgb(255, 165, 165, 165)),
            new SolidColorBrush(Color.FromArgb(255, 255, 192, 0)),
            new SolidColorBrush(Color.FromArgb(255, 68, 114, 196)),
            new SolidColorBrush(Color.FromArgb(255, 112, 173, 71)),
            new SolidColorBrush(Color.FromArgb(255, 37, 94, 145)),
            new SolidColorBrush(Color.FromArgb(255, 158, 72, 14)),
            new SolidColorBrush(Color.FromArgb(255, 99, 99, 99)),
            new SolidColorBrush(Color.FromArgb(255, 153, 115, 0))
        };
        private Brush _mainBrush;
        private Brush _secondaryBrush;
        private string _name;
        private int _index;
        private readonly ChartValues _values = new ChartValues();
        private readonly List<Rect> _realRects = new List<Rect>();
        private readonly List<Point> _realPoints = new List<Point>();
        private readonly BrushesCollection _pieBrushes;

        internal static string GetSectorData(DependencyObject obj)
        {
            return (string)obj.GetValue(SectorDataProperty);
        }

        internal static void SetSectorData(DependencyObject obj, string value)
        {
            obj.SetValue(SectorDataProperty, value);
        }

        /// <summary>
        /// Gets the collection of <see cref="ChartValue"/> objects associated with current series
        /// </summary>
        public ChartValues Values
        {
            get { return _values; }
        }
        /// <summary>
        /// Gets or sets series background
        /// </summary>
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
        /// <summary>
        /// Gets or sets series seconday background
        /// </summary>
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
        /// <summary>
        /// Gets or sets series name
        /// </summary>
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
        /// <summary>
        /// Gets or sets series index
        /// </summary>
        public int Index
        {
            get { return _index; }
            set
            {
                if (_index == value) return; _index = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Gets array of 10 brushes used for drawing chart sectors when <see cref="WPFChart.Chart.ChartStyle"/> property of control is set to <see cref="ChartStyle.SolidPie"/> or <see cref="ChartStyle.SlicedPie"/> or <see cref="WPFChart.ChartStyle.Doughnut"/>
        /// </summary>
        /// <remarks>
        /// Each series has its own copy of brushes array. By default all brushes are set to solid brushes equals with colors from Office 2013 chart color scheme, but you can replace them with your own brushes.
        /// </remarks>
        public BrushesCollection PieBrushes
        {
            get { return _pieBrushes; }
        }

        internal List<Rect> RealRects
        {
            get { return _realRects; }
        }

        internal Path Path { get; private set; }
        internal Path PositivePath { get; private set; }
        internal Path NegativePath { get; private set; }

        internal List<Point> RealPoints
        {
            get { return _realPoints; }
        }

        /// <summary>
        /// Initializes a new instance of Series object using specified name and sequence of values
        /// </summary>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public Series(string name, IEnumerable<double> values)
        {
            foreach (var v in values)
            {
                _values.Add(new ChartValue(v, null));
            }

            _values.CollectionChanged += Values_CollectionChanged;
            _pieBrushes = new BrushesCollection(PredefinedPieBrushes.Length, this);
            var brushes = PredefinedPieBrushes.OfType<SolidColorBrush>().ToArray();
            for (var i = 0; i < _pieBrushes.Length(); i++)
                _pieBrushes[i] = new SolidColorBrush(brushes[i].Color);
            Name = name;
            Path = new Path
            {
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeDashCap = PenLineCap.Round,
                Tag = this,
                ToolTip = new ToolTip { Placement = PlacementMode.Mouse }
            };
            PositivePath = new Path
            {
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeDashCap = PenLineCap.Round,
                Tag = this,
                ToolTip = new ToolTip { Placement = PlacementMode.Mouse }
            };
            NegativePath = new Path
            {
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeDashCap = PenLineCap.Round,
                Tag = this,
                ToolTip = new ToolTip { Placement = PlacementMode.Mouse }
            };
            _values.Path = Path;
        }

        /// <summary>
        /// Initializes a new instance of Series object using specified brush, name and sequence of values
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public Series(Brush mainBrush, string name, IEnumerable<double> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
        }

        /// <summary>
        /// Initializes a new instance of Series object using specified brush, name and sequence of values
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="secondaryBrush">Series secondary background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public Series(Brush mainBrush, Brush secondaryBrush, string name, IEnumerable<double> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
            SecondaryBrush = secondaryBrush;
        }

        /// <summary>
        /// Initializes a new instance of Series object using specified name and sequence of ChartValue objects
        /// </summary>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public Series(string name, IEnumerable<ChartValue> values)
        {
            foreach (var v in values)
            {
                _values.Add(new ChartValue(v.Value, v.CustomValue));
            }

            _values.CollectionChanged += Values_CollectionChanged;
            _pieBrushes = new BrushesCollection(PredefinedPieBrushes.Length, this);
            var brushes = PredefinedPieBrushes.OfType<SolidColorBrush>().ToArray();
            for (var i = 0; i < _pieBrushes.Length(); i++)
                _pieBrushes[i] = new SolidColorBrush(brushes[i].Color);
            Name = name;
            Path = new Path
            {
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeDashCap = PenLineCap.Round,
                Tag = this,
                ToolTip = new ToolTip { Placement = PlacementMode.Mouse }
            };
            PositivePath = new Path
            {
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeDashCap = PenLineCap.Round,
                Tag = this,
                ToolTip = new ToolTip { Placement = PlacementMode.Mouse }
            };
            NegativePath = new Path
            {
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeDashCap = PenLineCap.Round,
                Tag = this,
                ToolTip = new ToolTip { Placement = PlacementMode.Mouse }
            };
            _values.Path = Path;
        }

        /// <summary>
        /// Initializes a new instance of Series object using specified brush, name and sequence of ChartValue objects
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public Series(Brush mainBrush, string name, IEnumerable<ChartValue> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
        }

        /// <summary>
        /// Initializes a new instance of Series object using specified brush, name and sequence of ChartValue objects
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="secondaryBrush">Series secondary background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public Series(Brush mainBrush, Brush secondaryBrush, string name, IEnumerable<ChartValue> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
            SecondaryBrush = secondaryBrush;
        }

        private void Values_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!(sender is ChartValues chartValues)) return;
            var multiBindiing = BindingOperations.GetMultiBindingExpression(chartValues.Path, Path.DataProperty);
            if (multiBindiing != null)
                multiBindiing.UpdateTarget();
            OnPropertyChanged("Values");
        }

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
    }

    /// <summary>
    /// Represents collection of <see cref="ChartValue"/>
    /// </summary>
    public class ChartValues : ObservableCollection<ChartValue>
    {
        internal Path Path { get; set; }
    }

    /// <summary>
    /// Represents single chart value
    /// </summary>
    public class ChartValue
    {
        /// <summary>
        /// Gets or sets current numeric value
        /// </summary>
        public double Value { get; set; }
        /// <summary>
        /// Gets or sets custom value (usually string) associated with current value. This custom value will be displayed as chart point tooltip
        /// </summary>
        public object CustomValue { get; set; }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="value">Current value</param>
        public ChartValue(double value)
        {
            Value = value;
        }
        /// <summary>
        /// Initializes a new instance of ChartValue object
        /// </summary>
        /// <param name="value">Current value</param>
        /// <param name="customValue">Current custom value</param>
        public ChartValue(double value, object customValue)
            : this(value)
        {
            CustomValue = customValue;
        }
    }
}
