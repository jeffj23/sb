using System.ComponentModel.DataAnnotations.Schema;

namespace Scoreboard.Models;

[Table("ScoreboardSets")]
public class ScoreboardSetModel
{
    public int SetId { get; set; }
    public string SetName { get; set; } = string.Empty;

    // Navigation
    public ICollection<ScoreboardSetElementModel> SetElements { get; set; }
        = new List<ScoreboardSetElementModel>();
}