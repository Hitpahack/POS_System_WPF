using System.Windows.Controls;

namespace FalcaPOS.Director.View
{
    /// <summary>
    /// Interaction logic for PurchaseRate.xaml
    /// </summary>
    public partial class PurchaseRate : UserControl
    {
        public PurchaseRate()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.Equals("InvoiceDate"))
            {
                e.Cancel = true;
            }
        }

        private void DataGrid_AutoGeneratingColumn(object sender, Telerik.Windows.Controls.GridViewAutoGeneratingColumnEventArgs e)
        {

        }
    }
}
