using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ag.WPF.Chart.Values
{
    public abstract class ChartValue : IChartValue
    {
        #region Public properties
        /// <inheritdoc />
        public (double PlainValue, double HighValue, double LowValue, double CloseValue, double VolumeValue, double OpenValue) Value { get; set; }
        /// <inheritdoc />
        public object CustomValue { get; set; }
        #endregion
    }
}
