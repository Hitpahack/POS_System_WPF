using FalcaPOS.Entites.Common;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace FalcaPOS.Entites.Denomination
{
    public class DenominationModel : BindableBase
    {

        public string Store { get; set; }
        public string User { get; set; }
        private float _openingCash;

        public float OpeningCash
        {
            get { return _openingCash; }
            set { SetProperty(ref _openingCash, value); }
        }
        private float _totalSales;
        public float TotalSales { get { return _totalSales; } set { SetProperty(ref _totalSales, value); } }

        public float _cash;
        public float Cash { get { return _cash; } set { SetProperty(ref _cash, value); } }

        private float _upi;
        public float UPI { get { return _upi; } set { SetProperty(ref _upi, value); } }

        public float _availablecash;

        public float AvailableCash { get { return _availablecash; } set { SetProperty(ref _availablecash, value); } }

        private float _deposit;
        public float Deposit { get { return _deposit; } set { SetProperty(ref _deposit, value); } }

        private float _salesreturncash;
        public float SalesReturnCash { get { return _salesreturncash; } set { SetProperty(ref _salesreturncash, value); } }


        private float _closingCash;
        public float ClosingCash { get { return _closingCash; } set { SetProperty(ref _closingCash, value); } }

        private int _note_2000;
        public int notes_2000 { get { return _note_2000; } set { SetProperty(ref _note_2000, value); } }

        private int _note_500;
        public int notes_500 { get { return _note_500; } set { SetProperty(ref _note_500, value); } }

        private int _note_200;
        public int notes_200 { get { return _note_200; } set { SetProperty(ref _note_200, value); } }

        private int _note_100;
        public int notes_100 { get { return _note_100; } set { SetProperty(ref _note_100, value); } }

        private int _note_50;
        public int notes_50 { get { return _note_50; } set { SetProperty(ref _note_50, value); } }

        private int _note_20;
        public int notes_20 { get { return _note_20; } set { SetProperty(ref _note_20, value); } }

        private int _note_10;
        public int notes_10 { get { return _note_10; } set { SetProperty(ref _note_10, value); } }

        private int _coins;
        public int Coins { get { return _coins; } set { SetProperty(ref _coins, value); } }

        private float _total;
        public float Total
        {
            get
            {
                return _total = ((2000 * notes_2000) + (500 * notes_500) + (200 * notes_200)
                       + (100 * notes_100) + (50 * notes_50) + (20 * notes_20) + (10 * notes_10) + Coins);
            }
            set
            {
                SetProperty(ref _total, value);
            }
        }

        private decimal _totalin2000;
        public decimal Totalin2000
        {
            get { return _totalin2000 = (2000 * notes_2000); }

            set { SetProperty(ref _totalin2000, value); }
        }

        private decimal _totalin500;
        public decimal Totalin500 { get { return _totalin500 = (500 * notes_500); } set { SetProperty(ref _totalin500, value); } }

        private decimal _totalin200;
        public decimal Totalin200 { get { return _totalin200 = (200 * notes_200); } set { SetProperty(ref _totalin200, value); } }

        private decimal _totalin100;

        public decimal Totalin100 { get { return _totalin100 = (100 * notes_100); } set { SetProperty(ref _totalin100, value); } }

        private decimal _totalin50;

        public decimal Totalin50 { get { return _totalin50 = (50 * notes_50); } set { SetProperty(ref _totalin50, value); } }

        private decimal _totalin20;
        public decimal Totalin20 { get { return _totalin20 = (20 * notes_20); } set { SetProperty(ref _totalin20, value); } }

        private decimal _totalin10;

        public decimal Totalin10
        {
            get { return _totalin10 = (10 * notes_10); }
            set { SetProperty(ref _totalin10, value); }
        }

        private decimal _totalinCoins;
        public decimal TotalinCoins { get { return _totalinCoins = Coins; } set { SetProperty(ref _totalinCoins, value); } }

        private bool isopeningcash;

        public bool IsOpeningCash
        {
            get { return isopeningcash; }
            set { SetProperty(ref isopeningcash, value); }
        }


        private bool _isrole;

        public bool IsRole
        {
            get { return _isrole; }
            set { SetProperty(ref _isrole, value); }
        }
        private float _totalAmount;

        public float TotalAmount
        {
            get { return _totalAmount; }
            set { SetProperty(ref _totalAmount, value); }
        }

        private float _credit;
        public float Credit { get { return _credit; } set { SetProperty(ref _credit, value); } }

        private ObservableCollection<FileUploadInfo> fileUploadListInfo;
        public ObservableCollection<FileUploadInfo> FileUploadListInfo
        {
            get { return fileUploadListInfo; }
            set { SetProperty(ref fileUploadListInfo, value); }
        }

        public string DenominationDate { get; set; }
    }

    public class DenominationSearchModel
    {
        public string Store { get; set; }

        public int StoreId { get; set; }

        public DateTime OnDate { get; set; }
        public bool IsSameDate { get; set; }
    }

    public class DepositModel : BindableBase
    {

        private string _depositDate;
        public string DespositDate
        {
            get => _depositDate;
            set => SetProperty(ref _depositDate, value);

        }

        private float _depositAmount;
        public float DepostAmount
        {
            get => _depositAmount;
            set => SetProperty(ref _depositAmount, value);
        }

        private string _bankName;

        public string BankName
        {
            get => _bankName;
            set => SetProperty(ref _bankName, value);

        }

        private string _accountNo;
        public string AccountNo
        {
            get => _accountNo;
            set => SetProperty(ref _accountNo, value);
        }

        private string _ifscCode;
        public string IFSCCode
        {
            get => _ifscCode;
            set => SetProperty(ref _ifscCode, value);

        }

        private string _accounttype;
        public string AccoutType
        {
            get => _accounttype;
            set => SetProperty(ref _accounttype, value);

        }

        private string _branch;
        public string Branch
        {
            get => _branch;
            set => SetProperty(ref _branch, value);

        }

        private int _bankId;

        public int BankId
        {
            get => _bankId;
            set => SetProperty(ref _bankId, value);
        }



    }

    public class DepositViewModelResponse
    {
        public int Id { get; set; }
        public DateTime PosDepositDate { get; set; }

        public string DepositDate { get; set; }

        public string DocumnetNo { get; set; }

        public string StoreName { get; set; }

        public float DepositAmount { get; set; }

        public string BankName { get; set; }

        public string AccountType { get; set; }

        public string AccountNo { get; set; }


        public string IFSCCode { get; set; }

        public string Branch { get; set; }


        public int FileId { get; set; }


        public bool? IsVerified { get; set; }

        public string VerifiedDate { get; set; }

    }

    public class DepositExportModel
    {
        public DateTime PosDepositDate { get; set; }

        public string DepositDate { get; set; }

        public string DocumnetNo { get; set; }

        public string StoreName { get; set; }

        public float DepositAmount { get; set; }

        public string BankName { get; set; }

        public string AccountNo { get; set; }


        public string IFSCCode { get; set; }

        public string Branch { get; set; }



    }

    public class DepositBanksModel
    {
        public int BankId { get; set; }

        public string BankName { get; set; }

        public string AccountType { get; set; }

        public string AccountNo { get; set; }


        public string IFSCCode { get; set; }

        public string Branch { get; set; }

    }
    public class DenominationVerifyModel
    {
        public string DenominationDate { get; set; }

        public string Data { get; set; }
        public bool IsSuccess { get; set; }

        public string Error { get; set; }
    }

    public class AddDenominationModel
    {
        public string Store { get; set; }
        public string User { get; set; }
        public float OpeningCash { get; set; }
        public float TotalSales { get; set; }
        public float Cash { get; set; }
        public float UPI { get; set; }
        public float Deposit { get; set; }
        public float Credit { get; set; }
        public int SalesReturnCash { get; set; }
        public string DenominationDate { get; set; }

        public string NoteDetails { get; set; }
    }

    public class NotesModel
    {

        public int notes_2000 { get; set; }
        public int notes_500 { get; set; }
        public int notes_200 { get; set; }
        public int notes_100 { get; set; }
        public int notes_50 { get; set; }
        public int notes_20 { get; set; }
        public int notes_10 { get; set; }
        public int Coins { get; set; }
        public double Total { get; set; }
    }
}
