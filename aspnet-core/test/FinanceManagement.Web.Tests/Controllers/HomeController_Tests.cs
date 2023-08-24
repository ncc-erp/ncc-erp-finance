using System.Threading.Tasks;
using FinanceManagement.Models.TokenAuth;
using FinanceManagement.Web.Controllers;
using Shouldly;
using Xunit;

namespace FinanceManagement.Web.Tests.Controllers
{
    public class HomeController_Tests: FinanceManagementWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "admin"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}