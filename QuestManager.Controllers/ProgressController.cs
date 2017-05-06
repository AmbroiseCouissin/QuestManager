using Microsoft.AspNetCore.Mvc;
using QuestManager.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuestManager.Quests.Repositories;
using QuestManager.Scoring.Repositories;
using QuestManager.Models;
using System.Linq;
using System.Collections.Generic;

namespace QuestManager.Controllers
{
    [Route("api/[controller]")]
    public class ProgressController : Controller
    {
        private readonly IQuestRepository _questRepository;
        private readonly IPlayerScoringRepository _playerScoringRepository;

        public ProgressController(
            IQuestRepository questRepository,
            IPlayerScoringRepository playerScoringRepository)
        {
            _questRepository = questRepository;
            _playerScoringRepository = playerScoringRepository;
        }

        [HttpPost]
        public async Task<IActionResult> GetProgressAsync([FromBody]ProgressQueryViewModel progressQuery)
        {
            // defensive code
            GuardClause(ModelState, progressQuery);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // get the quest configuration
            QuestConfiguration questConfiguration = await _questRepository.GetQuestAsync();

            // get current score
            PlayerScoring playerScoring = await _playerScoringRepository.GetPlayerScoringAsync(progressQuery.PlayerId);
            decimal totalQuestPointAccumulated = playerScoring.Score
                + (progressQuery.ChipAmountBet * questConfiguration.RateFromBet) 
                + (progressQuery.PlayerLevel * questConfiguration.LevelBonusRate);

            // new milestones reached?
            IEnumerable<Milestone> newCompletedMilestones = questConfiguration.Milestones
                .Where(m => 
                    !playerScoring.CompletedMilestoneIndices.Contains(m.MilestoneIndex) // non completed yet
                    && m.PointsNeededToComplete <= totalQuestPointAccumulated // new milestone achieved
                );

            // upsert the user's score and new completed milestones player score
            await _playerScoringRepository.UpsertPlayerScoringAsync(
                progressQuery.PlayerId, 
                totalQuestPointAccumulated,
                newCompletedMilestones?.Select(m => m.MilestoneIndex) ?? Enumerable.Empty<int>());

            return Ok(new ProgressViewModel
            {
                QuestPointsEarned = totalQuestPointAccumulated,
                TotalQuestPercentCompleted = totalQuestPointAccumulated / questConfiguration.TotalPointNeededToComplete * 100,
                MilestonesCompleted = questConfiguration.Milestones
                    .Where(m => playerScoring.CompletedMilestoneIndices.Contains(m.MilestoneIndex)) // already completed milestones
                    .Concat(newCompletedMilestones)
                    .Select(m => 
                        new MilestoneViewModel
                        {
                            MilestoneIndex = m.MilestoneIndex,
                            ChipsAwarded = m.ChipAwarded
                        })
            });
        }

        private void GuardClause(ModelStateDictionary modelState, ProgressQueryViewModel progressQuery)
        {
            if (progressQuery == null)
                ModelState.AddModelError("Invalid body", "The body is null or couldn't get deserialized.");
            else
                if (string.IsNullOrEmpty(progressQuery.PlayerId))
                    ModelState.AddModelError("Invalid body", "PlayerId can't be null not empty.");
        }
    }
}
