using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Indent;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace FalcaPOS.Indent.ViewModels
{
    public class IndentCreateViewModel : BindableBase
    {
        private readonly IUnityContainer _container;

        public DelegateCommand AddIndentProductCardCommand { get; private set; }

        public DelegateCommand<object> RemoveIndentProductCardCommand { get; private set; }

        public DelegateCommand CreateIndentCommand { get; private set; }

        public DelegateCommand ClearIndentCommand { get; private set; }

        private readonly IIndentService _indentService;

        private readonly INotificationService _notificationService;

        private readonly ProgressService _progressService;

        private IEventAggregator _eventAggregator;

        private readonly Logger _logger;

        public IndentCreateViewModel(IUnityContainer container, Logger Logger, IIndentService indentService, INotificationService notificationService, ProgressService progressService, IEventAggregator eventAggregator)
        {

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _container = container ?? throw new ArgumentNullException(nameof(container));

            IndentProducts = new ObservableCollection<IndentProductCardViewModel>();

            IndentProducts.CollectionChanged -= IndentProducts_CollectionChanged;

            IndentProducts.CollectionChanged += IndentProducts_CollectionChanged;

            AddIndentProductCardCommand = new DelegateCommand(AddNewIndentProduct);

            RemoveIndentProductCardCommand = new DelegateCommand<object>(RemoveIndentProduct);

            CreateIndentCommand = new DelegateCommand(CreateIndent);

            ClearIndentCommand = new DelegateCommand(ClearIndent);

            _indentService = indentService ?? throw new ArgumentNullException();

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            GetCurrentPONumberAsync();

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            Type = "Retail Order";

            double userheightscreen = System.Windows.SystemParameters.PrimaryScreenHeight;

            MaxHeight = userheightscreen - 150;


            _logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            SlNo = 1;

        }

        private async void GetCurrentPONumberAsync()
        {

            try
            {
                PONumber = null;

                await Task.Run(async () =>
                {

                    var _result = await _indentService.GetCurrentPONumber();

                    Application.Current.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess && _result.Data.IsValidString())
                        {
                            PONumber = _result.Data;
                        }
                    });

                });
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

        }


        private string _type;

        public string Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }


        private string _poNumber;
        public string PONumber
        {
            get { return _poNumber; }
            set { SetProperty(ref _poNumber, value); }
        }

        public string PODate
        {
            get { return DateTime.Now.ToString("dd-MM-yyyy"); }
            set { }

        }

        private void ClearIndent()
        {
            IndentProducts?.Clear();
            Type = null;

            GetCurrentPONumberAsync();

            SlNo = 1;

        }

        private async void CreateIndent()
        {

            try
            {
                bool _isValidIndent = ValidateIndent();

                if (!_isValidIndent) return;


                var _indentList = GetIndentModel();

                if (Type == null)
                {
                    _notificationService.ShowMessage("Please select PO type", Common.NotificationType.Error);
                    return;

                }

                if (_indentList == null || _indentList.Products == null || !_indentList.Products.Any())
                {
                    return;
                }

                await _progressService.StartProgressAsync();



                await Task.Run(async () =>
                {

                    var _result = await _indentService.CreateIndent(_indentList);

                    Application.Current?.Dispatcher?.Invoke(async () =>
                     {

                         if (_result != null && _result.IsSuccess)
                         {
                             _notificationService.ShowMessage(_result?.Data ?? "Indent created successfully.!!!", Common.NotificationType.Success);
                             ClearIndent();
                             Type = "Retail Order";


                         }
                         else
                         {
                             _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                         }

                         await _progressService.StopProgressAsync();
                     });

                });
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

        }

        private IndentCreateModel GetIndentModel()
        {
            return new IndentCreateModel
            {
                PoNumber = PONumber,
                Type = Type,
                Products = GetIndentProducts()
            };


        }

        private IEnumerable<IndentProduct> GetIndentProducts()
        {
            foreach (var _product in IndentProducts)
            {
                if (_product == null || _product.SelectedProduct == null) continue;

                yield return new IndentProduct
                {
                    ProductId = (int)_product.SelectedProduct.ProductId,
                    Quantity = _product.Quantity
                };
            }

        }

        private bool ValidateIndent()
        {


            //Required atleast one product
            if (IndentProducts == null || IndentProducts.Count <= 0)
            {
                _notificationService.ShowMessage("Please add one or more products", Common.NotificationType.Error);

                return false;
            }

            foreach (var _product in IndentProducts)
            {
                bool _isValidProduct = ValidateIndentProduct(_product);

                if (!_isValidProduct)
                {
                    return false;
                }

            }

            //check if duplicates are present 

            var _result = IndentProducts.GroupBy(x => x.SelectedProduct.ProductId).Select(x => x).Where(x => x.Count() > 1);

            if (_result != null && _result.Count() > 0)
            {
                _notificationService.ShowMessage($"Duplicate product {_result?.FirstOrDefault()?.FirstOrDefault()?.SelectedProduct?.Name} added, remove the product or change the  product quantity", Common.NotificationType.Error);

                return false;

            }

            return true;
        }

        private bool ValidateIndentProduct(IndentProductCardViewModel product)
        {
            if (product == null)
            {
                _notificationService.ShowMessage("Invalid Indent product", Common.NotificationType.Error);

                return false;
            }

            if (product.SelectedProduct == null)
            {
                _notificationService.ShowMessage("Please select a product", Common.NotificationType.Error);

                product.HasError = true;

                return false;

            }
            if (product.Quantity <= 0)
            {
                _notificationService.ShowMessage("Please enter valid  product quantity", Common.NotificationType.Error);

                product.HasError = true;

                return false;

            }


            return true;
        }

        private void RemoveIndentProduct(object obj)
        {
            if (obj is Guid _productGUIDId)
            {
                var _productRemove = IndentProducts?.FirstOrDefault(x => x.IndentProductGUIDId == _productGUIDId);

                if (_productRemove != null)
                {
                    IndentProducts.Remove(_productRemove);
                    SlNo--;
                }
            }
        }

        private void AddNewIndentProduct()
        {

            var _product = _container.Resolve<IndentProductCardViewModel>();
            _product.SlNo = SlNo;
            IndentProducts.Add(_product);
            SlNo++;
        }

        private void IndentProducts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("IndentProducts");
        }

        private ObservableCollection<IndentProductCardViewModel> _indentProducts;
        public ObservableCollection<IndentProductCardViewModel> IndentProducts
        {
            get { return _indentProducts; }
            set { SetProperty(ref _indentProducts, value); }
        }

        private double _maxheight;

        public double MaxHeight
        {
            get { return _maxheight; }
            set
            {
                _maxheight = value;
                RaisePropertyChanged("MaxHeight");
            }
        }

        private int _slno;

        public int SlNo
        {
            get { return _slno; }
            set { SetProperty(ref _slno, value); }
        }

    }
}
