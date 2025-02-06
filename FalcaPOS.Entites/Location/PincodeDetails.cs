using System.Collections.Generic;

namespace FalcaPOS.Entites.Location
{

    public class PincodeDetails
    {
        public string Pincode { get; set; }
        public State State { get; set; }
        public District District { get; set; }
        public List<Village> Villages { get; set; }
    }

}
