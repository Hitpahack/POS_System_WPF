using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Entites.Indent;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FalcaPOS.Indent.ViewModels
{
    public class IndentHomeViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        private bool _isViewIndentVisible;
        public bool IsViewIndentVisible
        {
            get { return _isViewIndentVisible; }
            set { SetProperty(ref _isViewIndentVisible, value); }
        }
        private bool _isCreateIndentVisible;
        public bool IsCreateIndentVisible
        {
            get { return _isCreateIndentVisible; }
            set { SetProperty(ref _isCreateIndentVisible, value); }
        }
        private bool _isApproveIndentVisible;

        public bool IsApproveIndentVisible
        {
            get { return _isApproveIndentVisible; }
            set { SetProperty(ref _isApproveIndentVisible, value); }
        }

        List<IntentTab> TabList = new List<IntentTab>();
        public IndentHomeViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _ = eventAggregator.GetEvent<LoginedRole>().Subscribe(IndentVisibility);
            _ = eventAggregator.GetEvent<IntentNewTabCreateEvent>().Subscribe(CreateNewTab);
            _ = eventAggregator.GetEvent<IntentNewTabRemoveEvent>().Subscribe(RemoveTab);


        }

        public void CreateNewTab(IndentViewModel indentViewModel)
        {
            SelectedIndex = 2;
            IsApproveIndentVisible = true;
        }

        public void RemoveTab()
        {
            IsApproveIndentVisible = false;
        }
        //public void LoadIntentTab()
        //{


        //    //list.Add(new IntentTab()
        //    //{
        //    //    TabItem ="Create Intent",
        //    //    ContentView=AppConstants.REGION_INDENT_CREATE

        //    //});
        //    TabList.Add(new IntentTab()
        //    {
        //        TabItem = "View Intent",
        //        ContentView=AppConstants.REGION_INDENT_VIEW
        //    });
        //    Tab = new ObservableCollection<IntentTab>(TabList);

        //}
        private void IndentVisibility(string role)
        {
            switch (role)
            {
                
                case AppConstants.ROLE_SUPER_BACKEND:
                case AppConstants.ROLE_BACKEND:
                case AppConstants.ROLE_ADMIN:
                case AppConstants.ROLE_DIRECTOR:
                    IsViewIndentVisible = true;
                    IsApproveIndentVisible = false;
                    IsCreateIndentVisible = false;
                    IsBulkDownloadIndentVisible = false;
                    SelectedIndex = 1;
                    break;
                case AppConstants.ROLE_TERRITORY_MANAGER:
                case AppConstants.ROLE_REGIONAL_MANAGER:
                case AppConstants.ROLE_PURCHASE_MANAGER:
                case AppConstants.ROLE_STORE_PERSON:
                    IsViewIndentVisible = true;
                    IsCreateIndentVisible = true;
                    IsBulkDownloadIndentVisible = false;
                    SelectedIndex = 0;
                    break;
                case AppConstants.ROLE_FINANCE:
                    IsViewIndentVisible = true;
                    IsApproveIndentVisible = false;
                    IsCreateIndentVisible = false;
                    IsBulkDownloadIndentVisible = true;
                    SelectedIndex = 1;
                    break;

                default:
                    break;
            }

        }

        private ObservableCollection<IntentTab> _tab;
        public ObservableCollection<IntentTab> Tab
        {
            get { return _tab; }
            set { SetProperty(ref _tab, value); }
        }

        private int _slectedIndex;

        public int SelectedIndex
        {
            get { return _slectedIndex; }
            set { SetProperty(ref _slectedIndex, value); }
        }

        private bool _isBulkDownloadIndentVisible;
        public bool IsBulkDownloadIndentVisible
        {
            get { return _isBulkDownloadIndentVisible; }
            set { SetProperty(ref _isBulkDownloadIndentVisible, value); }
        }
    }

    public class IntentTab
    {
        public string TabItem { get; set; }

        public string ContentView { get; set; }

    }
}
