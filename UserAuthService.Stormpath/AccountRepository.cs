using Stormpath.SDK.Account;
using Stormpath.SDK.Client;
using Stormpath.SDK.Error;
using UserAuthService.Common.Interfaces;
using Stormpath.SDK;
using System;
using System.Threading.Tasks;

namespace UserAuthService.StormpathRepository
{
    public class AccountRepository : IUserAccountRepository
    {
        public AccountRepository() : this(StormpathConfig.Default.ApiKey, StormpathConfig.Default.ApiKeySecret)
        { }

        public AccountRepository(string ApiKey, string ApiKeySecret)
        {
            StormpathClient = Clients.Builder()
                .SetApiKeyId(StormpathConfig.Default.ApiKey)
                .SetApiKeySecret(StormpathConfig.Default.ApiKeySecret)
                .Build();
        }

        public IClient StormpathClient { get; private set; }
        public string ApiKey { get; private set; }
        public string ApiKeySecret { get; private set; }

        /// <summary>
        /// Create a new user in Stormpath, if successful the Id will be set to that accounrs HREF
        /// </summary>
        /// <param name="userAccount"></param>
        public async void CreateNewUser(IUserAccount userAccount)
        {
            var asyncApplication = StormpathClient.GetApplicationAsync(StormpathConfig.Default.ApplicationUrl);
            IAccount newAccount = SetAccountData(StormpathClient.Instantiate<IAccount>(), userAccount);
            var application = await asyncApplication;
            await application.CreateAccountAsync(newAccount);
            userAccount.Id = newAccount.Href;
        }

        /// <summary>
        /// Maps custom account object to stormpath's account object
        /// </summary>
        /// <param name="serverAccount"></param>
        /// <param name="newAccount"></param>
        /// <returns></returns>
        private IAccount SetAccountData(IAccount serverAccount, IUserAccount newAccount)
        {
            return serverAccount
                .SetGivenName(newAccount.FirstName)
                .SetSurname(newAccount.LastName)
                .SetUsername(newAccount.Username)
                .SetEmail(newAccount.Email)
                .SetPassword(newAccount.Password);
        }

        /// <summary>
        /// Update user account information
        /// </summary>
        /// <param name="userId">This is the stormpath HREF of the account to update</param>
        /// <param name="updatedAccount">UserAccount object with the modified changes</param>
        /// <exception cref="ResourceException"></exception>
        /// <exception cref="Exception"></exception>
        public async void UpdateUser(string userId, IUserAccount updatedAccount)
        {
            var asyncApplication = StormpathClient.GetApplicationAsync(StormpathConfig.Default.ApplicationUrl);
            var account = await StormpathClient.GetAccountAsync(userId);
            account = SetAccountData(account, updatedAccount);
            var application = await asyncApplication;
            // Attempt to authenticate user before saving changes
            // if user fails to authenticate Stormpath error is thrown
            var authentificationResult = await application.AuthenticateAccountAsync(updatedAccount.Username, updatedAccount.Password);
            if (authentificationResult.Success)
                await account.SaveAsync();
            else
                throw new Exception("Unable to authenticate account");
        }

        public IUserAccount GetUser(string username, string password)
        {
            var asyncAccount = AuthenticateAndGetUser(username, password);
            asyncAccount.RunSynchronously();
            var account = asyncAccount.Result;
            return null;
        }

        private async Task<IAccount> AuthenticateAndGetUser(string username, string password)
        {
            var asyncApplication = StormpathClient.GetApplicationAsync(StormpathConfig.Default.ApplicationUrl);
            var application = await asyncApplication;
            var authentificationResult = await application.AuthenticateAccountAsync(username, password);
            return await authentificationResult.GetAccountAsync();
        }
    }
}