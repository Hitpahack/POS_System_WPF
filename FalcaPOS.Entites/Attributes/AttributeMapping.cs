using System;

namespace FalcaPOS.Entites.Attributes
{
    public class AttributeMapping
    {

        public AttributeMapping()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public AttributeType AttributeType { get; set; }

        public string AttributeName { get; set; }
    }
}
