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
                Values.Add(new StockChartValue(v, null));
            }

            InitFields(name);
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified name and sequence of values
        /// </summary>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(string name, IEnumerable<(double highValue, double lowValue, double closeValue, string customValue)> values)
        {
            foreach (var (highValue, lowValue, closeValue, customValue) in values)
            {
                Values.Add(new StockChartValue((highValue, lowValue, closeValue), customValue));
            }

            InitFields(name);
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified name and sequence of values
        /// </summary>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(string name, IEnumerable<(double volumeValue, double highValue, double lowValue, double closeValue)> values)
        {
            foreach (var v in values)
            {
                Values.Add(new StockChartValue(v, null));
            }

            InitFields(name);
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified name and sequence of values
        /// </summary>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(string name, IEnumerable<(double volumeValue, double highValue, double lowValue, double closeValue, string customValue)> values)
        {
            foreach (var (volumeValue, highValue, lowValue, closeValue, customValue) in values)
            {
                Values.Add(new StockChartValue((volumeValue, highValue, lowValue, closeValue), customValue));
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
                Values.Add(new StockChartValue(v, null));
            }

            InitFields(name);
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified name and sequence of values
        /// </summary>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(string name, IEnumerable<(double volumeValue, double openValue, double highValue, double lowValue, double closeValue, string customValue)> values)
        {
            foreach (var (volumeValue, openValue, highValue, lowValue, closeValue, customValue) in values)
            {
                Values.Add(new StockChartValue((volumeValue, openValue, highValue, lowValue, closeValue), customValue));
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
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(Brush mainBrush, string name, IEnumerable<(double highValue, double lowValue, double closeValue, string customValue)> values)
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
        /// <param name="secondaryBrush">Series secondary background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(Brush mainBrush, Brush secondaryBrush, string name, IEnumerable<(double highValue, double lowValue, double closeValue, string customValue)> values)
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
        public StockSeries(Brush mainBrush, string name, IEnumerable<(double volumeValue, double highValue, double lowValue, double closeValue)> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified brush, name and sequence of values
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(Brush mainBrush, string name, IEnumerable<(double volumeValue, double highValue, double lowValue, double closeValue, string customValue)> values)
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
        public StockSeries(Brush mainBrush, Brush secondaryBrush, string name, IEnumerable<(double volumeValue, double highValue, double lowValue, double closeValue)> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
            SecondaryBrush = secondaryBrush;
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified brush, name and sequence of values
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="secondaryBrush">Series secondary background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(Brush mainBrush, Brush secondaryBrush, string name, IEnumerable<(double volumeValue, double highValue, double lowValue, double closeValue, string customValue)> values)
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
        public StockSeries(Brush mainBrush, string name, IEnumerable<(double volumeValue, double openValue, double highValue, double lowValue, double closeValue)> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
        }

        /// <summary>
        /// Initializes a new instance of StockSeries object using specified brush, name and sequence of values
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(Brush mainBrush, string name, IEnumerable<(double volumeValue, double openValue, double highValue, double lowValue, double closeValue, string customValue)> values)
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
        /// Initializes a new instance of StockSeries object using specified brush, name and sequence of values
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="secondaryBrush">Series secondary background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public StockSeries(Brush mainBrush, Brush secondaryBrush, string name, IEnumerable<(double volumeValue, double openValue, double highValue, double lowValue, double closeValue, string customValue)> values)
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
