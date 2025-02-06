using System;

namespace FalcaPOS.Entites.Common
{
    public class InvoiceSearchParams
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int? StoreId { get; set; }

        public bool IsDc { get; set; }
    }
}
