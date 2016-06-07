using UserAuthService.Common.Lookups;

namespace UserAuthService.Common.Interfaces
{
    public interface IUserAccount
    {
        string Email { get; set; }
        string FirstName { get; set; }
        string Id { get; set; }
        string LastName { get; set; }
        string MiddleName { get; set; }
        string Password { get; set; }
        UserAccountStatus Status { get; set; }
        string Username { get; set; }
    }
}