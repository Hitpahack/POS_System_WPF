using FalcaPOS.Common;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Sku;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FalcaPOS.Sku.ViewModels
{
    public class ApproveSKUViewModel : BindableBase
    {

        public DelegateCommand RefreshSKUCommand { get; private set; }

        private bool _lifetimeCheckBoxVisibility;
        public bool LifeTimeBoxVisibility
        {
            get { return _lifetimeCheckBoxVisibility; }
            set { SetProperty(ref _lifetimeCheckBoxVisibility, value); }
        }

        private bool _croptxtBoxVisibility;
        public bool CroptxtBoxVisibility
        {
            get { return _croptxtBoxVisibility; }
            set { SetProperty(ref _croptxtBoxVisibility, value); }
        }

        private bool _principleProsVisibility;

        public bool PrincipleProsVisibility
        {
            get { return _principleProsVisibility; }
            set { SetProperty(ref _principleProsVisibility, value); }
        }

        private bool _validuptxtBoxVisibility;
        public bool ValidtxtBoxVisibility
        {
            get { return _validuptxtBoxVisibility; }
            set { SetProperty(ref _validuptxtBoxVisibility, value); }
        }

        public DelegateCommand SearchCategoryCommand { get; private set; }
        public string headerNo { get; set; }

        private readonly ISkuService _skuService;

        private readonly ProgressService _progressService;

        private readonly Logger _logger;

        private readonly INotificationService _notificationService;

        public DelegateCommand<object> ApprovedSkuCommand { get; private set; }

        public DelegateCommand<int?> GetPDFCommand { get; private set; }

        private readonly IInvoiceFileService _invoiceFileService;

        private readonly IProductTypeService _productTypeService;

        public ApproveSKUViewModel(IProductTypeService productTypeService, IInvoiceFileService invoiceFileService, ProgressService progressService, ISkuService skuService, INotificationService notificationService, Logger logger)
        {
            RefreshSKUCommand = new DelegateCommand(ResetData);

            _skuService = skuService ?? throw new ArgumentNullException(nameof(skuService));

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _invoiceFileService = invoiceFileService ?? throw new ArgumentNullException(nameof(invoiceFileService));

            ApprovedSkuCommand = new DelegateCommand<object>(ApprovedSKURequest);

            GetPDFCommand = new DelegateCommand<int?>(GetPdfDownload);

            DepartmentNames = GetCertificateProduct();

            _productTypeService = productTypeService ?? throw new ArgumentNullException(nameof(productTypeService));

            LoadCategory();

            SearchCategoryCommand = new DelegateCommand(GetApproveList);
           

        }


        public async void GetApproveList()
        {
            try
            {
                if (SelectedCategory == null) {
                    _notificationService.ShowMessage("Please select category", NotificationType.Error);
                    return;
                }
                await _progressService.StartProgressAsync();

                await Task.Run(async () =>
                {
                    var result = await _skuService.GetApproveSKURequest(SelectedCategory.Id);
                    if (result!=null  && result.IsSuccess)
                    {
                        List<TypeViewModel> departments = new List<TypeViewModel>();
                        var List = new ObservableCollection<TypeViewModel>(result.Data.ToList());

                        if (List != null && List.Count > 0)
                        {

                            foreach (var item in List)
                            {
                                headerChange(SelectedCategory.CategoryName.ToLower());

                                item.SKUViewModel.IssueDate = item.SKUViewModel.IssueDate;

                                item.SKUViewModel.BrandName = item.SKUViewModel.BrandName;
                                item.SKUViewModel.SupplierName = item.SKUViewModel.SupplierName;
                                item.SKUViewModel.Number = item.SKUViewModel.Number;
                                item.SKUViewModel.Generic = item.SKUViewModel.Generic;
                                item.SKUViewModel.HeaderName = headerNo;
                                item.SKUViewModel.PictureId = item.SKUViewModel.PictureId;
                                item.SKUViewModel.GenericVisibilty = CroptxtBoxVisibility;
                                item.SKUViewModel.PrincipalVisibilty = PrincipleProsVisibility;
                                List<ProductCertificateViewModel> productDetails = new List<ProductCertificateViewModel>();
                                foreach (var product in item.SKUViewModel.ProductsList)
                                {
                                    productDetails.Add(new ProductCertificateViewModel()
                                    {
                                        ProductId = product.ProductId,
                                        ProductType = product.ProductType,
                                        Manufacturer = product.Manufacturer,
                                        Description = product.Description,
                                        SubUnitType = product.SubUnitType,
                                        ValidUpto = product.ValidUpto,
                                        LifeTime = (product.ValidUpto == null),
                                        LifeTimeVisiblity = LifeTimeBoxVisibility,
                                        ValidUptoVisiblity = PrincipleProsVisibility,
                                        Name = product.Name,
                                        ProductTypeManufacturerId = product.ProductTypeManufacturerId,
                                        SerailNumber = product.SerailNumber,
                                        ProductSKU = product.ProductSKU,
                                        Isenabled = product.Isenabled,
                                        LicenseNumber = product.LicenseNumber,

                                    });
                                }
                                item.SKUViewModel.ProductsList = productDetails;

                                departments.Add(new TypeViewModel()
                                {
                                    CreatedDate = item.CreatedDate,
                                    HumanizerDate = DateTimeHumanizer.HumanizeString(item.CreatedDate),
                                    DeptCode = item.DeptCode,
                                    Description = item.Description,
                                    Name = item.Name,
                                    RequestId = item.RequestId,
                                    Isenabled = item.Isenabled,
                                    ProductLastSKU = item.ProductLastSKU ?? "0000000",
                                    ProductTypeId = item.ProductTypeId,
                                    StoreName = item.StoreName,
                                    SKUViewModel = item.SKUViewModel,
                                    StoreId = item.StoreId,



                                }); ;
                            }

                            DepartmentList = new ObservableCollection<TypeViewModel>(departments);
                        }
                    }
                    else
                    {
                        DepartmentList = null;
                        _notificationService.ShowMessage(result.Error, NotificationType.Error);

                    }

                });
                await _progressService.StopProgressAsync();
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

        private async void ApprovedSKURequest(object obj)
        {
            try
            {
                var _viewModel = ((TypeViewModel)obj);
                if (_viewModel != null)
                {
                    _viewModel.Description = _viewModel.Name;

                    foreach (var it in _viewModel.SKUViewModel.ProductsList)
                    {
                        if (DepartmentNames.Contains(SelectedCategory.CategoryName.ToLower()))
                        {
                            if (it.SerailNumber == null)
                            {
                                _notificationService.ShowMessage("Please enter the Serial Number", NotificationType.Error);
                                return;
                            }
                        }


                        if (!it.ProductSKU.IsValidString())
                        {
                            _notificationService.ShowMessage("Product SKU  is required", NotificationType.Error);
                            return;
                        }


                        //produt sku should be 6 digit in lentgh

                        if (it.ProductSKU.Length != 8)
                        {
                            _notificationService.ShowMessage("Product SKU should be 8 characters", NotificationType.Error);
                            return;

                        }

                        if (string.IsNullOrEmpty(it.TechnicalName))
                        {
                            _notificationService.ShowMessage("Please enter technical name", NotificationType.Error);
                            return;

                        }

                        if (string.IsNullOrEmpty(it.PackingSize))
                        {
                            _notificationService.ShowMessage("Please enter packing size", NotificationType.Error);
                            return;

                        }

                        if (string.IsNullOrEmpty(it.UOM))
                        {
                            _notificationService.ShowMessage("Please enter UOM", NotificationType.Error);
                            return;

                        }

                        if (string.IsNullOrEmpty(it.Type)) {
                            _notificationService.ShowMessage("Please select Own or Trade", NotificationType.Error);
                            return;
                        }
                    }

                }
                List<SKURequestApproveProductModel> _approveProduct = new List<SKURequestApproveProductModel>();

                foreach (var item in _viewModel.SKUViewModel.ProductsList)
                {
                    _approveProduct.Add(new SKURequestApproveProductModel()
                    {
                        ProductId = (int)item.ProductId,
                        ProductSKU = item.ProductSKU,
                        IsEnable = item.Isenabled,
                        TechnicalName = item.TechnicalName,
                        PackingSize = item.PackingSize,
                        UOM = item.UOM,
                        Type=item.Type,

                    });
                }
                SKURequestApproveModel sKURequestApproveModel = new SKURequestApproveModel()
                {
                    DepartName = _viewModel.Name,
                    ProductTypeId = (int)_viewModel.ProductTypeId,
                    StoreId = _viewModel.StoreId,
                    sKURequestApproves = _approveProduct,
                    CategoryName=SelectedCategory.CategoryName,

                };
                await _progressService.StartProgressAsync();

                await Task.Run(async () =>
                {
                    var result = await _skuService.ApprovedSKURequest(sKURequestApproveModel);

                    Application.Current?.Dispatcher?.Invoke(async () =>
                    {

                        if (result.IsSuccess)
                        {
                            _notificationService.ShowMessage(result.Data, NotificationType.Success);
                            await _progressService.StopProgressAsync();
                            RefreshApprovePage();
                        }
                        else
                        {
                            _notificationService.ShowMessage(result.Error, NotificationType.Error);
                           
                        }
                    });
                });

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {
                _logger.LogError("Getting exception in approved method");
                await _progressService.StopProgressAsync();
            }
        }

        private async void GetPdfDownload(int? fileId)
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
                    }
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

            }
        }

        private ObservableCollection<TypeViewModel> _departmentList;

        public ObservableCollection<TypeViewModel> DepartmentList
        {
            get { return _departmentList; }
            set { SetProperty(ref _departmentList, value); }
        }


        private void headerChange(string category)
        {

            LifeTimeBoxVisibility = false;
            CroptxtBoxVisibility = false;
            PrincipleProsVisibility = true;

            switch (category.ToLower())
            {
                case "crop protection chemicals":
                    headerNo = "PC No";
                    LifeTimeBoxVisibility = true;
                    break;
                case "fertilizers":
                case "speciality nutrients":
                    headerNo = "O Form Number";

                    CroptxtBoxVisibility = true;
                    break;
                case "seeds":
                    headerNo = "Source Certificate Number";

                    break;
                default:
                    PrincipleProsVisibility = false;
                    break;
            }
        }

        private List<string> _departmentNames;
        public List<string> DepartmentNames
        {
            get { return _departmentNames; }
            set { SetProperty(ref _departmentNames, value); }
        }

        private List<string> GetCertificateProduct()
        {
            if (ApplicationSettings.CategoryCertificate != null && ApplicationSettings.CategoryCertificate.Any())
            {
                return new List<string>(ApplicationSettings.CategoryCertificate);

            }
            return null;

        }
        public void RefreshApprovePage()
        {
            GetApproveList();
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

        private async void LoadCategory()
        {

            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _productTypeService.GetAllCategory();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess) {
                            var addList = _result.Data.ToList();
                            addList.Insert(0, new CategoryModel() { Id = -1, CategoryName = "All" });
                           

                            CategoryList = new ObservableCollection<CategoryModel>(addList.ToList());
                        }
                           
                        else
                            CategoryList = new ObservableCollection<CategoryModel>();
                    });

                });
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loading category in approve product page", _ex);
            }
        }

        public void ResetData()
        {
            try
            {
                SelectedCategory = null;
                DepartmentList = null;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

        }

        

    }


}
