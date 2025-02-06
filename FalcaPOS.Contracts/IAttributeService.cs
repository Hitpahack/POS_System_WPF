using FalcaPOS.Entites.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IAttributeTypeService
    {
        Task<IEnumerable<AttributeType>> GetAttributeTypes(string querry = null);

        Task<AttributeType> CreateAttribute(AttributeType attributeType);
    }
}
