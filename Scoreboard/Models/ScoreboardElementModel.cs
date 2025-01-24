using System.ComponentModel.DataAnnotations.Schema;

namespace Scoreboard.Models
{
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
        public double? PosX { get; set; }
        public double? PosY { get; set; }
        public string? HorizontalAlignment { get; set; } // left, center, right
        public int? HorizontalOffset { get; set; }

        // Navigation
        public ICollection<ScoreboardSetElementModel> SetElements { get; set; }
            = new List<ScoreboardSetElementModel>();

    }
}