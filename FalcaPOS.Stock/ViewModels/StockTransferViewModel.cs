using ControlzEx.Standard;
using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.EwayBill;
using FalcaPOS.Entites.Indent;
using FalcaPOS.Entites.Sales;
using FalcaPOS.Entites.Stock;
using FalcaPOS.Stock.Views;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FalcaPOS.Stock.ViewModels
{
    public class StockTransferViewModel : BindableBase
    {
        private readonly ISalesService _salesService;

        private readonly IStoreStockService storeStockService;

        private readonly IProductTypeService productTypeService;

        private readonly IStoreService storeService;

        private readonly IProductService productService;

        private readonly IEventAggregator _eventAggregator;

        private readonly INotificationService notificationService;

        private readonly IStockTransferService stockTransferService;

        private readonly ProgressService _ProgressService;


        private readonly Logger logger;

        public DelegateCommand ProductTypeChange { get; private set; }

        public DelegateCommand ManufacturerChange { get; private set; }

        public DelegateCommand ProductNameChange { get; private set; }

        public DelegateCommand<StockTransferProductViewModel> ProductRemoveCommand { get; private set; }

        public DelegateCommand StockTransfer { get; private set; }

        public DelegateCommand<object> StockTransferConfirm { get; private set; }

        public DelegateCommand<object> GetProductCommand { get; private set; }

        public DelegateCommand GetRefreshCommand { get; private set; }



        public DelegateCommand<object> GetResetCommand { get; private set; }

        public DelegateCommand DownloadStockTranfer { get; private set; }

        public DelegateCommand<object> FetchTransferProductCommand { get; private set; }

        TransferViewModel StockTrnasferModel = new TransferViewModel();

        public DelegateCommand<object> AddTransportdetailsCommand { get; private set; }
      
        public DelegateCommand<object> GenerateEwayBillCommand { get; private set; }
        public DelegateCommand<object> DownloadEwayBillCommand { get; private set; }
        public DelegateCommand<object> RejectCommand { get; set; }

        public DelegateCommand<object> TransferPageRejectConfirmCommand { get; set; }

        public StockTransferViewModel(ISalesService salesService, IEventAggregator eventAggregator,
            IProductService ProductService,
            IProductTypeService ProductTypeService,
            IStoreService StoreService,
            Logger Logger,
            IStoreStockService StorestockService,
            INotificationService NotificationService,
            IStockTransferService StockTransferService, ProgressService ProgressService)
        {
            _salesService = salesService ?? throw new ArgumentNullException(nameof(salesService));

            notificationService = NotificationService ?? throw new ArgumentNullException(nameof(NotificationService));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));


            logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            productTypeService = ProductTypeService ?? throw new ArgumentNullException(nameof(ProductTypeService));

            productService = ProductService ?? throw new ArgumentNullException(nameof(ProductService));

            storeService = StoreService ?? throw new ArgumentNullException(nameof(StoreService));

            storeStockService = StorestockService ?? throw new ArgumentNullException(nameof(StorestockService));

            stockTransferService = StockTransferService ?? throw new ArgumentNullException(nameof(StockTransferService));

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            ProductRemoveCommand = new DelegateCommand<StockTransferProductViewModel>(RemoveStockTransferProduct);

            StockTransfer = new DelegateCommand(StockTrnasferSubmit);

            StockTransferConfirm = new DelegateCommand<object>(StockTransferUpdate);

            GetProductCommand = new DelegateCommand<object>(GetProduct);


            GetRefreshCommand = new DelegateCommand(RefreshLoadData);


            DownloadStockTranfer = new DelegateCommand(GetDownloadStockTransferPDF);


            GetResetCommand = new DelegateCommand<object>(ResetData);
            VehicleType = GetVehicleTypes();

            _eventAggregator.GetEvent<RegenerateEwayBill>().Subscribe(GenerateEwayBillCommon);


            //            //SignalRReloadStockTransferListForSahayaEvent
            //            eventAggregator.GetEvent<SignalRReloadStockTransferListForSahayaEvent>().Subscribe(
            //ResetData);


            //_eventAggregator.GetEvent<DummyEvent>().Subscribe(
            //ResetData);


            //eventAggregator.GetEvent<DenominationPageRefreshEvent>().Subscribe(() =>
            //{

            //});

            FetchTransferProductCommand = new DelegateCommand<object>(FetchTransferProduct);

            AddTransportdetailsCommand = new DelegateCommand<object>(AddTransportDetails);

            GenerateEwayBillCommand = new DelegateCommand<object>(GenerateEwayBill);

            DownloadEwayBillCommand = new DelegateCommand<object>(DownloadEwayBill);

            GetAccessToken();

            RejectCommand = new DelegateCommand<object>(RejectStockTransfer);

            TransferPageRejectConfirmCommand = new DelegateCommand<object>(RejectConfirmPopup);
        }

        public async void GetAccessToken()
        {
            try
            {
                //var _result = await stockTransferService.GetEwayBillAccessToken("testeway@mastersindia.co", "Client!@#Demo987", "TMDIIbTwzkCQWFTHpA", "BZpqtmFRIkfIrgjOfhyzQjLX", "password");
                //AppConstants.EwayBillAccesstoken = _result.access_token;

                var _result = await stockTransferService.GetEwayBillAccessToken();
                if (_result != null)
                {
                    AppConstants.EwayBillAccesstoken = _result.access_token;
                }

            }
            catch (Exception ex) {
                logger.LogError(ex.Message);
            }
        }
        public async void AddTransportDetails(object obj)
        {
            try
            {
                if (GetStockTransferList != null)
                {

                    Name = DocumentNo = DocumentDate = VehicleNo = SelectedVehicleType =  null;
                    if (GetStockTransferList.stockTransferProducts == null || GetStockTransferList.stockTransferProducts.Count <= 0)
                    {
                        notificationService.ShowMessage("Please add stock transfer product", NotificationType.Error);
                        return;
                    }

                    if (StockTransferProducts.Sum(x => x.TransferQty) != GetStockTransferList.StockTransferList.Sum(x => x.TransferQty))
                    {
                        notificationService.ShowMessage("Requested Product transfer qty  and Transfer Product transfer qty not matching ", NotificationType.Error);
                        return;
                    }


                }


                EwayBillGenerate Eway = new EwayBillGenerate();
                Eway.DataContext = this;
                await DialogHost.Show(Eway, "RootDialog", updateAddTransportEventHandler);

            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
        }

        public  void GenerateEwayBill(object obj)
        {
            try
            {
                
                if (string.IsNullOrEmpty(Name))
                {
                    notificationService.ShowMessage("Please enter Transporter Name",NotificationType.Error); return; 
                }

               
                //if (string.IsNullOrEmpty(DocumentNo))
                //{
                //    notificationService.ShowMessage("Please enter Transporter Document No", NotificationType.Error); return;
                //}

                //if (string.IsNullOrEmpty(DocumentDate))
                //{
                //    notificationService.ShowMessage("Please enter Transporter Document Date", NotificationType.Error); return;
                //}

                if (string.IsNullOrEmpty(VehicleNo))
                {
                    notificationService.ShowMessage("Please enter  Transporter Vehicle No", NotificationType.Error); return;
                }

                if (!isValidVehicleNumberPlate(VehicleNo))
                {
                    notificationService.ShowMessage("Please enter valid Transporter Vehicle no", NotificationType.Error); return;
                }
                if (string.IsNullOrEmpty(SelectedVehicleType))
                {
                    notificationService.ShowMessage("Please enter Transporter Vehicle Type", NotificationType.Error); return;
                }
                
                
                var TargetClose = ((Button)(obj));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);

            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
        }

        public async void FetchTransferProduct(object models)
        {
            try
            {

                var stockTransferData = (TransferViewModel)models;
                GetStockTransferList = stockTransferData;
                STReceiptNumber = await stockTransferService.GetCurrentStockTransferReceiptNumber();
                stockTransferData.STNumber = STReceiptNumber;
                stockTransferData.stockTransferProducts = null;
                stockTransferData.ProductCode = null;
                StockTransferProducts.Clear();



            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }
        public void ResetData(Object obj)
        {
            try
            {
                StockTransferProducts.Clear();

                GetStockTransferList = null;

                SRNumber = null;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
        }



        public void StockTransferUpdate(object param)
        {
            try
            {
                if (string.IsNullOrEmpty(GetStockTransferList.STNumber))
                {
                    notificationService.ShowMessage("ST Number should not be empty", NotificationType.Error);
                    return;
                }
                if (string.IsNullOrEmpty(GetStockTransferList.FromLocation))
                {
                    notificationService.ShowMessage("From should not be empty", NotificationType.Error);
                    return;
                }
                if (string.IsNullOrEmpty(GetStockTransferList.ToLocation))
                {
                    notificationService.ShowMessage("To should not be empty", NotificationType.Error);
                    return;
                }
                if (GetStockTransferList.stockTransferProducts == null || GetStockTransferList.stockTransferProducts.Count <= 0)
                {
                    notificationService.ShowMessage("Please add stock transfer product", NotificationType.Error);
                    return;
                }
                var TargetClose = ((Button)(param));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);

            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
        }

        public async void StockTrnasferSubmit()
        {
            try
            {
                if (GetStockTransferList != null)
                {

                    if (string.IsNullOrEmpty(GetStockTransferList.STNumber))
                    {
                        notificationService.ShowMessage("ST Number should not empty", NotificationType.Error);
                        return;
                    }
                    if (string.IsNullOrEmpty(GetStockTransferList.FromLocation))
                    {
                        notificationService.ShowMessage("From should not empty", NotificationType.Error);
                        return;
                    }
                    if (string.IsNullOrEmpty(GetStockTransferList.ToLocation))
                    {
                        notificationService.ShowMessage("To should not empty", NotificationType.Error);
                        return;
                    }
                    if (GetStockTransferList.stockTransferProducts == null || GetStockTransferList.stockTransferProducts.Count <= 0)
                    {
                        notificationService.ShowMessage("Please add stock transfer product", NotificationType.Error);
                        return;
                    }

                    if (StockTransferProducts.Sum(x => x.TransferQty) != GetStockTransferList.StockTransferList.Sum(x => x.TransferQty))
                    {
                        notificationService.ShowMessage("Requested Product transfer qty  and Transfer Product transfer qty not matching ", NotificationType.Error);
                        return;
                    }
                    //if (!GetStockTransferList.IsWayBillGenerated)
                    //{
                    //    if (GetStockTransferList.TranportDetails.Id==null)
                    //    {
                    //        notificationService.ShowMessage("Please generate E way bill", NotificationType.Error);
                    //        return;
                    //    }
                    //}
                    
                    

                    StockTransferConfirmPopup confirmPopup = new StockTransferConfirmPopup();
                    confirmPopup.DataContext = this;
                    await DialogHost.Show(confirmPopup, "RootDialog", updateEventHandler);
                }
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
        }

        private async void updateEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {
                var _viewmodel = (StockTransferViewModel)eventArgs.Parameter;
                if (_viewmodel != null)
                {
                    List<StockTransferProductDTO> productDTOs = new List<StockTransferProductDTO>();
                    if (GetStockTransferList != null && GetStockTransferList.stockTransferProducts.Count() > 0)
                    {
                        foreach (var item in GetStockTransferList.stockTransferProducts)
                        {
                            productDTOs.Add(new StockTransferProductDTO()
                            {
                                ProductId = item.ProductId,
                                StockProuductId = item.StockProuductId,
                                TransferQty = item.TransferQty
                            });

                        }

                        StockTransferDTO stockTrnasferModel = new StockTransferDTO()
                        {
                            STNumber = GetStockTransferList.STNumber,

                            StockTransferList = productDTOs,
                            SRNumber = GetStockTransferList.SRNumber,
                            STDate = GetStockTransferList.STDate,

                            ToParent_ref = GetStockTransferList.ToParent_ref,

                        };

                        if (StockTrnasferModel.ToParent_ref == null)
                        {
                            await _ProgressService.StartProgressAsync();

                            await Task.Run(async () =>
                            {

                                var _result = await stockTransferService.StockTransfer(stockTrnasferModel);

                                Application.Current?.Dispatcher?.Invoke(async () =>
                                {

                                    if (_result != null && _result.IsSuccess)
                                    {
                                        notificationService.ShowMessage(_result.Data, NotificationType.Success);

                                        StockTransferProducts.Clear();

                                        GetStockTransferList = null;

                                        SRNumber = null;
                                        STReceiptNumber = null;
                                        await _ProgressService.StopProgressAsync();
                                        RefreshLoadData();


                                    }
                                    else
                                    {
                                        notificationService.ShowMessage(_result.Error, NotificationType.Error);
                                        RefreshLoadData();
                                    }



                                });


                            });
                            await _ProgressService.StopProgressAsync();
                        }
                        else
                        {
                            if (GetStockTransferList.ToParent_ref != null)
                            {
                                await _ProgressService.StartProgressAsync();

                                await Task.Run(async () =>
                                {

                                    var _result = await stockTransferService.RspStockTransfer(stockTrnasferModel);

                                    Application.Current?.Dispatcher?.Invoke(async () =>
                                    {

                                        if (_result != null && _result.IsSuccess)
                                        {
                                            notificationService.ShowMessage(_result.Data, NotificationType.Success);

                                            StockTransferProducts.Clear();

                                            GetStockTransferList = null;

                                            SRNumber = null;
                                            STReceiptNumber = null;

                                            await _ProgressService.StopProgressAsync();
                                            RefreshLoadData();


                                        }



                                    });


                                });
                                await _ProgressService.StopProgressAsync();
                            }
                        }


                    }
                }
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            finally
            {
                await _ProgressService.StopProgressAsync();
            }
        }


        private TransferViewModel _getstockTransferList;
        public TransferViewModel GetStockTransferList
        {
            get { return _getstockTransferList; }
            set { SetProperty(ref _getstockTransferList, value); }
        }

        private ObservableCollection<TransferViewModel> _getstockTransferLists;
        public ObservableCollection<TransferViewModel> GetStockTransferLists
        {
            get { return _getstockTransferLists; }
            set { SetProperty(ref _getstockTransferLists, value); }
        }

        private string _productCode;
        public string ProductCode
        {
            get { return _productCode; }
            set { SetProperty(ref _productCode, value); }
        }

        private ObservableCollection<StockTransferProductViewModel> stockTransferProducts = new ObservableCollection<StockTransferProductViewModel>();
        public ObservableCollection<StockTransferProductViewModel> StockTransferProducts
        {
            get { return stockTransferProducts; }
            set { SetProperty(ref stockTransferProducts, value); }
        }





        private bool isTransferEnable;
        public bool IsTransferEnable
        {
            get { return isTransferEnable; }
            set { SetProperty(ref isTransferEnable, value); }
        }
        private async void GetProduct(object productcode)
        {
            StockTrnasferModel = ((TransferViewModel)productcode);

            ProductCode = StockTrnasferModel.ProductCode;


            if (GetStockTransferList == null)
            {
                notificationService.ShowMessage("Requested Product should not empty", NotificationType.Error);

                return;
            }
            if (string.IsNullOrEmpty(ProductCode))
            {
                notificationService.ShowMessage("Product code is required", NotificationType.Error);

                return;
            };



            SalesProduct _product = null;

            //if (_res)
            {
                //  if (_productId > 0)
                {
                    if (StockTransferProducts.Any(x => x.BarCode.Trim().ToLower() == ProductCode.Trim().ToLower()))
                    {
                        notificationService.ShowMessage("Product is already added.", NotificationType.Error);

                        ProductCode = string.Empty;

                        return;
                    }

                    await Task.Run(async () =>
                    {
                        _product = await _salesService.GetStockProduct(ProductCode);

                    });

                    if (_product != null)
                    {
                        var stocktransferQty = StockTransferProducts.Where(x => x.ProductSKU.Trim().ToLower() == _product.SKU.Trim()).Sum(x => x.TransferQty);

                        var requestQty = GetStockTransferList.StockTransferList.Where(x => x.ProductSKU.ToLower() == _product.SKU.ToLower()).Sum(x => x.TransferQty);


                        if (StockTransferProducts.Any(x => x.ProductSKU == _product.SKU) && stocktransferQty == requestQty)
                        {
                            notificationService.ShowMessage("Product is already added", NotificationType.Error);

                            ProductCode = string.Empty;

                            return;
                        }

                        if (!GetStockTransferList.StockTransferList.Any(x => x.ProductSKU == _product.SKU))
                        {
                            notificationService.ShowMessage("This product is not matching with requested product.", NotificationType.Error);

                            ProductCode = string.Empty;

                            return;
                        }

                        if (!GetStockTransferList.StockTransferList.Any(x => x.ProductSKU == _product.SKU))
                        {
                            notificationService.ShowMessage("This product is not matching with requested product.", NotificationType.Error);

                            ProductCode = string.Empty;

                            return;
                        }

                        if (!GetStockTransferList.StockTransferList.Any(x => x.ProductId == _product.SelectedbySKUProductId))
                        {
                            notificationService.ShowMessage("This product is not matching with requested product.", NotificationType.Error);

                            ProductCode = string.Empty;

                            return;
                        }

                        var TransferQty = GetStockTransferList.StockTransferList.FirstOrDefault(x => x.ProductSKU.ToLower() == _product.SKU.ToLower()).TransferQty;


                        //if (TransferQty>_product.AvailableQuantity)
                        //{
                        //    notificationService.ShowMessage("Request qty not avaialbe in stock qty", NotificationType.Error);

                        //    ProductCode = string.Empty;

                        //    return;
                        //}

                        var needToAdded = TransferQty - stocktransferQty;

                        var TransferedQty = needToAdded > _product.AvailableQuantity ? _product.AvailableQuantity : needToAdded;


                        StockTransferProductViewModel model = GetSalesModel(_product, TransferedQty);


                        StockTransferProducts.Add(model);



                        StockTrnasferModel.stockTransferProducts = StockTransferProducts.ToList();
                        StockTrnasferModel.ProductCode = string.Empty;



                    }
                    StockTrnasferModel.ProductCode = string.Empty;

                }

            }
        }

        private StockTransferProductViewModel GetSalesModel(SalesProduct product, int trnasferqty)
        {

            if (product == null) return null;

            var productModel = new StockTransferProductViewModel();



            productModel.SellingPrice = product.ProductSellingPrice;
            productModel.Brand = product.Manufacturer?.Name;
            productModel.Department = product.ProductType?.Name;
            productModel.ProductName = product.ProductName;
            productModel.ProductSKU = product.SKU;
            productModel.ProductId = product.SelectedbySKUProductId;
            productModel.StockProuductId = product.StockProductId;
            //stock qty
            productModel.StockQty = product.AvailableQuantity;
            productModel.BarCode = product.BarCode;
            productModel.TransferQty = trnasferqty;
            productModel.Rate = product.ProductRate;
            productModel.Gst = product.ProductGST;
            productModel.HsnCode = product.HSNcode;
            productModel.Description = product.Description;
            return productModel;

        }



        public void RemoveStockTransferProduct(StockTransferProductViewModel removeStockTransfer)
        {
            try
            {



                StockTrnasferModel.stockTransferProducts.Remove(removeStockTransfer);


            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
        }


        public void RefreshForSahaya(object obj)
        {
            try
            {
                 RefreshLoadData();
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }
        public async void RefreshLoadData()
        {
            try
            {
                await Task.Run(async () =>
                {

                    var _result = await stockTransferService.GetStockTransferList();

                    Application.Current.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess)
                        {
                            ObservableCollection<TransferViewModel> GetStockTransfer = new ObservableCollection<TransferViewModel>();
                            foreach (var item in _result.Data)
                            {

                                List<StockTransferProduct> list = new List<StockTransferProduct>();
                                if (item.stockTransferProducts != null && item.stockTransferProducts.Count > 0)
                                {
                                    foreach (var i in item.stockTransferProducts)
                                    {
                                        list.Add(new StockTransferProduct()
                                        {
                                            ProductId = i.ProductId,
                                            SellingPrice = i.SellingPrice,
                                            BarCode = i.BarCode,
                                            StockProuductId = i.StockProuductId,
                                            TransferQty = i.TransferQty,
                                            ProductSKU = i.ProductSKU,
                                            ProductName = i.ProductName,
                                            Brand = i.Brand,
                                            Department = i.Department,
                                            HsnCode=i.HsnCode,
                                            Description=i.Description,
                                        });
                                    }
                                }

                                int? TranportId = 0;
                                if (item.TranportDetails != null)
                                {
                                    TranportId = item.TranportDetails.Id;
                                }

                                GetStockTransfer.Add(new TransferViewModel()
                                {
                                    SRNumber = item.SRNumber,
                                    SRDate = item.SRDate,
                                    STNumber = item.STNumber,
                                    STDate = item.STDate,
                                    FromLocation = item.FromLocation,
                                    ToLocation = item.ToLocation,
                                    FromStoreId = item.FromStoreId,
                                    ToStoreId = item.ToStoreId,
                                    //TransportCharges = item.TransportCharges,
                                    //Others = item.Others,
                                    StockTransferList = list,
                                    DateHumnaizer = Convert.ToDateTime(item.SRDate).ToHumanReadableString(),
                                    ToParent_ref = item.ToParent_ref,
                                    TransferId = item.TransferId,
                                    ToAddress = item.ToAddress,
                                    ToCity = item.ToCity,
                                    ToState = item.ToState,
                                    ToPinCode = item.ToPinCode,
                                    FromAddress = item.FromAddress,
                                    FromCity = item.FromCity,
                                    FromPinCode = item.FromPinCode,
                                    FromState = item.FromState,
                                    IsWayBillGenerated = (TranportId>0)?true:false,
                                    EwayBillUrl=item.EwayBillUrl,
                                    TranportDetails=item.TranportDetails,

                                }) ;
                            }

                            GetStockTransferLists = GetStockTransfer;

                            GetStockTransferList = GetStockTransfer.FirstOrDefault();
                            StockTransferProducts.Clear();
                            ProductCode = string.Empty;

                        }
                        else
                        {
                            notificationService.ShowMessage(_result.Error, NotificationType.Error);
                            if (GetStockTransferLists != null && GetStockTransferLists.Count > 0)
                            {
                                GetStockTransferLists.Clear();
                            }


                            GetStockTransferList = null;
                        }
                    });

                });
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }

        }

        public async void GetDownloadStockTransferPDF()
        {
            try
            {
                if (GetStockTransferList != null)
                {
                    await _ProgressService.StartProgressAsync();


                    await Task.Run(async () =>
                    {
                        var result = await stockTransferService.GetStockTransferPdf(GetStockTransferList.SRNumber);

                        if (result != null && result.IsSuccess)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {


                                try
                                {


                                    string path = @"c:\PosInvoices";

                                    var _info = Directory.CreateDirectory(path);

                                    var _fileName = path + "\\" + $"{result.Data.FileName}";

                                    if (!File.Exists(_fileName))
                                    {
                                        File.WriteAllBytes(_fileName, result.Data.FileStream);

                                        ProcessStartInfo _p = new ProcessStartInfo
                                        {
                                            UseShellExecute = true,
                                            FileName = _fileName
                                        };

                                        Process.Start(_p);

                                        return;
                                    }
                                    else
                                    {
                                        //Fall back to save  if file name already present
                                        SaveFileManually(result.Data.FileStream);
                                    }
                                }
                                catch (UnauthorizedAccessException _ex)
                                {
                                    logger.LogError(_ex.Message);
                                    //fall back to save manually if file creation is blocked.
                                    SaveFileManually(result.Data.FileStream);
                                }
                                catch (Exception _ex)
                                {
                                    logger.LogError(_ex.Message);
                                    notificationService.ShowMessage("The Pdf preview is failed", NotificationType.Error);

                                }

                            });

                        }
                        else
                        {
                            if (result != null && !result.IsSuccess && result.Error.IsValidString())
                            {
                                notificationService.ShowMessage(result.Error, NotificationType.Error);
                            }
                        }

                    });

                    await _ProgressService.StopProgressAsync();
                }
                else
                {
                    notificationService.ShowMessage("No Data Found", NotificationType.Error);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
            finally {
                await _ProgressService.StopProgressAsync();
            }
        }


        private void SaveFileManually(byte[] stream)
        {
            try
            {
                SaveFileDialog _sd = new SaveFileDialog();

                //_sd.FileName = result.Data.FileName;
                _sd.AddExtension = true;
                _sd.DefaultExt = ".pdf";

                _sd.ShowDialog();

                if (_sd.FileName.IsValidString())
                {

                    File.WriteAllBytes(_sd.FileName, stream);

                    ProcessStartInfo _proccess = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        FileName = _sd.FileName
                    };
                    Process.Start(_proccess);

                }
            }
            catch (Exception ex)
            {

                logger.LogError(ex.Message);
            }
        }

        private  void updateAddTransportEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            var _viewmodel = (StockTransferViewModel)eventArgs.Parameter;

            if (_viewmodel != null && _viewmodel.GetStockTransferList != null)
            {
                var stockTransferModelForEWayModel = new StockTransferModelForEWayModel()
                {
                    SRNumber = GetStockTransferList.SRNumber,
                    DocumentNo = DocumentNo,
                    vehicleNo = VehicleNo,
                    VehicleType = SelectedVehicleType,
                    DocumentDate = DocumentDate,
                    TransferId = GetStockTransferList.TransferId,
                    FromLocation = _viewmodel.GetStockTransferList.FromLocation,
                    ToLocation = _viewmodel.GetStockTransferList.ToLocation
                };
                stockTransferModelForEWayModel.StockTransferProducts = new ObservableCollection<StockTransferProducts>();
                foreach (var c in _viewmodel.GetStockTransferList.stockTransferProducts)
                {
                    var sp = new StockTransferProducts()
                    {
                        SellingPrice = c.SellingPrice,
                        TransferQty = c.TransferQty,
                        Gst = c.Gst,
                        HsnCode = c.HsnCode,
                        ProductName = c.ProductName,
                        Description=c.Description,            
                        
                        
                    };
                    stockTransferModelForEWayModel.StockTransferProducts.Add(sp);
                }

                GenerateEwayBillCommon(stockTransferModelForEWayModel);
            }

         
            
        }

        public async void GenerateEwayBillCommon(StockTransferModelForEWayModel _viewmodel)
        {
            try
            {

                    List<EWaybillProduct> eWaybillProducts = new List<EWaybillProduct>();

                    double totalGST = 0;

                    foreach (var item in _viewmodel.StockTransferProducts)
                    {
                        float taxable = (((item.SellingPrice) / (100 + item.Gst)) * 100) * item.TransferQty;
                        var GSTamount = ((item.SellingPrice * item.TransferQty) - taxable);
                        eWaybillProducts.Add(new EWaybillProduct()
                        {
                            product_name = item.ProductName,
                            product_description = item.Description,
                            hsn_code = item.HsnCode,
                            quantity = item.TransferQty,
                            taxable_amount = taxable,
                            total_item_value = (item.SellingPrice * item.TransferQty),
                            cgst_rate = (double)item.Gst / 2,
                            sgst_rate = (double)item.Gst / 2,
                            igst_rate = (double)item.Gst

                        });
                        totalGST += (double)GSTamount;
                    }


                EWayBillModel eWayBillModel = new EWayBillModel()
                {
                        //access_token = "6971df2b43e2aa38764a76dd4ebed6ac1908a61d",
                        access_token = AppConstants.EwayBillAccesstoken,
                   

                        supply_type = "Outward",
                        sub_supply_type = "For Own Use",
                        document_type = "Delivery Challan",
                        document_number = _viewmodel.SRNumber + "QAS",
                        document_date = DateTime.Now.Date.ToString(),
                        legal_name_of_consignor = _viewmodel.FromLocation,
                         address1_of_consignor = "",
                        legal_name_of_consignee = _viewmodel.ToLocation,
                        transaction_type = 1,
                        other_value = 0,
                        //taxable_amount = (float)eWaybillProducts.Sum(x => x.taxable_amount),
                        //total_invoice_value = (float)eWaybillProducts.Sum(x => x.total_item_value),
                        //cgst_amount = totalGST / 2,
                        //sgst_amount = totalGST / 2,
                        //igst_amount = totalGST / 2,
                        taxable_amount = (double)Math.Round(eWaybillProducts.Sum(x => x.taxable_amount), 3),
                        total_invoice_value = (float)Math.Round(eWaybillProducts.Sum(x => x.total_item_value), 3),
                        cgst_amount = (double)Math.Round(totalGST / 2, 3),
                        sgst_amount = (double)Math.Round(totalGST / 2, 3),
                        igst_amount = (double)Math.Round(totalGST / 2, 3),
                        cess_amount = 0,
                        cess_nonadvol_value = 0,
                        transporter_id ="",
                        transporter_name = _viewmodel.DocumentName==null?Name:_viewmodel.DocumentName,
                        transportation_mode = "Road",
                        transportation_distance = 0,
                        transporter_document_number = _viewmodel.DocumentNo==null? (!string.IsNullOrEmpty(DocumentNo) ? DocumentNo : ""):_viewmodel.DocumentNo,
                        transporter_document_date = _viewmodel.DocumentDate == null ? DocumentDate:_viewmodel.DocumentDate,
                        vehicle_number = _viewmodel.vehicleNo == null?( VehicleNo != null ? VehicleNo.ToUpper() : null): _viewmodel.vehicleNo,
                        vehicle_type = _viewmodel.VehicleType == null ? SelectedVehicleType: _viewmodel.VehicleType,
                        generate_status = 1,
                        data_source = "erp",
                        auto_print = "Y",
                        email = "",
                        itemList = eWaybillProducts,


                    };

                    await _ProgressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {
                        var _result = await stockTransferService.GenerateEwayBill(eWayBillModel, _viewmodel.TransferId,_viewmodel.IsTransportAdded);
                        if(_result!=null && _result.isSuccess)
                        {
                            if (_result != null && _result.results != null && _result.results.status == "Success")
                            {
                                ProcessStartInfo _p = new ProcessStartInfo
                                {
                                    UseShellExecute = true,
                                    FileName = "http://" + _result.results.message.url,
                                };

                                Process.Start(_p);
                                GetStockTransferList.IsWayBillGenerated = true;
                                GetStockTransferList.EwayBillUrl = _result.results.message.url;
                                ResetTransportData();

                            }
                            else
                            {
                                notificationService.ShowMessage("An error occurred while generating EWay bill from the government portal. Please generate directly from the portal.", NotificationType.Error);
                               // GetStockTransferList.IsWayBillGenerated = false;
                                RefreshLoadData();
                            }

                        }
                        
                        else
                        {
                           // notificationService.ShowMessage(_result.Error, NotificationType.Error);
                           notificationService.ShowMessage("An error occurred while generating EWay bill from the government portal. Please generate directly from the portal.", NotificationType.Error);
                            //GetStockTransferList.IsWayBillGenerated = false;
                            RefreshLoadData();
                           
                        }
                    });

                    await _ProgressService.StopProgressAsync();

            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            finally
            {
                await _ProgressService.StopProgressAsync();
            }
        }

       public bool isValidVehicleNumberPlate(String NUMBERPLATE)
        {

            string regex = "^[A-Za-z]{2}[0-9]{2}[A-Za-z]{1,2}[0-9]{4}$";

            // Compile the ReGex
            Regex p = new Regex(regex);
          
            if (NUMBERPLATE == null)
            {
                return false;
            }
            
            Match m = p.Match(NUMBERPLATE);
            return m.Success;
        }
        public void ResetTransportData()
        {
            try
            {
                Name = null;
                DocumentNo = null;
                DocumentDate = null;
                VehicleNo = null;
                SelectedVehicleType = null;
            }
            catch(Exception _ex) { 
                logger.LogError(_ex.Message);
            }
        }

        public void DownloadEwayBill(object obj)
        {
            try
            {
                var _viewModel = (TransferViewModel)obj;
                if (_viewModel != null)
                {
                    if (_viewModel.EwayBillUrl != null)
                    {
                        ProcessStartInfo _p = new ProcessStartInfo
                        {
                            UseShellExecute = true,
                            FileName = "http://" +_viewModel.EwayBillUrl,
                        };

                        Process.Start(_p);
                    }
                    else
                        notificationService.ShowMessage("EWay bill not found", NotificationType.Error);
                    return;
                }
            }
            catch(Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
        }

        private string _streceiptNumber;
        public string STReceiptNumber
        {
            get { return _streceiptNumber; }
            set { SetProperty(ref _streceiptNumber, value); }
        }





        private string _srNumber;
        public string SRNumber
        {
            get { return _srNumber; }
            set { SetProperty(ref _srNumber, value); }
        }
        public string Name { get; set; }

        public int Distance {  get; set; }

        public string DocumentNo { get; set; }

        public string DocumentDate { get; set; }

        public string VehicleNo { get; set; }

        private ObservableCollection<string> _vehicleType;
        public ObservableCollection<string> VehicleType
        {
            get { return _vehicleType; }
            set { SetProperty(ref _vehicleType, value); }
        }
        private string _selectedVehicleType;
        public string SelectedVehicleType
        {
            get { return _selectedVehicleType; }
            set
            {
                SetProperty(ref _selectedVehicleType, value);                
            }
        }        
        private ObservableCollection<string> GetVehicleTypes()
        {
            return new ObservableCollection<string>
            {
                 "Regular","Over Dimentional Cargo"
            };
        }
        private string _remarks;

        public string Remarks
        {
            get { return _remarks; }
            set { SetProperty(ref _remarks , value); }
        }


        public void RejectConfirmPopup(object obj) 
        {
            try
            {
                if (string.IsNullOrEmpty(Remarks))
                {
                    notificationService.ShowMessage("Please enter Reason", NotificationType.Error);
                    return;
                }
                var TargetClose = ((Button)(obj));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);

            }
            catch(Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
        }
        public async void RejectStockTransfer(object obj)
        {
            try
            {
                TransferPageRejectConfirmPopup rejectConfirmPopup = new TransferPageRejectConfirmPopup();
                rejectConfirmPopup.DataContext = this;
                Remarks = null;
                await DialogHost.Show(rejectConfirmPopup, "RootDialog", RejectupdateEventHandler);
            }
            catch(Exception  ex)
            {
                logger.LogError(ex.Message);
            }
        }

        private async void RejectupdateEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var viewModel = (StockTransferViewModel)eventArgs.Parameter;
                if (viewModel != null)
                {

                    await _ProgressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {

                        var _result = await stockTransferService.UpdateStockTransferRejectV2(GetStockTransferList.TransferId, Remarks);

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            if (_result != null && _result.IsSuccess)
                            {
                                notificationService.ShowMessage(_result.Data, NotificationType.Success);
                                RefreshLoadData();
                                
                            }
                            else
                            {
                                notificationService.ShowMessage(_result.Error, NotificationType.Error);
                            }



                        });


                    });
                }
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            finally
            {
                await _ProgressService.StopProgressAsync();
            }

        }

    }





}
