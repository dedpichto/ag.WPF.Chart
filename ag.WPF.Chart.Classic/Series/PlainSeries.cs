using ag.WPF.Chart.Values;
using System.Collections.Generic;
using System.Windows.Media;

namespace ag.WPF.Chart.Series
{
    /// <summary>
    /// Represents plain series object with one value for each series point.
    /// </summary>
    public class PlainSeries : Series
    {
#nullable disable
        #region ctor
        /// <summary>
        /// Initializes a new instance of Series object using specified name and sequence of values
        /// </summary>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public PlainSeries(string name, IEnumerable<double> values)
        {
            foreach (var v in values)
            {
                Values.Add(new PlainChartValue(v, null));
            }

            InitFields(name);
        }

        /// <summary>
        /// Initializes a new instance of Series object using specified brush, name and sequence of values
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public PlainSeries(Brush mainBrush, string name, IEnumerable<double> values)
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
        public PlainSeries(Brush mainBrush, Brush secondaryBrush, string name, IEnumerable<double> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
            SecondaryBrush = secondaryBrush;
        }

        /// <summary>
        /// Initializes a new instance of Series object using specified name and sequence of PlainChartValue objects
        /// </summary>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public PlainSeries(string name, IEnumerable<IChartValue> values)
        {
            foreach (var v in values)
            {
                Values.Add(v.Clone());
            }
            InitFields(name);
        }

        /// <summary>
        /// Initializes a new instance of Series object using specified brush, name and sequence of PlainChartValue objects
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public PlainSeries(Brush mainBrush, string name, IEnumerable<IChartValue> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
        }

        /// <summary>
        /// Initializes a new instance of Series object using specified brush, name and sequence of PlainChartValue objects
        /// </summary>
        /// <param name="mainBrush">Series background</param>
        /// <param name="secondaryBrush">Series secondary background</param>
        /// <param name="name">Series name</param>
        /// <param name="values">Series values</param>
        public PlainSeries(Brush mainBrush, Brush secondaryBrush, string name, IEnumerable<IChartValue> values)
            : this(name, values)
        {
            MainBrush = mainBrush;
            SecondaryBrush = secondaryBrush;
        }
        #endregion
#nullable restore
    }
}
