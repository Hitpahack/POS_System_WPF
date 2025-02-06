using FalcaPOS.Common.Events;
using FalcaPOS.Common.Logger;
using FalcaPOS.Entites.Suppliers;
using Prism.Events;
using Prism.Mvvm;
using System;

namespace FalcaPOS.Suppliers.ViewModels
{
    public class SupplierHomeViewModel : BindableBase
    {

        private IEventAggregator eventAggregator;

        private readonly Logger logger;


        public SupplierHomeViewModel(IEventAggregator EventAggregator, Logger Logger)
        {
            eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));
            _ = eventAggregator.GetEvent<SupplierNewTabCreateEvent>().Subscribe(CreateNewTab);
            _ = eventAggregator.GetEvent<SupplierTabRemoveEvent>().Subscribe(RemoveTab);
            logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

        }


        public void CreateNewTab(SuppliersDetailsViewModel suppliers)
        {
            try
            {
                SelectedIndex = 1;
                IsSupplierDetailsVisible = true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        public void RemoveTab()
        {
            try
            {

                IsSupplierDetailsVisible = false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        private int slectedIndex;

        public int SelectedIndex
        {
            get { return slectedIndex; }
            set { SetProperty(ref slectedIndex, value); }
        }

        private bool issupplierdetailsVisible;

        public bool IsSupplierDetailsVisible
        {
            get { return issupplierdetailsVisible; }
            set { SetProperty(ref issupplierdetailsVisible, value); }
        }
    }
}
