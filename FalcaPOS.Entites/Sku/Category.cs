using Prism.Mvvm;
using System;

namespace FalcaPOS.Entites.Sku
{
    public class Category : BindableBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Createddatetime { get; set; }
    }
}
