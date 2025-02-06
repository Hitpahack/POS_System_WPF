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
    /// Interaction logic for EditStoreLicense.xaml
    /// </summary>
    public partial class EditStoreLicense : UserControl
    {
        public EditStoreLicense()
        {
            InitializeComponent();
        }
        public ICommand EditStoreLicenseCommand {
            get { return (ICommand)GetValue(EditStoreLicenseCommandProperty); }
            set { SetValue(EditStoreLicenseCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditStoreLicenseCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditStoreLicenseCommandProperty =
            DependencyProperty.Register("EditStoreLicenseCommand", typeof(ICommand), typeof(EditStoreLicense));



        public ICommand RemoveEditStoreLicenseCommand
        {
            get { return (ICommand)GetValue(RemoveEditStoreLicenseCommandProperty); }
            set { SetValue(RemoveEditStoreLicenseCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RemoveEditStoreLicenseCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemoveEditStoreLicenseCommandProperty =
            DependencyProperty.Register("RemoveEditStoreLicenseCommand", typeof(ICommand), typeof(EditStoreLicense));




        public int CategoryId
        {
            get { return (int)GetValue(CategoryIdProperty); }
            set { SetValue(CategoryIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CategoryId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CategoryIdProperty =
            DependencyProperty.Register("CategoryId", typeof(int), typeof(EditStoreLicense), new PropertyMetadata(0));

    }
}
