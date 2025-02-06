namespace FalcaPOS.Entites.AddInventory
{
    public class UpdateInvoiceProductViewModel
    {

        public int ProductId { get; set; }

        public float ProductDiscount { get; set; }

        public float ProductDiscountPerecent { get; set; }

        public float ProductDiscountFlat { get; set; }

        public string ProductDiscountMode { get; set; }

        public float ProductGST { get; set; }

        public float ProductTotal { get; set; }

        public float ProductSellingPrice { get; set; }

        public string ProductUniqGuid { get; set; }

        public float ProductMRP { get; set; }

        public float Margin { get; set; }
    }
}
