using FalcaPOS.AddInventory.ViewModels;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Suppliers;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FalcaPOS.AddInventory.Models
{
    public class InvoiceModel : BindableBase
    {
        public InvoiceModel()
        {
            StockProducts = new ObservableCollection<InvoiceProductCardViewModel>();
        }

        private string _invoiceNumber;
        public string InvoiceNumber
        {
            get { return _invoiceNumber; }
            set { SetProperty(ref _invoiceNumber, value); }
        }

        private DateTime _invoiceDate;
        public DateTime InvoiceDate
        {
            get { return _invoiceDate; }
            set { SetProperty(ref _invoiceDate, value); }
        }


        private string _supplierName;
        public string SupplierName
        {
            get { return _supplierName; }
            set { SetProperty(ref _supplierName, value); }
        }

        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value); }
        }

        private int _defectiveQuantity;
        public int DefectiveQuantity
        {
            get { return _defectiveQuantity; }
            set { SetProperty(ref _defectiveQuantity, value); }
        }

        private float _grossTotal;
        public float GrossTotal
        {
            get { return _grossTotal; }
            set { SetProperty(ref _grossTotal, value); }
        }

        private float _invoiceDiscountPerecent;
        public float InvoiceDiscountPerecent
        {
            get { return _invoiceDiscountPerecent; }
            set { SetProperty(ref _invoiceDiscountPerecent, value); }
        }


        private float _invoiceDiscountFlat;
        public float InvoiceDiscountFlat
        {
            get { return _invoiceDiscountFlat; }
            set { SetProperty(ref _invoiceDiscountFlat, value); }
        }


        private float _invoiceDiscount;
        public float InvoiceDiscount
        {
            get { return _invoiceDiscount; }
            set { SetProperty(ref _invoiceDiscount, value); }
        }



        //public string InvoiceDiscountMode { get; set; }

        private float _invoiceRoundOff;
        public float InvoiceRoundOff
        {
            get { return _invoiceRoundOff; }
            set { SetProperty(ref _invoiceRoundOff, value); }
        }

        private float _invoiceOthers;

        public float InvoiceOthers
        {
            get { return _invoiceOthers; }
            set { SetProperty(ref _invoiceOthers, value); }
        }

        private float _invoiceNetTotal;
        public float InvoiceNetTotal
        {
            get { return _invoiceNetTotal; }
            set { SetProperty(ref _invoiceNetTotal, value); }
        }

        private string _storeName;
        public string StoreName
        {
            get { return _storeName; }
            set { SetProperty(ref _storeName, value); }
        }


        private bool _isQADone;
        public bool IsQADone
        {
            get { return _isQADone; }
            set { SetProperty(ref _isQADone, value); }
        }

        private DateTime _createdDate;
        public DateTime CreatedDate
        {
            get { return _createdDate; }
            set { SetProperty(ref _createdDate, value); }
        }

        private ObservableCollection<InvoiceProductCardViewModel> _stockProducts;
        public ObservableCollection<InvoiceProductCardViewModel> StockProducts
        {
            get { return _stockProducts; }
            set { SetProperty(ref _stockProducts, value); }
        }


        private float _totalGST;
        public float TotalGST
        {
            get => _totalGST;
            set => SetProperty(ref _totalGST, value);
        }



        private float _transportcharges;
        public float Transportcharges
        {
            get { return _transportcharges; }
            set { SetProperty(ref _transportcharges, value); }
        }

        private int _invoiceId;
        public int InvoiceId
        {
            get { return _invoiceId; }
            set { SetProperty(ref _invoiceId, value); }
        }
        private bool _isDcNumber;
        public bool IsDcNumber
        {
            get { return _isDcNumber; }
            set { SetProperty(ref _isDcNumber, value); }
        }

        private string _dcNumber;
        public string DcNumber
        {
            get { return _dcNumber; }
            set { SetProperty(ref _dcNumber, value); }
        }
        private DateTime? _dcNumberDate;
        public DateTime? DcNumberDate
        {
            get { return _dcNumberDate; }
            set { SetProperty(ref _dcNumberDate, value); }
        }

        public IEnumerable<FileAttachment> FileAttachments { get; set; }


        public string DiscountApplyType { get; set; }

        private bool _hasDiscount;
        public bool HasDiscount
        {

            get { return _hasDiscount; }
            set { SetProperty(ref _hasDiscount, value); }

        }


        public string State { get; set; }

        public string GSTHeader { get; set; }

        public string GSTHeaderQty { get; set; }

        public string GSTHeaderPer { get; set; }

        private AddressViewModel _shippingAddress;
        public AddressViewModel shippingAddress
        {
            get
            {
                return _shippingAddress;
            }
            set
            {
                SetProperty(ref _shippingAddress, value);
            }
        }

        private bool _ishippingadress;

        public bool IshippingAddress
        {
            get { return _ishippingadress; }
            set { _ishippingadress = value; }
        }

    }
}
