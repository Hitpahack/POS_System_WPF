using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FalcaPOS.Indent.Views
{
    /// <summary>
    /// Interaction logic for IndentProductCard.xaml
    /// </summary>
    public partial class IndentProductCard : UserControl
    {
        public IndentProductCard()
        {
            InitializeComponent();
        }



        public ICommand AddIndentProductCardCommand
        {
            get { return (ICommand)GetValue(AddIndentProductCardCommandProperty); }
            set { SetValue(AddIndentProductCardCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddIndentProductCardCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddIndentProductCardCommandProperty =
            DependencyProperty.Register("AddIndentProductCardCommand", typeof(ICommand), typeof(IndentProductCard));



        public ICommand RemoveIndentProductCardCommand
        {
            get { return (ICommand)GetValue(RemoveIndentProductCardCommandProperty); }
            set { SetValue(RemoveIndentProductCardCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RemoveIndentProductCardCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemoveIndentProductCardCommandProperty =
            DependencyProperty.Register("RemoveIndentProductCardCommand", typeof(ICommand), typeof(IndentProductCard));




        public int StoreId
        {
            get { return (int)GetValue(StoreIdProperty); }
            set { SetValue(StoreIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StoreId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StoreIdProperty =
            DependencyProperty.Register("StoreId", typeof(int), typeof(IndentProductCard), new PropertyMetadata(0));




    }
}
