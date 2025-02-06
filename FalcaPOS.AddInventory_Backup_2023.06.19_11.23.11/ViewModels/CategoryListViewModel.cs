using FalcaPOS.Common.Events;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.ProductTypes;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class CategoryListViewModel : BindableBase
    {

        private readonly IProductTypeService productTypeService;

        private readonly IEventAggregator _eventAggregator;

        private readonly Logger _logger;
        public CategoryListViewModel(IEventAggregator eventAggregator, IProductTypeService ProductTypeService, Logger Logger)
        {
            productTypeService = ProductTypeService ?? throw new ArgumentNullException(nameof(ProductTypeService));

            _logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _eventAggregator.GetEvent<AddCategoryEvent>().Subscribe(x => LoadCategories());


            LoadCategories();


        }
        private async void LoadCategories()
        {
            try
            {

                await Task.Run(async () =>
                {
                    var _result = await productTypeService.GetAllCategory();
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

    }
}
