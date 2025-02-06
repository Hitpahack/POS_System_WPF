using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalcaPOS.Entites.Zone
{
    public class Territory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public String ZoneName { get; set; }
        public int ZoneId { get; set; }
        public int TerritorymanagerRef { get; set; }        
        public DateTime CreatedDateTime { get; set; }
    }
}
