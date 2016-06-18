using NUnit.Framework;
using System.Linq;
using UserAuthAPI.Modals;
using UserAuthService.Common;
using UserAuthService.StormpathWrapper;
using FluentAssertions;
using System;

namespace UserAuthService.IntegrationTests.Stormpath
{
    [TestFixture(Explicit = true, TestOf = typeof(StormpathRepository))]
    public class AccountRepositoryTests
    {
        private Settings configSettings;
        private Application configApp;
        private Directory configDir;
        private User configUser;
        private AccountUser testUser;

        [OneTimeSetUp]
        public void Init()
        {
            configSettings = Settings.Create();
            configApp = configSettings.Data.Stormpath.Applications.Where(app => app.name.Contains("UserAuthService")).Single();
            configDir = configSettings.Data.Stormpath.Directories.Where(dir => dir.name.Contains("IntegrationTests")).Single();
            configUser = configSettings.Data.Stormpath.User;
            testUser = new AccountUser
            {
                Email = "Integration.User@Superbanana.com",
                FirstName = "TESTF",
                LastName = "TESTL",
                MiddleName = "",
                Username = "Useri",
                Password = "Password1"
            };
        }

        [Test, Order(1)]
        public void Given_Account_Information_When_Create_User_Then_Success()
        {
            var accountRepository = new StormpathRepository(configUser.ApiKeyID, configUser.ApiKeySecret, configApp.href, configDir.href);
            
            Action createUser = () => accountRepository.CreateUser(testUser);

            createUser.ShouldNotThrow<Exception>();
        }

        [Test]
        public void Given_Username_And_Password_When_Get_User_Then_Returns_User()
        {
            var accountRepository = new StormpathRepository(configUser.ApiKeyID, configUser.ApiKeySecret, configApp.href, configDir.href);

            var user = accountRepository.GetUser(testUser.Username, testUser.Password);

            user.Should().NotBeNull();
        }

        [Test]
        public void Given_New_MiddleName_When_Update_User_Then_Success()
        {
            var accountRepository = new StormpathRepository(configUser.ApiKeyID, configUser.ApiKeySecret, configApp.href, configDir.href);

            testUser.MiddleName = "TESTM";
            accountRepository.UpdateUser(testUser);

            var newUser = accountRepository.GetUser(testUser.Username, testUser.Password);
            newUser.MiddleName.Should().Be(testUser.MiddleName);
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            var stormpathRepo = new StormpathRepository(configUser.ApiKeyID, configUser.ApiKeySecret, configApp.href, configDir.href);
            stormpathRepo.DeleteUser(testUser.Username, testUser.Password);
        }

    }
}
