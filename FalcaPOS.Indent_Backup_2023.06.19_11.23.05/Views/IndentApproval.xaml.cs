using FalcaPOS.Common.Events;
using Prism.Events;
using System.Windows.Controls;

namespace FalcaPOS.Indent.Views
{
    /// <summary>
    /// Interaction logic for IndentApproval.xaml
    /// </summary>
    public partial class IndentApproval : UserControl
    {
        public IndentApproval(IEventAggregator eventAggregator)
        {
            InitializeComponent();


            _ = eventAggregator.GetEvent<StepSelectedIndexChangeEvent>().Subscribe((x) =>
            {
                Dispatcher?.Invoke(() =>
                {
                    this.m_stepper.IsLinear = !this.m_stepper.IsLinear;
                    this.m_stepper.SelectedIndex = x;
                    this.m_stepper.IsLinear = !this.m_stepper.IsLinear;
                });

            });

            //double userheightscreen = System.Windows.SystemParameters.PrimaryScreenHeight;
            //this.MaxHeight = userheightscreen ;



        }


    }
}
