using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace FalcaPOS.Entites.Director
{
    public class PurchaseRateSearchModel : BindableBase
    {
        private DateTime? _frominvoiceDate;
        public DateTime? FromInvoiceDate
        {
            get { return _frominvoiceDate; }
            set { SetProperty(ref _frominvoiceDate, value); }
        }

        private DateTime? _toinvoiceDate;
        public DateTime? ToInvocieDate
        {
            get { return _toinvoiceDate; }
            set { SetProperty(ref _toinvoiceDate, value); }
        }
        public string SupplierName { get; set; }

        public int? SupplierId { get; set; }

        private string _ProductType;

        public string ProductType
        {
            get { return _ProductType; }
            set { SetProperty(ref _ProductType, value); }
        }

        private string _brand;

        public string Brand
        {
            get { return _brand; }
            set { SetProperty(ref _brand, value); }
        }

        private string _productName;

        public string ProductName
        {
            get { return _productName; }
            set { SetProperty(ref _productName, value); }
        }

        private string _SKU;

        public string SKU
        {
            get { return _SKU; }
            set { SetProperty(ref _SKU, value); }
        }

        private string _StoreName;

        public string StoreName
        {
            get { return _StoreName; }
            set { SetProperty(ref _StoreName, value); }
        }

        public int StoreId { get; set; }
    }

    public class PurchaseRateModel : BindableBase
    {
        public string SupplierName { get; set; }
        public string InvoiceNo { get; set; }

        private string _invoiceDate;
        public string InvoiceDate
        {
            get { return _invoiceDate; }
            set { SetProperty(ref _invoiceDate, value); }
        }


        public string Category { get; set; }
        public string Subcategory { get; set; }       
        public string Brand { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public string Store { get; set; }
        public double PurchaseRate { get; set; }
        public double SellingPrice { get; set; }
        public double Margin { get; set; }
        public int AvailableQty { get; set; }
    }

    public class StoreAssertModel : BindableBase
    {
        private int _rank;
        public int Rank
        {
            get { return _rank; }
            set { SetProperty(ref _rank, value); }
        }

        private string _store;
        public string Store { get { return _store; } set { SetProperty(ref _store, value); } }

        private double _networth;
        public double NetWorth { get { return _networth; } set { SetProperty(ref _networth, value); } }

        private string _net;
        public string Net { get { return _net; } set { SetProperty(ref _net, value); } }

        public List<AssertTypeModel> assertTypeModels { get; set; }

    }
    public class AssertTypeModel : BindableBase
    {
        private string _ProductType;
        public string ProductType
        {
            get { return _ProductType; }
            set { SetProperty(ref _ProductType, value); }
        }
        public double? DepartTotal { get; set; }

        private List<AssertModel> _productList;
        public List<AssertModel> ProductList
        {
            get { return _productList; }
            set { SetProperty(ref _productList, value); }
        }
    }

    public class AssertModel : BindableBase
    {

        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set { SetProperty(ref _productName, value); }
        }

        public string SKU { get; set; }
        public string Brand { get; set; }
        public int Qty { get; set; }
        public double Rate { get; set; }
        public double? Total { get; set; }
    }

}
