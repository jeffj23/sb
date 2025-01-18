using System.ComponentModel.DataAnnotations.Schema;

namespace Scoreboard.Models
{
    [Table("ScoreboardSetElements")]
    public class ScoreboardSetElementModel
    {
        public int SetId { get; set; }
        public ScoreboardSetModel Set { get; set; }

        public int ElementId { get; set; }
        public ScoreboardElementModel Element { get; set; }

        // Optional: override certain positions or properties for this element 
        // when in this set:
        // public double? OverridePosX { get; set; }
        // public double? OverridePosY { get; set; }
    }

    [Table("ScoreboardElements")]
    public class ScoreboardElementModel
    {
        public int Id { get; set; }
        public string ElementName { get; set; } = string.Empty;
        public string ElementType { get; set; } = string.Empty;
        public int NumDigits { get; set; }
        public double BulbSize { get; set; }
        public string BulbOnColor { get; set; } = string.Empty;
        public string BulbOffColor { get; set; } = string.Empty;
        public double PosX { get; set; }
        public double PosY { get; set; }

        // Navigation
        public ICollection<ScoreboardSetElementModel> SetElements { get; set; }
            = new List<ScoreboardSetElementModel>();
    }

    [Table("ScoreboardSets")]
    public class ScoreboardSetModel
    {
        public int SetId { get; set; }
        public string SetName { get; set; } = string.Empty;

        // Navigation
        public ICollection<ScoreboardSetElementModel> SetElements { get; set; }
            = new List<ScoreboardSetElementModel>();
    }

}