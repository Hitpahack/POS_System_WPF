using FalcaPOS.Entites.Attributes;
using FalcaPOS.Entites.Suppliers;
using System;
using System.Collections.Generic;

namespace FalcaPOS.Entites.AddInventory
{
    public class AddStockProductViewModel
    {


        public string InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        public int SupplierId { get; set; }

        public string SupplierName { get; set; }

        public int Quantity { get; set; }

        public int DefectiveQuantity { get; set; }

        public float GrossTotal { get; set; }

        public float InvoiceDiscountPerecent { get; set; }

        public float InvoiceDiscountFlat { get; set; }

        public float InvoiceDiscount { get; set; }

        public string InvoiceDiscountMode { get; set; }

        public float InvoiceRoundOff { get; set; }

        public float InvoiceOthers { get; set; }

        public float InvoiceNetTotal { get; set; }

        public int StoreID { get; set; }

        public string StoreName { get; set; }

        public bool IsQADone { get; set; }

        public DateTime CreatedDate { get; set; }

        public List<StockProductViewModel> StockProducts { get; set; }

        public float TotalGST { get; set; }

        public float TransportCharges { get; set; }

        public int InvoiceId { get; set; }

        public bool IsDcNumber { get; set; }

        public string DcNumber { get; set; }

        public DateTime? DcNumberDate { get; set; }

        public List<FileAttachment> FileAttachments { get; set; }


        /// <summary>
        /// Apply Discount after GST or before GST.
        /// </summary>
        public string ApplyDiscountType { get; set; }

        public int? PoId { get; set; }

        public string State { get; set; }

        public int? ShippingAddressId { get; set; }

        public AddressViewModel shippingAddress { get; set; }

    }

    public class AddStockProductModel
    {
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int SupplierId { get; set; }
        public int Quantity { get; set; }
        public int DefectiveQty { get; set; }
        public float GrossTotal { get; set; }

        public float InvoiceDiscount { get; set; }

        public string InvoiceDiscountMode { get; set; }

        public float InvoiceDiscountPerecent { get; set; }

        public float InvoiceDiscountFlat { get; set; }


        public float InvoiceRoundOff { get; set; }

        public float InvoiceOthers { get; set; }


        public float InvoiceNetTotal { get; set; }


        public int StoreID { get; set; }

        public List<StockProductModel> StockProducts { get; set; }


        public float Transportcharges { get; set; }

        public bool IsQADone { get; set; }


        public bool IsDcNumber { get; set; }

        public string DcNumber { get; set; }

        public DateTime? DcNumberDate { get; set; }



        public string ApplyDiscountType { get; set; }

        public int? PoId { get; set; }


        public int? ShippingAddressId { get; set; }

    }

    public class StockProductModel
    {


        public int ProductTypeId { get; set; }



        public int ManufacturerId { get; set; }



        public int ProductId { get; set; }



        public DateTime DateOfManufacture { get; set; }


        public DateTime? DateOfExpiry { get; set; }


        public int Quantity { get; set; }

        public int DefectiveQty { get; set; }


        public float ProductRate { get; set; }

        public float ProductDiscount { get; set; }


        public float ProductTotal { get; set; }

        public float ProductSellingPrice { get; set; }

        public string ProductDiscountMode { get; set; }


        public int? StroreId { get; set; }


        public float ProductMRP { get; set; }

        public float ProductGST { get; set; }

        public bool IsQADone { get; set; }
        public int ProductSubQty { get; set; }

        public bool IsGroupTrackMode { get; set; }

        public string LotNumber { get; set; }

        public float LandingRate { get; set; }


    }

    public class PurchaseInvoiceViewModel
    {
        public string InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string SupplierName { get; set; }

        public int Quantity { get; set; }

        public string StoreName { get; set; }


        public DateTime CreatedDate { get; set; }

        public float GrossTotal { get; set; }


        public int InvoiceId { get; set; }

        public bool IsDcNumber { get; set; }

        public string DcNumber { get; set; }

        public DateTime? DcNumberDate { get; set; }

        public List<FileAttachment> FileAttachments { get; set; }


    }
}
