using FalcaPOS.Entites.Customers;
using FalcaPOS.Entites.Indent;
using System.Collections.Generic;

namespace FalcaPOS.Entites.Stores
{
    public class StoreLicense
    {
        public int? Id { get; set; }
        public int StoreRef { get; set; }
        public string StoreName { get; set; }
        public int CategoryRef { get; set; }
        public string CategoryName { get; set; }
        public string WholesaleLicense { get; set; }
        public string NormalLicense { get; set; }
    }
   
}
