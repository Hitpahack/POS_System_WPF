using System.Windows.Controls;

namespace FalcaPOS.Reports.View
{
    /// <summary>
    /// Interaction logic for InventoryReport.xaml
    /// </summary>
    public partial class Report : UserControl {
        public Report() {
            InitializeComponent();
        }
        private void RadGridView_AutoGeneratingColumn(object sender, Telerik.Windows.Controls.GridViewAutoGeneratingColumnEventArgs e)
        {

        }
    }
}
