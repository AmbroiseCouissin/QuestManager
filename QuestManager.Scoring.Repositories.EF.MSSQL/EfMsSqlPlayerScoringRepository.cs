using System.Collections.Generic;
using System.Threading.Tasks;
using QuestManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace QuestManager.Scoring.Repositories.EF.MSSQL
{
    public class EfMsSqlPlayerScoringRepository : IPlayerScoringRepository
    {
        private readonly ScoringDbContext _context;
        public EfMsSqlPlayerScoringRepository(ScoringDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetPlayerScoreAsync(string playerId) =>
            (await _context.Scorings.FirstOrDefaultAsync())?.Score ?? 0;

        public async Task<PlayerScoring> GetPlayerScoringAsync(string playerId)
        {
            ScoringDto scoringDto = await _context.Scorings.FindAsync(playerId);
            PlayerScoring scoring;
            if (scoringDto != null)
                scoring = new PlayerScoring
                {
                    PlayerId = scoringDto.PlayerId,
                    Score = scoringDto.Score,
                    CompletedMilestoneIndices = 
                        scoringDto.CompletedMilestoneIndices?.Split(',').Select(m => int.Parse(m)) 
                        ?? Enumerable.Empty<int>()
                };
            else
            {
                scoring = new PlayerScoring { PlayerId = playerId };
            }

            return scoring;
        }

        public async Task UpsertPlayerScoringAsync(string playerId, decimal points, IEnumerable<int> milestoneIndices)
        {
            try
            {
                // get scoring
                ScoringDto scoringDto = await _context.Scorings.FindAsync(playerId);
                if (scoringDto != null)
                {
                    scoringDto.Score = points;
                    if (milestoneIndices != null && milestoneIndices.Any())
                        scoringDto.CompletedMilestoneIndices = string.Join(",", milestoneIndices.Select(m => m.ToString()));
                }
                else
                    await _context.Scorings.AddAsync(new ScoringDto { PlayerId = playerId, Score = points, });

                await _context.SaveChangesAsync();
            }
            catch
            {
            }
        }
    }
}
