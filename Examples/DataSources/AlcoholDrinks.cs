using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.DataSources
{
    public class AlcoholDrinks
    {
        public string Country { get; set; }
        public double Total { get; set; }
        public double Beer { get; set; }
        public double Wine { get; set; }
        public double Spirit { get; set; }
    }

    public class AlcoholDataSource : List<AlcoholDrinks>
    {
        public AlcoholDataSource()
        {
            Add(new AlcoholDrinks { Country = "Australia", Total = 13.4, Beer = 5.3466, Wine = 5.025, Spirit = 3.0284 });
            Add(new AlcoholDrinks { Country = "Canada", Total = 13.8, Beer = 6.2238, Wine = 3.5328, Spirit = 4.0434 });
            Add(new AlcoholDrinks { Country = "United States", Total = 13.7, Beer = 6.439, Wine = 2.4797, Spirit = 4.7813 });
            Add(new AlcoholDrinks { Country = "Russia", Total = 20.1, Beer = 7.839, Wine = 2.5728, Spirit = 9.6882 });
            Add(new AlcoholDrinks { Country = "China", Total = 12.9, Beer = 3.8184, Wine = 0.3999, Spirit = 8.6817 });
            Add(new AlcoholDrinks { Country = "France", Total = 16.7, Beer = 3.1396, Wine = 9.8196, Spirit = 3.7408 });
        }
    }
}
