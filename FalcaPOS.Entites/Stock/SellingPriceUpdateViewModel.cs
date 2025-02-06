using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalcaPOS.Entites.Stock
{
    public class SellingPriceUpdateViewModelV2
    {
        public int? SKUId { get; set; }
        public int? ZoneId { get; set; }
        public int? TerritoryId { get; set; }
        public int? StoreId { get; set; }
        //public String LotNumber { get; set; }

        public String Barcode { get; set; }
        public float NewSellingPrice { get; set; }

        public string Category { get; set; }
    }

    public class SellingPriceResponse
    {
        public int NoOfRows { get; set; }
        public string ErrorMsg { get; set; }
        public string MrpErrorMsg { get; set; }
        public string InvoiceNo { get; set; }
        public string lotnumber { get; set; }
        public float margin { get; set; }


    }
}
