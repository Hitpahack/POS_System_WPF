using System;

namespace FalcaPOS.Entites.Attributes
{
    public class AttributeType
    {
        public int AttributeId { get; set; }


        public string Name { get; set; }

        public string Description { get; set; }


        public bool Isenabled { get; set; }

        public DateTime Createdate { get; set; }
    }
}
