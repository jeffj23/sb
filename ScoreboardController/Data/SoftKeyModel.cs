using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SoftKey
{
    [Key]
    public int Id { get; set; } // Unique identifier

    [Required]
    public int SetId { get; set; } // Foreign key to ScoreboardSets

    [Required]
    [MaxLength(100)]
    public string Text { get; set; } // Text displayed on the button

    [Required]
    [MaxLength(20)]
    public string TextColor { get; set; } // Text color (e.g., "White", "#FFFFFF")

    [Required]
    [MaxLength(20)]
    public string BackgroundColor { get; set; } // Background color (e.g., "Blue", "#0000FF")

    [Required]
    [MaxLength(50)]
    public string Element { get; set; } // Name of the scoreboard element

    [Required]
    [MaxLength(50)]
    public string CommandType { get; set; } // Action type (e.g., "Increment", "Start")

    public int? Value { get; set; } // Optional value (e.g., 3 for "Home Score + 3")

    [ForeignKey("SetId")]
    public virtual ScoreboardSet ScoreboardSet { get; set; } // Navigation property to ScoreboardSets
}

public class ScoreboardSet
{
    [Key]
    public int SetId { get; set; } // Unique identifier for the scoreboard set

    [Required]
    [MaxLength(50)]
    public string SetName { get; set; } // Name of the set (e.g., "Football", "Basketball")
}