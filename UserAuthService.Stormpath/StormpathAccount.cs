using Stormpath.SDK.Account;
using UserAuthService.Common.Interfaces;
using UserAuthService.Common.Lookups;

namespace UserAuthService.StormpathWrapper
{
    public class StormpathAccount : IUserAccount
    {
        public StormpathAccount()
        {
        }

        public StormpathAccount(IAccount account)
        {
            Email = account.Email;
            FirstName = account.GivenName;
            LastName = account.Surname;
            MiddleName = account.MiddleName;
            Id = account.Href;
            if (account.Status == AccountStatus.Disabled)
            {
                Status = UserAccountStatus.Inactive;
            }
            else if (account.Status == AccountStatus.Enabled)
            {
                Status = UserAccountStatus.Active;
            }
            else
            {
                Status = UserAccountStatus.Unknown;
            }
            Username = account.Username;
        }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string Id { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Password { get; set; }

        public UserAccountStatus Status { get; set; }

        public string Username { get; set; }
    }
}