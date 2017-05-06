using Microsoft.AspNetCore.Mvc;
using Moq;
using QuestManager.Models;
using QuestManager.Quests.Repositories;
using QuestManager.Scoring.Repositories;
using QuestManager.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace QuestManager.Controllers.UnitTests
{
    public class ProgressControllerUnitTests
    {
        [Fact]
        public async Task AcceptanceTest()
        {
            // arrange
            var progressQuery = new ProgressQueryViewModel
            {
                PlayerId = "Ross",
                PlayerLevel = 4,
                ChipAmountBet = 600
            };

            var questRepositoryMock = new Mock<IQuestRepository>();
            questRepositoryMock
                .Setup(qr => qr.GetQuestAsync())
                .ReturnsAsync(new QuestConfiguration
                {
                    RateFromBet = 0.1m,
                    LevelBonusRate = 0.2m,
                    TotalPointNeededToComplete = 1000,
                    Milestones = new List<Milestone>
                    {
                        new Milestone { PointsNeededToComplete = 250, MilestoneIndex = 0, ChipAwarded = 100 },
                        new Milestone { PointsNeededToComplete = 500, MilestoneIndex = 1, ChipAwarded = 150 },
                        new Milestone { PointsNeededToComplete = 750, MilestoneIndex = 2, ChipAwarded = 200 },
                    }
                });

            var playerScoringRepositoryMock = new Mock<IPlayerScoringRepository>();
            playerScoringRepositoryMock
                .Setup(psr => psr.GetPlayerScoringAsync(It.IsAny<string>()))
                .ReturnsAsync((string playerId) => new PlayerScoring { PlayerId = playerId, CompletedMilestoneIndices = new List<int> { }, Score = 0 });
            playerScoringRepositoryMock
                .Setup(psr => psr.UpsertPlayerScoringAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<IEnumerable<int>>()))
                .Returns(Task.FromResult(0));

            var controller = new ProgressController(questRepositoryMock.Object, playerScoringRepositoryMock.Object);

            // act
            var actionResult = await controller.GetProgressAsync(progressQuery) as OkObjectResult;

            // assert
            Assert.NotNull(actionResult);
            Assert.Equal(200, actionResult.StatusCode);
            ProgressViewModel value = actionResult.Value as ProgressViewModel;
            Assert.NotNull(value);

            Assert.Equal(60.8m, value.QuestPointsEarned);
            Assert.Equal(6.08m, value.TotalQuestPercentCompleted);
        }
    }
}
