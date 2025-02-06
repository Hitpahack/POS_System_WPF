using System.Collections.Generic;

namespace FalcaPOS.Entites.Dashboard
{
    public class DashboardCustomer
    {
        public IEnumerable<CustomerByStore> CustomerByStores { get; set; }

        public int TotalCustomersCount { get; set; }
    }
    public class CustomerByStore
    {
        public int CustomerCount { get; set; }

        public string StoreName { get; set; }
    }


    public class SalesByStore
    {
        public double TotalSales { get; set; }

        public string StoreName { get; set; }
    }

    public class DashbordSalesVM
    {
        public IEnumerable<SalesByStore> salesByStores { get; set; }
    }


    public class SupplierByStore
    {
        public double TotalAmount { get; set; }
        public string SupplierName { get; set; }
        public string StoreName { get; set; }
    }

    public class DashbordSupplierVM
    {
        public IEnumerable<SupplierByStore> SupplierByStores { get; set; }
    }

    public class SalesByMonth
    {
        public double TotalSales { get; set; }
        public int MonthNumber { get; set; }
    }

    public class DashbordMonthSalesVM
    {
        public IEnumerable<SalesByMonth> MonthSales { get; set; }
    }


    public class MostNumberofsalesItem
    {
        public string BrandName { get; set; }
        public string ProductName { get; set; }
        public int BrandCount { get; set; }

        public int ProductCount { get; set; }
    }

    public class DashbordMostnumberofSales
    {
        public IEnumerable<MostNumberofsalesItem> MostNumberOfSales { get; set; }
    }
}
