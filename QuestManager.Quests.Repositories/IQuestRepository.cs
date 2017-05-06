using QuestManager.Models;
using System.Threading.Tasks;

namespace QuestManager.Quests.Repositories
{
    public interface IQuestRepository
    {
        Task<QuestConfiguration> GetQuestAsync();  
    }
}
