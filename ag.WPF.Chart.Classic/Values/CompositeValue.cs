using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ag.WPF.Chart.Values
{
    /// <summary>
    /// Represents various values
    /// </summary>
    public class CompositeValue
    {
        /// <summary>
        /// Gets or sets PlainValue
        /// </summary>
        public double PlainValue { get; set; }
        /// <summary>
        /// Gets or sets HighValue
        /// </summary>
        public double HighValue { get; set; }
        /// <summary>
        /// Gets or sets LowValue
        /// </summary>
        public double LowValue { get; set; }
        /// <summary>
        /// Gets or sets CloseValue
        /// </summary>
        public double CloseValue { get; set; }
        /// <summary>
        /// Gets or sets VolumeValue
        /// </summary>
        public double VolumeValue { get; set; }
        /// <summary>
        /// Gets or sets OpenValue
        /// </summary>
        public double OpenValue { get; set; }
    }
}
