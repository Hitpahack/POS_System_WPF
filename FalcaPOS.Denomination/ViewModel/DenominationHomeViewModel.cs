using Prism.Commands;
using Prism.Mvvm;

namespace FalcaPOS.Denomination.ViewModel
{
    /// <summary>
    /// This class handles the logics of the controls present in DenominationHomeView.
    /// </summary>
    public class DenominationHomeViewModel: BindableBase
    {
        #region Fields
        private bool _isEODCashAddPage;
        private bool _isCashViewPage;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DenominationHomeViewModel"/> class with the specified parameters
        /// </summary>
        public DenominationHomeViewModel()
        {
            BackToAddCashCommand = new DelegateCommand<object>(AddCashViewRequest);

            ViewDenominationsAndDepositsCommand = new DelegateCommand<object>(ViewDenominationsAndDepositsRequest);
            IsEODCashAddPage = true;
            IsCashViewPage = false;
        }

        /// <summary>
        /// Displays the Denominations and Deposits page in the EOD Menu.
        /// </summary>
        /// <param name="obj">Object.</param>
        private void ViewDenominationsAndDepositsRequest(object obj)
        {
            IsEODCashAddPage = false;
            IsCashViewPage = true;
        }

        /// <summary>
        /// Displays the EOD Cash Declaration Page in EOD Menu.
        /// </summary>
        /// <param name="obj">Object.</param>
        private void AddCashViewRequest(object obj)
        {
            IsEODCashAddPage = true;
            IsCashViewPage = false;
        }

        #region Properties

        /// <summary>
        /// Gets and Sets the value of the boolean to show the EOD Cash Addition page.
        /// </summary>
        public bool IsEODCashAddPage
        {
            get => _isEODCashAddPage;
            set => SetProperty(ref _isEODCashAddPage, value);
        }

        /// <summary>
        /// Gets and Sets the value of the boolean to show the Cash View page.
        /// </summary>
        public bool IsCashViewPage
        {
            get => _isCashViewPage;
            set => SetProperty(ref _isCashViewPage, value);
        }

        /// <summary>
        /// Delegate command to handle the button click operation of View button.
        /// </summary>
        public DelegateCommand<object> ViewDenominationsAndDepositsCommand { get; private set; }

        /// <summary>
        /// Delegate command to handle the button click operation of Back button.
        /// </summary>
        public DelegateCommand<object> BackToAddCashCommand { get; private set; }
        #endregion
    }
}
