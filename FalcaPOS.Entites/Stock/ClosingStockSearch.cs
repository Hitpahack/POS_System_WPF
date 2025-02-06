using System;

namespace FalcaPOS.Entites.Stock
{
    public class ClosingStockSearch
    {
        public DateTime FromData { get; set; }

        public DateTime ToDate { get; set; }

        public int StoreId { get; set; }
    }
}
