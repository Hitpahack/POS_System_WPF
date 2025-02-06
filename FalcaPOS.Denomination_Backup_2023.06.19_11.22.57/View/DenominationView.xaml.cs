using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FalcaPOS.Denomination.View
{
    /// <summary>
    /// Interaction logic for DenominationView.xaml
    /// </summary>
    public partial class DenominationView : UserControl
    {
        public DenominationView()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty DenominationModelProperty = DependencyProperty.Register("DenominationModel", typeof(ICommand), typeof(DenominationView));


        public ICommand DenominationModel
        {
            get { return (ICommand)GetValue(DenominationModelProperty); }
            set { SetValue(DenominationModelProperty, value); }
        }
    }
}
