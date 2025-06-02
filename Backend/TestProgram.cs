using TreasureHuntApi.Tests;

namespace TreasureHuntApi
{
    public class TestProgram
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "test")
            {
                TreasureHuntServiceTests.RunAllTests();
                return;
            }
            
            // Normal program execution
            var builder = WebApplication.CreateBuilder(args);
            // ... rest of Program.cs content
        }
    }
} 