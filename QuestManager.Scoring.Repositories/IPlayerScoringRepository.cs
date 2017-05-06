using QuestManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuestManager.Scoring.Repositories
{
    public interface IPlayerScoringRepository
    {
        Task<PlayerScoring> GetPlayerScoringAsync(string playerId);
        Task UpsertPlayerScoringAsync(string playerId, decimal points, IEnumerable<int> milestoneIndices);
    }
}
