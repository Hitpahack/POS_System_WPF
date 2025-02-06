using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalcaPOS.Entites.Stock
{
    public class StockTransferModelForEWayModel
    {
        public ObservableCollection<StockTransferProducts> StockTransferProducts { get; set; }

        public string SRNumber { get; set;  }

        public string DocumentNo { get; set; }

        public string DocumentDate { get; set; }

        public string VehicleType { get; set; }

        public string vehicleNo { get; set; }

        public string FromLocation { get; set; }

        public string ToLocation { get; set; }

        public int TransferId { get; set; }

        public string DocumentName { get; set; }

        public int Id { get; set; }

       public  bool IsTransportAdded { get; set; }


    }

    public class StockTransferProducts 
    {
       public string ProductName { get; set; }
        public float SellingPrice { get; set; }
        public string HsnCode { get; set; }
        public float Gst { get; set; }
        public int TransferQty { get; set; }

        public string Description { get; set; }

    }
}
