using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.DataSources
{
    public class GasPrice
    {
        public string Type { get; set; }
        public double[] Prices { get; set; }
    }

    public class GasPricesSource : List<GasPrice>
    {
        public GasPricesSource()
        {
            Add(new GasPrice { Type = "Diesel", Prices = new[] { 2.467, 2.992, 3.84, 3.968, 3.922, 3.825, 2.707, 2.304, 2.65, 3.178 } });
            Add(new GasPrice { Type = "Gasoline", Prices = new[] { 2.406, 2.835, 3.576, 3.68, 3.575, 3.437, 2.52, 2.25, 2.528, 2.813 } });
        }

        public int[] Years { get; } = new[] { 2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018 };
    }
}
