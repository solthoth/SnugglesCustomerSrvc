namespace UserAuthService.Common.Interfaces
{
    public interface IReadOnlyAccountRepository
    {
        IUserAccount GetUser(string username, string password);
        bool AuthenticateUser(string username, string password);
    }

    public interface ISaveableAccountRepository
    {
        void CreateNewUser(IUserAccount userAccount);
        void UpdateUser(string userId, IUserAccount updatedAccount);
    }

    public interface IAccountRepository : ISaveableAccountRepository, IReadOnlyAccountRepository
    {
    }
}