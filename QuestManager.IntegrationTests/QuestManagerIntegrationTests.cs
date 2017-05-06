using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using QuestManager.Sdk;
using QuestManager.ViewModels;
using System.Threading.Tasks;
using Xunit;

namespace QuestManager.IntegrationTests
{
    public class QuestManagerIntegrationTests
    {
        private readonly TestServer _server;
        private readonly IQuestManagerSdk _sdk;

        public QuestManagerIntegrationTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _sdk = new QuestManagerSdk(_server.CreateClient());
        }

        [Fact]
        public async Task AcceptanceIntegrationTest()
        {
            // arrange 
            var progressQuery = new ProgressQueryViewModel
            {
                PlayerId = "Ross",
                PlayerLevel = 4,
                ChipAmountBet = 600
            };

            // act
            ProgressViewModel progress = await _sdk.GetProgressAsync(progressQuery);

            // assert
            Assert.NotNull(progress);
        }
    }
}
