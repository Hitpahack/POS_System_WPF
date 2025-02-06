using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Common.Validations;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.ProductTypes;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class AddProducttypeViewModel : ValidationBase
    {
        private readonly IProductTypeService _productTypeService;

        private readonly IEventAggregator _eventAggregator;

        private readonly INotificationService _notificationService;

        private readonly Logger _logger;

        public DelegateCommand AddProductTypeCommand { get; private set; }

        public AddProducttypeViewModel(IProductTypeService productTypeService,
            IEventAggregator eventAggregator,
            INotificationService notificationService,
              Logger logger)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _productTypeService = productTypeService ?? throw new ArgumentNullException(nameof(productTypeService));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            AddProductTypeCommand = new DelegateCommand(AddProductType).ObservesCanExecute(() => IsValid);

            PopUpOpendCommand = new DelegateCommand(PopUpOpened);

            LoadCategories();
        }


        private string _currentDeptCode;
        public string CurrentDeptCode
        {
            get { return _currentDeptCode; }
            set { SetProperty(ref _currentDeptCode, value); }
        }

        private  void PopUpOpened()
        {

            try
            {
                //not using method commanded
                //var _result = await _productTypeService.GetCurrentDeptCode();

                //if (_result != null && _result.IsSuccess)
                //{
                //    CurrentDeptCode = _result.Data;
                //}

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in popup open method ", _ex);
            }

        }

        public DelegateCommand PopUpOpendCommand { get; private set; }


        private async void AddProductType()
        {

            if (!SelectCategory.CategoryName.IsValidString())
            {
                _notificationService.ShowMessage("Category is required.", NotificationType.Error);
                return;
            }

            if (!ProductTypeName.IsValidString())
            {
                _notificationService.ShowMessage("Sub Category is required.", NotificationType.Error);
                return;
            }
           
            //if (!DeptCode.IsValidString())
            //{
            //    _notificationService.ShowMessage("DepartName code is required.", NotificationType.Error);
            //    return;
            //}

            //if (DeptCode.Length != 3)
            //{
            //    _notificationService.ShowMessage("Department should be 3 characters in length.", NotificationType.Error);
            //    return;
            //}

            //if (!ShortCode.IsValidString())
            //{
            //    _notificationService.ShowMessage("Product type short code is required.", NotificationType.Error);
            //    return;
            //}

            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _productTypeService.AddSubCategory(SelectCategory.Id, ProductTypeName);

                    if (_result != null && _result.IsSuccess)
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            _eventAggregator.GetEvent<AddProductTypeEvent>().Publish(_result.Data);

                            _notificationService.ShowMessage($"Sub Category {ProductTypeName} added.", NotificationType.Success);

                            ProductTypeName = null;// string.Empty;
                                                   //HSNCode = null;

                            //CurrentDeptCode = null;

                            PopupClose = false;

                            //DeptCode = null;

                            //ShortCode = null;//string.Empty;
                            SelectCategory = null;
                            ClearErrors();
                        });


                    }
                    else {
                        _notificationService.ShowMessage(_result.Error, NotificationType.Error);

                    }
                });
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in  add product type", _ex);
            }
        }

        //private string _shortCode;
        //[Required(ErrorMessage = "Short code is required")]
        //[StringLength(3, MinimumLength = 3, ErrorMessage = "Short code should be 3 character.")]
        //public string ShortCode
        //{
        //    get { return _shortCode; }
        //    set
        //    {
        //        SetProperty(ref _shortCode, value?.ToUpper());
        //        if (value != null)
        //            ValidateProperty(value);
        //    }
        //}




        private string _productTypeName;

        [Required(ErrorMessage = "Sub Category is required")]
        public string ProductTypeName
        {
            get { return _productTypeName; }
            set
            {
                SetProperty(ref _productTypeName, value);
                if (value != null)
                    ValidateProperty(value);
            }
        }

        //private string _deptCode;

        //[Required(ErrorMessage = "Departname code is required")]
        //[MaxLength(199, ErrorMessage = "Allowed max length is 200 characters only")]
        //public string DeptCode
        //{
        //    get { return _deptCode; }
        //    set
        //    {
        //        SetProperty(ref _deptCode, value);
        //        if (value != null)
        //            ValidateProperty(value);
        //    }
        //}

        //private int? _hsnCode;
        //public int? HSNCode
        //{
        //    get { return _hsnCode; }
        //    set { SetProperty(ref _hsnCode, value); }
        //}


        private bool _popupClose;

        public bool PopupClose
        {
            get { return _popupClose; }
            set { SetProperty(ref _popupClose, value); }
        }

        private async void LoadCategories()
        {
            try
            {

                await Task.Run(async () =>
                {
                    var _result = await _productTypeService.GetAllCategory();
                    CategoryList = new ObservableCollection<CategoryModel>();
                    if (_result.IsSuccess && _result?.Data != null)
                        CategoryList = new ObservableCollection<CategoryModel>(_result.Data.ToList());
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError("error in load Category", ex);
            }
        }

        private ObservableCollection<CategoryModel> _categoryList = new ObservableCollection<CategoryModel>();
        public ObservableCollection<CategoryModel> CategoryList
        {
            get { return _categoryList; }
            set { SetProperty(ref _categoryList, value); }
        }

        private CategoryModel _selectCategory;
        public CategoryModel SelectCategory
        {
            get => _selectCategory;
            set => SetProperty(ref _selectCategory, value);
        }

       

    }
}
