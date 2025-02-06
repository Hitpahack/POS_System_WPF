using System;

namespace FalcaPOS.Entites.Stock
{
    public class UserInvoiceListViewModel
    {

        public int InvoiceId { get; set; }

        public string InvoiceNumber { get; set; }

        public string DcNumber { get; set; }

        public string StoreName { get; set; }

        public string SupplierName { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? DcNumberDate { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public double Total { get; set; }

        public bool IsDcNumber { get; set; }
    }
}
