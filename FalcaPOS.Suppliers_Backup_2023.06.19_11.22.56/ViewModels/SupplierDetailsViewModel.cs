using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Suppliers;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace FalcaPOS.Suppliers.ViewModels
{
    public class SupplierDetailsViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;

        private readonly Logger _logger;


        public DelegateCommand<object> UpdateCommand { get; private set; }

        public DelegateCommand<object> AddBankFileAttachement { get; private set; }


        private readonly INotificationService _notificationService;


        public DelegateCommand<Guid?> DeleteUploadFileCommand { get; private set; }



        private readonly ISupplierService _supplierService;

        public DelegateCommand<int?> DownloadFileCommand { get; private set; }


        private readonly IInvoiceFileService _invoiceFileService;

        private readonly ProgressService _ProgressService;

        public DelegateCommand<object> AddNewBankDetailsCommand { get; private set; }

        public DelegateCommand<object> ResetNewBankDetailsCommand { get; private set; }

        public DelegateCommand<object> EditBankFileAttachement { get; private set; }


        public DelegateCommand<object> EditDeleteUploadFileCommand { get; private set; }

        public DelegateCommand<object> UploadMSMECertificateFile { get; private set; }

        public DelegateCommand<object> DeleteUploadMSMECommand { get; private set; }

        public DelegateCommand<int?> DownloadMSMECertificateFile { get; private set; }

        public SupplierDetailsViewModel(ProgressService ProgressService, IInvoiceFileService InvoiceFileService, ISupplierService SupplierService, INotificationService NotificationService, IEventAggregator EventAggregator, Logger Logger)
        {
            _notificationService = NotificationService ?? throw new ArgumentNullException(nameof(NotificationService));

            _eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));

            _logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            UpdateCommand = new DelegateCommand<object>(Update);

            _ = _eventAggregator.GetEvent<SupplierNewTabCreateEvent>().Subscribe(GetViewSupplier);

            _eventAggregator.GetEvent<AddNewBankEvent>().Subscribe(LoadBankList);

            AddBankFileAttachement = new DelegateCommand<object>(AddFileOpenDialog);

            DeleteUploadFileCommand = new DelegateCommand<Guid?>(DeleteUploadFile);

            _supplierService = SupplierService ?? throw new ArgumentNullException(nameof(SupplierService));

            SupplierType = ApplicationSettings.SUPPLIER_TYPE.ToList();


            _ = _eventAggregator.GetEvent<SupplierIDCreateEvent>().Subscribe(AddShippingAddress);

            DownloadFileCommand = new DelegateCommand<int?>(DownloadFile);

            _invoiceFileService = InvoiceFileService ?? throw new ArgumentNullException(nameof(InvoiceFileService));

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            LoadBankList();

            AddNewBankDetailsCommand = new DelegateCommand<object>(AddNewBankDetail);

            ResetNewBankDetailsCommand = new DelegateCommand<object>(ResetData);

            EditBankFileAttachement = new DelegateCommand<object>(EditFileOpenDialog);

            EditDeleteUploadFileCommand = new DelegateCommand<object>(EditDelteUploadFile);

            UploadMSMECertificateFile = new DelegateCommand<object>(UploadMSMEFileOpen);

            DeleteUploadMSMECommand = new DelegateCommand<object>(DeleteMSMEFile);

            DownloadMSMECertificateFile = new DelegateCommand<int?>(DownloadMSMEFile);
        }

        public async void AddShippingAddress(ShippingAddress supplier)
        {
            try
            {
                if (supplier != null)
                {
                    supplier.SupplierId = SuppliersDetails.SupplierId;

                    if (supplier.State != SuppliersDetails.Address?.State)
                    {
                        _notificationService.ShowMessage("Shipping Address State and Billing Address State not matching", NotificationType.Error);
                        return;
                    }


                    await Task.Run(async () =>
                    {

                        var _result = await _supplierService.AddShippingAddress(supplier);

                        if (_result)
                        {
                            Application.Current?.Dispatcher?.Invoke(async () =>
                            {

                                _notificationService.ShowMessage("Address added ", NotificationType.Success);

                                _eventAggregator.GetEvent<ShippingAddressEvent>().Publish(true);

                                var resultDetails = await _supplierService.GetSupplierDetails(SuppliersDetails.SupplierId);
                                if (resultDetails != null)
                                {
                                    SuppliersDetails = resultDetails.Data;
                                }


                            });


                        }
                    });


                }


            }
            catch (Exception _ex)
            {

                _logger.LogError(_ex.Message);
            }
        }

        private void DeleteUploadFile(Guid? fileId)
        {
            try
            {
                if (fileId == null) return;

                var _file = FileUploadListInfo?.FirstOrDefault(x => x.FileId == fileId);

                if (_file != null)
                {
                    _logger.LogInformation($"File Deleted {_file.FileName}");

                    FileUploadListInfo?.Remove(_file);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }


        }
        public void AddFileOpenDialog(object bank)
        {
            try
            {

                if (FileUploadListInfo != null && FileUploadListInfo.Count >= 1)
                {
                    _notificationService.ShowMessage("1 files already added. Delete old file", NotificationType.Information);

                    _logger.LogWarning($"Max File attachments added");

                    return;
                }


                OpenFileDialog dialog = new OpenFileDialog
                {
                    Title = "Please select a file",
                    Multiselect = true,
                    Filter = ApplicationSettings.Invoice_File_Extension_Filter,
                };


                bool? _resultOk = dialog.ShowDialog();

                if (_resultOk == null || _resultOk != true)
                {
                    //user cancelled file selection return.
                    return;
                }


                if (dialog != null)
                {
                    if (FileUploadListInfo == null)
                    {
                        FileUploadListInfo = new ObservableCollection<FileUploadInfo>();
                    }
                    if (dialog.FileNames != null && dialog.FileNames.Count() > 1)
                    {
                        _notificationService.ShowMessage("Can't add more than 1 file", NotificationType.Information);

                        _logger.LogWarning($"Max File attachments added");

                        return;
                    }
                    foreach (string _fileName in dialog.FileNames)
                    {
                        if (_fileName.IsValidString())
                        {
                            if (File.Exists(_fileName))
                            {
                                //Check for each fil size ...
                                FileInfo _fileInfo = new FileInfo(_fileName);

                                if ((_fileInfo.Length / (1024 * 1024)) > 10)
                                {
                                    _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} size should be less than 10 mb in size", NotificationType.Error);

                                    _logger.LogWarning($"Filename {_fileName} has size of {_fileInfo.Length / (1024 * 1024)} mb");

                                    continue;
                                }

                                //dont again add the same file .causes file access issue while reading file stream

                                if (FileUploadListInfo.Any(x => x.FileName == Path.GetFileName(_fileName)))
                                {
                                    _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} is already added", NotificationType.Information);

                                    _logger.LogWarning($"File was already added skipping file {_fileName}");

                                    continue;
                                }

                                _logger.LogInformation($"Adding file {_fileName}");

                                FileUploadListInfo?.Add(new FileUploadInfo
                                {
                                    FileId = Guid.NewGuid(),
                                    FilePath = _fileName,
                                    FileExtension = Path.GetExtension(_fileName),
                                    FileName = Path.GetFileName(_fileName),
                                    Size = FileHelper.FormatSize(_fileInfo.Length)
                                });
                                PopupClose = true;
                            }
                        }
                    }
                }
                else
                {
                    _notificationService.ShowMessage("Maximum 5 files allowed", NotificationType.Error);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public async void Update(object obj)
        {
            try
            {
                bool _isValid = ValidateData();

                if (!_isValid) return;

                var _supplier = GetSupplier();

                await _ProgressService.StartProgressAsync();

                await Task.Run(async () =>
                {

                    var _result = await _supplierService.UpdateSupplierDetails(_supplier);

                    if (_result.IsSuccess)
                    {
                        SuppliersDetailsViewModel suppliersDetails = new SuppliersDetailsViewModel()
                        {
                            SupplierId = _supplier.SupplierId
                        };
                        GetViewSupplier(suppliersDetails);
                        _notificationService.ShowMessage("Supplier Details Updated", Common.NotificationType.Success);

                    }
                    else
                    {
                        _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);

                    }
                  

                });

                await _ProgressService.StopProgressAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally {
                await _ProgressService.StopProgressAsync();
            }

        }

        private SupplierEditViewModel GetSupplier()
        {
            var fileUpload = new FileUploadInfo();
            if (ISMSME == "Yes" && !string.IsNullOrEmpty(supplierDetails.MSMECertificate.FileName)) {
                fileUpload.FileId = supplierDetails.MSMECertificate.FileGUID;
                fileUpload.FileName = supplierDetails.MSMECertificate.FileName;
                fileUpload.FileSrc = FileSrc.remote;
                    
              
            }
               
            var supplierData = new SupplierEditViewModel();
            supplierData.SupplierId = SuppliersDetails.SupplierId;
            supplierData.Name = SuppliersDetails?.Name;
            supplierData.IsMSME = ISMSME;
            supplierData.MSME = ISMSME == "No" ? null : SuppliersDetails.MSME;
            supplierData.FileUploadMSME = ISMSME == "No" ? null : FileUploadMSME==null?fileUpload:FileUploadMSME;
            supplierData.Suppliertype = SelectedSuppliertype;
            supplierData.TallyCode = SuppliersDetails.TallyCode;
            var selectedBanks = ListOFBankDetails.Where(x => x.IsSelected).ToList();
            var bank = new List<EditBankDetails>();
            foreach (var item in selectedBanks)
            {
                bank.Add(new EditBankDetails()
                {
                    Bank = new Bank()
                    {
                        id = item.BankId,
                        BankName = item.BankName,

                    },
                    BrnachName = item.BrnachName,
                    BankDetailsId = item.BankDetailsId,
                    AccountNo = item.AccountNo,
                    AccountType = item.AccountType,
                    IFSC = item.IFSC,
                    FileUploadListInfo = item.FileUploadListInfo
                });

            }
            supplierData.ListOfBankDetails = bank;
            supplierData.Phone = SuppliersDetails.Address?.Phone;
            supplierData.Email = SuppliersDetails.Address?.Email;


            return supplierData;
        }
        private bool ValidateData()
        {

            if (String.IsNullOrEmpty(SuppliersDetails.Name))
            {
                _notificationService.ShowMessage("Supplier name is required", NotificationType.Error);

                return false;
            }
            else if (string.IsNullOrEmpty(SelectedSuppliertype))
            {
                _notificationService.ShowMessage("supplier type is required", NotificationType.Error);

                return false;
            }
            else if (string.IsNullOrEmpty(SuppliersDetails.Address?.Phone))
            {
                _notificationService.ShowMessage("Phone number is required", NotificationType.Error);

                return false;
            }
            else if (string.IsNullOrEmpty(ISMSME))
            {
                _notificationService.ShowMessage("Please select MSME register", NotificationType.Error);
                return false;
            }
            else if (ISMSME == "Yes" && string.IsNullOrEmpty(SuppliersDetails.MSME))
            {

                _notificationService.ShowMessage("MSME Number is required", NotificationType.Error);
                return false;

            }
            else if (!string.IsNullOrEmpty(SuppliersDetails.MSME) && ISMSME == "Yes" && SuppliersDetails.MSME.Length < 12)
            {
                _notificationService.ShowMessage("MSME Number is can't accept less than 12 digits", NotificationType.Error);
                return false;
            }
            else if (String.IsNullOrEmpty(supplierDetails.TallyCode))
            {
                _notificationService.ShowMessage("TallyCode is required", NotificationType.Error);
                return false;
            }
            else if (ISMSME == "Yes" && !string.IsNullOrEmpty(SuppliersDetails.MSME) && FileUploadMSME==null)
            {

                _notificationService.ShowMessage("MSME certificate is required", NotificationType.Error);
                return false;

            }
            else if (ListOFBankDetails != null && ListOFBankDetails.Count > 0)
            {
                foreach (var item in ListOFBankDetails)
                {
                    if (item.IsSelected)
                    {
                        if (string.IsNullOrEmpty(item.SelectedBanks.BankName))
                        {
                            _notificationService.ShowMessage("Bank name is required", NotificationType.Error);
                            return false;
                        }
                        else if (string.IsNullOrEmpty(item.BrnachName))
                        {
                            _notificationService.ShowMessage("Branch name is required", NotificationType.Error);
                            return false;
                        }
                        else if (string.IsNullOrEmpty(item.AccountNo))
                        {
                            _notificationService.ShowMessage("Account number is required", NotificationType.Error);
                            return false;
                        }
                        else if (!item.AccountNo.IsDigitsOnly())
                        {
                            _notificationService.ShowMessage("Account number is only digits", NotificationType.Error);
                            return false;
                        }
                        else if (string.IsNullOrEmpty(item.AccountType))
                        {
                            _notificationService.ShowMessage("Please select Account type", NotificationType.Error);
                            return false;
                        }
                        else if (string.IsNullOrEmpty(item.IFSC))
                        {
                            _notificationService.ShowMessage("IFSC number is required", NotificationType.Error);
                            return false;
                        }
                        else if (!item.IFSC.isIfscCodeValid())
                        {
                            _notificationService.ShowMessage("Invalid IFSC Code", NotificationType.Error);
                            return false;
                        }
                        else if ((item.FileUploadListInfo == null || item.FileUploadListInfo.Count == 0))
                        {
                            _notificationService.ShowMessage("Attachment is required", NotificationType.Error);
                            return false;
                        }
                        else if (item.FileUploadListInfo != null)
                        {
                            if (item.FileUploadListInfo.Count > 1)
                            {
                                _notificationService.ShowMessage("Attachment can't accept more than 1 file", NotificationType.Error);
                                return false;
                            }

                        }
                    }

                }

                return true;
            }
            else if (ListOFBankDetails == null || ListOFBankDetails.Count == 0)
            {
                _notificationService.ShowMessage("Please Add bank details", NotificationType.Error);
                return false;
            }
            return false;





        }

        public async void GetViewSupplier(SuppliersDetailsViewModel supplier)
        {
            try
            {

                if (supplier != null)

                    await Task.Run(async () =>
                    {
                        var resultDetails = await _supplierService.GetSupplierDetails(supplier.SupplierId);

                        if (resultDetails != null && resultDetails.IsSuccess)
                        {
                            SuppliersDetails = resultDetails.Data;
                            SelectedSuppliertype = resultDetails.Data.Suppliertype;
                            ISMSME = resultDetails.Data.IsMSME;
                            //FileUploadListInfo = null;
                            //List<FileAttachment> fileAttachments = new List<FileAttachment>();
                            //if(resultDetails.Data.BankDetails!=null && resultDetails.Data.BankDetails.FileAttachment!=null
                            //&& resultDetails.Data.BankDetails.FileAttachment.FileId>0)
                            //fileAttachments.Add(resultDetails.Data.BankDetails?.FileAttachment);
                            //FileAttachment = fileAttachments;
                            FileUploadMSME = null;
                            List<BankDetails> bankDetails = new List<BankDetails>();
                            if (resultDetails.Data.ListOfBankDetails != null && resultDetails.Data.ListOfBankDetails.Count > 0)
                            {
                                List<Bank> _banks = new List<Bank>();
                                foreach (var i in BankList)
                                {
                                    _banks.Add(new Bank()
                                    {
                                        id = i.id,
                                        BankName = i.BankName,
                                    });
                                }

                                int j = 1;
                                foreach (var item in resultDetails.Data.ListOfBankDetails)
                                {

                                    bankDetails.Add(new BankDetails()
                                    {
                                        IFSC = item.IFSC,
                                        BankId = item.Bank.id,
                                        AccountNo = item.AccountNo,
                                        AccountType = item.AccountType,
                                        BankName = item.Bank.BankName,
                                        BrnachName = item.BrnachName,
                                        FileAttachment = item.FileAttachment,
                                        Banks = _banks,
                                        SelectedBanks = _banks.FirstOrDefault(x => x.BankName == item.Bank.BankName),
                                        IsSelected = false,
                                        SrNo = j,
                                        BankDetailsId = item.BankDetailsId,
                                    });

                                    j++;
                                }
                            }

                            ListOFBankDetails = bankDetails;
                        }

                    });


            }
            catch (Exception _ex)
            {

                _logger.LogError(_ex.Message);
            }
        }

        private async void DownloadFile(int? fileId)
        {
            if (fileId == null || fileId <= 0) return;

            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _invoiceFileService.DownloadFile(fileId.Value);

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        //null check
                        if (_result != null && _result.IsSuccess && _result.Data != null && _result.Data.FileStream != null)
                        {
                            SaveFileDialog sd = new SaveFileDialog
                            {
                                CreatePrompt = true,
                                OverwritePrompt = true,
                                DefaultExt = "zip",
                            };
                            var _saveFile = sd.ShowDialog();

                            if (_saveFile == true && sd.FileName.IsValidString())
                            {
                                sd.FileName = Path.ChangeExtension(sd.FileName, "zip");

                                File.WriteAllBytes(sd.FileName, _result.Data.FileStream);
                            }
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                        }

                    });

                });


            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public async void LoadBankList()
        {
            try
            {
                await Task.Run(async () =>
                {

                    var _result = await _supplierService.GetBankList();

                    if (_result != null && _result.IsSuccess)
                    {

                        BankList = _result.Data.ToList();

                    }

                });

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

            }
        }

        private SuppliersDetailsViewModel supplierDetails;
        public SuppliersDetailsViewModel SuppliersDetails
        {
            get => supplierDetails;
            set => SetProperty(ref supplierDetails, value);
        }

        private ObservableCollection<FileUploadInfo> fileUploadListInfo;
        public ObservableCollection<FileUploadInfo> FileUploadListInfo
        {
            get { return fileUploadListInfo; }
            set { SetProperty(ref fileUploadListInfo, value); }
        }



        private string bankName;
        public string BankName { get { return bankName; } set { SetProperty(ref bankName, value); } }

        private string branchName;
        public string BrnachName { get { return branchName; } set { SetProperty(ref branchName, value); } }

        private string accountno;
        public string AccountNo { get { return accountno; } set { SetProperty(ref accountno, value); } }

        private string accounttype;
        public string AccountType { get { return accounttype; } set { SetProperty(ref accounttype, value); } }

        private string ifsc;
        public string IFSC { get { return ifsc; } set { SetProperty(ref ifsc, value); } }


        private List<Bank> _bankList;

        public List<Bank> BankList
        {
            get => _bankList;
            set => SetProperty(ref _bankList, value);
        }

        private Bank selectedBank;

        public Bank SelectedBank
        {
            get => selectedBank;
            set => SetProperty(ref selectedBank, value);
        }


        private List<string> suppliertype;
        public List<string> SupplierType { get { return suppliertype; } set { SetProperty(ref suppliertype, value); } }


        private string selectedsuppliertype;
        public string SelectedSuppliertype { get { return selectedsuppliertype; } set { SetProperty(ref selectedsuppliertype, value); } }

        private string isMSME;
        public string ISMSME
        {
            get { return isMSME; }
            set
            {

                SetProperty(ref isMSME, value);
                if ("No" == value)
                    ISMSMEenable = false;
                else
                    ISMSMEenable = true;
            }
        }

        private bool isMSMEenable;
        public bool ISMSMEenable
        {
            get { return isMSMEenable; }
            set { SetProperty(ref isMSMEenable, value); }
        }

        private List<FileAttachment> _fileattachement;
        public List<FileAttachment> FileAttachment { get { return _fileattachement; } set { SetProperty(ref _fileattachement, value); } }



        private List<BankDetails> _listofBankDetails;

        public List<BankDetails> ListOFBankDetails
        {
            get { return _listofBankDetails; }
            set { SetProperty(ref _listofBankDetails, value); }
        }

        private bool _popupClose;

        public bool PopupClose
        {
            get { return _popupClose; }
            set { SetProperty(ref _popupClose, value); }
        }

        public async void AddNewBankDetail(object ob)
        {
            try
            {
                if (SelectedBank == null || string.IsNullOrEmpty(SelectedBank.BankName))
                {
                    _notificationService.ShowMessage("Bank name is required", NotificationType.Error);
                    return;
                }
                else if (string.IsNullOrEmpty(BrnachName))
                {
                    _notificationService.ShowMessage("Branch name is required", NotificationType.Error);
                    return;
                }
                else if (string.IsNullOrEmpty(AccountNo))
                {
                    _notificationService.ShowMessage("Account number is required", NotificationType.Error);
                    return;
                }
                else if (!AccountNo.IsDigitsOnly())
                {
                    _notificationService.ShowMessage("Account number is only digits", NotificationType.Error);
                    return;
                }
                else if (string.IsNullOrEmpty(AccountType))
                {
                    _notificationService.ShowMessage("Please select Account type", NotificationType.Error);
                    return;
                }
                else if (string.IsNullOrEmpty(IFSC))
                {
                    _notificationService.ShowMessage("IFSC number is required", NotificationType.Error);
                    return;
                }
                else if (!IFSC.isIfscCodeValid())
                {
                    _notificationService.ShowMessage("Invalid IFSC Code", NotificationType.Error);
                    return;
                }
                else if ((FileAttachment == null || FileAttachment.Count == 0) && (FileUploadListInfo == null || FileUploadListInfo.Count == 0))
                {
                    _notificationService.ShowMessage("Attachment is required", NotificationType.Error);
                    return;
                }
                else if (FileUploadListInfo != null)
                {
                    if (FileUploadListInfo.Count > 1)
                    {
                        _notificationService.ShowMessage("Attachment can't accept more than 1 file", NotificationType.Error);
                        return;
                    }

                }

                var _result = await _supplierService.AddNewBankDetails(SuppliersDetails.SupplierId, SelectedBank.id, BrnachName, AccountNo, AccountType, IFSC, FileUploadListInfo?.ToArray());

                if (_result != null && _result.IsSuccess)
                {
                    _notificationService.ShowMessage(_result.Data, NotificationType.Success);
                    AccountNo = null;
                    BrnachName = null;
                    SelectedBank = null;
                    IFSC = null;
                    AccountType = null;
                    FileUploadListInfo.Clear();
                    SuppliersDetailsViewModel suppliers = new SuppliersDetailsViewModel()
                    {
                        SupplierId = SuppliersDetails.SupplierId
                    };
                    GetViewSupplier(suppliers);
                    return;
                }
                else
                {
                    _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                    return;
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public void ResetData(object obj)
        {
            try
            {
                AccountNo = null;
                BrnachName = null;
                SelectedBank = null;
                IFSC = null;
                AccountType = null;
                if (FileUploadListInfo != null && FileUploadListInfo.Count > 0)
                {
                    FileUploadListInfo.Clear();
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void EditFileOpenDialog(object bank)
        {
            try
            {

                var _viewmodel = (BankDetails)bank;

                if (_viewmodel.FileUploadListInfo != null && _viewmodel.FileUploadListInfo.Count >= 1)
                {
                    _notificationService.ShowMessage("1 files already added. Delete old file", NotificationType.Information);

                    _logger.LogWarning($"Max File attachments added");

                    return;
                }


                OpenFileDialog dialog = new OpenFileDialog
                {
                    Title = "Please select a file",
                    Multiselect = true,
                    Filter = ApplicationSettings.Invoice_File_Extension_Filter,
                };


                bool? _resultOk = dialog.ShowDialog();

                if (_resultOk == null || _resultOk != true)
                {
                    //user cancelled file selection return.
                    return;
                }


                if (dialog != null)
                {
                    if (_viewmodel.FileUploadListInfo == null)
                    {
                        _viewmodel.FileUploadListInfo = new ObservableCollection<FileUploadInfo>();
                    }
                    if (dialog.FileNames != null && dialog.FileNames.Count() > 1)
                    {
                        _notificationService.ShowMessage("Can't add more than 1 file", NotificationType.Information);

                        _logger.LogWarning($"Max File attachments added");

                        return;
                    }
                    foreach (string _fileName in dialog.FileNames)
                    {
                        if (_fileName.IsValidString())
                        {
                            if (File.Exists(_fileName))
                            {
                                //Check for each fil size ...
                                FileInfo _fileInfo = new FileInfo(_fileName);

                                if ((_fileInfo.Length / (1024 * 1024)) > 10)
                                {
                                    _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} size should be less than 10 mb in size", NotificationType.Error);

                                    _logger.LogWarning($"Filename {_fileName} has size of {_fileInfo.Length / (1024 * 1024)} mb");

                                    continue;
                                }

                                //dont again add the same file .causes file access issue while reading file stream

                                if (_viewmodel.FileUploadListInfo.Any(x => x.FileName == Path.GetFileName(_fileName)))
                                {
                                    _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} is already added", NotificationType.Information);

                                    _logger.LogWarning($"File was already added skipping file {_fileName}");

                                    continue;
                                }

                                _logger.LogInformation($"Adding file {_fileName}");

                                _viewmodel.FileUploadListInfo?.Add(new FileUploadInfo
                                {
                                    FileId = Guid.NewGuid(),
                                    FilePath = _fileName,
                                    FileExtension = Path.GetExtension(_fileName),
                                    FileName = Path.GetFileName(_fileName),
                                    Size = FileHelper.FormatSize(_fileInfo.Length),
                                    FileSrc = FileSrc.local,
                                    Id = _viewmodel.AccountNo,

                                });

                            }
                        }
                    }
                }
                else
                {
                    _notificationService.ShowMessage("Maximum 5 files allowed", NotificationType.Error);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        private void EditDelteUploadFile(object fileId)
        {
            try
            {
                if (fileId == null) return;

                var _togglemodel = Convert.ToInt16(((ToggleButton)fileId).Content);

                var _viewMdoel = ListOFBankDetails.FirstOrDefault(x => x.SrNo == _togglemodel);

                if (_viewMdoel != null)
                {
                    _logger.LogInformation($"File Deleted {_viewMdoel.FileUploadListInfo.FirstOrDefault().FileName}");

                    _viewMdoel.FileUploadListInfo?.Clear();
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }


        }

        private FileUploadInfo _fileUploadMSME;
        public FileUploadInfo FileUploadMSME
        {
            get => _fileUploadMSME;
            set => SetProperty(ref _fileUploadMSME, value);
        }

        public void UploadMSMEFileOpen(object obj)
        {
            try
            {
                if (FileUploadMSME != null)
                {
                    _notificationService.ShowMessage("file already added. Delete old file", NotificationType.Information);

                    _logger.LogWarning($"file already added. Delete old file");

                    return;
                }


                OpenFileDialog dialog = new OpenFileDialog
                {
                    Title = "Please select a file",
                    Multiselect = true,
                    Filter = ApplicationSettings.Invoice_File_Extension_Filter,
                };


                bool? _resultOk = dialog.ShowDialog();

                if (_resultOk == null || _resultOk != true)
                {
                    //user cancelled file selection return.
                    return;
                }


                if (dialog != null)
                {

                    if (FileUploadMSME == null)
                    {
                        FileUploadMSME = new FileUploadInfo();
                    }
                    if (dialog.FileNames != null && dialog.FileNames.Count() > 1)
                    {
                        _notificationService.ShowMessage("Can't add more than 1 file", NotificationType.Information);

                        _logger.LogWarning($"Max File attachments added");

                        FileUploadMSME = null;

                        return;
                    }
                    foreach (string _fileName in dialog.FileNames)
                    {
                        if (_fileName.IsValidString())
                        {
                            if (File.Exists(_fileName))
                            {
                                //Check for each fil size ...
                                FileInfo _fileInfo = new FileInfo(_fileName);

                                if ((_fileInfo.Length / (1024 * 1024)) > 10)
                                {
                                    _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} size should be less than 10 mb in size", NotificationType.Error);

                                    _logger.LogWarning($"Filename {_fileName} has size of {_fileInfo.Length / (1024 * 1024)} mb");
                                    FileUploadMSME = null;
                                    continue;
                                }

                                //dont again add the same file .causes file access issue while reading file stream

                                if (FileUploadMSME.FileName == Path.GetFileName(_fileName))
                                {
                                    _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} is already added", NotificationType.Information);

                                    _logger.LogWarning($"File was already added skipping file {_fileName}");
                                    FileUploadMSME = null;
                                    continue;
                                }

                                _logger.LogInformation($"Adding file {_fileName}");

                                FileUploadMSME.FileId = Guid.NewGuid();
                                FileUploadMSME.FilePath = _fileName;
                                FileUploadMSME.FileExtension = Path.GetExtension(_fileName);
                                FileUploadMSME.FileName = Path.GetFileName(_fileName);
                                FileUploadMSME.Size = FileHelper.FormatSize(_fileInfo.Length);
                                FileUploadMSME.FileSrc = FileSrc.local;



                            }
                        }
                    }
                }
                else
                {
                    _notificationService.ShowMessage("only one file allowed", NotificationType.Error);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void DeleteMSMEFile(object obj)
        {
            try
            {
                FileUploadMSME = null;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void DownloadMSMEFile(int? fileId)
        {

            if (fileId == null || fileId <= 0) return;

            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _invoiceFileService.DownloadFileMSME(fileId.Value);

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        //null check
                        if (_result != null && _result.IsSuccess && _result.Data != null && _result.Data.FileStream != null)
                        {
                            SaveFileDialog sd = new SaveFileDialog
                            {
                                CreatePrompt = true,
                                OverwritePrompt = true,
                                DefaultExt = "zip",
                            };
                            var _saveFile = sd.ShowDialog();

                            if (_saveFile == true && sd.FileName.IsValidString())
                            {
                                sd.FileName = Path.ChangeExtension(sd.FileName, "zip");

                                File.WriteAllBytes(sd.FileName, _result.Data.FileStream);
                            }
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                        }

                    });

                });


            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


    }

}
