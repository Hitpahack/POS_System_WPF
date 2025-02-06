using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Common.Helper
{
    public class ProgressService
    {
        private readonly MetroWindow parent;

        private ProgressDialogController controller;

        private CustomDialog customDialog;

        public ProgressService()
        {
            parent = Application.Current.MainWindow as MetroWindow;
            //customDialog = new CustomDialog(parent?.MetroDialogOptions) { Content = parent?.Resources["CustomDialogTest"], Title = "" };
        }

        public async Task StartProgressAsync(string title = "Please wait...", string message = "")
        {
            

            if (controller == null || !controller.IsOpen)
            {
                controller = await parent?.ShowProgressAsync(title, message);
                controller?.SetIndeterminate();
            }

            //if (customDialog != null)
            //    await parent?.ShowMetroDialogAsync(customDialog);


        }


        //public async Task ShowCustomDialogAsync()
        //{
        //    if (customDialog != null)
        //        await parent?.ShowMetroDialogAsync(customDialog);
        //}

        //public async Task CloseCustomDialogAsync()
        //{
        //    if (customDialog != null)
        //        await parent?.HideMetroDialogAsync(customDialog);
        //}


        public void UpdateProgressMessage(string msg)
        {
            if (controller != null && controller.IsOpen)
            {
                controller.SetMessage(msg);
            }
        }

        public async Task StopProgressAsync()
        {
            try
            {
                if (controller != null && controller.IsOpen)
                {
                    await controller?.CloseAsync();
                }

                //if (customDialog != null && parent != null)
                //    await parent?.HideMetroDialogAsync(customDialog);
            }
            catch (System.Exception)
            {
            }
        }

        /// <summary>
        /// Confirm dialog with "OK", and "Cancel"
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<MessageDialogResult> ConfirmAsync(string title, string message)
        {
            var _result = await parent.ShowMessageAsync(title, message, MessageDialogStyle.AffirmativeAndNegative);

            return _result;
        }

        /// <summary>
        /// Confirm dialog with "OK"
        /// </summary>
        /// <param name="titile"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<int> ConfirmOkAsync(string titile, string message)
        {
            return (int)await parent.ShowMessageAsync(titile, message, MessageDialogStyle.Affirmative);

        }
    }
}
