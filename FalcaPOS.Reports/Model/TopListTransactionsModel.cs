using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalcaPOS.Reports.Model
{
    public class TopListTransactionsModel : ReportBaseModel
    {
        public string InvoiceNumber { get; set; }
        public string SalesDate { get; set; }
        public int Items { get; set; }
        public int Quantity { get; set; }
        public decimal Gross { get; set; }
        public decimal SGST { get; set; }
        [DisplayName("CGST/IGST")]
        public decimal CGST_IGST { get; set; }
        public decimal Discount { get; set; }
        public decimal Net { get; set; }
        public decimal Cash { get; set; }
        public decimal UPI { get; set; }
        [DisplayName("NEFT/RTGS")]
        public decimal NEFT_RTGS { get; set; }
        public string RefNo { get; set; }
    }
}
