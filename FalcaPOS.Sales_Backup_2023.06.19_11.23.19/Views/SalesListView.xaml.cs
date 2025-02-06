using System.Windows.Controls;

namespace FalcaPOS.Sales.Views
{
    /// <summary>
    /// Interaction logic for SalesListView
    /// </summary>
    public partial class SalesListView : UserControl
    {
        public SalesListView()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "ProductSpecifications")
            {
                e.Cancel = true;

            }
            if (e.PropertyName == "GST")
            {
                e.Column.Header = "GST(%)";
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
