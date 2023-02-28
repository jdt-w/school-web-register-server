using Microsoft.AspNetCore.Hosting;
using Moq;

namespace SchoolWebRegister.Tests.IntegrationTests
{
    public class AuthenticationTests
    {
        private readonly HttpClient _client;
        public AuthenticationTests()
        {
            //var server = new TestServer(new WebHostBuilder().UseEnvironment("Development"));
            //_client = server.CreateClient();
        }

        [Fact]
        public void Is_Authenticated_With_Valid_Credentials()
        {

        }


        [Fact]
        public void Unable_To_Authenticate_With_Invalid_Credentials()
        {

        }
    }
}