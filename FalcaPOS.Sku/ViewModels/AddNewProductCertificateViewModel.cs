using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Sku;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace FalcaPOS.Sku.ViewModels
{
    public class AddNewProductCertificateViewModel : BindableBase
    {
        private readonly INotificationService _notificationService;
        private readonly ProgressService _ProgressService;
        private readonly ISkuService _skuService;
        private readonly Logger _logger;
        private readonly IEventAggregator _eventAggregator;

        private String _certificateno;
        private String _issuedate;
        private String _validupto;

        public String CertificateNo
        {
            get { return _certificateno; }
            set { SetProperty(ref _certificateno, value); }
        }
        public String IssueDate
        {
            get { return _issuedate; }
            set { SetProperty(ref _issuedate , value); }
        }

        public String ValidUpto
        {
            get { return _validupto; }
            set { SetProperty(ref _validupto, value); }
        }

        public String Brand
        {
            get { return _brand; }
            set { SetProperty(ref _brand, value); }
        }

        public int BrandId
        {
            get { return _brandId; }
            set { SetProperty(ref _brandId, value); }
        }

        public int SubcategoryId
        {
            get { return _subcategoryId; }
            set { SetProperty(ref _subcategoryId, value); }
        }

        private int _storeId;
        public int StoreId
        {
            get { return _storeId; }
            set { SetProperty(ref _storeId, value); }
        }


        private ObservableCollection<FileUploadInfo> fileUploadListInfo;
        private string _brand;
        private int _brandId;
        private int _subcategoryId;

        public ObservableCollection<FileUploadInfo> FileUploadListInfo
        {
            get { return fileUploadListInfo; }
            set { SetProperty(ref fileUploadListInfo, value); }
        }

        public DelegateCommand<object> AddFileAttachmentCommand { get; private set; }
        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> DeleteFileAttachmentCommand { get; private set; }
        public DelegateCommand<Object> RefreshCommand { get; private set; }
        public AddNewProductCertificateViewModel(INotificationService notificationService, Logger logger, ProgressService progressService, ISkuService skuService,IEventAggregator eventAggregator)
        {
            AddFileAttachmentCommand = new DelegateCommand<object>(AddFileAttachmentCommandEvent);
            DeleteFileAttachmentCommand = new DelegateCommand<object>(DeleteFileAttachmentCommandEvent);
            SaveCommand = new DelegateCommand<object>(SaveCommandEvent);
            RefreshCommand = new DelegateCommand<Object>(RefreshEvent);
            _notificationService = notificationService;
            _ProgressService = progressService;
            _skuService = skuService;
            _logger = logger;
            _eventAggregator = eventAggregator;
            
        }

        private void RefreshEvent(Object obj)
        {
            CertificateNo = "";
            IssueDate = null;
            ValidUpto = null;
            fileUploadListInfo.Clear();
        }

        private async void SaveCommandEvent(object obj)
        {
            try
            {
                if (string.IsNullOrEmpty(CertificateNo))
                {
                    _notificationService.ShowMessage("Please enter certificate no", NotificationType.Error);
                    return;
                }

                if (string.IsNullOrEmpty(Convert.ToString(IssueDate)))
                {
                    _notificationService.ShowMessage("Please enter date of issue", NotificationType.Error);
                    return;
                }

                if (string.IsNullOrEmpty(Convert.ToString(ValidUpto)))
                {
                    _notificationService.ShowMessage("Please enter valid upto date", NotificationType.Error);
                    return;
                }

                if (FileUploadListInfo == null || FileUploadListInfo.Count == 0)
                {
                    _notificationService.ShowMessage($"Please add an certificate", NotificationType.Error);
                    return;
                }


                UpdateSKVModel updateSKVModel = new UpdateSKVModel()
                {
                    IssueDate = IssueDate,
                    ValidUpto = ValidUpto,                    
                    Number = CertificateNo,

                };

                var TargetClose = ((Button)(obj));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);

                await _ProgressService.StartProgressAsync();
                var result = await _skuService.UpdateSKUSearch(StoreId,SubcategoryId,BrandId, updateSKVModel);
                if (result.IsSuccess)
                {
                    if (FileUploadListInfo != null && FileUploadListInfo.Count > 0 && FileUploadListInfo.Any(x => x.FileSrc == FileSrc.local))
                    {
                        List<string> resuestid = new List<string>();
                        resuestid.Add(result.Data);
                        var _updateresult = await _skuService.UploadFiles(resuestid, FileUploadListInfo.Where(x => x.FileSrc == FileSrc.local).ToArray());
                        if (_updateresult != null && _updateresult.IsSuccess)
                        {
                            _notificationService.ShowMessage("Certificate created successfully", NotificationType.Success);
                            RefreshEvent(new object());
                            _eventAggregator.GetEvent<ProductCertificateRefreshEvent>().Publish();
                        }
                    }
                }
                else
                {
                    _notificationService.ShowMessage(result.Error, NotificationType.Error);

                }



            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {
                await _ProgressService.StopProgressAsync();
            }
        }

        private void DeleteFileAttachmentCommandEvent(object obj)
        {
            try
            {
                if (FileUploadListInfo != null)
                {
                    FileUploadListInfo.Clear();
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        private void AddFileAttachmentCommandEvent(object obj)
        {
                try
                {

                    if (FileUploadListInfo != null && FileUploadListInfo.Count >= 1)
                    {
                        _notificationService.ShowMessage("1 files already added. Delete old file", NotificationType.Information);

                        _logger.LogWarning($"Max File attachments added");

                        return;
                    }


                    OpenFileDialog dialog = new OpenFileDialog
                    {
                        Title = "Please select a file",
                        Multiselect = true,
                        Filter = ApplicationSettings.Invoice_File_Extension_Filter,
                    };


                    bool? _resultOk = dialog.ShowDialog();

                    if (_resultOk == null || _resultOk != true)
                    {
                        //user cancelled file selection return.
                        return;
                    }


                    if (dialog != null)
                    {
                        if (FileUploadListInfo == null)
                        {
                            FileUploadListInfo = new ObservableCollection<FileUploadInfo>();
                        }
                        if (dialog.FileNames != null && dialog.FileNames.Count() > 1)
                        {
                            _notificationService.ShowMessage("Can't add more than 1 file", NotificationType.Information);

                            _logger.LogWarning($"Max File attachments added");

                            return;
                        }
                        foreach (string _fileName in dialog.FileNames)
                        {
                            if (_fileName.IsValidString())
                            {
                                if (File.Exists(_fileName))
                                {
                                    //Check for each fil size ...
                                    FileInfo _fileInfo = new FileInfo(_fileName);

                                    if ((_fileInfo.Length / (1024 * 1024)) > 10)
                                    {
                                        _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} size should be less than 10 mb in size", NotificationType.Error);

                                        _logger.LogWarning($"Filename {_fileName} has size of {_fileInfo.Length / (1024 * 1024)} mb");

                                        continue;
                                    }

                                    //dont again add the same file .causes file access issue while reading file stream

                                    if (FileUploadListInfo.Any(x => x.FileName == Path.GetFileName(_fileName)))
                                    {
                                        _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} is already added", NotificationType.Information);

                                        _logger.LogWarning($"File was already added skipping file {_fileName}");

                                        continue;
                                    }

                                    _logger.LogInformation($"Adding file {_fileName}");

                                    FileUploadListInfo?.Add(new FileUploadInfo
                                    {
                                        FileId = Guid.NewGuid(),
                                        FilePath = _fileName,
                                        FileExtension = Path.GetExtension(_fileName),
                                        FileName = Path.GetFileName(_fileName),
                                        Size = FileHelper.FormatSize(_fileInfo.Length),
                                        FileSrc = FileSrc.local

                                    });
                                    
                                }
                            }
                        }
                    }
                    else
                    {
                        _notificationService.ShowMessage("Maximum 5 files allowed", NotificationType.Error);
                    }
                }
                catch (Exception _ex)
                {
                    _logger.LogError(_ex.Message);
                }
            

        }
    }
}
