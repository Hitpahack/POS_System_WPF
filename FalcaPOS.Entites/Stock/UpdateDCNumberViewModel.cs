using System;

namespace FalcaPOS.Entites.Stock
{
    public class UpdateDCNumberViewModel
    {

        public int InvoiceId { get; set; }


        public string InvoiceNumber { get; set; }


        public DateTime InvoiceDate { get; set; }

        public string DcNumber { get; set; }
    }
}
