namespace TreasureHuntApi.Models
{
    public class TreasureHuntResponse
    {
        public double MinimumFuel { get; set; }
        public List<Position> Path { get; set; } = new();
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class Position
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int ChestNumber { get; set; }
        
        public Position(int row, int col, int chestNumber)
        {
            Row = row;
            Col = col;
            ChestNumber = chestNumber;
        }
    }
} 