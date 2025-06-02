using TreasureHuntApi.Models;
using TreasureHuntApi.Services;

namespace TreasureHuntApi.Tests
{
    public class OptimalAlgorithmTests
    {
        private readonly TreasureHuntService _greedyService;
        private readonly OptimalTreasureHuntService _optimalService;

        public OptimalAlgorithmTests()
        {
            _greedyService = new TreasureHuntService();
            _optimalService = new OptimalTreasureHuntService();
        }

        public void CompareAlgorithms()
        {
            Console.WriteLine("=== Comparing Greedy vs Optimal DP Algorithms ===\n");

            TestExample1();
            TestExample2();
            TestExample3();
        }

        private void TestExample1()
        {
            Console.WriteLine("--- Test 1 ---");
            var request = new TreasureHuntRequest
            {
                N = 3,
                M = 3,
                P = 3,
                Matrix = new int[][]
                {
                    new int[] { 3, 2, 2 },
                    new int[] { 2, 2, 2 },
                    new int[] { 2, 2, 1 }
                }
            };

            var greedyResult = _greedyService.SolveTreasureHunt(request);
            var optimalResult = _optimalService.SolveTreasureHunt(request);

            Console.WriteLine($"Expected Result: 4√2 = {4 * Math.Sqrt(2):F5}");
            Console.WriteLine($"Greedy Result:   {greedyResult.MinimumFuel:F5}");
            Console.WriteLine($"Optimal Result:  {optimalResult.MinimumFuel:F5}");
            
            Console.WriteLine("\nGreedy Path:");
            foreach (var pos in greedyResult.Path)
                Console.WriteLine($"  ({pos.Row}, {pos.Col}) - Chest: {pos.ChestNumber}");
                
            Console.WriteLine("\nOptimal Path:");
            foreach (var pos in optimalResult.Path)
                Console.WriteLine($"  ({pos.Row}, {pos.Col}) - Chest: {pos.ChestNumber}");
                
            Console.WriteLine();
        }

        private void TestExample2()
        {
            Console.WriteLine("--- Test 2 ---");
            var request = new TreasureHuntRequest
            {
                N = 3,
                M = 4,
                P = 3,
                Matrix = new int[][]
                {
                    new int[] { 2, 1, 1, 1 },
                    new int[] { 1, 1, 1, 1 },
                    new int[] { 2, 1, 1, 3 }
                }
            };

            var greedyResult = _greedyService.SolveTreasureHunt(request);
            var optimalResult = _optimalService.SolveTreasureHunt(request);

            Console.WriteLine($"Expected Result: 5.00000");
            Console.WriteLine($"Greedy Result:   {greedyResult.MinimumFuel:F5}");
            Console.WriteLine($"Optimal Result:  {optimalResult.MinimumFuel:F5}");
            
            Console.WriteLine("\nGreedy Path:");
            foreach (var pos in greedyResult.Path)
                Console.WriteLine($"  ({pos.Row}, {pos.Col}) - Chest: {pos.ChestNumber}");
                
            Console.WriteLine("\nOptimal Path:");
            foreach (var pos in optimalResult.Path)
                Console.WriteLine($"  ({pos.Row}, {pos.Col}) - Chest: {pos.ChestNumber}");
                
            Console.WriteLine();
        }

        private void TestExample3()
        {
            Console.WriteLine("--- Test 3 ---");
            var request = new TreasureHuntRequest
            {
                N = 3,
                M = 4,
                P = 12,
                Matrix = new int[][]
                {
                    new int[] { 1, 2, 3, 4 },
                    new int[] { 8, 7, 6, 5 },
                    new int[] { 9, 10, 11, 12 }
                }
            };

            var greedyResult = _greedyService.SolveTreasureHunt(request);
            var optimalResult = _optimalService.SolveTreasureHunt(request);

            Console.WriteLine($"Expected Result: 11.00000");
            Console.WriteLine($"Greedy Result:   {greedyResult.MinimumFuel:F5}");
            Console.WriteLine($"Optimal Result:  {optimalResult.MinimumFuel:F5}");
            
            Console.WriteLine("\nGreedy Path:");
            foreach (var pos in greedyResult.Path)
                Console.WriteLine($"  ({pos.Row}, {pos.Col}) - Chest: {pos.ChestNumber}");
                
            Console.WriteLine("\nOptimal Path:");
            foreach (var pos in optimalResult.Path)
                Console.WriteLine($"  ({pos.Row}, {pos.Col}) - Chest: {pos.ChestNumber}");
                
            Console.WriteLine();
        }
    }
} 