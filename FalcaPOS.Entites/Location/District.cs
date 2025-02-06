using System;

namespace FalcaPOS.Entites.Location
{
    public class District
    {
        public int DistrictId { get; set; }

        public string Name { get; set; }

        public int? StateId { get; set; }

        //public string Shortname { get; set; }

        public bool? Isvisible { get; set; }

        public DateTime? Createddatetime { get; set; }

        public string StateName { get; set; }

    }
}
