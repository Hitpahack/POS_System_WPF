using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Common.Validations;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Products;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Sku;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Sku.View;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
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
using System.Windows.Controls;
using Unity;
using Unity.Resolution;

namespace FalcaPOS.Sku.ViewModels
{
    public class AddSKUViewModel : ValidationBase
    {
        private readonly IUnityContainer _container;
        public DelegateCommand AddBrandCardCommand { get; private set; }

        public DelegateCommand<object> RemoveBrandCardCommand { get; private set; }

        private readonly IProductTypeService _productTypeService;

        private readonly Logger _logger;

        public DelegateCommand<object> AddFileAttachmentCommand { get; private set; }


        public DelegateCommand<object> ViewFileAttachmentCommand { get; private set; }

        public DelegateCommand<object> DeleteUploadFileCommand { get; private set; }



        private readonly INotificationService _notificationService;

        public DelegateCommand<object> CategorySelectionChangeCommand { get; private set; }

        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand<object> CreateSKURequest { get; private set; }

        public DelegateCommand ResetSKURequest { get; private set; }

        private readonly ISkuService _skuService;

        private readonly ProgressService _progressService;

        private IInvoiceFileService _invoiceFileService;

        public DelegateCommand<object> SubCategoryChangeCommand { get; private set; }

        private readonly IStoreService _storeService;

        private readonly IProductService _productService;
        public DelegateCommand<object> YesCommand { get; private set; }

        public DelegateCommand<object> NoCommand { get; private set; }

        public AddSKUViewModel(ProgressService progressService, IStoreService storeService, ISkuService skuService, IEventAggregator EventAggregator, INotificationService notificationService, ISupplierService supplierService, IUnityContainer container, IProductTypeService productTypeService, Logger logger, IInvoiceFileService invoiceFileService, IProductService productService)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));

            _skuService = skuService ?? throw new ArgumentNullException(nameof(skuService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            _invoiceFileService = invoiceFileService ?? throw new ArgumentNullException(nameof(invoiceFileService));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _productService = productService ?? throw new ArgumentNullException(nameof(productService));


            BrandCards = new ObservableCollection<BrandCardViewModel>();

            BrandCards.CollectionChanged -= BrandCards_CollectionChanged;

            BrandCards.CollectionChanged += BrandCards_CollectionChanged;

            AddBrandCardCommand = new DelegateCommand(AddBrand);

            RemoveBrandCardCommand = new DelegateCommand<object>(RemoveBrand);

            _productTypeService = productTypeService ?? throw new ArgumentNullException(nameof(productTypeService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));


            LoadProductTypes();

            CategorySelectionChangeCommand = new DelegateCommand<object>(CategoryChange);

            AddFileAttachmentCommand = new DelegateCommand<object>(AddFileOpenDialog);


            ViewFileAttachmentCommand = new DelegateCommand<object>(ViewFileAttachment);

            CreateSKURequest = new DelegateCommand<object>(CreateSKU);

            ResetSKURequest = new DelegateCommand(ResetAddSkuView);

            DeleteUploadFileCommand = new DelegateCommand<object>(DeleteUploadFile);

            _eventAggregator.GetEvent<SignalRProductTypeAddedEvent>().Subscribe(LoadDepartment);

            SubCategoryChangeCommand = new DelegateCommand<object>(SubCategorySelectionChange);

            YesCommand = new DelegateCommand<object>(Submit);

            NoCommand = new DelegateCommand<object>(Submit);

            LoadStores();

         
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
        private void RemoveBrand(object obj)
        {
            if (obj is Guid _productGUIDId)
            {
                var _productRemove = BrandCards?.FirstOrDefault(x => x.BrandGUIDId == _productGUIDId);

                if (_productRemove != null)
                {
                    BrandCards.Remove(_productRemove);
                   
                }
            }
        }
        private void AddBrand()
        {
            
            var _product = _container.Resolve<BrandCardViewModel>(new ParameterOverride("category", SelectedCategory),new ParameterOverride("SubCategory", SelectedSubCategory), new ParameterOverride("Store",SelectedStores));
            BrandCards.Add(_product);


        }



        private ObservableCollection<CategoryModel> _categoryList = new ObservableCollection<CategoryModel>();

        public ObservableCollection<CategoryModel> CategoryList
        {
            get => _categoryList;
            set => SetProperty(ref _categoryList, value);
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
                        {
                            //treat everything as non certificate cateogry
                            _result.Data.Select(x => { x.IsCertificate = false; return x; }).ToList();
                            CategoryList = new ObservableCollection<CategoryModel>(_result.Data);
                        }

                        else
                            CategoryList = new ObservableCollection<CategoryModel>();
                    });

                });
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loading product types in add new product", _ex);
            }
        }

        public void SubCategorySelectionChange(object obj)
        {
            try
            {
                if (SelectedCategory != null)
                {
                    if (FileUploadListInfo != null)
                    {
                        if (FileUploadListInfo.Count > 0)
                        {
                            FileUploadListInfo?.Clear();
                        }

                    }
                    if (SelectedCategory.IsCertificate)
                    {
                        if (SelectedStores == null)
                        {
                            _notificationService.ShowMessage("Selected certificate category Please select store", NotificationType.Error);
                            SelectedSubCategory = null;
                            return;
                        }
                    }

                    BrandCards?.Clear();
                    var viewModel = ((AddSKUViewModel)obj);
                    if (viewModel != null && viewModel.SelectedSubCategory != null)
                    {

                        var _product = _container.Resolve<BrandCardViewModel>(new ParameterOverride("category", viewModel.SelectedCategory), new ParameterOverride("SubCategory", viewModel.SelectedSubCategory), new ParameterOverride("Store", viewModel.SelectedStores));
                        BrandCards.Add(_product);
                        GetLastSKU(viewModel.SelectedSubCategory.Id);
                    }
                }


                
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loading product types in add new product", _ex);
            }
        }



        private void BrandCards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("BrandCards");
        }

        private ObservableCollection<BrandCardViewModel> _brandCards = new ObservableCollection<BrandCardViewModel>();
        public ObservableCollection<BrandCardViewModel> BrandCards
        {
            get { return _brandCards; }
            set { SetProperty(ref _brandCards, value); }
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
                var _viewModel = ((BrandCardViewModel)file);

                if (_viewModel.FileUploadListInfo != null && _viewModel.FileUploadListInfo.Count >= 1)
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
                    if (_viewModel.FileUploadListInfo == null)
                    {
                        _viewModel.FileUploadListInfo = new ObservableCollection<FileUploadInfo>();
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

                                if (_viewModel.FileUploadListInfo.Any(x => x.FileName == Path.GetFileName(_fileName)))
                                {
                                    _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} is already added", NotificationType.Information);

                                    _logger.LogWarning($"File was already added skipping file {_fileName}");

                                    continue;
                                }

                                _logger.LogInformation($"Adding file {_fileName}");

                                _viewModel.FileUploadListInfo?.Add(new FileUploadInfo
                                {
                                    FileId = Guid.NewGuid(),
                                    FilePath = _fileName,
                                    FileExtension = Path.GetExtension(_fileName),
                                    FileName = Path.GetFileName(_fileName),
                                    Size = FileHelper.FormatSize(_fileInfo.Length),
                                    FileSrc = FileSrc.local
                                });
                                _viewModel.ExistingCertificate = null;
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


        public async void CreateSKU(object obj)
        {
            try
            {

                FileUploadInfo = new List<FileUploadInfo>();
                Model = new List<CreateSKUModel>();
                var viewModel = ((AddSKUViewModel)obj);
                if (viewModel != null)
                {
                    if (SelectedCategory == null) {
                        _notificationService.ShowMessage("Please select category", NotificationType.Error);
                        return;
                    }

                    if (SelectedSubCategory == null)
                    {
                        _notificationService.ShowMessage("Please select sub category", NotificationType.Error);
                        return;
                    }

                    if (string.IsNullOrEmpty(LastSKU)) {
                        _notificationService.ShowMessage("Please check last product sku should not empty", NotificationType.Error);
                        return;
                    }

                    var department = GetCertificateCategory();
                    if ( viewModel.SelectedCategory != null && department.Contains(viewModel.SelectedCategory?.CategoryName.ToLower()) && viewModel.SelectedCategory.IsCertificate)
                    {
                        if (SelectedStores == null) {
                            _notificationService.ShowMessage("Please select store name", NotificationType.Error);
                            return;
                        }

                        if (viewModel.BrandCards != null && viewModel.BrandCards.Count > 0)
                        {
                            int i = 1;
                            var _lastSKU = LastSKU;
                            foreach (var item in viewModel.BrandCards)
                            {

                                bool isValid = ValidateData(item, i);

                                if (!isValid)
                                {
                                    return;
                                }

                                if (viewModel.SelectedCategory.CategoryName.ToLower() == department[2])
                                {
                                    if (string.IsNullOrEmpty(item.Author))
                                    {
                                        _notificationService.ShowMessage("Please enter Authority by whom", NotificationType.Error);
                                        return;
                                    }

                                }

                                List<CreateProductCertificateModel> productDetails = new List<CreateProductCertificateModel>();
                                int j = 1;
                                
                                foreach (var product in item.SKUProducts)
                                {
                                   
                                    product.ProductSKU =  Convert.ToString((Convert.ToInt32(_lastSKU) + 1));
                                    _lastSKU = product.ProductSKU;
                                    bool isValidProduct = ValidateDataProduct(product, j);

                                    if (!isValidProduct)
                                    {
                                        return;
                                    }

                                    productDetails.Add(new CreateProductCertificateModel()
                                    {

                                        Name = product.ProductName,
                                        Description = product.Description,
                                        ProductTypeManufacturerId = item.SelectedManufacturer.ProductTypeManufacturerId,
                                        SubUnitType = product.SelectedSubUnitType,
                                        ValidUpto = product.ValidupTo,
                                        LifeTime = product.LifeTime,
                                        SerailNumber = product.SerialNumber,
                                        LicenseNumber = product.License,
                                        ProductSKU=product.ProductSKU,
                                        PackingSize = product.PackingSize,
                                        TechnicalName=product.TechnicalName,
                                        UOM=product.UOM,
                                        Type=product.Type


                                    });

                                    j++;
                                }

                                if (item.FileUploadListInfo == null || item.FileUploadListInfo.Count == 0)
                                {

                                    _notificationService.ShowMessage("Please add attachment at row " + i, NotificationType.Error);
                                    return;

                                }

                                FileUploadInfo.Add(item.FileUploadListInfo.FirstOrDefault());
                                //checking validation for exsting certifacte
                                if (item.ExistingCertificate != null)
                                {

                                    if (item.ExistingCertificate.Number != item.Number)
                                    {
                                        _notificationService.ShowMessage("Please check not matching with existing " + item.headerNo + i, NotificationType.Error);
                                        return;
                                    }
                                    if (item.ExistingCertificate.Generic != item.Author)
                                    {
                                        _notificationService.ShowMessage("Please check not matching with existing Authority by Whom issued " + i, NotificationType.Error);
                                        return;
                                    }

                                    if (Convert.ToString(item.ExistingCertificate.IssueDate) != Convert.ToString(item.IssueDate))
                                    {
                                        _notificationService.ShowMessage("Please check not matching with existing issuedate" + i, NotificationType.Error);
                                        return;
                                    }
                                }



                                Model.Add(new CreateSKUModel()
                                {
                                    Number = item.Number,
                                    IssueDate = item.IssueDate,
                                    BrandId = item.SelectedManufacturer.ManufacturerId,
                                    ProductTypeId = (int)item.SelectedSubCategory.Id,
                                    ProductTypeName = item.SelectedSubCategory.SubCategoryName,
                                    Generic = item.Author,
                                    SupplierId = (int)item.SelectedSuppliers.SupplierId,
                                    productCertificateModels = productDetails,
                                    CategoryName=SelectedCategory.CategoryName,
                                    PictureId = item.FileUploadListInfo.FirstOrDefault()?.FileremoteSrcID,
                                    StoreId=SelectedStores.StoreId,

                                });



                                i++;

                            }

                            SKUCreateConfimationPopup confirmationPopup = new SKUCreateConfimationPopup();
                            confirmationPopup.DataContext = this;
                            await DialogHost.Show(confirmationPopup, "RootDialog", ClosingEventHandler);
                            
                        }
                    }
                    else
                    {
                        //normal product add 
                        if (viewModel.BrandCards != null && viewModel.BrandCards.Count > 0)
                        {
                            ProductDetails = new List<CreateProductModel>();
                            int i = 1;
                            var _lastSKU = LastSKU;
                            foreach (var item in viewModel.BrandCards)
                            {

                                if (item.SelectedManufacturer == null)
                                {
                                    _notificationService.ShowMessage("Please select brand name at row" + i, NotificationType.Error);
                                    return;
                                }


                                int j = 1;
                               
                                foreach (var product in item.SKUProducts)
                                {
                                    product.ProductSKU= Convert.ToString((Convert.ToInt32(_lastSKU)+1));
                                    _lastSKU = product.ProductSKU;

                                    if (string.IsNullOrEmpty(product.ProductName))
                                    {
                                        _notificationService.ShowMessage("Please enter product name at row" + j, NotificationType.Error);
                                        return;
                                    }
                                    if (string.IsNullOrEmpty(product.TechnicalName))
                                    {
                                        _notificationService.ShowMessage("Please enter technical name" + j, NotificationType.Error);
                                        return;
                                    }

                                    if (string.IsNullOrEmpty(product.Description))
                                    {
                                        _notificationService.ShowMessage("Please enter description at row" + j, NotificationType.Error);
                                        return;
                                    }


                                    if (string.IsNullOrEmpty(product.PackingSize))
                                    {
                                        _notificationService.ShowMessage("Please enter packing size" + j, NotificationType.Error);
                                        return;
                                    }

                                    if (string.IsNullOrEmpty(product.SelectedSubUnitType))
                                    {
                                        _notificationService.ShowMessage("Please select UOM at row" + j, NotificationType.Error);
                                        return;
                                    }

                                    if (string.IsNullOrEmpty(product.ProductSKU)) {
                                        _notificationService.ShowMessage("Please enter product sku" + j, NotificationType.Error);
                                        return;
                                    }
                                    if (!product.ProductSKU.IsValidString()) {
                                        _notificationService.ShowMessage("Product SKU  is required", NotificationType.Error);
                                        return;
                                    }

                                    //produt sku should be 6 digit in lentgh

                                    if (product.ProductSKU.Length != 8) {
                                        _notificationService.ShowMessage("Product SKU should be 8 characters", NotificationType.Error);
                                        return;

                                    }

                                    //if (string.IsNullOrEmpty(product.UOM)) {
                                    //    _notificationService.ShowMessage("Please enter uom" + j, NotificationType.Error);
                                    //    return;
                                    //}

                                    if (string.IsNullOrEmpty(product.HsnCode)) {
                                        _notificationService.ShowMessage("Please enter hsn code at row"+j, NotificationType.Error);
                                        return;

                                    }


                                    if (product.HsnCode.Length != 8)
                                    {
                                        _notificationService.ShowMessage("HSN Code should be 8 characters at row" + j, NotificationType.Error);
                                        return;

                                    }


                                    if (product.SelectedWarrantyService==null) {
                                        _notificationService.ShowMessage("Please select warranty/service at row" + j, NotificationType.Error);
                                        return;

                                    }

                                    if (product.SelectedGSTslab == null)
                                    {
                                        _notificationService.ShowMessage("Please select Gst at row" + j, NotificationType.Error);
                                        return;

                                    }

                                    if (product.MinMargin<0)
                                    {
                                        _notificationService.ShowMessage("Minimum margin percentage should not be negative" + j, NotificationType.Error);
                                        return;

                                    }


                                    ProductDetails.Add(new CreateProductModel()
                                    {
                                        Name = product.ProductName,
                                        Description = product.Description,
                                        ProductTypeManufacturerId = item.SelectedManufacturer.ProductTypeManufacturerId,
                                        SubUnitType = product.SelectedSubUnitType,
                                        TechnicalName=product.TechnicalName,
                                        PackingSize=product.PackingSize,
                                        ProductSKU=product.ProductSKU,
                                        Type=product.Type,
                                        UOM=product.UOM,
                                        Hsn=product.HsnCode,
                                        Warranty=product.SelectedWarrantyService.Name,
                                        GST=product.SelectedGSTslab.GstValue,
                                        MinMargin=product.MinMargin,

                                    });
                                }


                                i++;
                            }

                            SKUCreateConfimationPopup confirmationPopup = new SKUCreateConfimationPopup();
                            confirmationPopup.DataContext = this;
                            await DialogHost.Show(confirmationPopup, "RootDialog", NormalProductClosingEventHandler);

                           
                        }

                    }

                }



            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {
                await _progressService.StopProgressAsync();
            }
        }

        private void ResetAddSkuView()
        {
            BrandCards.Clear();
            SelectedSubCategory = null;
            SubCategory = null;
            LoadProductTypes();
            SelectedCategory=null;
            LastSKU = null;
            SelectedStores=null;
            LoadStores();
        }

        private bool ValidateData(BrandCardViewModel brandCardView, int i)
        {
            if (brandCardView.SelectedManufacturer == null)
            {
                _notificationService.ShowMessage("Please select brand name at row " + i, NotificationType.Error);
                return false;
            }

            if (brandCardView.SelectedSuppliers == null)
            {
                _notificationService.ShowMessage("Please select supplier name at row" + i, NotificationType.Error);
                return false;
            }

            if (string.IsNullOrEmpty(brandCardView.Number))
            {
                _notificationService.ShowMessage("Please enter " + brandCardView.headerNo.ToLower().Replace("*","") + " at row " + i, NotificationType.Error);
                return false;
            }

            if (string.IsNullOrEmpty(brandCardView.IssueDate))
            {
                _notificationService.ShowMessage("Please select issue date at row " + i, NotificationType.Error);
                return false;
            }




            return true;
        }

        private bool ValidateDataProduct(SKUProductCardViewModel productCardView, int j)
        {
            if ( productCardView.SerialNumber == null)
            {
                _notificationService.ShowMessage("Please enter the serial number at row " + j, NotificationType.Error);
                return false;
            }
            if (productCardView.SerialNumber == 0)
            {
                _notificationService.ShowMessage("Please enter valid the serial number at row " + j, NotificationType.Error);
                return false;
            }

            if (string.IsNullOrEmpty(productCardView.ProductName))
            {
                _notificationService.ShowMessage("Please enter product name at row " + j, NotificationType.Error);
                return false;
            }

            if (string.IsNullOrEmpty(productCardView.Description))
            {
                _notificationService.ShowMessage("Please enter description at row " + j, NotificationType.Error);
                return false;
            }

            if (string.IsNullOrEmpty(productCardView.SelectedSubUnitType))
            {
                _notificationService.ShowMessage("Please select sub unit type at row" + j, NotificationType.Error);
                return false;
            }

            if (string.IsNullOrEmpty(productCardView.ValidupTo) && productCardView.LifeTime == false)
            {
                _notificationService.ShowMessage("Please select valid up to at row " + j, NotificationType.Error);
                return false;
            }
            if (string.IsNullOrEmpty(productCardView.License))
            {
                _notificationService.ShowMessage("Please enter license number at row " + j, NotificationType.Error);
                return false;
            }

            if (!(string.IsNullOrEmpty(productCardView.ValidupTo)) && productCardView.LifeTime == true)
            {
                _notificationService.ShowMessage("Please select either or Valid Up to or Lifetime should not both " + j, NotificationType.Error);
                return false;
            }

            if (!productCardView.ProductSKU.IsValidString()) {
                _notificationService.ShowMessage("Product SKU  is required", NotificationType.Error);
                return false;
            }

            
            if (productCardView.ProductSKU.Length != 8) {
                _notificationService.ShowMessage("Product SKU should be 8 characters", NotificationType.Error);
                return false;

            }

            if (string.IsNullOrEmpty(productCardView.TechnicalName)) {
                _notificationService.ShowMessage("Please enter technical name" + j, NotificationType.Error);
                return false;
            }
            if (string.IsNullOrEmpty(productCardView.PackingSize)) {
                _notificationService.ShowMessage("Please enter packing size" + j, NotificationType.Error);
                return false;
            }
            if (string.IsNullOrEmpty(productCardView.UOM)) {
                _notificationService.ShowMessage("Please enter uom" + j, NotificationType.Error);
                return false;
            }
            if (string.IsNullOrEmpty(productCardView.Type)) {
                _notificationService.ShowMessage("Please enter type" + j, NotificationType.Error);
                return false;
            }

            return true;
        }

        private void DeleteUploadFile(object obj)
        {
            try
            {
                var viewModel = ((FileUploadInfo)obj);
                if (viewModel != null)
                {
                    if (BrandCards != null && BrandCards.Count > 0)
                    {
                        foreach (var item in BrandCards)
                        {
                            if ((item.FileUploadListInfo.Where(x => x.FileId == viewModel.FileId).FirstOrDefault() != null))
                            {
                                item.FileUploadListInfo.Clear();
                            }
                        }
                    }

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

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

        private List<string> GetCertificateCategory()
        {
            if (ApplicationSettings.CategoryCertificate != null && ApplicationSettings.CategoryCertificate.Any())
            {
                return new List<string>(ApplicationSettings.CategoryCertificate);

            }
            return null;

        }
        public void LoadDepartment(object obj)
        {
            try
            {
                LoadProductTypes();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        private CategoryModel _selectedCategory;


        public CategoryModel SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                SetProperty(ref _selectedCategory, value);
                if (value != null)
                    ValidateProperty(value);
            }
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

        public async void CategoryChange(object obj)
        {
            try
            {

                var _viewModel = (AddSKUViewModel)obj;
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

        public async void GetLastSKU(int SubCategoryId) {
            try {
                
                await Task.Run(async () => {

                    var _lastSKU = await _productService.ProductCurrentSKU(SubCategoryId);

                    LastSKU = _lastSKU.Data??"00000000";

                });
                
               
            }
            catch(Exception _ex) {
                _logger.LogError(_ex.Message);
            }
        }

        private string _lastSKU;

        public string LastSKU {
            get => _lastSKU;
            set => SetProperty(ref _lastSKU, value);
        }


        private string _newSKU;

        public string NewSKU {
            get => _newSKU;
            set => SetProperty(ref _newSKU, value);
        }


        private async void LoadStores() {
            try {
                Stores = null;

                await Task.Run(async () => {
                    var _result = await _storeService.GetStores();

                    if (_result != null && _result.Any()) {
                        Application.Current?.Dispatcher.Invoke(() => {
                            Stores = new ObservableCollection<Entites.Stores.Store>(_result.Where(x => x.Parent_ref == null && x.StoreId!=1));
                        });
                    }

                });
            }
            catch (Exception _ex) {
                _logger?.LogError("Error in loading  store ", _ex);
            }

        }

        private ObservableCollection<Store> _stores;
        public ObservableCollection<Entites.Stores.Store> Stores {
            get => _stores; 
            set => SetProperty(ref _stores, value); 
        }

        private Store _selectedStore;
        public Entites.Stores.Store SelectedStores {
            get => _selectedStore; 
            set => SetProperty(ref _selectedStore, value); 
        }

        private async void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs) {
            try {
                var _viewModel = (AddSKUViewModel)eventArgs.Parameter;
                if (_viewModel != null) {

                    await _progressService.StartProgressAsync();

                    var result = await _skuService.CreateSKURequestWithCertificate(_viewModel.Model);
                    if (result.IsSuccess) {
                        if (FileUploadInfo != null && FileUploadInfo.Count > 0 && FileUploadInfo.Any(x => x.FileSrc == FileSrc.local)) {
                            var _updateResult = await _skuService.UploadFiles(result.Data, FileUploadInfo.Where(x => x.FileSrc == FileSrc.local).ToArray());
                            if (_updateResult == null || !_updateResult.IsSuccess) {
                                _notificationService.ShowMessage("Attachment upload failed.", NotificationType.Error);
                            }
                        }
                        _notificationService.ShowMessage("SKU request sent successfully", NotificationType.Success);
                        BrandCards.Clear();
                        GetLastSKU(_viewModel.SelectedSubCategory.Id);

                    }
                    else {
                        _notificationService.ShowMessage(result.Error, NotificationType.Error);

                    }

                    await _progressService.StopProgressAsync();
                }


            }
            catch (Exception ex) {

                _logger.LogError(ex.Message);
            }
        }

        private async void NormalProductClosingEventHandler(object sender, DialogClosingEventArgs eventArgs) {
            try {
                var _viewModel = (AddSKUViewModel)eventArgs.Parameter;
                if (_viewModel != null) {

                    await _progressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {
                        var result = await _skuService.CreateSKURequest(ProductDetails);

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            if (result.IsSuccess && result.Data != null) {

                                _notificationService.ShowMessage(result.Data, NotificationType.Success);
                                BrandCards.Clear();
                                GetLastSKU(_viewModel.SelectedSubCategory.Id);

                            }
                            else {
                                _notificationService.ShowMessage(result.Error, NotificationType.Error);
                            }


                        });

                    });

                    await _progressService.StopProgressAsync();
                }


            }
            catch (Exception ex) {

                _logger.LogError(ex.Message);
            }
        }

        private List<CreateSKUModel> _model;
        public List<CreateSKUModel> Model {
            get => _model; 
            set => SetProperty(ref _model , value); 
        }

        private List<FileUploadInfo> _fileUploadInfo;
        public List<FileUploadInfo> FileUploadInfo {
            get => _fileUploadInfo;
            set => SetProperty(ref _fileUploadInfo, value);
        }

        private List<CreateProductModel> _productDetails;

        public List<CreateProductModel> ProductDetails {
            get => _productDetails; 
            set => SetProperty(ref _productDetails , value); 
        }


        public void Submit(object obj) {
            try {



                var TargetClose = ((Button)(obj));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);

            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                _notificationService.ShowMessage(AppConstants.CommonError, Common.NotificationType.Error);


            }
        }

        

    }


}
