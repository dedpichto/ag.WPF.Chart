﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ag.WPF.Chart.Values
{
    /// <summary>
    /// Represents single chart value
    /// </summary>
    public class PlainChartValue : ChartValue
    {
        #region ctor
        /// <summary>
        /// Initializes a new instance of PlainChartValue object
        /// </summary>
        /// <param name="plainValue">Current value</param>
        public PlainChartValue(double plainValue)
        {
            Value = (plainValue, 0, 0, 0, 0, 0);
            StockType = StockType.None;
        }

        /// <summary>
        /// Initializes a new instance of PlainChartValue object
        /// </summary>
        /// <param name="plainValue">Current value</param>
        /// <param name="customValue">Current custom value</param>
        public PlainChartValue(double plainValue, string customValue)
            : this(plainValue)
        {
            CustomValue = customValue;
        }
        #endregion

        #region Abstraction metods overrides
        public override IChartValue Clone()
        {
            return (PlainChartValue)MemberwiseClone();
        } 
        #endregion
    }
}
