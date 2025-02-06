using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalcaPOS.Entites.Coupon {
    public class CouponViewModel {
        public string OfferText { get; set; }
        public int? OfferType { get; set; }
        public double? AmtOrPer { get; set; }
        public double? RedeemedAmtOrPer { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Code { get; set; }
        public bool? IsValid { get; set; }
        public string Remarks { get; set; }
        public DateTime? CreatedDateTime { get; set; }
    }
}
