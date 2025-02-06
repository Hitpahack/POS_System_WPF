using System.Collections.Generic;

namespace FalcaPOS.Entites.Attributes
{
    public class ProductAttributeMapping
    {

        public ProductAttributeMapping()
        {
            AttributesList = new List<AttributeMap>();
        }

        public ProductAttribute ProductAttribute { get; set; }


        public List<AttributeMap> AttributesList { get; set; }



        public AttributeMap AttributeMap { get; set; }


    }


    public class AttributeMap
    {
        public int AttributeValueId { get; set; }

        public string AttributeValueName { get; set; }
    }
}
