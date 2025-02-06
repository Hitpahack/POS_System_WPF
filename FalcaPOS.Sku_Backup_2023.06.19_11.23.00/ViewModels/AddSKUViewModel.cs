using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Common.Validations;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Sku;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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



        public AddSKUViewModel(ProgressService progressService, ISkuService skuService, IEventAggregator EventAggregator, INotificationService notificationService, ISupplierService supplierService, IUnityContainer container, IProductTypeService productTypeService, Logger logger, IInvoiceFileService invoiceFileService)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));

            _skuService = skuService ?? throw new ArgumentNullException(nameof(skuService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            _invoiceFileService = invoiceFileService ?? throw new ArgumentNullException(nameof(invoiceFileService));

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
            var _product = _container.Resolve<BrandCardViewModel>(new ParameterOverride("category", SelectedCategory));
            BrandCards.Add(_product);


        }



        private ObservableCollection<CategoryModel> _categoryList = new ObservableCollection<CategoryModel>();

        public ObservableCollection<CategoryModel> CategoryList
        {
            get => _categoryList;
            set => SetProperty(ref _categoryList, value);
        }

        private ProductType _selectedProductType;


        public ProductType SelectedProductType
        {
            get { return _selectedProductType; }
            set
            {
                SetProperty(ref _selectedProductType, value);
                if (value != null)
                    ValidateProperty(value);
            }
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
                            CategoryList = new ObservableCollection<CategoryModel>(_result.Data);
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

                if (FileUploadListInfo != null)
                {
                    if (FileUploadListInfo.Count > 0)
                    {
                        FileUploadListInfo?.Clear();
                    }

                }

                BrandCards?.Clear();
                var viewModel = ((AddSKUViewModel)obj);
                if (viewModel != null && viewModel.SelectedSubCategory != null)
                {
                    var _product = _container.Resolve<BrandCardViewModel>(new ParameterOverride("category", viewModel.SelectedCategory), new ParameterOverride("SubCategory", viewModel.SelectedSubCategory));
                    BrandCards.Add(_product);
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
                List<CreateSKUModel> model = new List<CreateSKUModel>();

                List<FileUploadInfo> fileUploadInfos = new List<FileUploadInfo>();

                var viewModel = ((AddSKUViewModel)obj);
                if (viewModel != null)
                {
                    if (SelectedSubCategory == null)
                    {
                        _notificationService.ShowMessage("Please select sub category", NotificationType.Error);
                        return;
                    }
                    var department = GetCertificateCategory();
                    if ( viewModel.SelectedCategory != null && department.Contains(viewModel.SelectedCategory?.CategoryName.ToLower()) && viewModel.SelectedCategory.IsCertificate)
                    {
                        if (viewModel.BrandCards != null && viewModel.BrandCards.Count > 0)
                        {
                            int i = 1;
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
                                        SerailNumber = product.SerailNumber,
                                        LicenseNumber = product.License,

                                    });

                                    j++;
                                }

                                if (item.FileUploadListInfo == null || item.FileUploadListInfo.Count == 0)
                                {

                                    _notificationService.ShowMessage("Please add attachment at row " + i, NotificationType.Error);
                                    return;

                                }

                                fileUploadInfos.Add(item.FileUploadListInfo.FirstOrDefault());
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



                                model.Add(new CreateSKUModel()
                                {
                                    Number = item.Number,
                                    IssueDate = item.IssueDate,
                                    BrandId = item.SelectedManufacturer.ManufacturerId,
                                    ProducttypeId = (int)item.SelectedSubCategory.Id,
                                    ProducttypeName = item.SelectedSubCategory.SubCategoryName,
                                    Generic = item.Author,
                                    Supplierid = (int)item.SelectedSuppliers.SupplierId,
                                    productCertificateModels = productDetails,
                                    CategoryName=SelectedCategory.CategoryName,
                                    PictureId = item.FileUploadListInfo.FirstOrDefault()?.FileremoteSrcID

                                });



                                i++;

                            }
                            await _progressService.StartProgressAsync();

                            var result = await _skuService.CreateSKURequestWithCertificate(model);
                            if (result.IsSuccess)
                            {
                                if (fileUploadInfos != null && fileUploadInfos.Count > 0 && fileUploadInfos.Any(x => x.FileSrc == FileSrc.local))
                                {
                                    var _updateResult = await _skuService.UploadFiles(result.Data, fileUploadInfos.Where(x => x.FileSrc == FileSrc.local).ToArray());
                                    if (_updateResult == null || !_updateResult.IsSuccess)
                                    {
                                        _notificationService.ShowMessage("Attachment upload failed.", NotificationType.Error);
                                    }
                                }
                                _notificationService.ShowMessage("SKU request sent successfully", NotificationType.Success);
                                SelectedProductType = null;
                                BrandCards.Clear();

                            }
                            else
                            {
                                _notificationService.ShowMessage(result.Error, NotificationType.Error);

                            }
                        }
                    }
                    else
                    {
                        //normal product add 
                        if (viewModel.BrandCards != null && viewModel.BrandCards.Count > 0)
                        {
                            List<CreateProductModel> productDetails = new List<CreateProductModel>();
                            int i = 1;
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

                                    if (string.IsNullOrEmpty(product.ProductName))
                                    {
                                        _notificationService.ShowMessage("Please enter product name at row" + j, NotificationType.Error);
                                        return;
                                    }

                                    if (string.IsNullOrEmpty(product.Description))
                                    {
                                        _notificationService.ShowMessage("Please enter description at row" + j, NotificationType.Error);
                                        return;
                                    }

                                    if (string.IsNullOrEmpty(product.SelectedSubUnitType))
                                    {
                                        _notificationService.ShowMessage("Please select sub unit type at row" + j, NotificationType.Error);
                                        return;
                                    }

                                    productDetails.Add(new CreateProductModel()
                                    {
                                        Name = product.ProductName,
                                        Description = product.Description,
                                        ProductTypeManufacturerId = item.SelectedManufacturer.ProductTypeManufacturerId,
                                        SubUnitType = product.SelectedSubUnitType,

                                    });
                                }


                                i++;
                            }

                            await _progressService.StartProgressAsync();

                            await Task.Run(async () =>
                            {
                                var result = await _skuService.CreateSKURequest(productDetails);

                                Application.Current?.Dispatcher?.Invoke(() =>
                                {

                                    if (result.IsSuccess)
                                    {

                                        _notificationService.ShowMessage("SKU request sent successfully", NotificationType.Success);
                                        SelectedProductType = null;
                                        BrandCards.Clear();


                                    }
                                    else
                                    {
                                        _notificationService.ShowMessage(result.Error, NotificationType.Error);
                                    }


                                });

                            });
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
            LoadProductTypes();
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
            if (productCardView.SerailNumber == null)
            {
                _notificationService.ShowMessage("Please enter the serial number at row " + j, NotificationType.Error);
                return false;
            }
            if (productCardView.SerailNumber == 0)
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


    }


}
