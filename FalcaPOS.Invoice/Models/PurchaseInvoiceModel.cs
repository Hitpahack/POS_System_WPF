using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Common;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FalcaPOS.Invoice.Models
{
    public class PurchaseInvoiceModel : BindableBase
    {

        public PurchaseInvoiceModel()
        {
            TempFiles = new ObservableCollection<FileUploadInfo>();

            TempFiles.CollectionChanged += TempFiles_CollectionChanged;
        }

        private void TempFiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(TempFiles));
        }

        public string InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        // public int SupplierId { get; set; }

        public string SupplierName { get; set; }

        //public int Quantity { get; set; }

        //public int DefectiveQuantity { get; set; }

        public float GrossTotal { get; set; }

        // public float InvoiceDiscountPerecent { get; set; }

        //public float InvoiceDiscountFlat { get; set; }

        //public float InvoiceDiscount { get; set; }

        // public string InvoiceDiscountMode { get; set; }

        // public float InvoiceRoundOff { get; set; }

        //public float InvoiceOthers { get; set; }

        // public float InvoiceNetTotal { get; set; }

        // public int StoreID { get; set; }

        public string StoreName { get; set; }

        //public bool IsQADone { get; set; }

        public DateTime CreatedDate { get; set; }

        private List<InvoiceProductViewModel> _StockProducts;
        public List<InvoiceProductViewModel> StockProducts
        {
            get => _StockProducts;
            set => SetProperty(ref _StockProducts, value);
        }

        //public float TotalGST { get; set; }

        //public float TransportCharges { get; set; }

        public int InvoiceId { get; set; }

        public bool IsDcNumber { get; set; }

        public string DcNumber { get; set; }

        public DateTime? DcNumberDate { get; set; }

        //public List<FileAttachment> FileAttachments { get; set; }

        private ObservableCollection<FileAttachment> _fileAttachments;

        public ObservableCollection<FileAttachment> FileAttachments
        {
            get => _fileAttachments;
            set => _ = SetProperty(ref _fileAttachments, value);
        }


        private ObservableCollection<FileUploadInfo> _tempFiles;
        public ObservableCollection<FileUploadInfo> TempFiles
        {
            get => _tempFiles;
            set => _ = SetProperty(ref _tempFiles, value);
        }

        public bool IsEditButton { get; set; }

        public bool IsFileEditable { get; set; }

        public string InvoiceDays { get; set; }

        private bool _isDownloadInvocie;
        public bool IsDownloadInvoice { get { return _isDownloadInvocie; } set { SetProperty(ref _isDownloadInvocie, value); } }
    }
}
