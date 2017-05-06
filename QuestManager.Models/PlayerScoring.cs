using System.Collections.Generic;
using System.Linq;

namespace QuestManager.Models
{
    public class PlayerScoring
    {
        public string PlayerId { get; set; }
        public decimal Score { get; set; }
        public IEnumerable<int> CompletedMilestoneIndices { get; set; } = Enumerable.Empty<int>();
    }
}
