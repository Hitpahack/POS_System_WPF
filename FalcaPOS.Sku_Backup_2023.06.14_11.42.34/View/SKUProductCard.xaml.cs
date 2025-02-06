using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FalcaPOS.Sku.View
{
    /// <summary>
    /// Interaction logic for SKUProductCard.xaml
    /// </summary>
    public partial class SKUProductCard : UserControl
    {
        public SKUProductCard()
        {
            InitializeComponent();
        }

        public ICommand AddSKUProductCardCommand
        {
            get { return (ICommand)GetValue(AddSKUProductCardCommandProperty); }
            set { SetValue(AddSKUProductCardCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddIndentProductCardCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddSKUProductCardCommandProperty =
            DependencyProperty.Register("AddSKUProductCardCommand", typeof(ICommand), typeof(SKUProductCard));



        public ICommand RemoveSKUProductCardCommand
        {
            get { return (ICommand)GetValue(RemoveSKUProductCardCommandProperty); }
            set { SetValue(RemoveSKUProductCardCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RemoveIndentProductCardCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemoveSKUProductCardCommandProperty =
            DependencyProperty.Register("RemoveSKUProductCardCommand", typeof(ICommand), typeof(SKUProductCard));

    }
}
