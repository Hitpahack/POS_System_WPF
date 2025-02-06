using System;
using System.ComponentModel;

namespace FalcaPOS.Reports.Model
{
    public class TallyExportSalesModelReport :ReportBaseModel
    {
        [DisplayName("Voucher Type")]
        public string Type { get; set; }

        [DisplayName("Cost Centre/Classes")]
        public string CostCenter { get; set; }

        [DisplayName("Centre state")]
        public string StoreState { get; set; }

        [DisplayName("Sales No as per ERP")]
        public string ReferEPR { get; set; }

        [DisplayName("Date as per ERP")]
        public string DateEPR { get; set; }

        [DisplayName("Sales Invoice No")]
        public string InvoiceNo { get; set; }

        [DisplayName("Invoice Date")]
        public String InvoiceDate { get; set; }

        [DisplayName("Sales Ledger")]
        public string SalesLedger { get; set; }

        public string CustomerName { get; set; }

        [DisplayName("Customer (Bill To)")]
        public string Bill { get; set; }

        public string GSTIN { get; set; }

        [DisplayName("Address fields as per DSC/POS")]
        public string Address { get; set; }

        public string Pincode { get; set; }

        public string District { get; set; }

        public string State { get; set; }

        public string ProductName { get; set; }

        [DisplayName("Existing SKU(Old SKU)")]
        public string ExistingSKU { get; set; }
        public string SKU { get; set; }

        public string Godown { get; set; }
        public string Units { get; set; }

        [DisplayName("HSN Code")]
        public string HSN { get; set; }
        public int Qty { get; set; }

        public decimal Rate { get; set; }

        [DisplayName("Sub Total")]
        public decimal SubTotal { get; set; }

        [DisplayName("GST Rate")]
        public decimal GST { get; set; }



        [DisplayName("Output CGST")]
        public decimal CGST { get; set; }

        [DisplayName("Output SGST")]
        public decimal SGST { get; set; }

        [DisplayName("Output IGST")]
        public decimal IGST { get; set; }

        [DisplayName("Total Invoice Amount")]
        public decimal TotalAmount { get; set; }

        [DisplayName("CGST LEDGER")]
        public string LedgerCGST { get; set; }

        [DisplayName("SGST LEDGER")]
        public string LedgerSGST { get; set; }

        [DisplayName("IGST LEDGER")]
        public string LedgerIGST { get; set; }

        [DisplayName("Sourcing Employee")]
        public string SourceEmployee { get; set; }

        [DisplayName("Cash")]
        public decimal Cash { get; set; }

        [DisplayName("UPI")]
        public decimal UPI { get; set; }

        public decimal Cheque { get; set; }

        [DisplayName("UTRnumber")]
        public string UTRNumber { get; set; }

        [DisplayName("F-Shop/RSP")]
        public string StoreType { get; set; }

    }

}
