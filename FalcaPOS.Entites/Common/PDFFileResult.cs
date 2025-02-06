using Newtonsoft.Json;

namespace FalcaPOS.Entites.Common
{
    public class PDFFileResult
    {
        public byte[] FileStream { get; set; }

        public string FileName { get; set; }

    }

    public class Data
    {
        public SignTinyResponse sign_tiny_response { get; set; }

        [JsonProperty("sign_s3_urls")]
        public SignS3Urls sign_s3_urls { get; set; }
        public EmailResponse email_response { get; set; }

        public string invoice_type { get; set; }
        public string source { get; set; }
        public string document_number { get; set; }
    }

    public class EmailResponse
    {
        public string detail { get; set; }
    }

    public class ResponseInvoice
    {
        public bool success { get; set; }
        public Data data { get; set; }
        public string message { get; set; }
    }

    public class SignS3Urls
    {
        public string _A4 { get; set; }
        public string _4inch { get; set; }
    }

    public class SignTinyResponse
    {
        public string Original_For_Recipient { get; set; }

        public string A4 { get; set; }

        public string _A4 { get; set; }

        public string _4inch { get; set; }
    }


}
