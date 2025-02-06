
using System;

namespace FalcaPOS.Dashboard.Models
{
    public class ChartDataModel
    {
        public ChartDataModel()
        {
            //SeriesCollection = new SeriesCollection();
        }
        //public SeriesCollection SeriesCollection { get; set; }

        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public int TotalCount { get; set; }
        //public ChartValues<double> Results { get; set; }
    }
}
