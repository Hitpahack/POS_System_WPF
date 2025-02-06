using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Sku;
using FalcaPOS.Entites.Suppliers;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ProductType = FalcaPOS.Entites.ProductTypes.ProductType;

namespace FalcaPOS.Sku.ViewModels
{
    public class AlterSKUViewModel : BindableBase
    {
        public DelegateCommand<object> SearchSkuCommand { get; private set; }

        public DelegateCommand<object> DeleteUploadFileCommand { get; private set; }

        public DelegateCommand ClearSkuCommand { get; private set; }


        private readonly INotificationService _notificationService;

        private readonly Logger _logger;

        private ObservableCollection<Entites.Stores.Store> _stores;

        private Entites.Stores.Store _Selectedstores;


        private readonly IStoreService _storeService;

        public string headerNo { get; set; }

        public string AttachmentName { get; set; }

        public string SearchHeaderName { get; set; }

        private bool _validuptxtBoxVisibility;
        public bool ValidtxtBoxVisibility
        {
            get { return _validuptxtBoxVisibility; }
            set { SetProperty(ref _validuptxtBoxVisibility, value); }
        }


        private bool _lifetimeCheckBoxVisibility;
        public bool LifeTimeBoxVisibility
        {
            get { return _lifetimeCheckBoxVisibility; }
            set { SetProperty(ref _lifetimeCheckBoxVisibility, value); }
        }

        private bool _genericVisibility;

        public bool GenericVisibility
        {
            get { return _genericVisibility; }
            set { SetProperty(ref _genericVisibility, value); }
        }


        private bool _gridVisibility;
        public bool GridBoxVisibility
        {
            get { return _gridVisibility; }
            set { SetProperty(ref _gridVisibility, value); }
        }
        public DelegateCommand<object> AddFileAttachmentCommand { get; private set; }


        public DelegateCommand<object> ViewFileAttachmentCommand { get; private set; }

        private readonly ISkuService _skuService;

        private readonly ProgressService _ProgressService;

        public DelegateCommand<object> UpdateSkuCommand { get; private set; }


        private readonly IInvoiceFileService _invoiceFileService;

        public DelegateCommand<int?> GetPDFdownloadCommand { get; private set; }

        public DelegateCommand<Object> SearchExistingSKUCertificateCommand { get; private set; }

        private readonly ISupplierService _supplierService;

        private readonly IProductTypeService _productTypeService;

        public DelegateCommand<object> CategoryChangeCommand { get; private set; }


        public DelegateCommand<object> SubCategoryChangeCommand { get; private set; }


        public AlterSKUViewModel(IProductTypeService productTypeService, ISupplierService supplierService, IStoreService storeService, IInvoiceFileService invoiceFileService, ProgressService ProgressService, ISkuService skuService, Logger logger, INotificationService notificationService)
        {

            GridBoxVisibility = false;

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            SearchSkuCommand = new DelegateCommand<object>(SearchSku);

            ClearSkuCommand = new DelegateCommand(ClearSku);

            AddFileAttachmentCommand = new DelegateCommand<object>(AddFileOpenDialog);

            ViewFileAttachmentCommand = new DelegateCommand<object>(ViewFileAttachment);
            DeleteUploadFileCommand = new DelegateCommand<object>(DeleteUploadFile);

            _skuService = skuService ?? throw new ArgumentNullException(nameof(skuService));

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            _invoiceFileService = invoiceFileService ?? throw new ArgumentNullException(nameof(invoiceFileService));

            _productTypeService = productTypeService ?? throw new ArgumentNullException(nameof(productTypeService));


            UpdateSkuCommand = new DelegateCommand<object>(UpdateSKU);

            // GetPDFdownloadCommand = new DelegateCommand<int?>(GetPdfDownload);

            SearchExistingSKUCertificateCommand = new DelegateCommand<Object>(SearchExistingSKUCertificate);

            DepartmentList = GetCertificateProduct();

            LoadStores();

            LoadSuppliers();

            LoadProductTypes();

            LoadOldProductTypes();

            CategoryChangeCommand = new DelegateCommand<object>(CategoryChange);

            StoreVisibility = (AppConstants.USER_ROLES[0] == (AppConstants.ROLE_PURCHASE_MANAGER) || AppConstants.USER_ROLES[0] == AppConstants.ROLE_ADMIN) ? true : false;

            SubCategoryChangeCommand = new DelegateCommand<object>(LoadManufacturer);




        }



        private void DeleteUploadFile(object obj)
        {
            try
            {
                var viewModel = ((FileUploadInfo)obj);
                if (viewModel != null)
                {
                    if ((FileUploadListInfo.Where(x => x.FileId == viewModel.FileId).FirstOrDefault() != null))
                    {
                        FileUploadListInfo.Clear();
                    }

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        private async void SearchExistingSKUCertificate(Object args)
        {
            try
            {

                if (String.IsNullOrEmpty(SelectedSubCategory.SubCategoryName))
                {
                    _notificationService.ShowMessage("sub category can not to be empty", NotificationType.Error);
                    return;
                }

                if (String.IsNullOrEmpty(SKUmodel?.BrandName))
                {
                    _notificationService.ShowMessage("Brand can not to be empty", NotificationType.Error);
                    return;
                }

                //if (SKUmodel.SupplierName == null && SKUmodel.Supplierid == 0)
                //{
                //    if (SelectedSuppliers == null)
                //    {
                //        _notificationService.ShowMessage("Supplier can not  to be empty", NotificationType.Error);
                //        return;
                //    }
                //}
                //if (SKUmodel.SupplierName == null && SKUmodel.Supplierid == 0)
                //{
                //    SKUmodel.SupplierName = SelectedSuppliers.Name;
                //    SKUmodel.Supplierid = SelectedSuppliers.SupplierId;


                //}
                if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER)
                {
                    if (SelectedStores == null)
                    {
                        _notificationService.ShowMessage("Please select store", NotificationType.Error);
                        return;
                    }
                }
                int StoreId = AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER ? SelectedStores.StoreId : AppConstants.LoggedInStoreInfo.StoreId;


                await _ProgressService.StartProgressAsync();
                var _productcertificate = await _skuService.GetproductCertificate(new Entites.Sku.ProductCertificateSearch() { departmentId = (int)SelectedSubCategory.Id, manufactureID = SelectedManufacturer.ManufacturerId, storeId = StoreId });
                if (((_productcertificate?.IsSuccess == true)) && _productcertificate?.Data != null)
                {

                    FileUploadInfo fileUploadInfo = new FileUploadInfo()
                    {
                        FileremoteSrcID = _productcertificate.Data.FileremoteSrcID,
                        FileName = _productcertificate.Data.FileName,
                        FileId = _productcertificate.Data.FileId,
                        Size = _productcertificate.Data.Size,
                        FileSrc = FileSrc.remote
                    };

                    FileUploadListInfo?.Clear();

                    if (FileUploadListInfo == null)
                        FileUploadListInfo = new ObservableCollection<FileUploadInfo>();

                    FileUploadListInfo.Add(fileUploadInfo);

                    if (_productcertificate.Data.SKUmodel != null)
                    {
                        SKUmodel.Number = _productcertificate.Data.SKUmodel.Number;
                        SKUmodel.Generic = _productcertificate.Data.SKUmodel.Generic;

                        SKUmodel.IssueDate = String.Format("{0:dd-MM-yyyy}", _productcertificate.Data.SKUmodel.IssueDate);
                        ExistingCertificate = _productcertificate.Data.SKUmodel;
                    }


                }
                else
                {
                    _notificationService.ShowMessage(@"Attachment not found", NotificationType.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                await _ProgressService.StopProgressAsync();
            }
        }

        private async void ViewFileAttachment(object obj)
        {
            try
            {
                var fileView = ((FileUploadInfo)obj);
                if (fileView.FileSrc == FileSrc.local)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(fileView.FilePath);
                    Process.Start(startInfo);
                }
                else
                {
                    await GetPdfDownload(fileView?.FileremoteSrcID);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

        }

        private async void SearchSku(object obj)
        {
            try
            {


                

                if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER)
                {
                    if (SelectedStores == null)
                    {
                        _notificationService.ShowMessage("Please select store ", NotificationType.Error);
                        return;

                    }
                }
                if (SelectedCategory == null) {
                    _notificationService.ShowMessage("Please select category ", NotificationType.Error);
                    return;
                }

                if (SelectedSubCategory == null) {
                    _notificationService.ShowMessage("Please select sub category ", NotificationType.Error);
                    return;

                }

                if (SelectedManufacturer == null) {
                    _notificationService.ShowMessage("Please select brand ", NotificationType.Error);
                    return;

                }

                int StoreId = AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER ? SelectedStores.StoreId : AppConstants.LoggedInStoreInfo.StoreId;



                await _ProgressService.StartProgressAsync();


                var result = await _skuService.AlterSKUSearch(StoreId, (int)SelectedSubCategory.Id, SelectedManufacturer.ManufacturerId);

                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    if (result.IsSuccess && result.Data != null)
                    {
                        headerChange(SelectedCategory.CategoryName.ToLower());
                        //var searchproduct = result.Data.ProductLists.FirstOrDefault(x => x.ProductSKU == SearchSKU);
                        List<ProductCertificateViewModel> productDetails = new List<ProductCertificateViewModel>();
                        foreach (var item in result.Data.ProductsList)
                        {
                            productDetails.Add(new ProductCertificateViewModel()
                            {
                                ProductId = item.ProductId,
                                Name = item.Name,
                                ProductSKU = item.ProductSKU,
                                ProductType = item.ProductType,
                                ProductTypeManufacturerId = item.ProductTypeManufacturerId,
                                Manufacturer = item.Manufacturer,
                                LifeTimeVisiblity = LifeTimeBoxVisibility,
                                LifeTime = item.IsCertificate == true ? (item.ValidUpto != null ? false : true) : false,
                                ValidUpto = item.ValidUpto,
                                Description = item.Description,
                                SubUnitType = item.SubUnitType,
                                SerailNumber = item.SerailNumber,
                                LicenseNumber = item.LicenseNumber

                            });
                        }

                        SKUViewModelVM sKUModel = new SKUViewModelVM()
                        {
                            IssueDate = result.Data.IssueDate,
                            BrandName = result.Data.BrandName,
                            SupplierName = result.Data.SupplierName,
                            Generic = result.Data.Generic,

                            Number = result.Data.Number,
                            HeaderName = headerNo,
                            //Producttype = result.Data.Producttype,
                            GenericVisibilty = GenericVisibility,
                            AttachmentHeaderName = AttachmentName,
                            SearchHeaderName = SearchHeaderName,
                            Supplierid = result.Data.Supplierid,

                            ProductsList = productDetails,
                        };

                        SelectedSuppliers = Suppliers.FirstOrDefault(x => x.SupplierId == result.Data.Supplierid);
                        SKUmodel = sKUModel;
                        GridBoxVisibility = true;
                    }
                    else
                    {
                        _notificationService.ShowMessage(result.Error, NotificationType.Error);
                        SKUmodel = null;
                        GridBoxVisibility = false;
                    }
                });

                await _ProgressService.StopProgressAsync();

                if (FileUploadListInfo != null && FileUploadListInfo.Count > 0)
                {
                    FileUploadListInfo.Clear();
                }


            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {
                await _ProgressService.StopProgressAsync();

            }
        }

        private async void UpdateSKU(object obj)
        {
            try
            {
                var model = ((AlterSKUViewModel)obj).SKUmodel;
                if (model != null)
                {
                    if (model.SupplierName == null && model.Supplierid == 0)
                    {
                        if (SelectedSuppliers == null)
                        {
                            _notificationService.ShowMessage("Please select supplier", NotificationType.Error);
                            return;
                        }
                    }
                    if (string.IsNullOrEmpty(model.Number))
                    {
                        _notificationService.ShowMessage("Please enter number", NotificationType.Error);
                        return;
                    }

                    if (string.IsNullOrEmpty(Convert.ToString(model.IssueDate)))
                    {
                        _notificationService.ShowMessage("Please enter Date of issue", NotificationType.Error);
                        return;
                    }


                    if (SelectedCategory.CategoryName.ToLower() == "fertilizers")
                    {
                        if (string.IsNullOrEmpty(model.Generic))
                        {
                            _notificationService.ShowMessage("Please enter Authority by whom", NotificationType.Error);
                            return;
                        }

                    }
                    if (SelectedCategory.CategoryName.ToLower() == "")
                    {
                        foreach (var item in model.ProductsList)
                        {

                            if (!string.IsNullOrEmpty(Convert.ToString(item.ValidUpto)) && item.LifeTime == true)
                            {
                                _notificationService.ShowMessage("Please enter ValidUpto Date or LifeTime", NotificationType.Error);
                                return;
                            }

                            if (string.IsNullOrEmpty(Convert.ToString(item.ValidUpto)) && item.LifeTime == false)
                            {
                                _notificationService.ShowMessage("Please enter ValidUpto Date or LifeTime", NotificationType.Error);
                                return;
                            }
                        }
                    }

                    foreach (var item in model.ProductsList)
                    {
                        if (item.SerailNumber == null)
                        {
                            _notificationService.ShowMessage("Please enter the Serial Number", NotificationType.Error);
                            return;
                        }

                        if (string.IsNullOrEmpty(item.LicenseNumber))
                        {
                            _notificationService.ShowMessage("Please enter the License Number", NotificationType.Error);
                            return;
                        }
                    }

                    if (SelectedCategory.CategoryName.ToLower() != "crop protection chemicals")
                    {
                        foreach (var item in model.ProductsList)
                        {
                            item.LifeTime = false;
                            if (string.IsNullOrEmpty(Convert.ToString(item.ValidUpto)))
                            {
                                _notificationService.ShowMessage("Please enter Valid Upto Date", NotificationType.Error);
                                return;
                            }
                        }

                    }


                    if (FileUploadListInfo == null || FileUploadListInfo.Count == 0)
                    {
                        _notificationService.ShowMessage("Please Add Attachment", NotificationType.Error);
                        return;
                    }

                    var Exiting = ((AlterSKUViewModel)obj).ExistingCertificate;
                    if (Exiting != null)
                    {
                        if (Exiting.Number != model.Number)
                        {
                            _notificationService.ShowMessage("Please check does not matching with existing " + model.HeaderName, NotificationType.Error);
                            return;
                        }
                        if (Exiting.Generic != model.Generic)
                        {
                            _notificationService.ShowMessage("Please check does not matching with existing Authority by Whom issued ", NotificationType.Error);
                            return;
                        }

                        if (Convert.ToString(Exiting.IssueDate) != Convert.ToString(model.IssueDate))
                        {
                            _notificationService.ShowMessage("Please check does not matching with existing issue date", NotificationType.Error);
                            return;
                        }
                    }
                    model.PictureId = FileUploadListInfo?.FirstOrDefault()?.FileremoteSrcID;

                    int StoreId = AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER ? SelectedStores.StoreId : AppConstants.LoggedInStoreInfo.StoreId;



                    model.SupplierName = SelectedSuppliers.Name;
                    model.Supplierid = (int)SelectedSuppliers.SupplierId;

                    List<UpdateSKUProduct> updateSKUProduct = new List<UpdateSKUProduct>();
                    foreach (var item in model.ProductsList)
                    {
                        updateSKUProduct.Add(new UpdateSKUProduct()
                        {
                            ProductId = (int)item.ProductId,
                            LicenseNumber = item.LicenseNumber,
                            LifeTime = item.LifeTime,
                            SerailNumber = (int)item.SerailNumber,
                            ValidUpto = (DateTime?)item.ValidUpto,

                        });
                    };


                    UpdateSKVModel updateSKVModel = new UpdateSKVModel()
                    {
                        IssueDate = model.IssueDate,
                        PictureId = model.PictureId,
                        Generic = model.Generic,
                        Number = model.Number,
                        Supplierid = model.Supplierid,
                        SupplierName = model.SupplierName,
                        UpdateSKUProducts = updateSKUProduct,
                    };

                    await _ProgressService.StartProgressAsync();


                    var result = await _skuService.UpdateSKUSearch(StoreId, (int)SelectedSubCategory.Id, SelectedManufacturer.ManufacturerId, updateSKVModel);

                    if (result.IsSuccess)
                    {
                        if (FileUploadListInfo != null && FileUploadListInfo.Count > 0 && FileUploadListInfo.Any(x => x.FileSrc == FileSrc.local))
                        {

                            List<string> resuestid = new List<string>();
                            resuestid.Add(result.Data);
                            var _updateresult = await _skuService.UploadFiles(resuestid, FileUploadListInfo.Where(x => x.FileSrc == FileSrc.local).ToArray());
                            if (_updateresult != null && _updateresult.IsSuccess)
                            {
                                _notificationService.ShowMessage("SKU Product Details Updated", NotificationType.Success);
                            }
                        }
                        _notificationService.ShowMessage("SKU Product Details Updated", NotificationType.Success);

                        SKUmodel = null;
                        GridBoxVisibility = false;
                    }
                    else
                    {
                        _notificationService.ShowMessage(result.Error, NotificationType.Error);

                    }
                    await _ProgressService.StopProgressAsync();
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
                await _ProgressService.StopProgressAsync();
            }
        }

        private void ClearSku()
        {
            try
            {
                GridBoxVisibility = false;
                SKUmodel = null;
                SearchSKU = null;
                SelectedManufacturer = null;
                SelectedProductType = null;
                if (FileUploadListInfo != null && FileUploadListInfo.Count > 0)
                {
                    FileUploadListInfo.Clear();
                }

                SelectedStores = null;
                SelectedCategory = null;
                SelectedSubCategory = null;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private string _searchSku;

        public string SearchSKU
        {
            get { return _searchSku; }
            set { SetProperty(ref _searchSku, value); }
        }

        private string _departName;

        public string DepartName
        {
            get { return _departName; }
            set { SetProperty(ref _departName, value); }
        }


        private SKUViewModelVM _skumodel;

        public SKUViewModelVM SKUmodel
        {
            get => _skumodel;
            set => SetProperty(ref _skumodel, value);
        }

        private void headerChange(string category)
        {
            GenericVisibility = false;
            LifeTimeBoxVisibility = false;

            switch (category.ToLower())
            {
                case "crop protection chemicals":
                    headerNo = "PC No *";
                    SearchHeaderName = "Search PC";
                    AttachmentName = "ADD PC ATTACHMENT";
                    LifeTimeBoxVisibility = true;

                    break;
                case "fertilizers":
                case "speciality nutrients":
                    headerNo = "O Form Number *";
                    SearchHeaderName = "Search O Form";
                    AttachmentName = "ADD O FORM ATTACHMENT";
                    GenericVisibility = true;
                    break;
                case "seeds":
        
                    headerNo = "Source Certificate Number *";
                    SearchHeaderName = "Search Source Certificate";
                    AttachmentName = "ADD SOURCE CERTIFICATE ATTACHMENT";

                    break;
                default:

                    break;
            }
        }

        private ObservableCollection<FileUploadInfo> fileUploadListInfo;
        public ObservableCollection<FileUploadInfo> FileUploadListInfo
        {
            get { return fileUploadListInfo; }
            set { SetProperty(ref fileUploadListInfo, value); }
        }



        public void AddFileOpenDialog(object file)
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
                                    Size = FileHelper.FormatSize(_fileInfo.Length),
                                    FileSrc = FileSrc.local

                                });
                                if (ExistingCertificate != null)
                                {
                                    ExistingCertificate = null;
                                }

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

        //private async void GetPdfDownload(int? fileId)
        //{
        //    try
        //    {
        //        if (fileId != null && fileId.Value > 0)
        //        {
        //            var _result = await _invoiceFileService.DownloadFile(fileId.Value);

        //            if (_result != null)
        //            {
        //                if (_result != null && _result.IsSuccess && _result.Data != null && _result.Data.FileStream != null)
        //                {
        //                    SaveFileDialog sd = new SaveFileDialog
        //                    {
        //                        CreatePrompt = true,
        //                        OverwritePrompt = true,
        //                        DefaultExt = "zip",
        //                    };
        //                    var _saveFile = sd.ShowDialog();

        //                    if (_saveFile == true && sd.FileName.IsValidString())
        //                    {
        //                        sd.FileName = Path.ChangeExtension(sd.FileName, "zip");

        //                        File.WriteAllBytes(sd.FileName, _result.Data.FileStream);
        //                    }
        //                }
        //                else
        //                {
        //                    _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception _ex)
        //    {
        //        _logger.LogError(_ex.Message);

        //    }
        //}


        private async Task GetPdfDownload(int? fileId)
        {
            try
            {
                if (fileId != null && fileId.Value > 0)
                {
                    var _result = await _invoiceFileService.DownloadFile(fileId.Value);

                    if (_result != null)
                    {
                        if (_result != null && _result.IsSuccess && _result.Data != null && _result.Data.FileStream != null)
                        {

                            try
                            {
                                if (!Directory.Exists(ApplicationSettings.POSTempPath))
                                {
                                    Directory.CreateDirectory(ApplicationSettings.POSTempPath);
                                }

                                string path = ApplicationSettings.POSTempPath;



                                _result.Data.FileName = Path.ChangeExtension(_result.Data.FileName, "zip");

                                File.WriteAllBytes(Path.Combine(ApplicationSettings.POSTempPath, _result.Data.FileName), _result.Data.FileStream);
                                string uncompressedFile = null;
                                using (var z = ZipFile.OpenRead(Path.Combine(ApplicationSettings.POSTempPath, _result.Data.FileName)))
                                {
                                    foreach (var entry in z.Entries)
                                    {
                                        using (var r = new StreamReader(entry.Open()))
                                        {
                                            uncompressedFile = Path.Combine(ApplicationSettings.POSTempPath + "\\" + _result.Data.FileName, entry.Name);
                                            Process p = new Process();
                                            ProcessStartInfo s = new ProcessStartInfo(uncompressedFile);
                                            p.StartInfo = s;
                                            p.Start();
                                        }
                                    }
                                }

                            }
                            catch (Exception ex)
                            {

                                _logger.LogError(ex.Message);
                            }


                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                        }
                    }
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

            }
        }

        private async void LoadStores()
        {
            try
            {
                Stores = null;

                await Task.Run(async () =>
                {
                    var _result = await _storeService.GetStores();

                    if (_result != null && _result.Any())
                    {
                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            Stores = new ObservableCollection<Entites.Stores.Store>(_result.Where(x => x.Parent_ref == null));
                        });
                    }

                });
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in loading  store ", _ex);
            }

        }

        public ObservableCollection<Entites.Stores.Store> Stores
        {
            get { return _stores; }
            set { SetProperty(ref _stores, value); }
        }

        public Entites.Stores.Store SelectedStores
        {
            get { return _Selectedstores; }
            set { SetProperty(ref _Selectedstores, value); }
        }

        private bool _storeVisibility;

        public bool StoreVisibility
        {
            get { return _storeVisibility; }
            set { SetProperty(ref _storeVisibility, value); }
        }


        private async void LoadSuppliers()
        {

            try
            {
                _logger.LogInformation("Getting suppliers ");

                await Task.Run(async () =>
                {
                    var _result = await _supplierService.GetSuppliers("isenabled=true");

                    if (_result != null && _result.Any())
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            _logger.LogInformation($"Suppliers get success --count-{_result.Count()}");

                            Suppliers = new ObservableCollection<SuppliersViewModel>(_result);

                            //let user select the supplier
                            //if (SelectedSupplier == null)
                            //{
                            //    SelectedSupplier = Suppliers[0];
                            //}
                        });
                    }

                });

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loading suppliers", _ex);
            }
        }

        private ObservableCollection<SuppliersViewModel> _suppliers;

        public ObservableCollection<SuppliersViewModel> Suppliers
        {
            get => _suppliers;
            set => SetProperty(ref _suppliers, value);
        }


        private SuppliersViewModel _selectedSuppliers;

        public SuppliersViewModel SelectedSuppliers
        {
            get => _selectedSuppliers;
            set => SetProperty(ref _selectedSuppliers, value);
        }


        private SKUGeneric _existingCertificate;

        public SKUGeneric ExistingCertificate
        {
            get => _existingCertificate;
            set => SetProperty(ref _existingCertificate, value);
        }


        private ProductType _selectedProductType;
        public ProductType SelectedProductType
        {
            get { return _selectedProductType; }
            set { SetProperty(ref _selectedProductType, value); }
        }



        private List<string> _departmentList;
        public List<string> DepartmentList
        {
            get { return _departmentList; }
            set { SetProperty(ref _departmentList, value); }
        }

        private ObservableCollection<Manufacturer> _manufacturers;
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get { return _manufacturers; }
            set { SetProperty(ref _manufacturers, value); }
        }

        private Manufacturer _selectedManufacturer;
        public Manufacturer SelectedManufacturer
        {
            get { return _selectedManufacturer; }
            set { SetProperty(ref _selectedManufacturer, value); }
        }

        private List<string> GetCertificateProduct()
        {
            if (ApplicationSettings.CategoryCertificate != null && ApplicationSettings.CategoryCertificate.Any())
            {
                return new List<string>(ApplicationSettings.CategoryCertificate);

            }
            return null;

        }

        private async void LoadProductTypes()
        {

            try
            {

                await Task.Run(async () =>
                {
                    var _result = await _productTypeService.GetAllCategory();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess)
                            CategoryList = new ObservableCollection<CategoryModel>(_result.Data.ToList().Where(x=>x.IsCertificate).ToList());
                        else
                            CategoryList = new ObservableCollection<CategoryModel>();
                    });

                });


            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in stock search view model", _ex);
            }
        }

        private CancellationTokenSource _cancellationTokenSource;

        private async void LoadManufacturer(object obj)
        {

            try
            {
                if (_cancellationTokenSource != null)
                    _cancellationTokenSource.Cancel();

                _cancellationTokenSource = new CancellationTokenSource();



                SelectedManufacturer = null;



                Manufacturers = null;


                if (SelectedSubCategory == null) return;

                await Task.Run(async () =>
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var _result = await _productTypeService.GetProductTypeManufacturers((int)SelectedSubCategory.Id, _cancellationTokenSource.Token);

                    if (_result != null && _result.Any())
                    {
                        Manufacturers = new ObservableCollection<Manufacturer>(_result.OrderBy(x => x.Name).ToList());


                    }
                }, _cancellationTokenSource.Token);

            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in stock search view model", _ex);
            }


        }

        public async void CategoryChange(object obj)
        {
            try
            {

                var _viewModel = (AlterSKUViewModel)obj;
                if (_viewModel != null && _viewModel.SelectedCategory != null)
                {
                    await Task.Run(async () =>
                    {
                        var _result = await _productTypeService.GetSubCategory(_viewModel.SelectedCategory.Id);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (_result != null && _result.IsSuccess)
                            {

                               
                               SubCategory = new ObservableCollection<SubCategoryModel>(_result.Data);
                               
                            }
                            else
                                SubCategory = new ObservableCollection<SubCategoryModel>();


                        });



                    });
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private ObservableCollection<CategoryModel> _categoryList;

        public ObservableCollection<CategoryModel> CategoryList
        {
            get => _categoryList;
            set => SetProperty(ref _categoryList, value);
        }

        private CategoryModel _selectedCategory;

        public CategoryModel SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        private ObservableCollection<SubCategoryModel> _subCategory;
        public ObservableCollection<SubCategoryModel> SubCategory
        {
            get => _subCategory;
            set => SetProperty(ref _subCategory, value);
        }

        private SubCategoryModel _selectedSubCategory;
        public SubCategoryModel SelectedSubCategory
        {
            get => _selectedSubCategory;
            set => SetProperty(ref _selectedSubCategory, value);
        }

        private async void LoadOldProductTypes()
        {

            try
            {

                await Task.Run(async () =>
                {
                    var _result = await _productTypeService.GetProductTypes("isenabled=true");

                    if (_result != null && _result.Any())
                    {
                        if (DepartmentList != null && DepartmentList.Count > 0)
                        {
                            List<Entites.ProductTypes.ProductType> productTypes = new List<Entites.ProductTypes.ProductType>();
                            //filter only four departments
                            foreach (var item in DepartmentList)
                            {
                                productTypes.Add(_result.FirstOrDefault(x => x.Name.ToLower().Contains(item)));
                            }

                            ProductTypes = new ObservableCollection<ProductType>(productTypes);
                        }


                    }
                    else
                        ProductTypes = new ObservableCollection<ProductType>();
                });
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in stock search view model", _ex);
            }
        }

        private ObservableCollection<ProductType> _productTypes;

        public ObservableCollection<ProductType> ProductTypes
        {
            get => _productTypes;
            set => SetProperty(ref _productTypes, value);
        }

        private ProductType _selectedType;

        public ProductType SelectedType
        {
            get => _selectedType;
            set => SetProperty(ref _selectedType, value);
        }

       

    }




}
