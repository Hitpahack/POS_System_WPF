using FalcaPOS.Common.Events;
using FalcaPOS.Entites.Products;
using FalcaPOS.Entites.ProductTypes;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace FalcaPOS.Shell.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private string _license = "Copyright @ Falca e-Solutions Private Limited";
        private string _buildNumber;
        private Visibility _isShowWelcomeMessage;
        private int _NotificationCount;
        private IEventAggregator _eventAggregator;
        public DelegateCommand SignOutCommand { get; private set; }

        public DelegateCommand ShowReleaseNotesCommand { get; private set; }
        public DelegateCommand ShowGSTCalcCommand { get; private set; }

        public DelegateCommand ShowNotificationCommand { get; private set; }


        public string License
        {
            get { return _license; }
            set { SetProperty(ref _license, value); }
        }


        public string BuildNumber
        {
            get { return _buildNumber; }
            set { SetProperty(ref _buildNumber, value); }
        }

        public List<ProductDetails> AddedProducts = new List<ProductDetails>();

        public List<ProductType> ProductsType = new List<ProductType>();
        public ShellViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            SignOutCommand = new DelegateCommand(() => { ExecuteSignOutCommand(); });
            IsShowWelcomeMessage = Visibility.Collapsed;
            _eventAggregator.GetEvent<FalcaPOS.Common.Events.Login>().Subscribe((IsLoginedIn) =>
            {
                IsShowWelcomeMessage = IsLoginedIn ? Visibility.Visible : Visibility.Collapsed;
            });
            BuildNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ShowReleaseNotesCommand = new DelegateCommand(ShowReleaseNotes);
            //_eventAggregator.GetEvent<SignalRNewProductAddEvent>().Subscribe(ProductAdded);
            //_eventAggregator.GetEvent<SignalRNewBrandAddEvent>().Subscribe(BrnadAdded);
            //_eventAggregator.GetEvent<SignalRProductTypeAddedEvent>().Subscribe(ProductTypeAdded);
            ShowNotificationCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<ShowNotificationEvent>().Publish();
                NotificationCount = 0;
            });
            _eventAggregator.GetEvent<NotificationCountEvent>().Subscribe((count) => NotificationCount += 1);
            ShowGSTCalcCommand = new DelegateCommand(ShowGSTCalcCommandExecute);


        }



        private void ShowGSTCalcCommandExecute()
        {
            _eventAggregator.GetEvent<ShowGSTCalculator>().Publish(true);
        }

        private void ShowReleaseNotes()
        {
            _eventAggregator.GetEvent<ShowReleseNote>().Publish(true);
        }

        public Visibility IsShowWelcomeMessage
        {
            get { return _isShowWelcomeMessage; }
            set { SetProperty(ref _isShowWelcomeMessage, value); }
        }

        public int NotificationCount
        {
            get { return _NotificationCount; }
            set { SetProperty(ref _NotificationCount, value); }
        }

        public void ExecuteSignOutCommand()
        {
            //Exit app.
            Environment.Exit(0);
            //need to check 
            //_eventAggregator.GetEvent<FalcaPOS.Common.Events.Login>().Publish(false);

            //var _view = _container.Resolve<FalcaPOS.Login.Views.Login>();
            //IRegionManager regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
            //var _region = regionManager.Regions["LoginRegion"];
            //_region.Deactivate(_region.Views.LastOrDefault());
            //_region.Activate(_region.Views.FirstOrDefault());
        }

    }
}
