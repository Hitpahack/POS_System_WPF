using System.Windows.Controls;

namespace FalcaPOS.Dashboard.Views
{
    /// <summary>
    /// Interaction logic for DoughnutChartView.xaml
    /// </summary>
    public partial class DoughnutChartView : UserControl
    {
        public DoughnutChartView()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.Equals("ProductName") || e.Column.Header.Equals("ProductCount"))
            {
                e.Cancel = true;
            }
        }

        private void DataGrid_AutoGeneratingColumn_1(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.Equals("BrandName") || e.Column.Header.Equals("BrandCount"))
            {
                e.Cancel = true;
            }
        }
    }
}
