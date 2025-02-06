using Prism.Mvvm;
using System;

namespace FalcaPOS.Entites.Sales
{
    public class SearchParams
    {
        public string InvoiceNumber { get; set; }

        public DateTime? InvoiceFromDate { get; set; }

        public DateTime? InvoiceToDate { get; set; }

        public string CustomerName { get; set; }

        public string CustomerPhone { get; set; }

        public string OrderTacknBy { get; set; }


        public bool IsParent { get; set; }

        public int? StoreId { get; set; }

    }

    public class SalesFlyout : BindableBase
    {


        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set { SetProperty(ref _isOpen, value); }
        }

        private bool _isParent;
        public bool IsParent
        {
            get { return _isParent; }
            set { SetProperty(ref _isParent, value); }
        }
    }
}
