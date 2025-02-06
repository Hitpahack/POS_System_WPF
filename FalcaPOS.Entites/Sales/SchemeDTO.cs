using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FalcaPOS.Entites.Sales
{
    public class SchemeDTO
    {
        [JsonPropertyName("coupon_code")]
        public string Coupon_code { get; set; }
        [JsonPropertyName("demography")]
        public Demography Demography { get; set; }
        [JsonPropertyName("customer")]
        public Customer Customer { get; set; }
        [JsonPropertyName("products")]
        public List<Products> Products { get; set; }
    }


    public class Customer
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("total_billing_amount_milestone")]
        public float Total_billing_amount_milestone { get; set; }

        [JsonPropertyName("gst")]
        public string GST { get; set; }
    }

    public class Demography
    {
        [JsonPropertyName("store_name")]
        public string Store_name { get; set; }

        //[JsonProperty("store_address")]
        //public string Store_address { get; set; }

        //[JsonProperty("store_phone")]
        //public string Store_phone { get; set; }

    }

    public class Products
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public float Price { get; set; }

        [JsonPropertyName("priceInclusiveGST")]
        public float PriceInclusiveGST { get; set; }

        [JsonPropertyName("GST")]
        public float GST { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("brand")]
        public string Brand { get; set; }

        [JsonPropertyName("subcategory")]
        public string Subcategory { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }
    }

    public class SchemeOutPutDTO
    {
        public List<SchemeProductDTO> Products { get; set; }
        public float TotalPayableAmount { get; set; }
        public float TotalDiscount { get; set; }
        public string CouponId { get; set; }
        public string CouponCode { get; set; }
        public string CouponName { get; set; }
        public string CouponType { get; set; }
        public string BillingId { get; set; }
        public float NetTotal { get; set; }
        public float TotalGST { get; set; }
        public int OTP { get; set; }
        public float RoundedOff { get; set; }
        public string FutureOffers { get; set; }
    }

    public class SchemeProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public float Total { get; set; }
        public string Discount { get; set; }
        public float PriceAfterDiscount { get; set; }
        public float GST { get; set; }
        public float TotalTax { get; set; }
        public float NetTotal { get; set; } 
        public float PriceInclusiveGST { get; set; }
        public float PriceInclusiveGSTAfterDiscount { get; set; }
        public float DiscountPerProduct { get; set; }
    }
}
