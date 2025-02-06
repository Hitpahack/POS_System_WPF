using System.Windows.Controls;

namespace FalcaPOS.Stock.Views
{
    /// <summary>
    /// Interaction logic for Stock.xaml
    /// </summary>
    public partial class Stock : UserControl
    {

        public Stock()
        {
            InitializeComponent();
        }

        //private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        //{
        //    if (e.Column.Header.Equals( "Name") || e.Column.Header.Equals("Value")||e.Column.Header.Equals("stockRowDetails")||e.Column.Header.Equals("productid"))
        //    {     
        //       e.Cancel = true;

        //    }
        //}

        //private async void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    var row = (DataGridRow)sender;

        //        row.DetailsVisibility = row.DetailsVisibility == Visibility.Collapsed ?
        //            Visibility.Visible : Visibility.Collapsed;
        //        var Model = (StoreStockModelResponse)row.Item;
        //        if (Model.productid != 0)
        //        {
        //            var data = await _stockService.GetStockDetails(Model.productid);
        //            if (data == null)
        //            {

        //            List<StockRowDetail> rows = new List<StockRowDetail>();
        //            rows.Add(new StockRowDetail()
        //            {
        //               Name = "size",
        //               Value="120opx"
        //            });
        //            rows.Add(new StockRowDetail()
        //            {
        //                Name = "color",
        //                Value = "Red"
        //            });
        //            Model.stockRowDetails = rows;
        //        }


        //        }

        //}

    }
}
