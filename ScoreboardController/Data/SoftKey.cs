using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;
using Microsoft.IdentityModel.Tokens;

public class SoftKey
{
    [Key]
    public int Id { get; set; } // Unique identifier

    [Required]
    [MaxLength(20)]
    public string Tag { get; set; }

    [Required]
    public int Position { get; set; }

    [Required]
    public int SetId { get; set; } // Foreign key to ScoreboardSets

    [MaxLength(100)]
    public string? Text { get; set; } // Text displayed on the button

    [MaxLength(20)]
    public string? TextColor { get; set; } // Text color (e.g., "White", "#FFFFFF")

    [MaxLength(20)]
    public string? BackgroundColor { get; set; } // Background color (e.g., "Blue", "#0000FF")

    [MaxLength(50)]
    public string? Element { get; set; } // Name of the scoreboard element

    [MaxLength(50)]
    public string? CommandType { get; set; } // Action type (e.g., "Increment", "Start")

    [MaxLength(20)]
    public string? Value { get; set; } // Optional value (e.g., 3 for "Home Score + 3")

    [ForeignKey("SetId")]
    public virtual ScoreboardSet ScoreboardSet { get; set; } // Navigation property to ScoreboardSets

    public Visibility Visibility
    {
        get
        {
            return (!Text.IsNullOrEmpty()) ? Visibility.Visible : Visibility.Hidden;
        }
    }

}