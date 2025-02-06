using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace FalcaPOS.Common.Helper
{
    public class ParseJSON
    {
        public T ParseJson<T>(String jsonfilenameinconfig)
        {
            try
            {

                var filename = Environment.CurrentDirectory;

                var config = ConfigurationManager.AppSettings[jsonfilenameinconfig].ToString();

                String json = File.ReadAllText(filename + @"\\" + config, Encoding.UTF8);
                return JsonConvert.DeserializeObject<T>(json.Replace(@"\", "").ToString());
            }
            catch (Exception ex)
            {

                return default(T);
            }
        }
    }
}
