using AutoMapper;
using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.Products;
using FalcaPOS.Entites.ProductTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalcaPOS.Entites.Dtos {
    public class MapperConfig {

        public static Mapper InitializeAutomapper() {


            var _config = new MapperConfiguration(pg => {
                pg.CreateMap<ProductDTO, ProductDetails>();
                pg.CreateMap<CategoryDTO, CategoryModel>();
                pg.CreateMap<SubCategoryDTO, ProductType>();
                pg.CreateMap<ManufactureDTO, Manufacturer>();

            });

            var _mapper = new Mapper(_config);
            return _mapper;
        }
    }
}
