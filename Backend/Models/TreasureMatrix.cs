using System.ComponentModel.DataAnnotations;

namespace TreasureHuntApi.Models
{
    public class TreasureMatrix
    {
        public int Id { get; set; }
        
        [Required]
        [Range(1, 500)]
        public int N { get; set; }
        
        [Required]
        [Range(1, 500)]
        public int M { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int P { get; set; }
        
        [Required]
        public string MatrixData { get; set; } = string.Empty;
        
        public double Result { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public string? SolutionPath { get; set; }
    }
} 