namespace FalcaPOS.Entites.Rsp
{
    public class RspSummaryModel
    {
        public string StoreName { get; set; }

        public string OnDate { get; set; }

        public float Business { get; set; }

        public float Cash { get; set; }

        public float Card { get; set; }

        public float Cheque { get; set; }
    }
}
