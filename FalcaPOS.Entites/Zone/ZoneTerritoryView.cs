using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalcaPOS.Entites.Zone
{
    public class ZoneTerritoryView: BindableBase
    {
        public int StoreID { get; set; }
        public int UserID { get; set; }
        public String State { get; set; }
        public String Zone { get; set; }
        public String Territory { get; set; }
        public int ZoneID { get; set; }
        public int TerritoryID { get; set; }
        public String Store { get; set; }        
        public int RegionalManagerUserId { get; set; }
        public int TerritoryManagerUserId { get; set; }

        private String _regionalmanager;
        public String RegionalManager
        {
            get { return _regionalmanager; }
            set { SetProperty(ref _regionalmanager, value); }
        }

        private String _territoryManager;

        public String TerritoryManager
        {
            get { return _territoryManager; }
            set { SetProperty ( ref _territoryManager , value); }

        }
      
    }
}
