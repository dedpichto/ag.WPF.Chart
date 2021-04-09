using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace ag.WPF.Chart.Values
{
    /// <summary>
    /// Represents collection of <see cref="ChartValue"/>
    /// </summary>
    public class ChartValues : ObservableCollection<IChartValue>
    {
        //internal Path Path { get; set; }
    }
}
