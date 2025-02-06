using System.Windows.Controls;

namespace FalcaPOS.ExpiryProducts.View
{
    /// <summary>
    /// Interaction logic for ExpiredProduct.xaml
    /// </summary>
    public partial class ExpiredProduct : UserControl
    {
        public ExpiredProduct()
        {
            InitializeComponent();
        }

        private void expiredProductsRadGridView_DataLoading(object sender, Telerik.Windows.Controls.GridView.GridViewDataLoadingEventArgs e)
        {

        }
    }
}
