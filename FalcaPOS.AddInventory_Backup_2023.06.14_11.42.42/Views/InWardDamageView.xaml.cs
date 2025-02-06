using System.Windows.Controls;

namespace FalcaPOS.AddInventory.Views
{
    /// <summary>
    /// Interaction logic for InWardDamageView.xaml
    /// </summary>
    public partial class InWardDamageView : UserControl
    {
        public InWardDamageView()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.Equals("IsSellingPriceUpdated") || e.Column.Header.Equals("StoreId") || e.Column.Header.Equals("InvoiceId") || e.Column.Header.Equals("CreatedDate") || e.Column.Header.Equals("IsSelectedShallow") || e.Column.Header.Equals("InvoiceDate") || e.Column.Header.Equals("IsSelected") || e.Column.Header.Equals("IsBusy") || e.Column.Header.Equals("HasInformation") || e.Column.Header.Equals("DefectiveProductList"))
            {
                e.Cancel = true;
            }
        }



        private void DataGrid_AutoGeneratingColumn_1(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.Equals("DefectiveGroup")
                || e.Column.Header.Equals("InventoryTrackMode")
                || e.Column.Header.Equals("IsGroupTrackMode")
                || e.Column.Header.Equals("Misc")
                || e.Column.Header.Equals("AttributesSelectedList")
                || e.Column.Header.Equals("ProductMRP")
                || e.Column.Header.Equals("ProductGST")
                || e.Column.Header.Equals("ProductUniqGuid")
                || e.Column.Header.Equals("StoreName")
                || e.Column.Header.Equals("StroreId")
                || e.Column.Header.Equals("SerialNumbers")
                || e.Column.Header.Equals("IsSerialNumberManual")
                || e.Column.Header.Equals("ProductDiscountMode")
                || e.Column.Header.Equals("ProductTotal")
                || e.Column.Header.Equals("ProductDiscount")
                || e.Column.Header.Equals("SalesDefectiveQty")
                || e.Column.Header.Equals("DateOfExpiry")
                || e.Column.Header.Equals("DateOfManufacture")
                || e.Column.Header.Equals("Quantity")
                || e.Column.Header.Equals("Status")
                || e.Column.Header.Equals("DefectiveQuantity")
                || e.Column.Header.Equals("HSNCode")
                || e.Column.Header.Equals("ProductId")
                || e.Column.Header.Equals("ProductTypeId")
                || e.Column.Header.Equals("ManufacturerId")
                || e.Column.Header.Equals("InvoiceNo")
                || e.Column.Header.Equals("SalesInvoice")
                || e.Column.Header.Equals("ProductTypeName"))
            {
                e.Cancel = true;
            }
        }
    }
}
