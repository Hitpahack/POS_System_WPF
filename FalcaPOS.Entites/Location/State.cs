using System;
using System.Collections.Generic;

namespace FalcaPOS.Entites.Location
{
    public class State
    {

        public int StateId { get; set; }

        public List<District> Districts { get; set; }

        // public string Shortname { get; set; }

        public string Name { get; set; }

        public bool? Isvisible { get; set; }

        public DateTime? Createddatetime { get; set; }
    }
}
