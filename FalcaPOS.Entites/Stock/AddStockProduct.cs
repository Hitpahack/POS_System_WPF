using FalcaPOS.Entites.Attributes;
using System;
using System.Collections.Generic;

namespace FalcaPOS.Entites.Stock
{
    public class AddStockProduct
    {
        public int SupplierId { get; set; }

        public int ProductTypeId { get; set; }

        public int ManufacturerId { get; set; }

        public int ProductId { get; set; }

        public int StroreId { get; set; }


        public List<ProductAttributeMapping> AttributesSelectedList { get; set; }



        public string InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        public int Quantity { get; set; }

        public int DefectiveQuantity { get; set; }

        public float InvoiceRate { get; set; }

        public float InvoiceDiscountPerecent { get; set; }

        public float InvoiceDiscountFlat { get; set; }

        public float InvoiceDiscount { get; set; }

        public float InvoiceRoundOff { get; set; }

        public float InvoiceTotal { get; set; }

        public string SerialNumber { get; set; }

        public bool IsQADone { get; set; }

        public DateTime DateOfManufacture { get; set; }

        public DateTime DateOfExpiry { get; set; }

        public string Location { get; set; }

        public string WarrantyService { get; set; }

        public float ProductRate { get; set; }

        public float ProductDiscount { get; set; }

        public float ProductTotal { get; set; }

        public float ProductSellingPrice { get; set; }

        public string ProductDiscountMode { get; set; }

        public string InvoiceDiscountMode { get; set; }
    }
}
