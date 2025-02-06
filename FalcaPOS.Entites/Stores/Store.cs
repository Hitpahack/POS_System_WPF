using FalcaPOS.Entites.Customers;
using System.Collections.Generic;

namespace FalcaPOS.Entites.Stores
{
    public class Store
    {
        public int StoreId { get; set; }

        public string StoreType { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }

        public string StoreInvoiceFormat { get; set; }

        public int? LastSequenceNumber { get; set; }

        public bool IsParent { get; set; }

        public int? Parent_ref { get; set; }

        public string ImageName { get; set; }
        public int? ZoneUser_ref { get; set; }
        public int? TerritoryUser_ref { get; set; }
        public int? Territory_ref { get; set; }
        public string Zone { get; set; }
        public string TerritoryName { get; set; }
        public IEnumerable<StoreLicense> Licenses { get; set; }
    }

    public class StoreFlyout
    {
        public bool IsOpen { get; set; }

        public bool IsParent { get; set; }
    }

    public class StoreSearchParams
    {       

        public int? StateId { get; set; }

        public int? DistrictId { get; set; }
    }

}
