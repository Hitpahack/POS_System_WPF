using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace FalcaPOS.Entites.AddInventory
{
    public class InvoiceListViewModel : BindableBase
    {
        public string InvoiceNumber { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Quantity { get; set; }
        public int DefectiveQty { get; set; }

        public int StoreId { get; set; }


        public int InvoiceId { get; set; }

        public string SupplierName { get; set; }


        public string Store { get; set; }
        public bool IsSelectedShallow { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, IsSelectedShallow); }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        public bool HasInformation { get; set; }

        private List<StockProductViewModel> _defectiveProductList;

        public List<StockProductViewModel> DefectiveProductList
        {
            get => _defectiveProductList;
            set => SetProperty(ref _defectiveProductList, value);

        }

    }

    public class InwardInvoiceDamageViewModel : BindableBase
    {
        public string InvoiceNumber { get; set; }

        public DateTime? InvoiceDate { get; set; }
        public bool IsSellingPriceUpdated { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Quantity { get; set; }
        public int DefectiveQty { get; set; }

        public int StoreId { get; set; }
        public decimal Discount { get; set; }

        public double RoundOff { get; set; }

        public decimal Others { get; set; }
        public decimal Transportcharges { get; set; }

        public decimal Total { get; set; }

        public int InvoiceId { get; set; }

        public string SupplierName { get; set; }


        public string Store { get; set; }
        public bool IsSelectedShallow { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, IsSelectedShallow); }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        public bool HasInformation { get; set; }

        private List<StockProductViewModel> _defectiveProductList;

        public List<StockProductViewModel> DefectiveProductList
        {
            get
            {
                return _defectiveProductList;
            }

            set
            {
                SetProperty(ref _defectiveProductList, value);
            }
        }
    }

    public class InvoiceModelRequest
    {

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public string Invoiceno { get; set; }
        public string SupplierName { get; set; }

    }

    public class InvoiceVm
    {
        public IEnumerable<InwardInvoiceDamageViewModel> invoiceListViews { get; set; }
    }
}
