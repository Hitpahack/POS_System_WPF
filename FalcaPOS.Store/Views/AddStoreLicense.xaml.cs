using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FalcaPOS.Store.Views
{
    /// <summary>
    /// Interaction logic for AddStoreLicense.xaml
    /// </summary>
    public partial class AddStoreLicense : UserControl
    {
        public AddStoreLicense()
        {
            InitializeComponent();
        }
        public ICommand AddStoreLicenseCommand
        {
            get { return (ICommand)GetValue(AddStoreLicenseCommandProperty); }
            set { SetValue(AddStoreLicenseCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddStoreLicenseCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddStoreLicenseCommandProperty =
            DependencyProperty.Register("AddStoreLicenseCommand", typeof(ICommand), typeof(AddStoreLicense));



        public ICommand RemoveAddStoreLicenseCommand
        {
            get { return (ICommand)GetValue(RemoveAddStoreLicenseCommandProperty); }
            set { SetValue(RemoveAddStoreLicenseCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RemoveAddStoreLicenseCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemoveAddStoreLicenseCommandProperty =
            DependencyProperty.Register("RemoveAddStoreLicenseCommand", typeof(ICommand), typeof(AddStoreLicense));




        public int CategoryId
        {
            get { return (int)GetValue(CategoryIdProperty); }
            set { SetValue(CategoryIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CategoryId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CategoryIdProperty =
            DependencyProperty.Register("CategoryId", typeof(int), typeof(AddStoreLicense), new PropertyMetadata(0));

    }
}
