using FalcaPOS.Entites.Customers;
using FalcaPOS.Entites.Stores;

namespace FalcaPOS.Entites.User
{
    public class User
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool Changepassword { get; set; }

        public string[] Roles { get; set; }

        public string Name { get; set; }

        public bool? IsAlive { get; set; }

        public Store Store { get; set; }

        public int StoreId { get; set; }

        public Address Address { get; set; }
    }

    public class UpdateUserModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool Changepassword { get; set; }

        public string[] Roles { get; set; }

        public string Name { get; set; }

        public bool? IsAlive { get; set; }

        public int StoreId { get; set; }

        public Address Address { get; set; }
    }
}
