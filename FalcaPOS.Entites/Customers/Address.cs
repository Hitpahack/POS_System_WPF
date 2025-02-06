using FalcaPOS.Entites.Location;
using System;

namespace FalcaPOS.Entites.Customers
{
    public class Address
    {
        public int? AddressId { get; set; }

        public string RAddress { get; set; }

        public string Phone { get; set; }

        public string Alternatephone { get; set; }

        public string Email { get; set; }

        public string Street { get; set; }

        public int? Pincode { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string District { get; set; }

        public DateTime? Createddate { get; set; }

        public int StateID { get; set; }

        public int DistrictID { get; set; }       

        public Village Village { get; set; }

    }


}
