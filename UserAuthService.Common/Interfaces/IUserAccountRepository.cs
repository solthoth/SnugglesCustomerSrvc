namespace UserAuthService.Common.Interfaces
{
    public interface IReadOnlyUserAccountRepository
    {
        IUserAccount GetUser(string username, string password);
    }

    public interface ISaveableUserAccountRepository
    {
        void CreateNewUser(IUserAccount userAccount);
        void UpdateUser(string userId, IUserAccount updatedAccount);
    }

    public interface IUserAccountRepository : ISaveableUserAccountRepository, IReadOnlyUserAccountRepository
    {
    }
}