using FalcaPOS.Common.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace FalcaPOS.Indent.ViewModels {
    public class AddSupplierContentTemplateSelector: DataTemplateSelector {
        public DataTemplate AddSupplierTemplate { get; set; }
        public DataTemplate AddPaymentsTemplate { get; set; }
        public DataTemplate AddProductPriceTemplate { get; set; }

        public DataTemplate AddDiscountTemplate { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            
           var stepInfo = ((StepInfo)item); ;
            if (stepInfo.Name == "Add supplier") {
                return this.AddSupplierTemplate;
            }
            else if (stepInfo.Name == "Add Payment") {
                return this.AddPaymentsTemplate;
            }
            else if (stepInfo.Name == "Add Product Price") {
                return this.AddProductPriceTemplate;
            }
            else if (stepInfo.Name == "Terms & Conditions")
            {
                return this.AddDiscountTemplate;
            }
            return base.SelectTemplate(item, container);
        }

    }
    
}
