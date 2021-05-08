using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.DataSources
{
    public class SpaceSignals : List<double>
    {
        public SpaceSignals()
        {
            var rnd = new Random();
            for (var i = 0; i < 1000; i++)
            {
                var d = rnd.NextDouble();
                d = Math.Round(d, 10);
                Add(d);
            }
        }
    }
}
