using System.Collections.Generic;

namespace FalcaPOS.Entites.AddInventory
{
    public class UpdateInvoiceViewModel
    {
        public int InvoiceID { get; set; }

        public IEnumerable<UpdateInvoiceProductViewModel> Products { get; set; }
    }
}
