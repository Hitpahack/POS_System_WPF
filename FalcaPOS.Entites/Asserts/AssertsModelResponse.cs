using System.ComponentModel;

namespace FalcaPOS.Entites.Asserts
{
    public class AssertsModelResponse
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Class { get; set; }
        public string Type { get; set; }

        public string Category { get; set; }

        public string Store { get; set; }

        public int GoodStock { get; set; }

        public int DamageStock { get; set; }

        public int TotalStock { get; set; }

        public string Remarks { get; set; }

        public string History { get; set; }

        public bool IsVisibility { get; set; }
    }

    public class AssertsModelExport
    {

        [DisplayName("Asset Code")]
        public string Code { get; set; }

        [DisplayName("Asset Class")]
        public string Class { get; set; }

        [DisplayName("Asset Type")]
        public string Type { get; set; }


        [DisplayName("Asset Category")]
        public string Category { get; set; }


        [DisplayName("StoreName")]
        public string Store { get; set; }

        public int GoodStock { get; set; }

        public int DamageStock { get; set; }

        public int TotalStock { get; set; }

        public string Remarks { get; set; }
    }
}
