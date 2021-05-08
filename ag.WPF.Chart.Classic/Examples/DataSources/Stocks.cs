using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.DataSources
{
    public class Stock
    {
        public string Name { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public DateTime Date { get; set; }
    }

    public class StocksDataSource : List<Stock>
    {
        public StocksDataSource()
        {
            Add(new Stock { Name = "Chart Ltd", Open = 40.0, High = 50.0, Low = 35.0, Close = 45.0, Date = DateTime.Parse("11 Oct 2017") });
            Add(new Stock { Name = "Chart Ltd", Open = 45.0, High = 50.0, Low = 30.0, Close = 35.0, Date = DateTime.Parse("12 Oct 2017") });
            Add(new Stock { Name = "Chart Ltd", Open = 35.0, High = 45.0, Low = 30.0, Close = 40.0, Date = DateTime.Parse("13 Oct 2017") });
            Add(new Stock { Name = "Chart Ltd", Open = 40.0, High = 50.0, Low = 35.0, Close = 45.0, Date = DateTime.Parse("14 Oct 2017") });
            Add(new Stock { Name = "Chart Ltd", Open = 45.0, High = 50.0, Low = 30.0, Close = 35.0, Date = DateTime.Parse("15 Oct 2017") });
            Add(new Stock { Name = "Chart Ltd", Open = 35.0, High = 45.0, Low = 30.0, Close = 40.0, Date = DateTime.Parse("16 Oct 2017") });
            Add(new Stock { Name = "Chart Ltd", Open = 40.0, High = 50.0, Low = 35.0, Close = 45.0, Date = DateTime.Parse("17 Oct 2017") });
            Add(new Stock { Name = "Chart Ltd", Open = 40.0, High = 50.0, Low = 35.0, Close = 45.0, Date = DateTime.Parse("18 Oct 2017") });
            Add(new Stock { Name = "Chart Ltd", Open = 45.0, High = 50.0, Low = 30.0, Close = 35.0, Date = DateTime.Parse("19 Oct 2017") });
            Add(new Stock { Name = "Chart Ltd", Open = 35.0, High = 45.0, Low = 30.0, Close = 40.0, Date = DateTime.Parse("20 Oct 2017") });
            Add(new Stock { Name = "Chart Ltd", Open = 40.0, High = 50.0, Low = 35.0, Close = 45.0, Date = DateTime.Parse("21 Oct 2017") });
        }
    }
}
