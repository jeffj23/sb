using System.ComponentModel.DataAnnotations;

public class ScoreboardSet
{
    [Key]
    public int SetId { get; set; } // Unique identifier for the scoreboard set

    [Required]
    [MaxLength(50)]
    public string SetName { get; set; } // Name of the set (e.g., "Football", "Basketball")
}