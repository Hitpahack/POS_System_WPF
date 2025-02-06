using System;

namespace FalcaPOS.Entites.Sales
{
    public class SalesPayment
    {
        public DateTime? CreatedDate { get; set; }

        public float Cash { get; set; }

        public float Card { get; set; }

        public string CardNumber { get; set; }

        public float UPI { get; set; }

        public string UPINumber { get; set; }

        public DateTime? PayMentDate { get; set; }

        public string ChequeNumber { get; set; }

        public float Cheque { get; set; }

        public string stringCheque { get; set; }


    }


    public class SalesPaymentModel
    {

        public float Cash { get; set; }

        public float Card { get; set; }

        public string CardNumber { get; set; }

        public float UPI { get; set; }

        public string UPINumber { get; set; }

        public string ChequeNumber { get; set; }

        public float Cheque { get; set; }


    }
}
