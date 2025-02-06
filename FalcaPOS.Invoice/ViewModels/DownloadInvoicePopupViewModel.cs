using Prism.Mvvm;

namespace FalcaPOS.Invoice.ViewModels
{
    public class DownloadInvoicePopupViewModel : BindableBase
    {
        public DownloadInvoicePopupViewModel()
        {

        }




        private bool _popupClose;

        public bool PopupClose
        {
            get { return _popupClose; }
            set { SetProperty(ref _popupClose, value); }
        }
    }
}
