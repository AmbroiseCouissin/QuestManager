using System.Threading.Tasks;
using QuestManager.Models;

namespace QuestManager.Quests.Repositories.Json
{
    public class JsonQuestRepository : IQuestRepository
    {
        private readonly QuestConfiguration _questConfiguration;
        public JsonQuestRepository(QuestConfiguration questConfiguration)
        {
            _questConfiguration = questConfiguration;
        }

        public Task<QuestConfiguration> GetQuestAsync() => 
            Task.FromResult(_questConfiguration);
    }
}
