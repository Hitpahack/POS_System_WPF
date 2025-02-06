using FalcaPOS.Entites.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalcaPOS.Reports.Model {
    public class TopListItemModel :TopListModel{
        public string SKU { get; set; }
        public string ProductName { get; set; }
    }

    public class TopListBrandModel : TopListModel {
        public string Brand { get; set; }
 
    }

    public class TopListModel:ReportBaseModel {
        public int Quantity { get; set; }
        public decimal Gross { get; set; }
        public decimal SGST { get; set; }

        [DisplayName("CGST/IGST")]
        public decimal CGST_IGST { get; set; }
        public decimal Discount { get; set; }
        public decimal Net { get; set; }
    }
}
