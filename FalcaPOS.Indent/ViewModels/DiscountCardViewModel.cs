using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalcaPOS.Indent.ViewModels
{
    public class DiscountCardViewModel:BindableBase 
    {
        public Guid DiscountCardGUIDId { get; set; }

        public string SKU { get; set; }
        public DiscountCardViewModel() 
        { 
            DiscountCardGUIDId = Guid.NewGuid();
           
        }

        private int _discountValue;

        public int DisocuntValue
        {
            get { return _discountValue; }
            set { 
                SetProperty(ref _discountValue , value);                
            }
        }

        private int _discountPercentage;

        public int DisocuntPercentage
        {
            get { return _discountPercentage; }
            set { 
                SetProperty(ref _discountPercentage, value);              
                if (_discountPercentage>100)
                    _discountPercentage = 0;
            }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

    }
}
