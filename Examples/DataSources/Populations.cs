using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.DataSources
{
    public class PopulationGrow
    {
        public string Country { get; set; }
        public double[] Percents { get; set; }
    }

    public class PopulationDataSource : List<PopulationGrow>
    {
        public PopulationDataSource()
        {
            Add(new PopulationGrow { Country = "Bahrain", Percents = new[] { 1.92, 2.49, 2.49, 6.67, 2.01, 4.26 } });
            Add(new PopulationGrow { Country = "Oman", Percents = new[] { 9.13, 2.06, 2.06, 3.83, 6.45, 4.08 } });
            Add(new PopulationGrow { Country = "Niger", Percents = new[] { 3.84, 3.28, 3.28, 3.75, 3.84, 3.81 } });
            Add(new PopulationGrow { Country = "Serbia", Percents = new[] { -0.48, -0.46, -0.46, -0.41, -0.4, -0.34 } });
            Add(new PopulationGrow { Country = "Portugal", Percents = new[] { -0.29, 0.12, 0.12, 0.16, -0.44, -0.39 } });
            Add(new PopulationGrow { Country = "Ukraine", Percents = new[] { -0.25, -0.64, -0.64, -0.48, -0.5, -0.49 } });
        }
    }
}
