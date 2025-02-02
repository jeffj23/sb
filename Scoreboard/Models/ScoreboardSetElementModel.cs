using System.ComponentModel.DataAnnotations.Schema;

namespace Scoreboard.Models;

[Table("ScoreboardSetElements")]
public class ScoreboardSetElementModel
{
    public int SetId { get; set; }
    public ScoreboardSetModel Set { get; set; }

    public int ElementId { get; set; }
    public ScoreboardElementModel Element { get; set; }

}