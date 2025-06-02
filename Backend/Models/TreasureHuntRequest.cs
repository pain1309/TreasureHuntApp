using System.ComponentModel.DataAnnotations;

namespace TreasureHuntApi.Models
{
    public class TreasureHuntRequest
    {
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
        public int[][] Matrix { get; set; } = Array.Empty<int[]>();
    }
} 