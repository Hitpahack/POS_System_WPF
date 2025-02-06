using Prism.Mvvm;

namespace FalcaPOS.Entites.Asserts
{
    public class AddAssertModel
    {


        public int CategoryId { get; set; }

        public int StoreId { get; set; }

        public int GoodStock { get; set; }

        public int DamageStock { get; set; }

        public int TotalStock { get; set; }

        public string DamageReason { get; set; }

        public string Remarks { get; set; }
    }

    public class EditAssertModel : BindableBase
    {
        public int Id { get; set; }


        public int GoodStock
        {
            get; set;
        }


        public int DamageStock
        {
            get; set;
        }

        public int TotalStock { get; set; }

        public string Reason { get; set; }
    }
}
