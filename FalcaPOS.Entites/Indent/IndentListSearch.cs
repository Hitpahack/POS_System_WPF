using System;

namespace FalcaPOS.Entites.Indent
{
    public class IndentListSearch
    {
        public string Status { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int? StoreId { get; set; }

        public String PaymentMode { get; set; }

        public string PONumber { get; set; }
    }
}
