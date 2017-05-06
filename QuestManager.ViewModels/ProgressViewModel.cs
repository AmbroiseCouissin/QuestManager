using System.Collections.Generic;
using System.Linq;

namespace QuestManager.ViewModels
{
    public class ProgressViewModel
    {
        public decimal QuestPointsEarned { get; set; }
        public decimal TotalQuestPercentCompleted { get; set; }
        public IEnumerable<MilestoneViewModel> MilestonesCompleted { get; set; } = Enumerable.Empty<MilestoneViewModel>();
    }

    public class MilestoneViewModel
    {
        public int MilestoneIndex { get; set; }
        public decimal ChipsAwarded { get; set; }
    }
}
