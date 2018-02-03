using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using XamChat.Core;

namespace XamChat.Tests
{
    [TestFixture]
    public class LoginViewModelTests
    {
        LoginViewModel loginViewModel;

        ISettings settings;

        [SetUp]
        public void SetUp()
        {
            Test.SetUp();

            settings = ServiceContainer.Resolve<ISettings>();

            loginViewModel = new LoginViewModel();
        }

        [Test]
        public async Task LoginSuccessfully()
        {
            loginViewModel.Username = "testuser";

            loginViewModel.Password = "password";

            await loginViewModel.Login();

            Assert.That(settings.User, Is.Not.Null);
        }

        [Test]
        public void LoginWithNoUsernameOrPassword()
        {
            /* Throw an exception */
            var exception = Assert.ThrowsAsync<Exception>(async () => await loginViewModel.Login());

            Assert.That(exception.Message, Is.EqualTo("Username is blank."));
        }
    }
}
