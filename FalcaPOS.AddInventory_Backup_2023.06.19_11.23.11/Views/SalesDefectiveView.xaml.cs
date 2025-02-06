using System.Windows.Controls;

namespace FalcaPOS.AddInventory.Views
{
    /// <summary>
    /// Interaction logic for SalesDefectiveView.xaml
    /// </summary>
    public partial class SalesDefectiveView : UserControl
    {
        public SalesDefectiveView()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.Equals("ProductTypeId") || e.Column.Header.Equals("ManufacturerId") || e.Column.Header.Equals("ProductId") || e.Column.Header.Equals("DateOfManufacture")
                || e.Column.Header.Equals("DateOfExpiry") || e.Column.Header.Equals("StroreId") || e.Column.Header.Equals("SerialNumbers") || e.Column.Header.Equals("IsSerialNumberManual")
                || e.Column.Header.Equals("AttributesSelectedList") || e.Column.Header.Equals("InventoryTrackMode") || e.Column.Header.Equals("IsGroupTrackMode")
                || e.Column.Header.Equals("DefectiveQuantity") || e.Column.Header.Equals("ProductDiscountMode") || e.Column.Header.Equals("ProductUniqGuid") || e.Column.Header.Equals("StoreName") || e.Column.Header.Equals("ProductMRP")
                || e.Column.Header.Equals("ProductGST") || e.Column.Header.Equals("DefectiveGroup") || e.Column.Header.Equals("Misc") || e.Column.Header.Equals("SubDefectiveQty") || e.Column.Header.Equals("HSNCode") || e.Column.Header.Equals("Quantity") || e.Column.Header.Equals("ProductDiscount") ||
                e.Column.Header.Equals("ProductTotal") || e.Column.Header.Equals("ProductSubQty") || e.Column.Header.Equals("Status") || e.Column.Header.Equals("Location"))
            {
                e.Cancel = true;
            }
        }
    }
}
