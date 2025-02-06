using FalcaPOS.Common.Constants;
using System.Windows;
using System.Windows.Controls;

namespace FalcaPOS.Denomination.View
{
    /// <summary>
    /// Interaction logic for DenominationHome.xaml
    /// </summary>
    public partial class DenominationHome : UserControl
    {
        public DenominationHome()
        {
            InitializeComponent();

            add.Visibility = storeview.Visibility = AppConstants.USER_ROLES[0] == AppConstants.ROLE_STORE_PERSON ? Visibility.Visible : Visibility.Collapsed;
            finaceview.Visibility = AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE ? Visibility.Visible : Visibility.Collapsed;
            finaceview.IsSelected = AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE ? true : false;
            depositview.Header = AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE ? "Deposit Approve/View" : "Deposit View";
        }
    }
}
