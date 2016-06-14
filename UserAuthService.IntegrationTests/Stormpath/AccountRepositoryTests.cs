using NUnit.Framework;
using System.Linq;
using UserAuthAPI.Modals;
using UserAuthService.Common;
using UserAuthService.StormpathRepository;
using FluentAssertions;
using System;

namespace UserAuthService.IntegrationTests.Stormpath
{
    [TestFixture(Explicit = true, TestOf = typeof(AccountRepository))]
    public class AccountRepositoryTests
    {
        private Settings configSettings;
        private Application configApp;
        private User configUser;
        private AccountUser testUser;

        [SetUp]
        public void Init()
        {
            configSettings = Settings.Create();
            configApp = configSettings.Data.Stormpath.Applications.Where(app => app.name.Contains("IntegrationTests")).Single();
            configUser = configSettings.Data.Stormpath.User;
            testUser = new AccountUser
            {
                Email = "Integration.User@Superbanana.com",
                FirstName = "TESTF",
                LastName = "TESTL",
                MiddleName = "",
                Username = "Useri",
                Password = "password1"
            };
        }

        [Test]
        public void Given_Account_Information_When_Create_User_Then_Success()
        {
            var accountRepository = new AccountRepository(configUser.ApiKeyID, configUser.ApiKeySecret, configApp.Url);
            
            Action createUser = () => accountRepository.CreateNewUser(testUser);

            createUser.ShouldNotThrow<Exception>();
        }
    }
}
