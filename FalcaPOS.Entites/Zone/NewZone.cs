using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalcaPOS.Entites.Zone
{
    public class NewZone
    {
        public int Id { get; set; }
        public string StateName { get; set; }
        public int StateId { get; set; }       
        public string Name { get; set; }
        public int ZonalmanagerRef { get; set; }        
        public DateTime CreatedDateTime { get; set; }
    }
}
