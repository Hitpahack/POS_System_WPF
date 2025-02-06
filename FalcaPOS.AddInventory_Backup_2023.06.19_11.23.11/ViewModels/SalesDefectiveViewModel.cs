using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class SalesDefectiveViewModel : BindableBase
    {

        private readonly IInvoiceService _invoiceService;

        private CancellationTokenSource _cancellationTokenSource { get; set; }

        public DelegateCommand RefreshDataGrid { get; set; }


        public SalesDefectiveViewModel(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
            _cancellationTokenSource = new CancellationTokenSource();
            RefreshDataGrid = new DelegateCommand(RefresGrid);
            LoadDefectiveData();
        }


        public void RefresGrid()
        {
            LoadDefectiveData();
        }

        public async void LoadDefectiveData()
        {

            if (_cancellationTokenSource != null)
                _cancellationTokenSource.Cancel();

            _cancellationTokenSource = new CancellationTokenSource();



            await Task.Run(async () =>
            {
                var _result = await _invoiceService.GetSalesDefectiveLsit();

                if (_result != null && _result.IsSuccess)
                {
                    SalesDefectiveList = _result.Data.ToList();
                }
                else
                {
                    SalesDefectiveList = null;
                }



            });



        }


        private List<StockProductViewModel> _salesdefectiveList;
        public List<StockProductViewModel> SalesDefectiveList
        {
            get { return _salesdefectiveList; }

            set { SetProperty(ref _salesdefectiveList, value); }
        }
    }
}
