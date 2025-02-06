using FalcaPOS.Common.Events;
using FalcaPOS.Common.Logger;
using FalcaPOS.Entites.Sales;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;

namespace FalcaPOS.Sales.ViewModels
{
    public class SalesHomeViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;

        public DelegateCommand<Object> ViewSalesCommand { get; private set; }

        public DelegateCommand<Object> BackCommand { get; private set; }
        private readonly Logger _logger;
        public SalesHomeViewModel(IEventAggregator eventAggregator, Logger logger)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _ = eventAggregator.GetEvent<AppOrderMoveToSalePageEvent>().Subscribe(SalesPageTab);

            ViewSalesCommand = new DelegateCommand<object>(ViewRequest);
            BackCommand = new DelegateCommand<object>(BackRequest);
            IsSalesCreatePage = true;
            IsSalesViewPage = false;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void SalesPageTab(AppOrderModel model)
        {
            try
            {
                SelectedIndex = 0;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private int _slectedIndex;

        public int SelectedIndex
        {
            get { return _slectedIndex; }
            set { SetProperty(ref _slectedIndex, value); }
        }

        public void ViewRequest(object obj)
        {
            IsSalesCreatePage = false;
            IsSalesViewPage = true;
        }

        public void BackRequest(object obj)
        {
            IsSalesCreatePage = true;
            IsSalesViewPage = false;
        }

        private bool _isCreate;

        public bool IsSalesCreatePage
        {
            get => _isCreate;
            set => SetProperty(ref _isCreate, value);
        }

        private bool _isView;
        public bool IsSalesViewPage
        {
            get => _isView;
            set => SetProperty(ref _isView, value);
        }
        //private readonly ISalesService _salesService;

        //private readonly ICustomerService _customerService;
        //private readonly IEventAggregator _eventAggregator;

        //public DelegateCommand GetProductCommand { get; private set; }

        //public DelegateCommand SaveSalesDetatilsCommand { get; private set; }

        //public DelegateCommand ResetSalesCommand { get; private set; }

        //public DelegateCommand<object> RemoveProductCommand { get; private set; }

        //public DelegateCommand GetCustomerCommand { get; private set; }

        //public DelegateCommand StateSelectionCommand { get; private set; }


        //public SalesHomeViewModel(ISalesService salesService, ICustomerService customerService,IEventAggregator eventAggregator)
        //{
        //    _salesService = salesService;

        //    _customerService = customerService;
        //    _eventAggregator = eventAggregator;
        //    GetProductCommand = new DelegateCommand(GetProduct);

        //    GetCustomerCommand = new DelegateCommand(GetCustomerInfo);

        //    RemoveProductCommand = new DelegateCommand<object>(RemoveProduct);

        //    SaveSalesDetatilsCommand = new DelegateCommand(SaveSalesDetatils);

        //    ResetSalesCommand = new DelegateCommand(ResetSales);

        //    StateSelectionCommand = new DelegateCommand(LoadDistricts);

        //    LoadStates();

        //}

        //private async void LoadStates()
        //{
        //    try
        //    {
        //        await Task.Run(async () =>
        //        {
        //           var _result= await _salesService.GetStates();

        //            States = new ObservableCollection<State>(_result);
        //        });
        //    }
        //    catch (Exception _ex)
        //    {
        //    }
        //}

        //private async void LoadDistricts()
        //{
        //    Districts = null;

        //    SelectDistrict = null;

        //    if (SelectState == null || SelectState.StateId <= 0) return;



        //    await Task.Run(async () =>
        //    {
        //        var _result = await _salesService.GetDistricts(SelectState.StateId);

        //        Districts = new ObservableCollection<District>(_result);

        //    });


        //}


        //private District _selectDistrict;
        //public District SelectDistrict
        //{
        //    get { return _selectDistrict; }
        //    set { SetProperty(ref _selectDistrict, value); }
        //}

        //private ObservableCollection<District> _districts;
        //public ObservableCollection<District> Districts
        //{
        //    get { return _districts; }
        //    set { SetProperty(ref _districts, value); }
        //}


        //private State _selectState;
        //public State SelectState
        //{
        //    get { return _selectState; }
        //    set { SetProperty(ref _selectState, value); }
        //}

        //private ObservableCollection<State> _states;
        //public ObservableCollection<State> States
        //{
        //    get { return _states; }
        //    set { SetProperty(ref _states, value); }
        //}


        //private CancellationTokenSource _cancellationTokenSource;

        //private async void GetCustomerInfo()
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(OldCustomerPhone))
        //        {
        //            _eventAggregator.GetEvent<NotifyMessage>().Publish(new Common.Models.ToastMessage
        //            {
        //                Message = "Phone number is required.",
        //                MessageType = Common.NotificationType.Error
        //            });
        //            return;
        //        }

        //        if (_cancellationTokenSource != null)
        //            _cancellationTokenSource.Cancel();


        //        _cancellationTokenSource = new CancellationTokenSource();

        //        await Task.Run(async () =>
        //        {
        //            _cancellationTokenSource.Token.ThrowIfCancellationRequested();

        //            var _result = await _customerService.GetCustomerByPhone(OldCustomerPhone, _cancellationTokenSource.Token);

        //            if (_result != null)
        //            {
        //                FarmerName = _result.Name;
        //                FarmerPhone = _result.Phone;
        //                PinCode = _result.PinCode;
        //                OldCustomerId = _result.CustomerId;

        //                try
        //                {
        //                    SelectState = States.FirstOrDefault(x => x.Name == _result.Address.State);                                                      

        //                    if(SelectState != null)
        //                    {
        //                        var _districts = await _salesService.GetDistricts(SelectState.StateId);

        //                        if (_districts != null && _districts.Any())
        //                        {

        //                            SelectDistrict = _districts.FirstOrDefault(x => x.Name == _result.Address.District);
        //                        }
        //                    }                            

        //                }
        //                catch (Exception _ex)
        //                {

        //                }
        //            }
        //            else
        //            {
        //                FarmerName = string.Empty;
        //                FarmerPhone = string.Empty;
        //                PinCode = 0;
        //                OldCustomerId = 0;
        //                _eventAggregator.GetEvent<NotifyMessage>().Publish(new Common.Models.ToastMessage
        //                {
        //                    Message = "Cutomer not found.",
        //                    MessageType = Common.NotificationType.Information
        //                });
        //            }

        //        }, _cancellationTokenSource.Token);
        //    }
        //    catch (Exception _ex)
        //    {


        //    }
        //}

        //private void ResetSales()
        //{

        //    SalesProducts?.Clear();

        //    ResetFormerDetails();
        //    GrossTotal = 0;
        //    Discount = 0;
        //    SGST = 0;
        //    CGST = 0;
        //    NetTotal = 0;
        //    Cash = 0;
        //    Card = 0;
        //    CardNumber = string.Empty;
        //    UPI = 0;
        //    UPINumber = string.Empty;

        //}

        //private void ResetFormerDetails()
        //{

        //    SelectDistrict = null;
        //    SelectState = null;
        //    FarmerName = string.Empty;
        //    FarmerPhone = string.Empty;
        //    FarmerAlternatePhone = string.Empty;
        //    InvoiceDate = null;
        //    InvoiceNumber = string.Empty;
        //    SelectedState = string.Empty;
        //    SelectedDistrict = string.Empty;
        //    PinCode = 0;
        //    OldCustomerId = 0;
        //    OldCustomerPhone = string.Empty;
        //    //Check.
        //    //IsOldCustomer = false;            

        //}

        //private async void SaveSalesDetatils()
        //{
        //    //TODO : Validate 

        //    var _salesDetails = GetSalesDetails();

        //    if (_salesDetails == null) return;

        //    bool result = false;

        //    await Task.Run(async () =>
        //    {
        //        result = await _salesService.AddSales(_salesDetails);

        //    });

        //    if (result)
        //    {
        //        ResetSales();
        //    }



        //}

        //private AddSales GetSalesDetails()
        //{
        //    var _adSales = new AddSales();

        //    try
        //    {
        //        _adSales.InvoiceNumber = InvoiceNumber;
        //        _adSales.InvoiceDate = InvoiceDate;
        //        _adSales.GrossTotal = GrossTotal;
        //        _adSales.Discount = Discount;
        //        _adSales.GST = GST;
        //        _adSales.SalesPayment = GetSalesPayMent();
        //        _adSales.SalesProducts = GetSalesProducts(SalesProducts);
        //        //Check
        //        _adSales.DiscountType = "Stock";
        //        _adSales.SalesType = "salesType";
        //        _adSales.Total = NetTotal;
        //        if (!IsOldCustomer)
        //        {
        //            _adSales.IsOldCustomer = false;
        //            _adSales.CustomerId = 0;
        //            _adSales.CustomerDetails = GetCustomerDetails();
        //        }
        //        else
        //        {
        //            _adSales.IsOldCustomer = true;
        //            _adSales.CustomerId = OldCustomerId;
        //        }



        //    }
        //    catch (Exception _ex)
        //    {

        //        return null;
        //    }



        //    return _adSales;
        //}

        //private List<SalesProduct> GetSalesProducts(ObservableCollection<SalesProductModel> _salesProducts)
        //{
        //    var _Products = new List<SalesProduct>();

        //    foreach (var _prod in _salesProducts)
        //    {

        //        var _product = new SalesProduct
        //        {

        //            Discount = _prod.Discount,
        //            DiscountMode = _prod.DiscountMode,
        //            Manufacturer = _prod.Manufacturer,
        //            ProductGST = _prod.ProductGST,
        //            ProductName = _prod.ProductName,
        //            ProductSellingPrice = _prod.ProductSellingPrice,
        //            ProductTotal = _prod.ProductTotal,
        //            ProductSpecifications = _prod.ProductSpecifications,
        //            ProductType = _prod.ProductType,
        //            StockProductId = _prod.StockProductId,
        //            WarrentyDate = _prod.WarrentyDate
        //        };

        //        _Products.Add(_product);
        //    }

        //    return _Products;
        //}

        //private SalesPayment GetSalesPayMent()
        //{
        //    var _salesPayment = new SalesPayment();

        //    _salesPayment.Card = Card;
        //    _salesPayment.CardNumber = CardNumber;
        //    _salesPayment.Cash = Cash;
        //    _salesPayment.UPI = UPI;
        //    _salesPayment.UPINumber = UPINumber;

        //    return _salesPayment;
        //}

        //private CustomerDetails GetCustomerDetails()
        //{
        //    try
        //    {
        //        var _customer = new CustomerDetails
        //        {
        //            AlternatePhone = FarmerAlternatePhone,
        //            Name = FarmerName,
        //            PinCode = PinCode,
        //            Phone = FarmerPhone,
        //            Address = GetCustomerAddress(),
        //        };

        //        return _customer;


        //    }
        //    catch (Exception _ex)
        //    {


        //    }

        //    return null;
        //}

        //private Address GetCustomerAddress()
        //{
        //    var _address = new Address
        //    {

        //        //Check
        //        Alternatephone = FarmerAlternatePhone,
        //        City = "City",
        //        Email = "Email",
        //        Phone = FarmerPhone,
        //        District = "",
        //        State = SelectState.Name,
        //        Street = "Street",
        //        Pincode = PinCode,
        //    };

        //    return _address;
        //}

        //private void RemoveProduct(object obj)
        //{
        //    var _product = obj as SalesProductModel;

        //    if (_product == null) return;

        //    SalesProducts.Remove(_product);


        //}


        //private async void GetProduct()
        //{
        //    if (string.IsNullOrEmpty(ProductCode)) return;


        //    bool _res = int.TryParse(ProductCode, out int _productId);


        //    SalesProduct _product = null;

        //    if (_res)
        //    {
        //        if (_productId > 0)
        //        {
        //            if (SalesProducts.Any(x => x.StockProductId == _productId))
        //            {

        //                _eventAggregator.GetEvent<NotifyMessage>().Publish(new Common.Models.ToastMessage
        //                {
        //                    Message = "Product is already added.",
        //                    MessageType = Common.NotificationType.Error
        //                });
        //                ProductCode = string.Empty;
        //                return;
        //            }

        //            await Task.Run(async () =>
        //            {

        //                _product = await _salesService.GetStockProduct(_productId);


        //            });

        //            if (_product != null)
        //            {
        //                SalesProductModel model = GetSalesModel(_product);

        //                SalesProducts.Add(model);
        //                ProductCode = string.Empty;
        //            }
        //            else
        //            {
        //                _eventAggregator.GetEvent<NotifyMessage>().Publish(new Common.Models.ToastMessage
        //                {
        //                    Message = "Product not found.",
        //                    MessageType = Common.NotificationType.Information
        //                });
        //            }

        //        }

        //    }
        //}

        //private SalesProductModel GetSalesModel(SalesProduct product)
        //{

        //    if (product == null) return null;

        //    var _salesModel = new SalesProductModel();

        //    _salesModel.Discount = product.Discount;
        //    _salesModel.Manufacturer = product.Manufacturer;
        //    _salesModel.ProductType = product.ProductType;
        //    _salesModel.WarrentyDate = product.WarrentyDate;
        //    _salesModel.ProductName = product.ProductName;
        //    _salesModel.ProductSpecifications = product.ProductSpecifications;
        //    _salesModel.DiscountMode = product.DiscountMode;
        //    _salesModel.ProductSellingPrice = product.ProductSellingPrice;
        //    _salesModel.ProductTotal = product.ProductTotal;
        //    _salesModel.StockProductId = product.StockProductId;

        //    return _salesModel;

        //}




        //#region Props

        //private int _oldCustomerId;
        //public int OldCustomerId
        //{
        //    get { return _oldCustomerId; }
        //    set { SetProperty(ref _oldCustomerId, value); }
        //}


        //private bool _isOldCutomer;
        //public bool IsOldCustomer
        //{
        //    get { return _isOldCutomer; }
        //    set { SetProperty(ref _isOldCutomer, value); }
        //}

        //private string _oldCustomerPhone;
        //public string OldCustomerPhone
        //{
        //    get { return _oldCustomerPhone; }
        //    set { SetProperty(ref _oldCustomerPhone, value); }
        //}

        //private string _productCode;
        //public string ProductCode
        //{
        //    get { return _productCode; }
        //    set { SetProperty(ref _productCode, value); }
        //}

        //private ObservableCollection<SalesProductModel> _salesProducts = new ObservableCollection<SalesProductModel>();
        //public ObservableCollection<SalesProductModel> SalesProducts
        //{
        //    get { return _salesProducts; }
        //    set { SetProperty(ref _salesProducts, value); }
        //}

        //private string _invoiceNumber;
        //public string InvoiceNumber
        //{
        //    get { return _invoiceNumber; }
        //    set { SetProperty(ref _invoiceNumber, value); }
        //}


        //private string _farmerName;
        //public string FarmerName
        //{
        //    get { return _farmerName; }
        //    set { SetProperty(ref _farmerName, value); }
        //}

        //private string _farmerPhone;
        //public string FarmerPhone
        //{
        //    get { return _farmerPhone; }
        //    set { SetProperty(ref _farmerPhone, value); }
        //}


        //private string _farmerAlternatePhone;
        //public string FarmerAlternatePhone
        //{
        //    get { return _farmerAlternatePhone; }
        //    set { SetProperty(ref _farmerAlternatePhone, value); }
        //}

        //private DateTime? _invoiceDate;
        //public DateTime? InvoiceDate
        //{
        //    get { return _invoiceDate; }
        //    set { SetProperty(ref _invoiceDate, value); }
        //}

        //private string _selectedState;
        //public string SelectedState
        //{
        //    get { return _selectedState; }
        //    set { SetProperty(ref _selectedState, value); }
        //}

        //private string _selectedDistrict;
        //public string SelectedDistrict
        //{
        //    get { return _selectedDistrict; }
        //    set { SetProperty(ref _selectedDistrict, value); }
        //}

        //private int _pinCode;
        //public int PinCode
        //{
        //    get { return _pinCode; }
        //    set { SetProperty(ref _pinCode, value); }
        //}


        //private float _grossTotal;
        //public float GrossTotal
        //{
        //    get { return _grossTotal; }
        //    set { SetProperty(ref _grossTotal, value); }
        //}

        //private float _discount;
        //public float Discount
        //{
        //    get { return _discount; }
        //    set { SetProperty(ref _discount, value); }
        //}


        //private float _sGST;
        //public float SGST
        //{
        //    get { return _sGST; }
        //    set { SetProperty(ref _sGST, value); }
        //}

        //private float _cGST;
        //public float CGST
        //{
        //    get { return _cGST; }
        //    set { SetProperty(ref _cGST, value); }
        //}


        //private float _gSt;
        //public float GST
        //{
        //    get { return _gSt; }
        //    set { SetProperty(ref _gSt, value); }
        //}

        //private float _netTotal;
        //public float NetTotal
        //{
        //    get { return _netTotal; }
        //    set { SetProperty(ref _netTotal, value); }
        //}


        //private float _cash;
        //public float Cash
        //{
        //    get { return _cash; }
        //    set { SetProperty(ref _cash, value); }
        //}


        //private float _card;
        //public float Card
        //{
        //    get { return _card; }
        //    set { SetProperty(ref _card, value); }
        //}


        //private float _upi;
        //public float UPI
        //{
        //    get { return _upi; }
        //    set { SetProperty(ref _upi, value); }
        //}


        //private string _cardNumber;
        //public string CardNumber
        //{
        //    get { return _cardNumber; }
        //    set { SetProperty(ref _cardNumber, value); }
        //}

        //private string _upiNumber;
        //public string UPINumber
        //{
        //    get { return _upiNumber; }
        //    set { SetProperty(ref _upiNumber, value); }
        //}


        //#endregion

    }
}
