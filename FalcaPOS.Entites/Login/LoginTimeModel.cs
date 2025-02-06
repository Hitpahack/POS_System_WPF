using System;

namespace FalcaPOS.Entites.Login
{
    public class LoginTimeModel
    {

        public string StoreName { get; set; }
        public string Date { get; set; }
        public TimeSpan LoginTime { get; set; }
        public string Time { get; set; }
        public DateTime CreateDateTime { get; set; }

    }
}
