namespace UserAuthService.Common.Interfaces
{
    public interface IReadOnlyAccountRepository
    {
        IUserAccount GetUser(string username, string password);
    }

    public interface ISaveableAccountRepository
    {
        void CreateUser(IUserAccount userAccount);
        void UpdateUser(IUserAccount updatedAccount);
    }

    public interface IAccountRepository : ISaveableAccountRepository, IReadOnlyAccountRepository
    {
    }
}