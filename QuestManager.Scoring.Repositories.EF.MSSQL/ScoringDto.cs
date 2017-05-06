using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuestManager.Scoring.Repositories.EF.MSSQL
{
    public class ScoringDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string PlayerId { get; set; }
        public decimal Score { get; set; }
        public string CompletedMilestoneIndices { get; set; }
    }
}
