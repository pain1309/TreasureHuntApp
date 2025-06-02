using TreasureHuntApi.Models;

namespace TreasureHuntApi.Services
{
    public class OptimalTreasureHuntService
    {
        public TreasureHuntResponse SolveTreasureHunt(TreasureHuntRequest request)
        {
            try
            {
                // Use a simpler but more efficient approach
                // TSP-like solution with constraints
                var result = SolveOptimal(request);

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

        private (double fuel, List<Position> path) SolveOptimal(TreasureHuntRequest request)
        {
            var matrix = request.Matrix;
            var n = request.N;
            var m = request.M;
            var p = request.P;

            // Find positions of all chests
            var chestPositions = new Dictionary<int, List<(int row, int col)>>();
            
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    int chestNumber = matrix[i][j];
                    var pos = (i + 1, j + 1); // Convert to 1-indexed
                    
                    if (!chestPositions.ContainsKey(chestNumber))
                        chestPositions[chestNumber] = new List<(int row, int col)>();
                    chestPositions[chestNumber].Add(pos);
                }
            }

            // For small p, use brute force with all permutations
            if (p <= 8)
            {
                return SolveBruteForce(chestPositions, p);
            }
            else
            {
                // For larger p, use improved greedy
                return SolveImprovedGreedy(chestPositions, p);
            }
        }

        private (double fuel, List<Position> path) SolveBruteForce(
            Dictionary<int, List<(int row, int col)>> chestPositions, int p)
        {
            double minFuel = double.MaxValue;
            List<Position> bestPath = null;

            // Generate all valid collection orders
            var chestOrder = Enumerable.Range(1, p).ToArray();
            
            // Try all permutations of chest positions
            var bestCombination = FindBestCombination(chestPositions, chestOrder);
            
            return bestCombination;
        }

        private (double fuel, List<Position> path) FindBestCombination(
            Dictionary<int, List<(int row, int col)>> chestPositions, int[] chestOrder)
        {
            double minFuel = double.MaxValue;
            List<Position> bestPath = null;

            // Use dynamic programming for optimal path through selected positions
            var positions = new List<(int row, int col, int chest)>();
            positions.Add((1, 1, 0)); // Starting position

            // Try all combinations of chest positions
            GenerateCombinations(chestPositions, chestOrder, 0, new List<(int, int, int)>(), 
                ref minFuel, ref bestPath, positions);

            return (minFuel, bestPath ?? new List<Position>());
        }

        private void GenerateCombinations(
            Dictionary<int, List<(int row, int col)>> chestPositions,
            int[] chestOrder, int index, List<(int row, int col, int chest)> currentPath,
            ref double minFuel, ref List<Position> bestPath, List<(int row, int col, int chest)> basePath)
        {
            if (index == chestOrder.Length)
            {
                // Calculate fuel for this path
                var (fuel, path) = CalculatePathFuel(basePath.Concat(currentPath).ToList());
                if (fuel < minFuel)
                {
                    minFuel = fuel;
                    bestPath = path;
                }
                return;
            }

            int chestNum = chestOrder[index];
            foreach (var pos in chestPositions[chestNum])
            {
                currentPath.Add((pos.row, pos.col, chestNum));
                GenerateCombinations(chestPositions, chestOrder, index + 1, currentPath, 
                    ref minFuel, ref bestPath, basePath);
                currentPath.RemoveAt(currentPath.Count - 1);
            }
        }

        private (double fuel, List<Position> path) CalculatePathFuel(List<(int row, int col, int chest)> positions)
        {
            var path = new List<Position>();
            double totalFuel = 0;

            // Use TSP-like approach to find optimal visiting order
            var unvisited = positions.Skip(1).ToList(); // Skip starting position
            var current = positions[0];
            path.Add(new Position(current.row, current.col, current.chest));

            while (unvisited.Count > 0)
            {
                // Find next best position considering constraints
                int bestIndex = FindNextBestPosition(current, unvisited, path);
                var next = unvisited[bestIndex];
                
                double distance = CalculateDistance((current.row, current.col), (next.row, next.col));
                totalFuel += distance;
                
                path.Add(new Position(next.row, next.col, next.chest));
                current = next;
                unvisited.RemoveAt(bestIndex);
            }

            return (totalFuel, path);
        }

        private int FindNextBestPosition((int row, int col, int chest) current, 
            List<(int row, int col, int chest)> unvisited, List<Position> pathSoFar)
        {
            // Find next position that can be legally collected
            int bestIndex = -1;
            double minDistance = double.MaxValue;
            
            for (int i = 0; i < unvisited.Count; i++)
            {
                var candidate = unvisited[i];
                
                // Check if we can collect this chest (have the required key)
                if (CanCollectChest(candidate.chest, pathSoFar))
                {
                    double distance = CalculateDistance((current.row, current.col), (candidate.row, candidate.col));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        bestIndex = i;
                    }
                }
            }
            
            // If no legal move found, find minimum distance (should not happen with valid input)
            if (bestIndex == -1)
            {
                for (int i = 0; i < unvisited.Count; i++)
                {
                    double distance = CalculateDistance((current.row, current.col), (unvisited[i].row, unvisited[i].col));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        bestIndex = i;
                    }
                }
            }
            
            return bestIndex;
        }

        private bool CanCollectChest(int chestNumber, List<Position> pathSoFar)
        {
            if (chestNumber == 1) return true; // Can always collect chest 1 with key 0
            
            // Check if we have collected chest (chestNumber - 1)
            return pathSoFar.Any(p => p.ChestNumber == chestNumber - 1);
        }

        private (double fuel, List<Position> path) SolveImprovedGreedy(
            Dictionary<int, List<(int row, int col)>> chestPositions, int p)
        {
            // Improved greedy: consider multiple steps ahead
            var path = new List<Position>();
            double totalFuel = 0;
            
            var currentPos = (row: 1, col: 1);
            path.Add(new Position(currentPos.row, currentPos.col, 0));

            for (int targetChest = 1; targetChest <= p; targetChest++)
            {
                var candidates = chestPositions[targetChest];
                
                // Find best position considering next few chests
                var bestPos = FindBestPositionLookahead(currentPos, candidates, chestPositions, targetChest, p, 2);
                
                double distance = CalculateDistance(currentPos, bestPos);
                totalFuel += distance;
                currentPos = bestPos;
                path.Add(new Position(currentPos.row, currentPos.col, targetChest));
            }

            return (totalFuel, path);
        }

        private (int row, int col) FindBestPositionLookahead(
            (int row, int col) currentPos, List<(int row, int col)> candidates,
            Dictionary<int, List<(int row, int col)>> allChestPositions, int currentChest, int totalChests, int lookahead)
        {
            double bestScore = double.MaxValue;
            var bestPos = candidates[0];

            foreach (var candidate in candidates)
            {
                double score = CalculateDistance(currentPos, candidate);
                
                // Add lookahead cost
                if (currentChest < totalChests && lookahead > 0)
                {
                    var nextCandidates = allChestPositions.ContainsKey(currentChest + 1) 
                        ? allChestPositions[currentChest + 1] 
                        : new List<(int row, int col)>();
                    
                    if (nextCandidates.Any())
                    {
                        double minNextDistance = nextCandidates.Min(next => CalculateDistance(candidate, next));
                        score += minNextDistance * 0.5; // Weight future cost less
                    }
                }
                
                if (score < bestScore)
                {
                    bestScore = score;
                    bestPos = candidate;
                }
            }

            return bestPos;
        }

        private double CalculateDistance((int row, int col) pos1, (int row, int col) pos2)
        {
            return Math.Sqrt(Math.Pow(pos1.row - pos2.row, 2) + Math.Pow(pos1.col - pos2.col, 2));
        }
    }
} 