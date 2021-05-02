using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.DataSources
{
    public class NbaStats
    {
        public string Name { get; set; }
        public double FreeGoals { get; set; }
        public double ThreePointShots { get; set; }
        public double FreeThrows { get; set; }
        public double OfRebounds { get; set; }
        public double DefRebounds { get; set; }
        public double Rebounds { get; set; }
        public double Assists { get; set; }
        public double Blocks { get; set; }
    }

    public class NbaStatsDataSource : List<NbaStats>
    {
        public NbaStatsDataSource()
        {
            Add(new NbaStats { Name = "Stephen Curry", FreeGoals = 10.2,ThreePointShots= 5.1,FreeThrows= 5.6,OfRebounds= 0.5,DefRebounds= 5.1,Rebounds= 5.6,Assists= 5.8,Blocks= 0.1 });
            Add(new NbaStats { Name = "Clint Capela", FreeGoals = 6.8, ThreePointShots = 0, FreeThrows = 2.1, OfRebounds = 4.9, DefRebounds = 9.8, Rebounds = 14.7, Assists = 0.8, Blocks = 2.2 });
            Add(new NbaStats { Name = "Russell Westbrook", FreeGoals = 8.3, ThreePointShots = 1.3, FreeThrows = 3.9, OfRebounds = 1.7, DefRebounds = 9.5, Rebounds = 11.1, Assists = 10.9, Blocks = 0.3 });
            Add(new NbaStats { Name = "Myles Turner", FreeGoals = 4.4, ThreePointShots = 1.5, FreeThrows = 2.4, OfRebounds = 1.3, DefRebounds = 5.2, Rebounds = 6.5, Assists = 1, Blocks = 3.4 });
            Add(new NbaStats { Name = "Jimmy Butler", FreeGoals = 7.1, ThreePointShots = 0.4, FreeThrows = 6.7, OfRebounds = 1.9, DefRebounds = 5.2, Rebounds = 7, Assists = 7.3, Blocks = 0.4 });
            Add(new NbaStats { Name = "Rudy Gobert", FreeGoals = 5.5, ThreePointShots = 0, FreeThrows = 3.4, OfRebounds = 3.4, DefRebounds = 10.2, Rebounds = 13.6, Assists = 1.3, Blocks = 2.8 });
        }
    }
}
