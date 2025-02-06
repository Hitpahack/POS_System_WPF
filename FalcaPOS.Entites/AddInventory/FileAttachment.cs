using System;

namespace FalcaPOS.Entites.AddInventory
{
    public class FileAttachment
    {
        public int FileId { get; set; }

        public string FileName { get; set; }

        public Guid FileGUID { get; set; }

        public String Size { get; set; }


    }
}