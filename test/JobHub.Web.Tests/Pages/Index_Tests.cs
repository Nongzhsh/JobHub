using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace JobHub.Pages
{
    public class Index_Tests : JobHubWebTestBase
    {
        [Fact]
        public async Task Welcome_Page()
        {
            var response = await GetResponseAsStringAsync("/");
            response.ShouldNotBeNull();
        }
    }
}
