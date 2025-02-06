using FalcaPOS.Entites.Stores;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace FalcaPOS.Sales.Models
{
    public class SalesSearchModel : BindableBase
    {

        private string _invoiceNumber;
        public string InvoiceNumber
        {
            get { return _invoiceNumber; }
            set { SetProperty(ref _invoiceNumber, value); }
        }


        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set { SetProperty(ref _customerName, value); }
        }


        private DateTime? _invoiceFromDate;
        public DateTime? InvoiceFromDate
        {
            get { return _invoiceFromDate; }
            set { SetProperty(ref _invoiceFromDate, value); }
        }

        private DateTime? _invoiceToDate;
        public DateTime? InvoiceToDate
        {
            get { return _invoiceToDate; }
            set { SetProperty(ref _invoiceToDate, value); }
        }


        private string _customerPhone;
        public string CustomerPhone
        {
            get { return _customerPhone; }
            set { SetProperty(ref _customerPhone, value); }
        }

        private string _OderTackenBy;
        public string OderTackenBy
        {
            get { return _OderTackenBy; }
            set { SetProperty(ref _OderTackenBy, value); }
        }


        private bool _isParent;
        public bool IsParent
        {
            get { return _isParent; }
            set { SetProperty(ref _isParent, value); }
        }

        private ObservableCollection<Store> _stores;
        public ObservableCollection<Store> Stores
        {
            get { return _stores; }
            set { SetProperty(ref _stores, value); }
        }

        private Store _selectedstores;
        public Store SelectedStores
        {
            get { return _selectedstores; }
            set { SetProperty(ref _selectedstores, value); }
        }
    }






}

