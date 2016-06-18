using System;
using UserAuthService.Common.Interfaces;
using UserAuthService.Common.Lookups;

namespace UserAuthAPI.Modals
{
    public class AccountUser : IUserAccount
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string Id { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Password { get; set; }
        public UserAccountStatus Status { get; set; }
        public string Username { get; set; }

        public AccountUser Clone()
        {
            return (AccountUser)this.MemberwiseClone();
        }
    }
}