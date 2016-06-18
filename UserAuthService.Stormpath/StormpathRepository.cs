using Stormpath.SDK.Account;
using Stormpath.SDK.Client;
using System;
using System.Threading.Tasks;
using UserAuthService.Common.Interfaces;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Http;

namespace UserAuthService.StormpathWrapper
{
    public class StormpathRepository : IAccountRepository
    {
        private string UserApiKey;
        private string UserApiKeySecret;
        private string ApplicationUrl;
        private string DirectoryUrl;
        private IClient StormpathClient;

        /// <summary>
        /// Instantiate a new Stormpath account repository
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="apiKeySecret"></param>
        /// <param name="applicationUrl"></param>
        public StormpathRepository(string apiKey, string apiKeySecret, string applicationUrl, string directoryUrl)
        {
            UserApiKey = apiKey;
            UserApiKeySecret = apiKeySecret;
            ApplicationUrl = applicationUrl;
            DirectoryUrl = directoryUrl;
                        
            StormpathClient = Clients.Builder()
                .SetApiKeyId(UserApiKey)
                .SetApiKeySecret(UserApiKeySecret)
                .SetHttpClient(HttpClients.Create().RestSharpClient())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .Build();
        }

        /// <summary>
        /// Deletes a user from Stormpath, DO NOT USE THIS METHOD!
        /// </summary>
        /// <param name="username"></param>
        public void DeleteUser(string username, string password)
        {
            var asyncAccount = AuthenticateAndGetUser(username, password);
            asyncAccount.Wait();
            var account = asyncAccount.Result;
            account.DeleteAsync().Wait();
        }

        /// <summary>
        /// Create a new user in Stormpath, if successful the Id will be set to that accounrs HREF
        /// </summary>
        /// <param name="userAccount"></param>
        public void CreateUser(IUserAccount userAccount)
        {
            var asyncDir = StormpathClient.GetDirectoryAsync(DirectoryUrl);
            var newAccount = SetAccountData(StormpathClient.Instantiate<IAccount>(), userAccount);
            asyncDir.Wait();
            var directory = asyncDir.Result;
            var asyncAccount = directory.CreateAccountAsync(newAccount);
            asyncAccount.Wait();
            var persistedAccount = asyncAccount.Result;
            userAccount.Id = persistedAccount.Href;
        }

        public IUserAccount GetUser(string username, string password)
        {
            var asyncAccount = AuthenticateAndGetUser(username, password);
            asyncAccount.Wait();
            var account = asyncAccount.Result;
            return account != null ? new StormpathAccount(account) : null;
        }

        /// <summary>
        /// Update user account information
        /// </summary>
        /// <param name="updatedAccount">UserAccount object with the modified changes</param>
        /// <exception cref="ResourceException"></exception>
        /// <exception cref="Exception"></exception>
        public void UpdateUser(IUserAccount updatedAccount)
        {
            var asyncAccount = StormpathClient.GetAccountAsync(updatedAccount.Id);
            asyncAccount.Wait();
            var account = asyncAccount.Result;
            account = SetAccountData(account, updatedAccount);
            account.SaveAsync().Wait();
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
                .SetMiddleName(newAccount.MiddleName)
                .SetUsername(newAccount.Username)
                .SetEmail(newAccount.Email)
                .SetPassword(newAccount.Password)
                ;
        }
    }
}