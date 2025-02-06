using FalcaPOS.Common.Events;
using Prism.Events;
using System.Windows.Controls;

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

            _ = eventAggregator.GetEvent<StepSelectedPopIndexChangeEvent>().Subscribe((x) =>
            {
                Dispatcher?.Invoke(() =>
                {
                    this.m_stepper.IsLinear = !this.m_stepper.IsLinear;
                    this.m_stepper.SelectedIndex = x;
                    this.m_stepper.IsLinear = !this.m_stepper.IsLinear;
                });

            });
        }

    }
}
