using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalcaPOS.Reports.Model
{
    public class TopListCategoryModel : ReportBaseModel
    {
        public string Category { get; set; }

        public int Quantity { get; set; }

        public decimal Gross { get; set; }

        public decimal SGST { get; set; }


        [DisplayName("CGST/IGST")]
        public decimal CGST_IGST { get; set; }

        public decimal Discount { get; set; }

        public decimal Net { get; set; }
    }
}
