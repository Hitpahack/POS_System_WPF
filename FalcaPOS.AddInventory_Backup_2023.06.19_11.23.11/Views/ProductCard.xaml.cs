using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FalcaPOS.AddInventory.Views
{
    /// <summary>
    /// Interaction logic for ProductCard.xaml
    /// </summary>
    public partial class ProductCard : UserControl
    {
        public ProductCard()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty AddProductCardCommandProperty = DependencyProperty.Register("AddProductCardCommand", typeof(ICommand), typeof(ProductCard));
        public static readonly DependencyProperty RemoveProductCardCommandProperty = DependencyProperty.Register("RemoveProductCardCommand", typeof(ICommand), typeof(ProductCard));
        public static readonly DependencyProperty AddDefectiveQtyCommandProperty = DependencyProperty.Register("AddDefectiveQtyCommand", typeof(ICommand), typeof(ProductCard));



        public ICommand AddProductCardCommand
        {
            get { return (ICommand)GetValue(AddProductCardCommandProperty); }
            set { SetValue(AddProductCardCommandProperty, value); }
        }

        public ICommand RemoveProductCardCommand
        {
            get { return (ICommand)GetValue(RemoveProductCardCommandProperty); }
            set { SetValue(RemoveProductCardCommandProperty, value); }
        }

        public ICommand AddDefectiveQtyCommand
        {
            get { return (ICommand)GetValue(AddDefectiveQtyCommandProperty); }
            set { SetValue(AddDefectiveQtyCommandProperty, value); }
        }

    }
}
