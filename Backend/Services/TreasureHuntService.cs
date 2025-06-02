using TreasureHuntApi.Models;

namespace TreasureHuntApi.Services
{
    public class TreasureHuntService
    {
        public TreasureHuntResponse SolveTreasureHunt(TreasureHuntRequest request)
        {
            try
            {
                // Validate input
                if (!ValidateInput(request))
                {
                    return new TreasureHuntResponse
                    {
                        Success = false,
                        ErrorMessage = "Invalid input parameters"
                    };
                }

                var matrix = request.Matrix;
                var n = request.N;
                var m = request.M;
                var p = request.P;

                // Find positions of all chests
                var chestPositions = FindChestPositions(matrix, n, m, p);

                // Validate that all chest numbers from 1 to p exist
                for (int i = 1; i <= p; i++)
                {
                    if (!chestPositions.ContainsKey(i))
                    {
                        return new TreasureHuntResponse
                        {
                            Success = false,
                            ErrorMessage = $"Chest number {i} not found in matrix"
                        };
                    }
                }

                // Calculate minimum fuel using greedy algorithm
                var result = CalculateMinimumFuel(chestPositions, p);

                return new TreasureHuntResponse
                {
                    Success = true,
                    MinimumFuel = result.fuel,
                    Path = result.path
                };
            }
            catch (Exception ex)
            {
                return new TreasureHuntResponse
                {
                    Success = false,
                    ErrorMessage = $"Error solving treasure hunt: {ex.Message}"
                };
            }
        }

        private bool ValidateInput(TreasureHuntRequest request)
        {
            if (request.N <= 0 || request.M <= 0 || request.P <= 0)
                return false;

            if (request.Matrix == null || request.Matrix.Length != request.N)
                return false;

            for (int i = 0; i < request.N; i++)
            {
                if (request.Matrix[i] == null || request.Matrix[i].Length != request.M)
                    return false;

                for (int j = 0; j < request.M; j++)
                {
                    if (request.Matrix[i][j] < 1 || request.Matrix[i][j] > request.P)
                        return false;
                }
            }

            return true;
        }

        private Dictionary<int, List<(int row, int col)>> FindChestPositions(int[][] matrix, int n, int m, int p)
        {
            var positions = new Dictionary<int, List<(int row, int col)>>();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    int chestNumber = matrix[i][j];
                    if (!positions.ContainsKey(chestNumber))
                    {
                        positions[chestNumber] = new List<(int row, int col)>();
                    }
                    positions[chestNumber].Add((i + 1, j + 1)); // Convert to 1-indexed
                }
            }

            return positions;
        }

        private (double fuel, List<Position> path) CalculateMinimumFuel(
            Dictionary<int, List<(int row, int col)>> chestPositions, int p)
        {
            var path = new List<Position>();
            double totalFuel = 0;
            
            // Start at position (1, 1) with key 0
            int currentRow = 1, currentCol = 1;
            path.Add(new Position(currentRow, currentCol, 0));

            // For each chest from 1 to p, find the nearest one
            for (int targetChest = 1; targetChest <= p; targetChest++)
            {
                var candidates = chestPositions[targetChest];
                double minDistance = double.MaxValue;
                (int row, int col) bestPosition = (0, 0);

                // Find the nearest chest with number targetChest
                foreach (var (row, col) in candidates)
                {
                    double distance = CalculateEuclideanDistance(currentRow, currentCol, row, col);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        bestPosition = (row, col);
                    }
                }

                // Move to the best position
                totalFuel += minDistance;
                currentRow = bestPosition.row;
                currentCol = bestPosition.col;
                path.Add(new Position(currentRow, currentCol, targetChest));
            }

            return (totalFuel, path);
        }

        private double CalculateEuclideanDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
    }
} 