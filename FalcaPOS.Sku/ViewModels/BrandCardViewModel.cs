using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Common.Validations;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.Products;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Sku;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Entites.Suppliers;
using FalcaPOS.Sku.View;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using Unity.Resolution;

namespace FalcaPOS.Sku.ViewModels
{
    public class BrandCardViewModel : ValidationBase
    {
        private readonly IUnityContainer _container;

        private readonly IProductService _productService;

        private readonly ISupplierService _supplierService;

        private readonly INotificationService _notificationService;
        private ISkuService _skuService;
        private CancellationTokenSource _cancellationTokenSource;

        private readonly Logger _logger;
        private readonly ProgressService _ProgressService;

        public Guid BrandGUIDId { get; set; }


        private readonly IProductTypeService _productTypeService;

        public DelegateCommand ProductTypeChangedCommand { get; private set; }

        public DelegateCommand AddskuProductCardCommand { get; private set; }
        public DelegateCommand SearchExistingSKUCertificateCommand { get; private set; }

        public DelegateCommand<object> RemoveSKuProductCardCommand { get; private set; }

        private readonly IEventAggregator _eventAggregator;

        public string headerNo { get; set; }

        public string AttachmentName { get; set; }

        public string SearchHeaderName { get; set; }



        private bool _principleProsVisibility;

        public bool PrincipleProsVisibility
        {
            get { return _principleProsVisibility; }
            set { SetProperty(ref _principleProsVisibility, value); }
        }

        private bool _croptxtBoxVisibility;
        public bool CroptxtBoxVisibility
        {
            get { return _croptxtBoxVisibility; }
            set { SetProperty(ref _croptxtBoxVisibility, value); }
        }


        public BrandCardViewModel(IEventAggregator EventAggregator, IUnityContainer container, CategoryModel category, IProductService productService, IProductTypeService productTypeService, ISupplierService supplierService, INotificationService notificationService, Logger logger, ISkuService skuService, ProgressService ProgressService,SubCategoryModel SubCategory,Store Store)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));

            SKUProducts = new ObservableCollection<SKUProductCardViewModel>();

            SKUProducts.CollectionChanged -= SkuProducts_CollectionChanged;

            SKUProducts.CollectionChanged += SkuProducts_CollectionChanged;

            BrandGUIDId = Guid.NewGuid();

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _productService = productService ?? throw new ArgumentNullException(nameof(productService));

            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));

            _productTypeService = productTypeService ?? throw new ArgumentNullException(nameof(productTypeService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _skuService = skuService ?? throw new ArgumentNullException(nameof(skuService));

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            AddskuProductCardCommand = new DelegateCommand(AddNewSKUProduct);

            SearchExistingSKUCertificateCommand = new DelegateCommand(SearchExistingSKUCertificate);

            RemoveSKuProductCardCommand = new DelegateCommand<object>(RemoveProduct);

            ProductTypeChangedCommand = new DelegateCommand(ProductTypeChanged);

            _eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));
     
            if (SubCategory != null)
                SelectedSubCategory = SubCategory;

            if (category != null)
            {
                SelectedCategory = category;
                ProductTypeChanged();
                headerChange(category);

            }
            var _product = _container.Resolve<SKUProductCardViewModel>(new ParameterOverride("category", SelectedCategory));
            SKUProducts.Add(_product);

            //LoadSuppliers();

            _ = EventAggregator.GetEvent<AddSKUOpenEvent>().Subscribe(Updateadttachment);

            GetCertificateCategory();

            //if (Store != null)
            //    SelectedStore = Store;
            

        }

        private async void SearchExistingSKUCertificate()
        {
            try
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

                if (SelectedManufacturer == null)
                {
                    _notificationService.ShowMessage("Please select Brand", NotificationType.Error);
                    return;
                }


                await _ProgressService.StartProgressAsync();
                var _productcertificate = await _skuService.GetproductCertificate(new Entites.Sku.ProductCertificateSearch() { departmentId = (int)SelectedSubCategory.Id, manufactureID = SelectedManufacturer.ManufacturerId, storeId =  SelectedStore.StoreId});
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
                        Number = _productcertificate.Data.SKUmodel.Number;
                        Author = _productcertificate.Data.SKUmodel.Generic;

                        IssueDate = Convert.ToString(_productcertificate.Data.SKUmodel.IssueDate);
                        ExistingCertificate = _productcertificate.Data.SKUmodel;
                        SelectedSuppliers = Suppliers.FirstOrDefault(x => x.SupplierId == _productcertificate.Data.SKUmodel.Supplierid);
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

        public void Updateadttachment(ObservableCollection<FileUploadInfo> fileUploadInfos)
        {
            FileUploadListInfo = fileUploadInfos;
        }




        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set
            {
                SetProperty(ref _hasError, value);
            }
        }


        private ProductSearchModel _selectedProductSearch;
        public ProductSearchModel SelectedProductSearch
        {
            get { return _selectedProductSearch; }
            set { SetProperty(ref _selectedProductSearch, value); }
        }

        private ObservableCollection<ProductSearchModel> _productsSearchList;
        public ObservableCollection<ProductSearchModel> ProductsSearchList
        {
            get { return _productsSearchList; }
            set { SetProperty(ref _productsSearchList, value); }
        }

        private bool _isDropDownOpen;
        public bool IsDropDownOpen
        {
            get { return _isDropDownOpen; }
            set { SetProperty(ref _isDropDownOpen, value); }
        }


        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                SetProperty(ref _quantity, value);
                // RaisePropertyChanged(nameof(Amount));
            }
        }

        private decimal _unitPrice;
        public decimal UnitPrice
        {
            get { return _unitPrice; }
            set
            {
                SetProperty(ref _unitPrice, value);
                //RaisePropertyChanged(nameof(Amount));
            }
        }

        public string Amount => Math.Round(Quantity * UnitPrice, 2, MidpointRounding.AwayFromZero).ToString("C");


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
            set
            {
                SetProperty(ref _selectedManufacturer, value);
                if (value != null)
                    ValidateProperty(value);
            }
        }

        private SubCategoryModel _selectedSubCategory;


        public SubCategoryModel SelectedSubCategory
        {
            get => _selectedSubCategory;
            set
            {
                SetProperty(ref _selectedSubCategory, value);
                if (value != null)
                    ValidateProperty(value);
            }
        }

        private CategoryModel _selectedCategory;
        public CategoryModel SelectedCategory {
            get => _selectedCategory;
            set {
                SetProperty(ref _selectedCategory, value);
                if (value != null)
                    ValidateProperty(value);
            }
        }

        private Store _selectedStore;
        public Store SelectedStore {
            get => _selectedStore; 
            set =>  SetProperty(ref _selectedStore , value); 
        }



        private async void ProductTypeChanged()
        {
            try
            {

                Manufacturers = new ObservableCollection<Manufacturer>();
                SelectedManufacturer = null;
                // LastkSKU = null;

                if (SelectedSubCategory != null)
                {

                    if (_cancellationTokenSource != null)
                        _cancellationTokenSource.Cancel();

                    _cancellationTokenSource = new CancellationTokenSource();

                    var _subCategory = SelectedSubCategory;



                    await Task.Run(async () =>
                    {
                        var _result = await _productTypeService
                                .GetProductTypeManufacturers(_subCategory.Id, _cancellationTokenSource.Token);

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            if (_result != null && _result.Any())
                            {
                                Manufacturers = new ObservableCollection<Manufacturer>(_result.OrderBy(x => x.Name).ToList());
                                // SelectedManufacturer = Manufacturers[0];
                            }
                        }, System.Windows.Threading.DispatcherPriority.Normal, _cancellationTokenSource.Token);

                    }, _cancellationTokenSource.Token);

                    //await Task.Run(async () =>
                    //{

                    //    var _lastSKU = await _productService
                    //    .ProductCurrentSKU(SelectedSubCategory.Id, _cancellationTokenSource.Token);



                    //}, _cancellationTokenSource.Token);

                }


            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in new product type added signalr event", _ex);
            }
        }

        private void SkuProducts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("SKUProducts");
        }

        private ObservableCollection<SKUProductCardViewModel> _skuProducts;
        public ObservableCollection<SKUProductCardViewModel> SKUProducts
        {
            get { return _skuProducts; }
            set { SetProperty(ref _skuProducts, value); }
        }

        private void RemoveProduct(object obj)
        {
            if (obj is Guid _productGUIDId)
            {
                
                var _productRemove = SKUProducts?.FirstOrDefault(x => x.SKUProductGUIDId == _productGUIDId);
                if (SKUProducts.Count != 1)
                {
                    if (_productRemove != null)
                    {
                        SKUProducts.Remove(_productRemove);
                       
                    }
                }


            }
        }
        private void AddNewSKUProduct()
        {
            
            var _product = _container.Resolve<SKUProductCardViewModel>(new ParameterOverride("category", SelectedCategory));
            SKUProducts.Add(_product);
        }

        private ObservableCollection<SuppliersViewModel> _suppliers;

        public ObservableCollection<SuppliersViewModel> Suppliers
        {
            get => _suppliers;
            set => SetProperty(ref _suppliers, value);
        }


        private SuppliersViewModel _selectedsuppliers;

        public SuppliersViewModel SelectedSuppliers
        {
            get => _selectedsuppliers;
            set => SetProperty(ref _selectedsuppliers, value);
        }

        private string _number;

        public string Number
        {
            get { return _number; }
            set { SetProperty(ref _number, value); }
        }

        private string _issueDate;

        public string IssueDate
        {
            get { return _issueDate; }
            set { SetProperty(ref _issueDate, value); }
        }


        private string _license;

        public string Licesnse
        {
            get { return _license; }
            set { SetProperty(ref _license, value); }
        }



        private string _author;

        public string Author
        {
            get { return _author; }
            set { SetProperty(ref _author, value); }
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

        private void headerChange(CategoryModel category)
        {
            PrincipleProsVisibility = false;
            CroptxtBoxVisibility = false;
            
        }

        private ObservableCollection<FileUploadInfo> fileUploadListInfo;
        public ObservableCollection<FileUploadInfo> FileUploadListInfo
        {
            get => fileUploadListInfo;
            set => SetProperty(ref fileUploadListInfo, value);
        }


        private SKUGeneric _existingCertifacate;

        public SKUGeneric ExistingCertificate
        {
            get => _existingCertifacate;
            set => SetProperty(ref _existingCertifacate, value);
        }

        private List<string> GetCertificateCategory() {
            //if (ApplicationSettings.CategoryCertificate != null && ApplicationSettings.CategoryCertificate.Any()) {
            //    return new List<string>(ApplicationSettings.CategoryCertificate);

            //}
            return null;

        }

       


    }
}
