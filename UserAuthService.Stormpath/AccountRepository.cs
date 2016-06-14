using Stormpath.SDK.Account;
using Stormpath.SDK.Client;
using System;
using System.Threading.Tasks;
using UserAuthService.Common.Interfaces;

namespace UserAuthService.StormpathRepository
{
    public class AccountRepository : IAccountRepository
    {
        private string UserApiKey;
        private string UserApiKeySecret;
        private string ApplicationUrl;
        private IClient StormpathClient;

        /// <summary>
        /// Instantiate a new Stormpath account repository
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="apiKeySecret"></param>
        /// <param name="applicationUrl"></param>
        public AccountRepository(string apiKey, string apiKeySecret, string applicationUrl)
        {
            UserApiKey = apiKey;
            UserApiKeySecret = apiKeySecret;
            ApplicationUrl = applicationUrl;
            StormpathClient = Clients.Builder()
                .SetApiKeyId(UserApiKey)
                .SetApiKeySecret(UserApiKeySecret)
                .Build();
        }

        /// <summary>
        /// Authenticates user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool AuthenticateUser(string username, string password)
        {
            bool result;
            try
            {
                result = AuthenticateAndGetUser(username, password) != null;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Create a new user in Stormpath, if successful the Id will be set to that accounrs HREF
        /// </summary>
        /// <param name="userAccount"></param>
        public async void CreateNewUser(IUserAccount userAccount)
        {
            var asyncApplication = StormpathClient.GetApplicationAsync(ApplicationUrl);
            IAccount newAccount = SetAccountData(StormpathClient.Instantiate<IAccount>(), userAccount);
            var application = await asyncApplication;
            await application.CreateAccountAsync(newAccount);
            userAccount.Id = newAccount.Href;
        }

        public IUserAccount GetUser(string username, string password)
        {
            var asyncAccount = AuthenticateAndGetUser(username, password);
            asyncAccount.RunSynchronously();
            var account = asyncAccount.Result;
            return null;
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
            var asyncApplication = StormpathClient.GetApplicationAsync(ApplicationUrl);
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

        private async Task<IAccount> AuthenticateAndGetUser(string username, string password)
        {
            var asyncApplication = StormpathClient.GetApplicationAsync(ApplicationUrl);
            var application = await asyncApplication;
            var authentificationResult = await application.AuthenticateAccountAsync(username, password);
            return await authentificationResult.GetAccountAsync();
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
    }
}