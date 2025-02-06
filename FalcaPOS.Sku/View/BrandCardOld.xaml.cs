using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FalcaPOS.Sku.View
{
    /// <summary>
    /// Interaction logic for BrandCard.xaml
    /// </summary>
    public partial class BrandCardOld : UserControl
    {
        public BrandCardOld()
        {
            InitializeComponent();
        }

        public ICommand AddBrandCardCommand
        {
            get { return (ICommand)GetValue(AddBrandCardCommandProperty); }
            set { SetValue(AddBrandCardCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddProductCardCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddBrandCardCommandProperty =
            DependencyProperty.Register("AddBrandCardCommand", typeof(ICommand), typeof(BrandCard));



        public ICommand RemoveBrandCardCommand
        {
            get { return (ICommand)GetValue(RemoveBrandCardCommandProperty); }
            set { SetValue(RemoveBrandCardCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RemoveProductCardCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemoveBrandCardCommandProperty =
            DependencyProperty.Register("RemoveBrandCardCommand", typeof(ICommand), typeof(BrandCard));

        public ICommand AddFileAttachmentCommand
        {
            get { return (ICommand)GetValue(AddFileAttachmentCommandProperty); }
            set { SetValue(AddFileAttachmentCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddProductCardCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddFileAttachmentCommandProperty =
            DependencyProperty.Register("AddFileAttachmentCommand", typeof(ICommand), typeof(BrandCard));


        public ICommand FileUploadListInfo
        {
            get { return (ICommand)GetValue(FileUploadListInfoPros); }
            set { SetValue(FileUploadListInfoPros, value); }
        }

        // Using a DependencyProperty as the backing store for AddProductCardCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileUploadListInfoPros =
            DependencyProperty.Register("FileUploadListInfo", typeof(ICommand), typeof(BrandCard));


        public ICommand ViewFileAttachmentCommand
        {
            get { return (ICommand)GetValue(ViewFileAttachmentCommandProperty); }
            set { SetValue(ViewFileAttachmentCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddProductCardCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewFileAttachmentCommandProperty =
            DependencyProperty.Register("ViewFileAttachmentCommand", typeof(ICommand), typeof(BrandCard));

        public ICommand DeleteUploadFileCommand
        {
            get { return (ICommand)GetValue(DeleteFileAttachmentCommandProperty); }
            set { SetValue(DeleteFileAttachmentCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddProductCardCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteFileAttachmentCommandProperty =
            DependencyProperty.Register("DeleteUploadFileCommand", typeof(ICommand), typeof(BrandCard));


    }
}
