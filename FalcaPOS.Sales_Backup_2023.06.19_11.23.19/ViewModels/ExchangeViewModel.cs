using FalcaPOS.Common;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Sales;
using FalcaPOS.Sales.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FalcaPOS.Sales.ViewModels
{
    public class ExchangeViewModel : BindableBase
    {
        private readonly ISalesService _salesService;
        public DelegateCommand GetProductDetails { get; private set; }



        public DelegateCommand<object> GetProductCommand { get; private set; }
        public DelegateCommand<object> ReturnQty { get; private set; }


        public DelegateCommand<object> RefreshInvoiceCommand { get; private set; }

        public DelegateCommand SaveSalesDetatilsCommand { get; private set; }

        public DelegateCommand ResetSalesCommand { get; private set; }
        public DelegateCommand<object> RemoveProductCommand { get; private set; }

        public DelegateCommand<object> AddReturnProduct { get; private set; }

        private readonly INotificationService _notificationService;
        private string _salesInvoiceNumber;
        public string SalesInvoiceNumber
        {
            get { return _salesInvoiceNumber; }
            set { SetProperty(ref _salesInvoiceNumber, value); }
        }

        private string _productCode;

        public string ProductCode
        {
            get { return _productCode; }
            set { SetProperty(ref _productCode, value); }
        }


        private bool isgetReturnProduct;

        public bool IsGetReturnProduct
        {
            get { return isgetReturnProduct; }
            set { SetProperty(ref isgetReturnProduct, value); }
        }


        private SalesInvoice _getExchangeProduct = new SalesInvoice();
        public SalesInvoice GetExchangeProduct
        {
            get { return _getExchangeProduct; }
            set { SetProperty(ref _getExchangeProduct, value); }
        }


        private string _remarks;

        public string Remarks
        {
            get { return _remarks; }
            set { SetProperty(ref _remarks, value); }
        }
        private float _returnAmount;

        public float ReturnAmount
        {
            get { return _returnAmount; }
            set { SetProperty(ref _returnAmount, value); }
        }

        private float _cash;

        public float Cash
        {
            get { return _cash; }
            set { SetProperty(ref _cash, value); }
        }
        private float _card;

        public float Card
        {
            get { return _card; }
            set { SetProperty(ref _card, value); }
        }
        private float _upi;

        public float Upi
        {
            get { return _upi; }
            set { SetProperty(ref _upi, value); }
        }

        private ObservableCollection<SalesProduct> _salesProducts = new ObservableCollection<SalesProduct>();
        public ObservableCollection<SalesProduct> SalesProducts
        {
            get { return _salesProducts; }
            set
            {
                SetProperty(ref _salesProducts, value);
            }
        }


        public ExchangeViewModel(ISalesService salesService, INotificationService notificationService)
        {
            _salesService = salesService ?? throw new ArgumentNullException(nameof(salesService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            GetProductDetails = new DelegateCommand(LoadSalesProduct);

            ReturnQty = new DelegateCommand<object>(ChechSoldQty);

            RefreshInvoiceCommand = new DelegateCommand<object>(RefreshInvoice);
            SaveSalesDetatilsCommand = new DelegateCommand(SaveSalesDetatils);
            ResetSalesCommand = new DelegateCommand(ResetSales);
            RemoveProductCommand = new DelegateCommand<object>(RemoveProduct);
            GetProductCommand = new DelegateCommand<object>(GetProduct);
            AddReturnProduct = new DelegateCommand<object>(GetProductDirect);



        }
        public void GetProductDirect(object obj)
        {
            string productcode = (obj as SalesProduct).BarCode;
            if (!String.IsNullOrEmpty(productcode))
            {
                if (SalesProducts != null && SalesProducts.Count > 0)
                {

                    var productmatching = SalesProducts.Where(x => x.BarCode == productcode).FirstOrDefault();
                    if (productmatching != null)
                    {
                        if ((bool)!productmatching.isreturn)
                        {


                            if (!productmatching.IsReturnProduct)
                            {
                                if (productmatching.IsGroupTrackMode)
                                {
                                    SalesProducts.Where(x => x.BarCode == productcode).FirstOrDefault().IsReturnProduct = true;
                                    SalesProducts.Where(x => x.BarCode == productcode).FirstOrDefault().IsChecked = true;
                                    ReturnAmountCalculation();
                                }
                                else
                                {
                                    SalesProducts.Where(x => x.BarCode == productcode).FirstOrDefault().IsReturnProduct = true;
                                    SalesProducts.Where(x => x.BarCode == productcode).FirstOrDefault().IsChecked = true;
                                    ReturnAmountCalculation();
                                }


                            }
                            else
                            {
                                if (productmatching.IsGroupTrackMode)
                                {
                                    SalesProducts.Where(x => x.BarCode == productcode).FirstOrDefault().IsReturnProduct = false;
                                    SalesProducts.Where(x => x.BarCode == productcode).FirstOrDefault().IsChecked = false;
                                    ReturnAmountCalculation();

                                }
                                else
                                {

                                    SalesProducts.Where(x => x.BarCode == productcode).FirstOrDefault().IsReturnProduct = false;
                                    SalesProducts.Where(x => x.BarCode == productcode).FirstOrDefault().IsChecked = false;
                                    ReturnAmountCalculation();

                                }
                                return;
                            }
                        }
                        else
                        {
                            _notificationService.ShowMessage("Product is already Returned", NotificationType.Error);
                            return;
                        }

                    }
                    else
                    {
                        _notificationService.ShowMessage("Product is not available in this invoice", NotificationType.Error);
                        return;
                    }


                    ProductCode = null;
                }
            }

        }
        public void GetProduct(object obj)
        {
            string productcode = (obj as TextBox).Text;
            if (!String.IsNullOrEmpty(productcode))
            {
                if (SalesProducts != null && SalesProducts.Count > 0)
                {

                    var productmatching = SalesProducts.Where(x => x.BarCode == productcode).FirstOrDefault();
                    if (productmatching != null)
                    {
                        if ((bool)!productmatching.isreturn)
                        {


                            if (!productmatching.IsReturnProduct)
                            {
                                if (productmatching.IsGroupTrackMode)
                                {
                                    SalesProducts.Where(x => x.BarCode == productcode).FirstOrDefault().IsReturnProduct = true;
                                    SalesProducts.Where(x => x.BarCode == productcode).FirstOrDefault().IsChecked = true;
                                    ReturnAmountCalculation();

                                }
                                else
                                {

                                    SalesProducts.Where(x => x.BarCode == productcode).FirstOrDefault().IsReturnProduct = true;
                                    SalesProducts.Where(x => x.BarCode == productcode).FirstOrDefault().IsChecked = true;
                                    ReturnAmountCalculation();

                                }


                            }
                            else
                            {
                                _notificationService.ShowMessage("Product is already added", NotificationType.Error);
                                return;
                            }
                        }
                        else
                        {
                            _notificationService.ShowMessage("Product is already Returned", NotificationType.Error);
                            return;
                        }

                    }
                    else
                    {
                        _notificationService.ShowMessage("Product is not available in this invoice", NotificationType.Error);
                        return;
                    }


                    ProductCode = null;
                }
            }


        }

        private void RemoveProduct(object obj)
        {
            var _product = obj as SalesProduct;

            if (_product == null) return;

            SalesProducts.Remove(_product);
            ReturnAmountCalculation();
            if (SalesProducts.Count == 0)
            {
                GetExchangeProduct = null;
                ProductCode = null;
                SalesInvoiceNumber = null;
                IsGetReturnProduct = false;
            }




        }

        public void ResetSales()
        {

        }
        public async void SaveSalesDetatils()
        {
            SalesInvoice salesInvoice = new SalesInvoice();
            int indexx = 0;
            foreach (var item in SalesProducts)
            {
                if ((bool)item.isreturn)
                {
                    _notificationService.ShowMessage($"Product Already Returned Row {indexx + 1}", NotificationType.Error);

                    return;
                }

                indexx++;
            }
            int index = 0;
            foreach (var item in SalesProducts)
            {
                if (!item.IsReturnProduct)
                {
                    _notificationService.ShowMessage($"Please Add Return Product Row  {index + 1}", NotificationType.Error);

                    return;
                }

                index++;
            }
            foreach (var item in SalesProducts)
            {
                if (item.IsReturnProduct)
                {

                    if (item.ReturnDefectiveQty == 0 && item.ReturnQty == 0)
                    {
                        _notificationService.ShowMessage("Please enter return qty or return defective qty", NotificationType.Error);

                        return;
                    }



                }
            }
            if ((Cash + Card + Upi) == 0)
            {
                _notificationService.ShowMessage("Enter valid amount for UPI/Cash/Card", NotificationType.Error);
                return;
            }

            if (ReturnAmount != (Cash + Card + Upi))
            {
                _notificationService.ShowMessage("Return amount should match the entered amount", NotificationType.Error);
                return;
            }


            if (GetExchangeProduct != null && SalesProducts.Count > 0)
            {
                salesInvoice.InvoiceNumber = GetExchangeProduct.InvoiceNumber;
                salesInvoice.InvoiceDate = GetExchangeProduct.InvoiceDate;
                salesInvoice.Remarks = Remarks;
                salesInvoice.Cash = Cash;
                salesInvoice.Card = Card;
                salesInvoice.UPI = Upi;
                salesInvoice.Total = ReturnAmount;
                salesInvoice.SalesProducts = SalesProducts.Where(x => x.IsReturnProduct).ToList();
            }
            else
            {
                _notificationService.ShowMessage("Please Add Return Product", NotificationType.Error);
                return;
            }

            await Task.Run(async () =>
            {
                var _return = await _salesService.FarmerReturnProduct(salesInvoice);
                if (_return != null && _return.IsSuccess)
                {
                    _notificationService.ShowMessage("Product Returned Successfully", NotificationType.Success);
                    GetExchangeProduct = null;
                    SalesProducts = null;
                    ReturnAmount = 0;
                    SalesInvoiceNumber = null;
                    IsGetReturnProduct = false;
                    return;

                }
                else
                {
                    _notificationService.ShowMessage("Product Return Faild", NotificationType.Error);

                    return;
                }

            });
        }

        public void RefreshInvoice(object obj)
        {
            GetExchangeProduct = null;
            SalesInvoiceNumber = null;
            SalesProducts = null;
            IsGetReturnProduct = false;
            ReturnAmount = 0;
        }

        public void ChechSoldQty(object obj)
        {
            var returnproduct = (obj as SalesProduct);
            if (returnproduct != null)
            {
                if (returnproduct.IsGroupTrackMode)
                {
                    if (returnproduct.ReturnQty > 0 || returnproduct.ReturnDefectiveQty > 0)
                    {
                        int returnboth = returnproduct.ReturnDefectiveQty + returnproduct.ReturnQty;
                        if (returnboth > returnproduct.SellingQty)
                        {
                            _notificationService.ShowMessage("Should not allow more than Selling quantity", NotificationType.Error);
                            returnproduct.ReturnQty = 0;
                            returnproduct.ReturnDefectiveQty = 0;
                            return;
                        }

                    }
                }
                else
                {
                    if (returnproduct.ReturnQty > 0 || returnproduct.ReturnDefectiveQty > 0)
                    {
                        if (returnproduct.ReturnDefectiveQty > 0 && returnproduct.ReturnQty > 0)
                        {
                            _notificationService.ShowMessage("can not return both defective qty and qty", NotificationType.Error);
                            returnproduct.ReturnQty = 0;
                            returnproduct.ReturnDefectiveQty = 0;
                            return;
                        }
                        int returnqty = Convert.ToInt32(returnproduct.ReturnQty);
                        int returndefectiveqty = returnproduct.ReturnDefectiveQty;
                        var produtdetails = SalesProducts.Where(x => x.ProductName == returnproduct.ProductName && x.Manufacturer == returnproduct.Manufacturer && x.ProductType == x.ProductType).FirstOrDefault();
                        if (returnqty > produtdetails.SellingQty || returndefectiveqty > returnproduct.SellingQty)
                        {
                            _notificationService.ShowMessage("Should not allow more than Selling quantity", NotificationType.Error);
                            returnproduct.ReturnQty = 0;
                            returnproduct.ReturnDefectiveQty = 0;
                            return;
                        }



                    }
                }




            }
            ReturnAmountCalculation();

        }



        public async void LoadSalesProduct()
        {
            if (string.IsNullOrEmpty(SalesInvoiceNumber))
            {
                _notificationService.ShowMessage("InvocueNumber  is required", NotificationType.Error);

                return;
            };


            await Task.Run(async () =>
            {
                var _product = await _salesService.GetExchangeProduct(SalesInvoiceNumber);
                if (_product != null && _product.IsSuccess)
                {
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        GetExchangeProduct = _product.Data;
                        SalesProducts = new ObservableCollection<SalesProduct>(_product.Data.SalesProducts.Where(x => x.SellingQty != 0).ToList());
                        IsGetReturnProduct = true;
                        ReturnAmount = 0;
                        Remarks = null;
                        Card = 0;
                        Cash = 0;
                        Upi = 0;
                    });

                }
                else
                {
                    _notificationService.ShowMessage("Please check Invocie No", NotificationType.Error);
                    GetExchangeProduct = null;
                    SalesProducts = null;
                    IsGetReturnProduct = false;
                    return;
                }

            });
        }




        private SalesProductModel GetSalesModel(SalesProduct product)
        {

            if (product == null) return null;

            var _salesModel = new SalesProductModel(_notificationService);

            _salesModel.Discount = product.Discount;
            _salesModel.Manufacturer = product.Manufacturer;
            _salesModel.ProductType = product.ProductType;
            _salesModel.WarrentyDate = product.WarrentyDate;
            _salesModel.ProductName = product.ProductName;
            _salesModel.ProductSpecifications = product.ProductSpecifications;
            _salesModel.DiscountMode = product.DiscountMode;
            _salesModel.ProductSellingPrice = product.ProductSellingPrice;
            _salesModel.ProductTotal = product.ProductTotal;
            _salesModel.StockProductId = product.StockProductId;
            _salesModel.ProductGST = product.ProductGST;
            _salesModel.ProductDiscountPercent = product.ProductDiscountPercent;
            _salesModel.ProductDiscountFlat = product.ProductDiscountFlat;
            _salesModel.BarCode = product.BarCode;

            _salesModel.IsGroupTrackMode = product.IsGroupTrackMode;
            _salesModel.AvailableQuantity = product.AvailableQuantity;
            _salesModel.SellingQty = 1;

            return _salesModel;

        }


        public void ReturnAmountCalculation()
        {
            ReturnAmount = 0;
            if (SalesProducts != null && SalesProducts.Count > 0)
            {
                foreach (var sales in SalesProducts)
                {
                    if (sales.IsReturnProduct)
                    {
                        if (sales.IsGroupTrackMode)
                        {
                            var returnqty = sales.ReturnQty + sales.ReturnDefectiveQty;
                            var servicecharge = sales.ServiceChargeAmount * returnqty;
                            ReturnAmount = ReturnAmount + (sales.SoldSellingPrice * returnqty) + (float)servicecharge;
                        }
                        else
                        {
                            if (sales.ReturnDefectiveQty > 0 || sales.ReturnQty > 0)
                            {
                                ReturnAmount = ReturnAmount + sales.SoldSellingPrice + (float)sales.ServiceChargeAmount;
                            }
                        }


                    }
                }
            }

        }






    }


}
