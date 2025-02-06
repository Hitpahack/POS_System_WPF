using FalcaPOS.Common.Events;
using FalcaPOS.Entites.Sales;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace FalcaPOS.Sales.ViewModels
{
    public class OrderedProductspopupViewModel : BindableBase
    {

        private readonly IEventAggregator eventAggregator;
        public OrderedProductspopupViewModel(IEventAggregator EventAggregator)
        {
            eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _ = eventAggregator.GetEvent<AppOrderMoveToSalePageEvent>().Subscribe(LoadData);
        }

        public void LoadData(AppOrderModel appOrderModel)
        {

            OrderProduct = appOrderModel.Products;
        }

        private List<Product> orderProduct;

        public List<Product> OrderProduct
        {
            get { return orderProduct; }
            set { SetProperty(ref orderProduct, value); }
        }
    }
}
