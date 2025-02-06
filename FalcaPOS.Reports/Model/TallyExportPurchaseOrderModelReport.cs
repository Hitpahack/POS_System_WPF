using System;
using System.ComponentModel;

namespace FalcaPOS.Reports.Model
{
    public class TallyExportPurchaseOrderModelReport: ReportBaseModel
    {
        [DisplayName("Voucher Type")]
        public string Type { get; set; }

        [DisplayName("Cost Centre/Classes")]
        public string StoreName { get; set; }

        [DisplayName("Centre state")]
        public string StoreState { get; set; }

        [DisplayName("Reference No as per ERP")]
        public String ERPuniqueNo { get; set; }
        //[DisplayName("Reference No as per ERP")]
        //public string ReferEPR { get; set; }

        [DisplayName("Date as per ERP")]
        public string DateEPR { get; set; }

        [DisplayName("Invoice No")]
        public string InvoiceNo { get; set; }

        [DisplayName("Invoice Date")]
        public String InvoiceDate { get; set; }

        [DisplayName("Purchase Ledger")]
        public string PurchaseLedger { get; set; }

        [DisplayName("Supplier Name")]
        public string SupplierName { get; set; }


        [DisplayName("Supplier (Bill From)")]
        public string Bill { get; set; }

        public string GSTIN { get; set; }

        [DisplayName("Address fields as per DSC/POS")]
        public string Address { get; set; }

        public int Pincode { get; set; }

        public string District { get; set; }

        public string State { get; set; }

        [DisplayName("Category")]
        public string CategoryName { get; set; }

        [DisplayName("Sub Category")]
        public string DepartmentName { get; set; }
        public string ProductName { get; set; }

        public string Godown { get; set; }

        [DisplayName("Existing SKU(Old SKU)")]
        public string ExistingSKU { get; set; }

        public string SKU { get; set; }


        public string Units { get; set; }

        [DisplayName("HSN Code")]
        public string HSN { get; set; }

        public int Qty { get; set; }

        public decimal Rate { get; set; }

        [DisplayName("Sub Total")]
        public decimal SubTotal { get; set; }

        [DisplayName("GST Rate")]
        public decimal GST { get; set; }

        [DisplayName("Input CGST")]
        public decimal CGST { get; set; }

        [DisplayName("Input SGST")]
        public decimal SGST { get; set; }

        [DisplayName("Input IGST")]
        public decimal IGST { get; set; }

        public decimal Roundoff { get; set; }

        [DisplayName("Total Invoice Amount")]
        public decimal TotalAmount { get; set; }

        [DisplayName("Input Credit Ledger-CGST")]
        public string LedgerCGST { get; set; }

        [DisplayName("Input Credit Ledger-SGST")]
        public string LedgerSGST { get; set; }

        [DisplayName("Input Credit Ledger-IGST")]
        public string LedgerIGST { get; set; }

        [DisplayName("Sourcing Employee")]
        public string SourceEmployee { get; set; }

        [DisplayName("Freight Charges Clearing")]
        public decimal Transportcharges { get; set; }

        [DisplayName("Loading and Unloading Charges Clearing")]
        public string LoadandUnloadCharge { get; set; }

        [DisplayName("Gunny Bags Expenses Clearing")]
        public string GunnyBags { get; set; }

        [DisplayName("Weighbridge Charges Clearing")]
        public string Weighbridge { get; set; }

        [DisplayName("APMC Cess Clearing")]
        public string APMCCess { get; set; }

        [DisplayName("Lives Deduct Charges Clearing")]
        public string LivesDeduct { get; set; }

        [DisplayName("Discount-Before GST")]
        public decimal BeforeGST { get; set; }


        [DisplayName("Discount-After GST")]
        public decimal AfterGST { get; set; }


        [DisplayName("Farmer Advance Clearing")]
        public string FarmerAdvance { get; set; }

        [DisplayName("Transporter Advance Clearing")]
        public string TransporterAdvance { get; set; }

        [DisplayName("Others")]
        public decimal Others { get; set; }

        [DisplayName("F-Shop/RSP")]
        public string StoreType { get; set; }

        public decimal TDS { get; set; }


    }

}
