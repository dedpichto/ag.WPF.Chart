using ag.WPF.Chart.Annotations;
using ag.WPF.Chart.Values;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ag.WPF.Chart.Series
{
    /// <inheritdoc />
    public class StockSeries : Series
    {
        #region ctor
        /// <summary>
        /// Initializes a new instance of StockSeries object using specified name and sequence of values
        /// </summary>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(string name, IEnumerable<(double highValue, double lowValue, double closeValue)> values)
        {
            foreach (var v in values)
            {
                Values.Add(new StockChartValue(v.highValue, v.lowValue, v.closeValue));
            }

            InitFields(name);
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified name and sequence of values
        /// </summary>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        /// <param name="volumeOrClose">Boolen, specifies whether Volume or Open value should be set. Send True for Volume and False for Open</param>
        public StockSeries(string name, IEnumerable<(double volumeValue, double highValue, double lowValue, double closeValue)> values, bool volumeOrClose)
        {
            foreach (var v in values)
            {
                Values.Add(new StockChartValue(v.volumeValue, v.highValue, v.lowValue, v.closeValue, volumeOrClose));
            }

            InitFields(name);
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified name and sequence of values
        /// </summary>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(string name, IEnumerable<(double volumeValue, double openValue, double highValue, double lowValue, double closeValue)> values)
        {
            foreach (var v in values)
            {
                Values.Add(new StockChartValue(v.volumeValue, v.openValue, v.highValue, v.lowValue, v.closeValue));
            }

            InitFields(name);
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified brush, name and sequence of values
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(Brush mainBrush, string name, IEnumerable<(double highValue, double lowValue, double closeValue)> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified brush, name and sequence of values
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="secondaryBrush">Series secondary background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(Brush mainBrush, Brush secondaryBrush, string name, IEnumerable<(double highValue, double lowValue, double closeValue)> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
            SecondaryBrush = secondaryBrush;
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified brush, name and sequence of values
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        /// <param name="volumeOrClose">Boolen, specifies whether Volume or Open value should be set. Send True for Volume and False for Open</param>
        public StockSeries(Brush mainBrush, string name, IEnumerable<(double volumeValue, double highValue, double lowValue, double closeValue)> values, bool volumeOrClose)
            : this(name, values, volumeOrClose)
        {
            MainBrush = mainBrush;
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified brush, name and sequence of values
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="secondaryBrush">Series secondary background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        /// <param name="volumeOrClose">Boolen, specifies whether Volume or Open value should be set. Send True for Volume and False for Open</param>
        public StockSeries(Brush mainBrush, Brush secondaryBrush, string name, IEnumerable<(double volumeValue, double highValue, double lowValue, double closeValue)> values, bool volumeOrClose)
            : this(name, values, volumeOrClose)
        {
            MainBrush = mainBrush;
            SecondaryBrush = secondaryBrush;
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified brush, name and sequence of values
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(Brush mainBrush, string name, IEnumerable<(double volumeValue, double openValue, double highValue, double lowValue, double closeValue)> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified brush, name and sequence of values
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="secondaryBrush">Series secondary background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(Brush mainBrush, Brush secondaryBrush, string name, IEnumerable<(double volumeValue, double openValue, double highValue, double lowValue, double closeValue)> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
            SecondaryBrush = secondaryBrush;
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified name and sequence of StockChartValue objects
        /// </summary>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(string name, IEnumerable<IChartValue> values)
        {
            foreach (var v in values)
            {
                Values.Add(v.Clone());
            }
            InitFields(name);
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified brush, name and sequence of StockChartValue objects
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(Brush mainBrush, string name, IEnumerable<IChartValue> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified brush, name and sequence of StockChartValue objects
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="secondaryBrush">Series secondary background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(Brush mainBrush, Brush secondaryBrush, string name, IEnumerable<IChartValue> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
            SecondaryBrush = secondaryBrush;
        }
        #endregion
    }
}
