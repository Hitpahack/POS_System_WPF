using System;
using System.ComponentModel;

namespace FalcaPOS.Entites.Finance
{
    public class TallyExportModel
    {

        public string type { get; set; }

        public string StoreName { get; set; }

        public string StoreType { get; set; }

        [DisplayName("Centre state")]
        public string StoreState { get; set; }

        public string NoasEPR { get; set; }

        public DateTime DateasEPR { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }

        public string PurchaseLedger { get; set; }

        public string SupplierName { get; set; }

        public string Bill { get; set; }

        public string GSTIN { get; set; }
        public string CustomerName { get; set; }

        public string Address { get; set; }
        public int Pincode { get; set; }

        public string District { get; set; }

        public string State { get; set; }

        public string DepartmentName { get; set; }

        public string ProductName { get; set; }

        public string ExistingSKU { get; set; }
        public string SKU { get; set; }

        public string Godown { get; set; }
        public string Units { get; set; }

        public string HsnCode { get; set; }

        public int Qty { get; set; }

        public int SubQty { get; set; }

        public decimal Rate { get; set; }

        public decimal SubTotal { get; set; }

        public decimal GST { get; set; }

        public decimal Cash { get; set; }

        public decimal UPI { get; set; }

        public decimal Cheque { get; set; }
        public decimal CGST { get; set; }

        public decimal SGST { get; set; }

        public decimal IGST { get; set; }

        public decimal Roundoff { get; set; }

        public decimal ServiceCharges { get; set; }
        public decimal TotalAmount { get; set; }

        public decimal TransportCharges { get; set; }

        public decimal BeforeGST { get; set; }

        public decimal AfterGST { get; set; }

        public decimal Others { get; set; }

        public string UTRNumber { get; set; }
        public string ERPuniqueNo { get; set; }
        public string CategoryName { get; set; }
        public decimal TDS { get; set; }

        public string FalcaGSTIN { get; set; }

    }

    public class TallyExportSearchModel
    {
        public string FromInvoiceDate { get; set; }

        public string ToInvoiceDate { get; set; }

        public string Type { get; set; }
    }
}
