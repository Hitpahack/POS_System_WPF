using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace FalcaPOS.Entites.Sales
{
    public class InvoiceDetailsViewModel : BindableBase
    {
        public String InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public int? InvoiceQty { get; set; }
        public String SupplierName { get; set; }
        public List<SlotProducts> ProductDetails { get; set; } = new List<SlotProducts>();

        public int TotalSold { get; set; }
        public int TotalStock { get; set; }

    }

    public class SlotProducts : BindableBase
    {
        private int _slotstockcount;
        private int _slotsoldcount;
        private string _slotno;

        public string SlotNo
        {
            get { return _slotno; }
            set
            {
                SetProperty(ref _slotno, value);
            }
        }



        public int SlotStockCount
        {
            get { return _slotstockcount = (_slotstockcount - _slotsoldcount); }
            set
            {
                SetProperty(ref _slotstockcount, value);
            }
        }

        public int SlotSoldCount
        {
            get { return _slotsoldcount; }
            set
            {
                SetProperty(ref _slotsoldcount, value);
            }
        }

        public List<ProductInfo> ProductInvoices { get; set; }

    }

    public class ProductInfo
    {
        public string ProductType { get; set; }
        public string DepartCode { get; set; }

        public string Brand { get; set; }
        public string ProductName { get; set; }
        public string ProductSKU { get; set; }

        public float Rate { get; set; }
        public float GST { get; set; }
        public float Discount { get; set; }
        public float SellingPrice { get; set; }
        public float MRP { get; set; }
        public string Status { get; set; }
        public SaleCustomer Customer { get; set; }

        public string LotNumber { get; set; }
        public List<SaleCustomer> Customers { get; set; }

        public int Qty { get; set; }

        public int QtySold { get; set; }
    }

    public class SaleCustomer
    {
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string State { get; set; }
        public string District { get; set; }
        public string Info
        {
            get
            {
                return (InvoiceNo != null) ? (InvoiceNo + " >> " + InvoiceDate?.ToString("dd-MM-yyyy") + " >> " + Name + " >> " + Phone + $" >> Qty {QtyPurchased}") : null;
            }

        }
        public int QtyPurchased { get; set; }
    }
}
