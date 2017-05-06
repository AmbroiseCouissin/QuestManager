using System.Collections.Generic;
using System.Linq;

namespace QuestManager.Models
{
    public class QuestConfiguration
    {
        public decimal RateFromBet { get; set; }
        public decimal LevelBonusRate { get; set; }
        public decimal TotalPointNeededToComplete { get; set; }
        public IEnumerable<Milestone> Milestones { get; set; } = Enumerable.Empty<Milestone>();
    }

    public class Milestone
    {
        public int MilestoneIndex { get; set; }
        public decimal PointsNeededToComplete { get; set; }
        public decimal ChipAwarded { get; set; }
    }
}
