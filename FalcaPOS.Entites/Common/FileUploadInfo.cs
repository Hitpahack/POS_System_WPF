using Prism.Mvvm;
using System;

namespace FalcaPOS.Entites.Common
{
    public class FileUploadInfo : BindableBase
    {

        public Guid FileId { get; set; }

        public string FilePath { get; set; }

        private string _fileName;
        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }

        public string FileExtension { get; set; }

        public String Size { get; set; }
        public FileSrc FileSrc { get; set; }

        public int? FileremoteSrcID { get; set; }

        //this id using bank details id
        public string Id { get; set; }

    }

    public enum FileSrc
    {
        local = 1,
        remote = 2,
    }
}
