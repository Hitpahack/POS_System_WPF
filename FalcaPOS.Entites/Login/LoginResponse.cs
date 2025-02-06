using FalcaPOS.Entites.Stores;

namespace FalcaPOS.Entites.Login
{
    public class LoginResponse
    {
        public string username { get; set; }

        public string token { get; set; }

        public string[] roles { get; set; }

        public bool issuccess { get; set; }

        public string error { get; set; }

        public Store StoreInfo { get; set; }

        public string StoreCityLocation { get; set; }

        public string State { get; set; }

        public string CurrentDate { get; set; }

        public int Id { get; set; }

        public string FalcaGSTIN { get; set; }

        public string Printer { get; set; }
    }
}
