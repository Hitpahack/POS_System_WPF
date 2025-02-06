using System;

namespace FalcaPOS.Entites.Finance
{
    public class FinanceSearch
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int? StoreId { get; set; }

        public string InvoiceNumber { get; set; }

        public string StoreName { get; set; }

        public int? ProductTypeId { get; set; }

        public int? CategoryId { get; set; }


    }
}
