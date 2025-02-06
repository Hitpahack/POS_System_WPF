using System.Windows.Controls;

namespace FalcaPOS.Dashboard.Views
{
    /// <summary>
    /// Interaction logic for BusinessView.xaml
    /// </summary>
    public partial class BusinessView : UserControl
    {
        public BusinessView()
        {
            InitializeComponent();

        }

        private void business_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.Equals("BusinessModelResponseIndetails") || e.Column.Header.Equals("BusinessModelProductIndetails") || e.Column.Header.Equals("Store") || e.Column.Header.Equals("IsBusy") || e.Column.Header.Equals("IsSelected") || e.Column.Header.Equals("IsSelectedShallow") || e.Column.Header.Equals("HasInformation"))

            {
                e.Cancel = true;
            }
            if (!string.IsNullOrEmpty(this.fromdate.Text) || !string.IsNullOrEmpty(this.todate.Text) && !string.IsNullOrEmpty(this.cmbstorename.SelectedItem.ToString()))
            {
                if (e.Column.Header.Equals("MonthBusiness") || e.Column.Header.Equals("Average"))
                {
                    e.Cancel = true;
                }
            }
            else
            {
                if (e.Column.Header.Equals("MonthBusiness") || e.Column.Header.Equals("Average"))
                {
                    e.Cancel = false;
                }
            }

        }


        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.Equals("CustomerName") || e.Column.Header.Equals("Phone") || e.Column.Header.Equals("InvoiceNo") || e.Column.Header.Equals("Cash") || e.Column.Header.Equals("Card") || e.Column.Header.Equals("UPI") || e.Column.Header.Equals("Total") || e.Column.Header.Equals("TotalServiceCharge"))

            {
                e.Cancel = true;
            }


        }
    }
}
