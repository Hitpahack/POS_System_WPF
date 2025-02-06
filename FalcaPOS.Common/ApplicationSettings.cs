using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace FalcaPOS.Common
{
    public static class ApplicationSettings
    {
        public static string ServerUrl;

        public readonly static string APP_ENVIRONMENT;

        public readonly static string APP_UPDATE_URL;

        public readonly static string REPORTS_PATH;

        public readonly static string TALLYREPORTS_PATH;

        public readonly static string Invoice_File_Extension_Filter;

        public static readonly IEnumerable<float> GST_VALUES;

        public static readonly string[] PRODUCT_LOCATIONS;

        public static readonly string[] WARRENTY_SERVICE;


        public static readonly string[] SUPPLIER_TYPE;


        public static string CachePath;

        public static string POSTempPath;

        public static bool CacheEnabled;

        public static readonly string[] CategoryCertificate;

        public readonly static string BULKPAYMENT_PATH;

        public readonly static string FALCA_DEBIT_ACCOUT;

        public readonly static string CREDITNOTE_PATH;

        public readonly static string DENOMIATIONPORTS_PATH;

        public readonly static string EXPIRY_PATH;
        public readonly static string PREVIEW_PATH;

        public readonly static string ASSERTS_PATH;
        public readonly static string FALCA2324COUPONSTARTWITH;

        public readonly static string ICIC_FALCA_DEBIT_ACNO;

        public static readonly string[] CUSTOMER_TYPE;

        public static readonly string[] TRANSPORT_CHARGES_PAYER;

        static ApplicationSettings()
        {
            ServerUrl = ConfigurationManager.AppSettings["ServerURL"].ToString();

            APP_ENVIRONMENT = ConfigurationManager.AppSettings["ENVIRONMENT"]?.ToString();

            APP_UPDATE_URL = ConfigurationManager.AppSettings["UPDATEPATH"]?.ToString();

            REPORTS_PATH = ConfigurationManager.AppSettings["REPORTSPATH"]?.ToString();

            Invoice_File_Extension_Filter = ConfigurationManager.AppSettings["InvoiceFileExtensionFilter"]?.ToString();

            GST_VALUES = ConfigurationManager
                .AppSettings["GSTVALUE"].Split(',')
                .Select(x => { return float.Parse(x); });

            PRODUCT_LOCATIONS = ConfigurationManager.AppSettings["PRODUCTLOCATION"].Split(',');

            WARRENTY_SERVICE = ConfigurationManager.AppSettings["WARRENTYSERVICES"].Split(',');

            TALLYREPORTS_PATH = ConfigurationManager.AppSettings["TALLYREPORTSPATH"]?.ToString();

            SUPPLIER_TYPE = ConfigurationManager.AppSettings["SUPPLIERTYPE"].Split(',');

            CacheEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["CacheEnabled"].ToString());

            CachePath = ConfigurationManager.AppSettings["CachePath"].ToString();

            POSTempPath = ConfigurationManager.AppSettings["POSTemp"].ToString();

            CategoryCertificate = ConfigurationManager.AppSettings["CategoryCertificate"].Split(',');

            BULKPAYMENT_PATH = ConfigurationManager.AppSettings["BULKPAYMENTPATH"]?.ToString();

            FALCA_DEBIT_ACCOUT = ConfigurationManager.AppSettings["FALCADEBITACCOUNT"]?.ToString();


            CREDITNOTE_PATH = ConfigurationManager.AppSettings["CREDITSUMMARY"]?.ToString();

            DENOMIATIONPORTS_PATH = ConfigurationManager.AppSettings["DENOMIANTIONPATH"]?.ToString();

            EXPIRY_PATH = ConfigurationManager.AppSettings["EXPIRYPATH"]?.ToString();

            ASSERTS_PATH = ConfigurationManager.AppSettings["ASSERTSPATH"]?.ToString();

            ICIC_FALCA_DEBIT_ACNO = ConfigurationManager.AppSettings["ICICFALCADEBITACNo"]?.ToString();

            CUSTOMER_TYPE = ConfigurationManager.AppSettings["CUSTOMERTYPE"].Split(',');

            PREVIEW_PATH = ConfigurationManager.AppSettings["PreviewImagePath"]?.ToString();

            FALCA2324COUPONSTARTWITH = ConfigurationManager.AppSettings["Falca2324CouponStartwith"]?.ToString();

            TRANSPORT_CHARGES_PAYER = ConfigurationManager.AppSettings["TRANSPORTCHARGESPAYER"].Split(',');

        }
    }
}
