using FalcaPOS.AddInventory.Models;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Attributes;
using FalcaPOS.Entites.Stores;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class InvoiceViewModel : BindableBase
    {
        private readonly IInvoiceService _invoiceService;

        public DelegateCommand InvoiceSelectionChanged { get; private set; }

        public DelegateCommand RefreshInvoiceCommand { get; private set; }

        private CancellationTokenSource _cancellationTokenSource { get; set; }

        public DelegateCommand CancelCommand { get; private set; }

        public DelegateCommand UpdateInvoiceCommad { get; private set; }

        public DelegateCommand<Object> HideColumnCommand { get; private set; }

        private readonly INotificationService _notificationService;

        private ColumnDefinition dgvHideRef { get; set; }


        public DelegateCommand<int?> DownloadFileCommand { get; private set; }

        private readonly IInvoiceFileService _invoiceFileService;

        private readonly Logger _logger;

        private readonly IStoreService _storeService;

        public DelegateCommand<object> StoreSelectionChangeCommand { get; private set; }
        public InvoiceViewModel(IInvoiceService invoiceService, IStoreService storeService, Logger logger, INotificationService notificationService, IInvoiceFileService invoiceFileService)
        {
            _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            InvoiceSelectionChanged = new DelegateCommand(async () => await GetInvoiceDetails());

            RefreshInvoiceCommand = new DelegateCommand(async () => await RefreshInvoice());

            HideColumnCommand = new DelegateCommand<object>((param) => HideColumnExec(param));

            _cancellationTokenSource = new CancellationTokenSource();

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            CancelCommand = new DelegateCommand(Cancel);

            UpdateInvoiceCommad = new DelegateCommand(async () => await UpdateInvoice());

            LoadInvoices();

            DownloadFileCommand = new DelegateCommand<int?>(DownloadFile);

            _invoiceFileService = invoiceFileService ?? throw new ArgumentNullException(nameof(invoiceFileService));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            LoadStoresAsync();

            StoreSelectionChangeCommand = new DelegateCommand<object>(Storechange);




        }

        public async void Storechange(object obj)
        {
            try
            {

                var value = ((InvoiceViewModel)(obj)).SelectedStore;
                if (value != null && value.Name != "All")
                {
                    await LoadInvoices();

                    InvoiceList = new ObservableCollection<InvoiceListViewModel>(InvoiceList?.Where(x => x.Store == value.Name).ToList());
                }
                else
                {
                    await LoadInvoices();
                }





            }
            catch (Exception _ex)
            {
                _logger.LogError("getting error in selection change store", _ex);

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
                _logger?.LogError("Error in invoice model", _ex);
            }
        }

        private void HideColumnExec(Object param)
        {
            if (param is ColumnDefinition)
            {
                var dgvhide = (ColumnDefinition)param;

                dgvhide.Width = (dgvhide.Width.Value != 0) ? new GridLength(0, GridUnitType.Pixel) : new GridLength(1.5, GridUnitType.Star);

                dgvHideRef = dgvhide;
            }

        }

        private async Task UpdateInvoice()
        {

            try
            {
                if (SelectedInvoice == null || InvoiceModel == null)
                {
                    _notificationService.ShowMessage("Select an invoice to update", Common.NotificationType.Error);
                    return;
                }

                bool _isValid = ValidateInvoice(InvoiceModel);

                if (!_isValid) return;

                UpdateInvoiceViewModel _invoice = GetUpDateInvoiceModel(InvoiceModel);

                if (_invoice != null)
                {
                    await Task.Run(async () =>
                    {
                        var _result = await _invoiceService.UpdateInvoiceDetails(_invoice.InvoiceID, _invoice);

                        if (_result != null && _result.IsSuccess && _result.Data.IsValidString())
                        {
                            Application.Current?.Dispatcher.Invoke(() =>
                            {
                                _notificationService.ShowMessage("Invoice details updated", Common.NotificationType.Success);

                                var _inv = InvoiceList.FirstOrDefault(x => x.InvoiceId == Convert.ToInt32(_result.Data));
                                if (_inv != null)
                                {
                                    InvoiceList.Remove(_inv);
                                }
                                Cancel();
                            });

                        }
                    });
                }

            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in invoice model", _ex);
            }


        }

        private UpdateInvoiceViewModel GetUpDateInvoiceModel(InvoiceModel invoice)
        {
            if (invoice == null) return null;

            return new UpdateInvoiceViewModel
            {
                InvoiceID = invoice.InvoiceId,
                Products = GetInvoiceModelProducts(invoice.StockProducts.ToList())
            };
        }

        private IEnumerable<UpdateInvoiceProductViewModel> GetInvoiceModelProducts(List<InvoiceProductCardViewModel> list)
        {

            var _products = new List<UpdateInvoiceProductViewModel>();

            foreach (var _product in list)
            {
                UpdateInvoiceProductViewModel _p = GetInvoiceProduct(_product);
                if (_p != null)
                {
                    _products.Add(_p);
                }
            }
            return _products;
        }

        private UpdateInvoiceProductViewModel GetInvoiceProduct(InvoiceProductCardViewModel product)
        {
            //flat discount only.
            return new UpdateInvoiceProductViewModel
            {
                ProductDiscount = product.ProductDiscountFlat,
                ProductDiscountFlat = product.ProductDiscountFlat,
                //ProductDiscountPerecent = product.ProductDiscountPerecent,
                ProductGST = product.ProductGST,
                ProductId = product.ProductId,
                ProductDiscountMode = "Flat",//(product.ProductDiscountFlat > 0) ? "Flat" : "Flat",
                ProductSellingPrice = product.ProductSellingPrice,
                ProductTotal = product.ProductTotal,
                ProductUniqGuid = product.ProductUniqGuid,
                ProductMRP = product.ProductMRP,
                Margin = product.Margin
            };
        }

        private bool ValidateInvoice(InvoiceModel invoice)
        {
            if (invoice == null) return false;

            if (!invoice.StockProducts.Any())
            {
                _notificationService.ShowMessage("Invoice should have atleast one product", Common.NotificationType.Error);
                return false;
            }

            foreach (var _product in invoice.StockProducts)
            {

                //Check for selling price                     

                if (_product.ProductSellingPrice <= 0)
                {
                    _notificationService.ShowMessage($"Enter the selling price for the product {_product.ProductName}", Common.NotificationType.Error);
                    return false;

                }

                //check for product type fertilizer selling price should be equal or less than MRP

                if (_product.ProductTypeName.ToLower().Contains("fertilizer"))
                {
                    if (_product.ProductSellingPrice > _product.ProductMRP)
                    {
                        _notificationService
                            .ShowMessage($"Selling price should be less than or equal to MRP", Common.NotificationType.Error);
                        return false;
                    }
                }


                //for some product gst wont be there ?
                //if (_product.ProductGST <= 0)
                //{
                //    _notificationService.ShowMessage($"Enter valid product GST for {_product.ProductName}", Common.NotificationType.Error);

                //    return false;
                //}

            }

            return true;
        }

        private void Cancel()
        {

            SelectedInvoice = null;
            InvoiceModel = null;
            if (dgvHideRef != null)
            {
                dgvHideRef.Width = new GridLength(1.5, GridUnitType.Star);
            }


        }


        private async Task RefreshInvoice()
        {

            LoadStoresAsync();
            await LoadInvoices();
        }

        private async Task GetInvoiceDetails()
        {
            IsProgress = true;

            InvoiceModel = null;

            if (_cancellationTokenSource != null)
                _cancellationTokenSource.Cancel();

            _cancellationTokenSource = new CancellationTokenSource();

            if (SelectedInvoice != null && SelectedInvoice.InvoiceId > 0)
            {

                await Task.Run(async () =>
                {
                    var _result = await _invoiceService.GetInvoiceDetails(SelectedInvoice.InvoiceId, _cancellationTokenSource.Token);
                    if (_result != null && _result.IsSuccess)
                    {
                        UpdateInvoiceDetails(_result.Data, _cancellationTokenSource.Token);

                    }
                    else
                    {
                        _notificationService.ShowMessage(_result.Error ?? "Invoice not found", Common.NotificationType.Error);
                    }

                });
            }

            IsProgress = false;
        }

        private bool _isProgress;
        public bool IsProgress
        {
            get { return _isProgress; }
            set { SetProperty(ref _isProgress, value); }
        }
        private void UpdateInvoiceDetails(AddStockProductViewModel data, CancellationToken token)
        {
            try
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                  {
                      InvoiceModel = GetInvoiceModel(data);

                  }, System.Windows.Threading.DispatcherPriority.Normal, token);
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in invoice model", _ex);
            }
        }



        private InvoiceModel GetInvoiceModel(AddStockProductViewModel data)
        {
            var _invoice = new InvoiceModel();
            _invoice.CreatedDate = data.CreatedDate;
            _invoice.DefectiveQuantity = data.DefectiveQuantity;
            _invoice.GrossTotal = data.GrossTotal;
            _invoice.InvoiceDate = data.InvoiceDate;
            _invoice.InvoiceDiscount = data.InvoiceDiscount;
            _invoice.InvoiceDiscountFlat = data.InvoiceDiscountFlat;

            _invoice.InvoiceNetTotal = data.InvoiceNetTotal;
            _invoice.InvoiceNumber = data.InvoiceNumber;
            _invoice.InvoiceRoundOff = data.InvoiceRoundOff;
            _invoice.InvoiceOthers = data.InvoiceOthers;
            _invoice.IsQADone = data.IsQADone;
            _invoice.Quantity = data.Quantity;
            _invoice.StoreName = data.StoreName;
            _invoice.SupplierName = data.SupplierName;
            _invoice.State = data.State;
            _invoice.StockProducts = GetProducts(data.StockProducts, data.InvoiceDiscountPerecent, data.InvoiceDiscountFlat, data.ApplyDiscountType, data.State);
            _invoice.InvoiceOthers = data.InvoiceOthers;
            _invoice.TotalGST = data.TotalGST;
            _invoice.Transportcharges = data.TransportCharges;
            _invoice.InvoiceId = data.InvoiceId;
            _invoice.shippingAddress = data.shippingAddress;
            _invoice.IshippingAddress = data.shippingAddress != null ? true : false;
            //dc number
            _invoice.IsDcNumber = data.IsDcNumber;
            _invoice.DcNumber = data.DcNumber;
            _invoice.DcNumberDate = data.DcNumberDate;
            _invoice.FileAttachments = data.FileAttachments;
            _invoice.DiscountApplyType = data.ApplyDiscountType;
            _invoice.HasDiscount = data.InvoiceDiscount > 0;
            _invoice.InvoiceDiscountPerecent = GetInvoiceLevelDiscountPercent(data.InvoiceDiscountPerecent, data.ApplyDiscountType, data.TotalGST, data.InvoiceNetTotal);  //data.InvoiceDiscountPerecent;
            _invoice.GSTHeader = data.State == AppConstants.STATE ? AppConstants.GSTHEADER : AppConstants.IGSTHEADER;

            return _invoice;
        }

        private float GetInvoiceLevelDiscountPercent(float invoiceDiscountPerecent,
            string applyDiscountType,
            float totalGST,
            float invoiceNetTotal)
        {
            if (applyDiscountType == AppConstants.APPLY_DISCOUNT_AFTER_GST)
            {
                return invoiceDiscountPerecent;

                //return (float)Math.Round(invoiceDiscountPerecent * 100 / (invoiceNetTotal + totalGST),2,MidpointRounding.AwayFromZero);
            }

            return (float)Math.Round(invoiceDiscountPerecent, 2, MidpointRounding.AwayFromZero);
        }

        private ObservableCollection<InvoiceProductCardViewModel> GetProducts(List<StockProductViewModel> stockProducts, float discountPercent, float discountFlat, string applyDiscounttype, string state)
        {
            var _products = new ObservableCollection<InvoiceProductCardViewModel>();

            if (stockProducts == null || !stockProducts.Any()) return _products;

            var _totalProductCount = stockProducts.Sum(x =>
            {
                if (x.IsGroupTrackMode)
                {
                    return x.Quantity * x.ProductSubQty;
                }
                return x.Quantity;

            });

            foreach (var _product in stockProducts)
            {
                InvoiceProductCardViewModel _p = GetProduct(_product, discountPercent, discountFlat, _totalProductCount, applyDiscounttype, state);
                if (_p != null)
                {
                    _products.Add(_p);
                }
            }
            return _products;
        }

        private InvoiceProductCardViewModel GetProduct(StockProductViewModel product, float discountPercent, float discountFlat, int productTotalCount, string applyDiscounttype, string state)
        {
            if (product == null) return null;

            var _product = new InvoiceProductCardViewModel(_notificationService);

            _product.DateOfExpiry = product.DateOfExpiry;
            _product.DateOfManufacture = product.DateOfManufacture;
            _product.DefectiveQuantity = product.DefectiveQuantity;
            _product.Location = product.Location;
            _product.ManufactureName = product.ManufactureName;
            _product.ProductDiscount = product.ProductDiscount;
            //invoice level discount calculation
            _product.InvoiceProductDiscount = product.ProductInvoiceDiscount; //discountPercent > 0 ? discountPercent : discountFlat;         
            _product.isInvoiceProductDiscount = discountPercent > 0 || discountFlat > 0;
            _product.ProductDiscountRate = discountPercent > 0 ?
                //CalculationProductDiscountPercent(discountPercent, product.ProductRate) 
                CalculationProductDiscountPercentV2(discountPercent, product.ProductRate, product.ProductSubQty, product.Quantity, product.IsGroupTrackMode, productTotalCount, applyDiscounttype, product.ProductGST)
                : CalculationProductDiscountFlatv2(discountFlat, product.ProductRate, product.ProductSubQty, product.Quantity, product.IsGroupTrackMode, productTotalCount, applyDiscounttype, product.ProductGST);
            _product.InvoiceDiscountHeader = discountPercent > 0 ? "Discount(%)" : discountFlat > 0 ? "Discount(Flat)" : "";

            _product.ProductDiscountMode = "Flat";
            _product.ProductId = product.ProductId;
            _product.WarrantyService = product.WarrantyService;
            _product.ProductName = product.ProductName;
            _product.ProductRate = product.ProductRate;
            _product.ProductSellingPrice = product.ProductSellingPrice;
            _product.OrginalProductSellingPrice = product.ProductSellingPrice;
            _product.ProductTotal = product.ProductTotal;
            _product.ProductTypeName = product.ProductTypeName;
            _product.Quantity = product.IsGroupTrackMode == true ? product.Quantity : product.Quantity - product.DefectiveQuantity;
            _product.SerialNumbers = new ObservableCollection<string>(product.SerialNumbers);
            _product.AttributesSelectedList = product.AttributesSelectedList;
            _product.ProductUniqGuid = product.ProductUniqGuid;
            _product.ProductGST = product.ProductGST;
            _product.ProductMRP = product.ProductMRP;
            //Deprecated
            //_product.HSNCode = product.HSNCode;
            _product.Misc = product.Misc;
            //additional fields
            _product.BaseUnitType = product.BaseunitType;
            _product.SubUnitType = product.SubunitType;
            _product.ProductSubQty = product.ProductSubQty;
            _product.IsGroupTrackMode = product.IsGroupTrackMode;
            _product.InventoryTrackMode = product.InventoryTrackMode;
            _product.IsGroupdefective = product.DefectiveGroup.Count > 0 ? true : false;
            _product.DefectiveList = product.IsGroupTrackMode == true ? getDefeiveList(product.DefectiveGroup, product.BaseunitType, product.SubunitType) : null;
            //HSN Code
            _product.HSN = product.HSN;

            //lot number
            _product.LotNumber = product.Lotnumber;

            //product sku and dept code..
            _product.DeptCode = product.DeptCode;
            _product.ProductSKU = product.ProductSKU;

            _product.ProductGSTperQuantity = product.ProductGSTperQuantity;

            _product.GSTHeaderPer = state == AppConstants.STATE ? AppConstants.GSTHEADERPER : AppConstants.IGSTHEADERPER;
            _product.GSTHeaderQty = state == AppConstants.STATE ? AppConstants.GSTHEADERQTY : AppConstants.IGSTHEADERQTY;

            _product.RSP = product.RSP;
            return _product;

        }

        private float CalculationProductDiscountPercentV2(float discountPercent,
            float productRate,
            int productSubQty,
            int quantity,
            bool isGroupTrackMode,
            int productTotalCount,
            string applyDiscounttype,
            float productGST)
        {
            try
            {

                if (applyDiscounttype == AppConstants.APPLY_DISCOUNT_AFTER_GST)
                {


                    float _gstRate = productRate * productGST / 100;

                    float _gstAdded = productRate + _gstRate;


                    float _applyDiscountPercentage = _gstAdded * discountPercent / 100;

                    return (float)Math.Round(_gstAdded - _applyDiscountPercentage, 2, MidpointRounding.AwayFromZero);


                    //return (float)Math.Round(productRate + _gstRate - discountPercent,2,MidpointRounding.AwayFromZero);




                }

                if (applyDiscounttype == AppConstants.APPLY_DISCOUNT_BEFORE_GST)
                {

                    //var discountamount = (discount * productrate) / 100;

                    //float _beforeGst = productRate - discountPercent;

                    //var discountamount = (discount * productrate) / 100;
                    //return productrate - discountamount;

                    float _rateDiscount = productRate * discountPercent / 100;

                    float _applyDiscountRate = productRate - _rateDiscount;

                    return (float)Math.Round(_applyDiscountRate, 2, MidpointRounding.AwayFromZero);

                    //return (float)Math.Round(productRate - discountPercent,2,MidpointRounding.AwayFromZero);
                }


            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in invoice model", _ex);
            }

            return default;
        }

        private float CalculationProductDiscountFlatv2(float discountFlat,
                                                       float productRate,
                                                       int productSubQty,
                                                       int quantity,
                                                       bool isGroupTrackMode,
                                                       int productTotalCount,
                                                       string applyDiscounttype,
                                                       float productGST)
        {
            if (applyDiscounttype == AppConstants.APPLY_DISCOUNT_AFTER_GST)
            {

                var _rateWithGST = productRate + (productRate * (productGST / 100));


                var _totalDiscountFlat = discountFlat / productTotalCount;

                return (float)Math.Round(_rateWithGST - _totalDiscountFlat, 2, MidpointRounding.AwayFromZero);


            }
            else if (applyDiscounttype == AppConstants.APPLY_DISCOUNT_BEFORE_GST)
            {





                var _totalDiscountFlat = discountFlat / productTotalCount;

                var _disRate = (float)Math.Round(productRate - _totalDiscountFlat, 2, MidpointRounding.AwayFromZero);

                return _disRate;

            }
            else
            {
                //invalid case or nothing is selected.
            }

            return default;
        }

        public float CalculationProductDiscountPercent(float discount, float productrate)
        {
            var discountamount = (discount * productrate) / 100;
            return productrate - discountamount;

        }

        public float CalculationProductDiscountFlat(float discount, float productrate, int subqty, int qty, bool isgroup)
        {
            if (isgroup)
            {
                var disocuntperamount = discount / (subqty * qty);
                return productrate - disocuntperamount;
            }
            else
            {
                var disocuntperamount = discount / qty;
                return productrate - disocuntperamount;
            }


        }

        //private ObservableCollection<string> GetSerialNumbers(List<string> serialNumbers)
        //{
        //    if(serialNumbers!=null && serialNumbers.Any())
        //    {
        //        var _slNumers = new ObservableCollection<string>();

        //         _slNumers.AddRange(serialNumbers);

        //        return _slNumers;
        //    }
        //    return null;
        //}

        private InvoiceModel _invoceModel;
        public InvoiceModel InvoiceModel
        {
            get { return _invoceModel; }
            set { SetProperty(ref _invoceModel, value); }
        }



        private ObservableCollection<InvoiceListViewModel> _invoiceList;
        public ObservableCollection<InvoiceListViewModel> InvoiceList
        {
            get { return _invoiceList; }
            set { SetProperty(ref _invoiceList, value); }
        }


        private InvoiceListViewModel _selectedInvoice;
        public InvoiceListViewModel SelectedInvoice
        {
            get { return _selectedInvoice; }
            set { SetProperty(ref _selectedInvoice, value); }
        }

        //private bool _hideColumn;

        //public bool HideColumn
        //{
        //    get { return _hideColumn; }
        //    set
        //    {
        //        SetProperty(ref _hideColumn, value);

        //    }
        //}


        private async Task LoadInvoices()
        {
            InvoiceList?.Clear();


            await Task.Run(async () =>
            {

                var _result = await _invoiceService.GetInvoices();

                if (_result.IsSuccess)
                {
                    InvoiceList = new ObservableCollection<InvoiceListViewModel>(_result.Data);
                }

            });

        }

        public ObservableCollection<Dictionary<AttributeMap, AttributeMap>> getDefeiveList(List<AttributeMap> attributeMaps, string selectedunit, string selectedsubunit)
        {
            ObservableCollection<Dictionary<AttributeMap, AttributeMap>> DefectiveList = new ObservableCollection<Dictionary<AttributeMap, AttributeMap>>();
            if (attributeMaps != null && attributeMaps.Count > 0)
            {
                foreach (var item in attributeMaps)
                {
                    Dictionary<AttributeMap, AttributeMap> keyValues = new Dictionary<AttributeMap, AttributeMap>();

                    AttributeMap attributeMapkey = new AttributeMap();
                    attributeMapkey.AttributeValueName = selectedunit;
                    attributeMapkey.AttributeValueId = Convert.ToInt32(item.AttributeValueName);
                    AttributeMap attributeMapkeyValue = new AttributeMap();
                    attributeMapkeyValue.AttributeValueName = selectedsubunit;
                    attributeMapkeyValue.AttributeValueId = item.AttributeValueId;
                    keyValues.Add(attributeMapkey, attributeMapkeyValue);
                    DefectiveList.Add(keyValues);
                }
            }


            return DefectiveList;
        }

        private async void LoadStoresAsync()
        {
            var _result = await _storeService.GetStores("isenabled=true");
            if (_result != null && _result.Count() > 0)
            {

                var addAllList = _result.ToList();
                addAllList.Insert(0, new Store() { StoreId = -1, Name = "All" });
                _result = addAllList;

                Stores = new ObservableCollection<Store>(_result.Where(x => x.Parent_ref == null));
            }
        }

        private Store _selectedStore;
        public Store SelectedStore
        {
            get { return _selectedStore; }
            set { SetProperty(ref _selectedStore, value); }
        }

        private ObservableCollection<Store> _stores;
        public ObservableCollection<Store> Stores
        {
            get { return _stores; }
            set { SetProperty(ref _stores, value); }
        }
    }
}
