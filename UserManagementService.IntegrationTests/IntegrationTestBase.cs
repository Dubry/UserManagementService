using System.Net.Http;

namespace UserManagementService.IntegrationTests
{
    public abstract class IntegrationTestBase
    {
        protected readonly HttpClient Client;
        protected IntegrationTestBase(CustomWebApplicationFactory factory)
        {
            Client = factory.CreateClient();
            Client.DefaultRequestHeaders.Add("X-API-KEY", "test-api-key");
        }
    }
}
