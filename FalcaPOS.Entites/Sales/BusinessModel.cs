using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace FalcaPOS.Entites.Sales
{
    public class BusinessModelResponse : BindableBase
    {

        public String Store { get; set; }
        public string Date { get; set; }
        public Decimal DayBusiness { get; set; }
        public Decimal DayCash { get; set; }
        public Decimal DayCard { get; set; }
        public Decimal DayUPI { get; set; }
        public Decimal DayCheque { get; set; }
        public Decimal DayTotal { get; set; }
        public Decimal MonthBusiness { get; set; }

        //public Decimal MonthCash { get; set; }
        //public Decimal MonthCard { get; set; }
        //public Decimal MonthCheque { get; set; }
        //public Decimal MonthTotal { get; set; }

        public Decimal Average { get; set; }
        //public Decimal Projected { get; set; }
        //public DateTime OrderDate { get; set; }
        //public string OrderNo { get; set; }
        //public DateTime InvoiceDate { get; set; }
        //public string InvoiceNo { get; set; }



        //public DateTime? InvoiceDateNull { get; set; }

        //public DateTime ToDate { get; set; }

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

        private List<BusinessResponse> _businessModelResponseIndetails;
        public List<BusinessResponse> BusinessModelResponseIndetails
        {
            get { return _businessModelResponseIndetails; }
            set { SetProperty(ref _businessModelResponseIndetails, value); }
        }

        private List<BusinessModelResponseIndetails> _businessModelProductIndetails;
        public List<BusinessModelResponseIndetails> BusinessModelProductIndetails
        {
            get { return _businessModelProductIndetails; }
            set { SetProperty(ref _businessModelProductIndetails, value); }
        }
    }

    public class BusinessModelRequest : BindableBase
    {
        private string _fromDate;
        public string FromDate { get { return _fromDate; } set { SetProperty(ref _fromDate, value); } }
        private string _todate;
        public string ToDate { get { return _todate; } set { SetProperty(ref _todate, value); } }


        private string _seletedstoreName;
        public string SelectedStoreName { get { return _seletedstoreName; } set { SetProperty(ref _seletedstoreName, value); } }
        private List<string> _storeName;
        public List<string> StoreName { get { return _storeName; } set { SetProperty(ref _storeName, value); } }
    }


    public class BusinessResponse
    {
        public string CustomerName { get; set; }
        public string Phone { get; set; }

        public string InvoiceNo { get; set; }

        public double total { get; set; }

        public double TotalServiceCharge { get; set; }

        public double TotalAmount { get { return total + TotalServiceCharge; } }

        public List<BusinessModelResponseIndetails> ProductDetails { get; set; }
    }

    public class BusinessModelResponseIndetails
    {

        public string CustomerName { get; set; }
        public string Phone { get; set; }

        public string ProductType { get; set; }

        public string Brand { get; set; }
        public string ProductName { get; set; }

        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public double SellingPrice { get; set; }

        public double Cash { get; set; }

        public double Card { get; set; }
        public double UPI { get; set; }

        public double Total { get; set; }

        public double TotalServiceCharge { get; set; }

        public long SoldQty { get; set; }

        public double ProductTotalServiceCharge { get; set; }

        public double ExtraTotalDiscount { get; set; }




    }

}
