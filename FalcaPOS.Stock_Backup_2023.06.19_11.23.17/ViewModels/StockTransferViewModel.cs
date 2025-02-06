using FalcaPOS.Common;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
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
using System.IO;
using System.Linq;
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

        private readonly IEventAggregator eventAggregator;

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



        public DelegateCommand GetResetCommand { get; private set; }

        public DelegateCommand DownloadStockTranfer { get; private set; }

        public DelegateCommand<object> FetchTransferProductCommand { get; private set; }

        TransferViewModel StockTrnasferModel = new TransferViewModel();
        public StockTransferViewModel(ISalesService salesService, EventAggregator EventAggregator,
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

            eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));

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

            GetResetCommand = new DelegateCommand(ResetData);



            FetchTransferProductCommand = new DelegateCommand<object>(FetchTransferProduct);

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
        public void ResetData()
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
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            finally {
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
                                        });
                                    }
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

                                });
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
                                    notificationService.ShowMessage("Ann error occred while showing  pdf", NotificationType.Error);

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

    }





}
