using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Common;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FalcaPOS.Entites.Suppliers
{

    public class Supplier : BindableBase
    {
        public int SupplierId { get; set; }
        public string GstNumber { get; set; }
        public string Name { get; set; }

        private bool _isenabled;
        public bool Isenabled { get { return _isenabled; } set { SetProperty(ref _isenabled, value); } }
        public AddressViewModel Address { get; set; }
        public BankDetails BankDetails { get; set; }

        public string Suppliertype { get; set; }

        public string MSME { get; set; }

        public string IsMSME { get; set; }

        public List<AddressViewModel> shippingAddress { get; set; }

        public string PAN { get; set; }
        public DateTime? Createddate { get; set; }
        public List<BankDetails> ListOfBankDetails { get; set; }

        public List<StoreReturnModel> ReturnModels { get; set; }

    }


    public class AddressViewModel
    {
        public int? AddressId { get; set; }
        public string Phone { get; set; }
        public string Alternatephone { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public int? Pincode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string District { get; set; }

        public int StateID { get; set; }

        public int DistrictID { get; set; }

        public bool IsSelectedAddress { get; set; }

        public string supplierName { get; set; }

        public string GST { get; set; }
    }

    //public class Supplier:BindableBase
    //{
    //    public int SupplierId { get; set; }

    //    private string _name;
    //    public string Name {
    //        get { return _name; }
    //        set { SetProperty(ref _name,value); }
    //    }

    //    private string _gstnumber;
    //    public string GstNumber {
    //        get { return _gstnumber; }
    //        set { SetProperty(ref _gstnumber,value); }
    //    }

    //    private AddressViewModel _address;
    //    public AddressViewModel Address
    //    {
    //        get { return _address; }
    //        set {SetProperty(ref _address , value); }
    //    }
    //}

    //public class AddressViewModel:BindableBase
    //{
    //    private string _phone;
    //    public string Phone 
    //    {
    //        get { return _phone; }
    //        set { SetProperty(ref _phone, value); }
    //    }
    //    private string _alternatephone;
    //    public string Alternatephone {
    //        get { return _alternatephone; }
    //        set { SetProperty(ref _alternatephone, value); }
    //    }
    //    private string _email;
    //    public string Email {
    //        get { return _email; }
    //        set { SetProperty(ref _email, value); }
    //    }
    //    private string _street;
    //    public string Street {
    //        get { return _street; }
    //        set { SetProperty(ref _street, value); }
    //    }
    //    private int? _pincode;
    //    public int? Pincode {
    //        get { return _pincode; }
    //        set { SetProperty(ref _pincode, value); }
    //    }
    //    private string _city;

    //    public string City {
    //        get { return _city; }
    //        set { SetProperty(ref _city, value); }
    //    }
    //    private string _state;
    //    public string State {
    //        get { return _state; }
    //        set { SetProperty(ref _state, value); }
    //    }
    //    private string _district;
    //    public string District {
    //        get { return _district; }
    //        set { SetProperty(ref _district, value); }
    //    }

    //}



    public class BankDetails : BindableBase
    {

        private int _srNo;

        public int SrNo
        {
            get { return _srNo; }
            set { SetProperty(ref _srNo, value); }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        public int BankId { get; set; }

        private string _bankName;
        public string BankName { get { return _bankName; } set { SetProperty(ref _bankName, value); } }

        private string _branchName;
        public string BrnachName { get { return _branchName; } set { SetProperty(ref _branchName, value); } }

        private string _accountno;
        public string AccountNo { get { return _accountno; } set { SetProperty(ref _accountno, value); } }

        private string _accounttype;
        public string AccountType { get { return _accounttype; } set { SetProperty(ref _accounttype, value); } }

        private string _ifsc;
        public string IFSC { get { return _ifsc; } set { SetProperty(ref _ifsc, value); } }

        private FileAttachment _fileAttachment;
        public FileAttachment FileAttachment { get { return _fileAttachment; } set { SetProperty(ref _fileAttachment, value); } }

        private List<Bank> _banks;
        public List<Bank> Banks { get { return _banks; } set { SetProperty(ref _banks, value); } }


        private Bank _selectedbanks;
        public Bank SelectedBanks { get { return _selectedbanks; } set { SetProperty(ref _selectedbanks, value); } }

        private ObservableCollection<FileUploadInfo> fileUploadListInfo;
        public ObservableCollection<FileUploadInfo> FileUploadListInfo
        {
            get { return fileUploadListInfo; }
            set { SetProperty(ref fileUploadListInfo, value); }
        }

        private List<FileAttachment> _fileattachements;
        public List<FileAttachment> FileAttachments { get { return _fileattachements; } set { SetProperty(ref _fileattachements, value); } }

        public int BankDetailsId { get; set; }

        public int SupplierId { get; set; }
    }

    public class ShippingAddress
    {
        public int SupplierId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? Pincode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string District { get; set; }
        public DateTime? Createddate { get; set; }

        public int StateID { get; set; }

        public int DistrictID { get; set; }


    }

    public class Bank
    {
        public int id { get; set; }

        public string BankName { get; set; }
    }

    public class SuppliersViewModel : BindableBase
    {
        public int? SupplierId { get; set; }

        public string Name { get; set; }

        public string GstNumber { get; set; }

        private bool _isenabled;
        public bool Isenabled
        {
            get => _isenabled;
            set => SetProperty(ref _isenabled, value);
        }

        public AddressViewModel Address { get; set; }

        public DateTime? Createddate { get; set; }

        public BanksDetails BankDetails { get; set; }

        public string Suppliertype { get; set; }

        public string MSME { get; set; }

        public string IsMSME { get; set; }

        public string PAN { get; set; }

        public string TallyCode { get; set; }


    }

    public class BanksDetails : BindableBase
    {
        private int _srNo;

        public int SrNo
        {
            get => _srNo;
            set => SetProperty(ref _srNo, value);
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public Bank Bank { get; set; }
        public string BrnachName { get; set; }

        public string AccountNo { get; set; }

        public string AccountType { get; set; }

        public string IFSC { get; set; }

        public int BankDetailsId { get; set; }
    }

    public class FileAttachement : BanksDetails
    {
        public FileAttachment FileAttachment { get; set; }
    }

    public class SuppliersDetailsViewModel
    {
        public int SupplierId { get; set; }


        public string Name { get; set; }


        public string GstNumber { get; set; }

        public bool? Isenabled { get; set; }

        public AddressViewModel Address { get; set; }

        public DateTime? Createddate { get; set; }

        public string Suppliertype { get; set; }

        public string MSME { get; set; }

        public string IsMSME { get; set; }

        public string PAN { get; set; }

        public List<AddressViewModel> shippingAddress { get; set; }

        public List<FileAttachement> ListOfBankDetails { get; set; }

        public string TallyCode { get; set; }

        public FileAttachment MSMECertificate { get; set; }

       
    }
    public class CreateSupplierModel
    {

        public string Name { get; set; }


        public string GstNumber { get; set; }


        public string Suppliertype { get; set; }


        public AddressModel Address { get; set; }

        public string TallyCode { get; set; }
    }


    public class AddressModel
    {
        public string Phone { get; set; }
        public string Alternatephone { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public int? Pincode { get; set; }
        public string City { get; set; }

        public int StateID { get; set; }

        public int DistrictID { get; set; }


    }


    public class SupplierEditViewModel
    {

        public int SupplierId { get; set; }
        public string Name { get; set; }

        public string Suppliertype { get; set; }

        public string MSME { get; set; }

        public string IsMSME { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public List<EditBankDetails> ListOfBankDetails { get; set; }

        public string TallyCode { get; set; }

        public FileUploadInfo FileUploadMSME { get; set; }

    }

    public class EditBankDetails
    {

        public Bank Bank { get; set; }
        public string BrnachName { get; set; }

        public string AccountNo { get; set; }

        public string AccountType { get; set; }

        public string IFSC { get; set; }

        public int BankDetailsId { get; set; }

        public ObservableCollection<FileUploadInfo> FileUploadListInfo { get; set; }

    }

    public class SupplierTDSStatus
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsTDSApplicable { get; set; }
        public double PurchasedAmount { get; set; }
        public double CreditNoteAmount { get; set; }
        public double TDSLimit { get; set; }
        public double TDSDeduct { get; set; }
    }
}
