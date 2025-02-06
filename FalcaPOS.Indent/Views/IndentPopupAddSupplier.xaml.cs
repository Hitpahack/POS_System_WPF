using FalcaPOS.Common.Events;
using FalcaPOS.Indent.ViewModels;
using Prism.Events;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace FalcaPOS.Indent.Views
{
    /// <summary>
    /// Interaction logic for IndentPopupAddSupplier.xaml
    /// </summary>
    public partial class IndentPopupAddSupplier : UserControl
    {
        public IndentPopupAddSupplier(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            _ = eventAggregator.GetEvent<StepSelectedPopIndexChangeEvent>().Subscribe((x) => {
                Dispatcher?.Invoke(() => {
                    
                    this.stepProgressBar.SelectedIndex = x;
                   
                });

            });

        }


    }
}
