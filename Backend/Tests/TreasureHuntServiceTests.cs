using TreasureHuntApi.Models;
using TreasureHuntApi.Services;

namespace TreasureHuntApi.Tests
{
    public class TreasureHuntServiceTests
    {
        private readonly TreasureHuntService _service;

        public TreasureHuntServiceTests()
        {
            _service = new TreasureHuntService();
        }

        public void TestExample1()
        {
            // Test case 1: Expected result 4√2 ≈ 5.65685
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

            var result = _service.SolveTreasureHunt(request);
            
            Console.WriteLine($"Test 1 - Expected: 5.65685, Actual: {result.MinimumFuel:F5}");
            Console.WriteLine($"Success: {result.Success}");
            if (result.Path != null)
            {
                Console.WriteLine("Path:");
                foreach (var pos in result.Path)
                {
                    Console.WriteLine($"  Step: ({pos.Row}, {pos.Col}) - Chest: {pos.ChestNumber}");
                }
            }
            Console.WriteLine();
        }

        public void TestExample2()
        {
            // Test case 2: Expected result 5
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

            var result = _service.SolveTreasureHunt(request);
            
            Console.WriteLine($"Test 2 - Expected: 5, Actual: {result.MinimumFuel:F5}");
            Console.WriteLine($"Success: {result.Success}");
            if (result.Path != null)
            {
                Console.WriteLine("Path:");
                foreach (var pos in result.Path)
                {
                    Console.WriteLine($"  Step: ({pos.Row}, {pos.Col}) - Chest: {pos.ChestNumber}");
                }
            }
            Console.WriteLine();
        }

        public void TestExample3()
        {
            // Test case 3: Expected result 11
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

            var result = _service.SolveTreasureHunt(request);
            
            Console.WriteLine($"Test 3 - Expected: 11, Actual: {result.MinimumFuel:F5}");
            Console.WriteLine($"Success: {result.Success}");
            if (result.Path != null)
            {
                Console.WriteLine("Path:");
                foreach (var pos in result.Path)
                {
                    Console.WriteLine($"  Step: ({pos.Row}, {pos.Col}) - Chest: {pos.ChestNumber}");
                }
            }
            Console.WriteLine();
        }

        public static void RunAllTests()
        {
            var tests = new TreasureHuntServiceTests();
            
            Console.WriteLine("=== Treasure Hunt Algorithm Tests ===");
            Console.WriteLine();
            
            tests.TestExample1();
            tests.TestExample2();
            tests.TestExample3();
            
            Console.WriteLine("=== Tests Completed ===");
        }
    }
} 