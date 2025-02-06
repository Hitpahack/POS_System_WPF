using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalcaPOS.Entites.EwayBill
{
    public class EWayBillModel
    {
        public string access_token { get; set; }
        public string userGstin { get; set; }
        public string supply_type { get; set; }
        public string sub_supply_type { get; set; }
        public string sub_supply_description { get; set; }
        public string document_type { get; set; }
        public string document_number { get; set; }
        public string document_date { get; set; }
        public string gstin_of_consignor { get; set; }
        public string legal_name_of_consignor { get; set; }
        public string address1_of_consignor { get; set; }
        public string address2_of_consignor { get; set; }
        public string place_of_consignor { get; set; }
        public int pincode_of_consignor { get; set; }
        public string state_of_consignor { get; set; }
        public string actual_from_state_name { get; set; }
        public string gstin_of_consignee { get; set; }
        public string legal_name_of_consignee { get; set; }
        public string address1_of_consignee { get; set; }
        public string address2_of_consignee { get; set; }
        public string place_of_consignee { get; set; }
        public int pincode_of_consignee { get; set; }
        public string state_of_supply { get; set; }
        public string actual_to_state_name { get; set; }
        public int transaction_type { get; set; }
        public int other_value { get; set; }
        public float total_invoice_value { get; set; }
        public double taxable_amount { get; set; }
        public double cgst_amount { get; set; }
        public double sgst_amount { get; set; }
        public double igst_amount { get; set; }
        public double cess_amount { get; set; }
        public int cess_nonadvol_value { get; set; }
        public string transporter_id { get; set; }
        public string transporter_name { get; set; }
        public string transporter_document_number { get; set; }
        public string transporter_document_date { get; set; }
        public string transportation_mode { get; set; }
        public int transportation_distance { get; set; }
        public string vehicle_number { get; set; }
        public string vehicle_type { get; set; }
        public int generate_status { get; set; }
        public string data_source { get; set; }
        public string user_ref { get; set; }
        public string location_code { get; set; }
        public string eway_bill_status { get; set; }
        public string auto_print { get; set; }
        public string email { get; set; }
        public List<EWaybillProduct> itemList { get; set; }
    }
    public class EWaybillProduct
    {
        public string product_name { get; set; }
        public string product_description { get; set; }
        public string hsn_code { get; set; }
        public int quantity { get; set; }
        public string unit_of_product { get; set; }
        public double cgst_rate { get; set; }
        public double sgst_rate { get; set; }
        public double igst_rate { get; set; }
        public int cess_rate { get; set; }

        [JsonProperty(" cessNonAdvol")]
        public int cessNonAdvol { get; set; }
        public float taxable_amount { get; set; }

        public float total_item_value { get; set; }

    }
    public class Message
    {
        public long ewayBillNo { get; set; }
        public string ewayBillDate { get; set; }
        public string validUpto { get; set; }
        public string alert { get; set; }
        public bool error { get; set; }
        public string url { get; set; }
    }

    public class Results
    {
        public Message message { get; set; }
        public string status { get; set; }
        public int code { get; set; }
        public string requestId { get; set; }
    }

    public class EwayBillResponse
    {
        public Results results { get; set; }
        public bool isSuccess { get; set; }
        public string Error { get; set; }
    }

    public class AccessTokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
    }
}

