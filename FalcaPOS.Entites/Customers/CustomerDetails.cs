using FalcaPOS.Entites.Location;
using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace FalcaPOS.Entites.Customers
{
    public class CustomerDetails
    {
        public int CustomerId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public int PinCode { get; set; }

        public string AlternatePhone { get; set; }

        public Address Address { get; set; }

        public DateTime? CreatedDate { get; set; }


        public string GSTNumber { get; set; }

        public string CustomerType { get; set; }

        public PincodeDetails PincodeDetailsInfo { get; set; }

        public float Acreage { get; set; }
        public string Crops { get; set; }
        public string LandType { get; set; }

        public float TotalBillingAmount { get; set; }

    }

    public class CustomerModelRequest : BindableBase
    {
        private string _name;
        public string Name { get { return _name; } set { SetProperty(ref _name, value); } }

        private string _phone;
        public string Phone { get { return _phone; } set { SetProperty(ref _phone, value); } }

        private string _salesno;
        public string SalesNo { get { return _salesno; } set { SetProperty(ref _salesno, value); } }

        private string _invoceno;
        public string InvoiceNo { get { return _invoceno; } set { SetProperty(ref _invoceno, value); } }

        private string _invoicedate;
        public string InvoiceDate { get { return _invoicedate; } set { SetProperty(ref _invoicedate, value); } }

        private string _fromdate;
        public string FromDate { get { return _fromdate; } set { SetProperty(ref _fromdate, value); } }

        private string _todate;
        public string ToDate { get { return _todate; } set { SetProperty(ref _todate, value); } }

        private string _location;
        public string Location { get { return _location; } set { SetProperty(ref _location, value); } }
    }
    public class CustomerModelResponse
    {

        public string SalesNo { get; set; }

        public string Phone { get; set; }

        public string Category { get; set; }
        public string Producttype { get; set; }

        public string Brnad { get; set; }
        public string ProductName { get; set; }

        public string SerialNo { get; set; }
        public string Expirydate { get; set; }
        public string WarantyService { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string DiscountType { get; set; }
        public double Discount { get; set; }

        public Int64 Gst { get; set; }
        public double GstAmount { get; set; }

        public double Total { get; set; }
        public string InvoiceNo { get; set; }

        public string InvoiceDate { get; set; }
        public string CustomerName { get; set; }



        public string Email { get; set; }

        public string street { get; set; }

        public string state { get; set; }

        public string city { get; set; }

        public Int64 Pincode { get; set; }

        public string district { get; set; }

        public double Cash { get; set; }

        public double Card { get; set; }
        public double UPI { get; set; }

        public string Payment { get; set; }

    }


    public class CustomerSearchModel : BindableBase
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        private string _invoceno;
        public string InvoiceNo
        {
            get => _invoceno;
            set => SetProperty(ref _invoceno, value);
        }


        private string _fromdate;
        public string FromDate
        {
            get => _fromdate;
            set => SetProperty(ref _fromdate, value);
        }

        private string _todate;
        public string ToDate
        {
            get => _todate;
            set => SetProperty(ref _todate, value);
        }

        private string _location;
        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }
    }


    public class CustomerModel
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public int PinCode { get; set; }

        public string AlternatePhone { get; set; }

        public SalesAddressModel Address { get; set; }

        public string GSTNumber { get; set; }

        public string CustomerType { get; set; }

        public float Acreage { get; set; }
        public List<string> Crops { get; set; }
        public List<string> LandType { get; set; }



    }


    public class SalesAddressModel
    {

        public string Phone { get; set; }
        public string Alternatephone { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public int? Pincode { get; set; }
        public string City { get; set; }
        public int StateID { get; set; }
        public int DistrictID { get; set; }
        public string State { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
        public int VillageId { get; set; }

    }

}
