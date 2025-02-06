using Prism.Mvvm;

namespace FalcaPOS.Denomination.ViewModel
{
    /// <summary>
    /// DenominationDepositDetailsViewModel class.
    /// </summary>
    public class DenominationDepositDetailsViewModel : BindableBase
    {
        #region Fields
        /// <summary>
        /// Stores the boolean value of Denomination Radio button.
        /// </summary>
        private bool _isDenominationSelected;
       

        /// <summary>
        /// Stores the boolean value of Deposit Radio button.
        /// </summary>
        private bool _isDepositSelected;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DenominationDepositDetailsViewModel"/> class with the specified parameters
        /// </summary>
        public DenominationDepositDetailsViewModel()
        {
            IsDenominationSelected = true;
        }

        #region Properties

        /// <summary>
        /// Gets and sets the boolean value of the state of Denomination Radio button.
        /// </summary>
        public bool IsDenominationSelected
        {
            get { return _isDenominationSelected; }
            set { SetProperty(ref _isDenominationSelected, value); }
        }

        /// <summary>
        /// Gets and sets the boolean value of the state of Deposit Radio button.
        /// </summary>
        public bool IsDepositSelected
        {
            get { return _isDepositSelected; }
            set { SetProperty(ref _isDepositSelected, value); }
        }
        #endregion
    }
}
