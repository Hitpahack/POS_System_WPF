using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FalcaPOS.Stock.Views
{
    /// <summary>
    /// Interaction logic for StockProductCode.xaml
    /// </summary>
    public partial class StockProductCode : UserControl
    {
        public StockProductCode()
        {
            InitializeComponent();
        }


        public ICommand AddProductCardCommand
        {
            get { return (ICommand)GetValue(StockProductCardCommandProperty); }
            set { SetValue(StockProductCardCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddIndentProductCardCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StockProductCardCommandProperty =
            DependencyProperty.Register("AddProductCardCommand", typeof(ICommand), typeof(StockProductCode));



        public ICommand RemoveProductCardCommand
        {
            get { return (ICommand)GetValue(RemoveStockProductCardCommandProperty); }
            set { SetValue(RemoveStockProductCardCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RemoveIndentProductCardCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemoveStockProductCardCommandProperty =
            DependencyProperty.Register("RemoveProductCardCommand", typeof(ICommand), typeof(StockProductCode));


    }
}
