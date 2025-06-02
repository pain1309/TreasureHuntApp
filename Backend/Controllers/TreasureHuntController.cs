using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TreasureHuntApi.Data;
using TreasureHuntApi.Models;
using TreasureHuntApi.Services;

namespace TreasureHuntApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreasureHuntController : ControllerBase
    {
        private readonly TreasureHuntService _greedyService;
        private readonly OptimalTreasureHuntService _optimalService;
        private readonly TreasureHuntContext _context;

        public TreasureHuntController(TreasureHuntService greedyService, OptimalTreasureHuntService optimalService, TreasureHuntContext context)
        {
            _greedyService = greedyService;
            _optimalService = optimalService;
            _context = context;
        }

        [HttpPost("solve")]
        public async Task<ActionResult<TreasureHuntResponse>> SolveTreasureHunt([FromBody] TreasureHuntRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Use optimal algorithm by default
            var response = _optimalService.SolveTreasureHunt(request);

            if (response.Success)
            {
                // Save to database
                var treasureMatrix = new TreasureMatrix
                {
                    N = request.N,
                    M = request.M,
                    P = request.P,
                    MatrixData = JsonSerializer.Serialize(request.Matrix),
                    Result = response.MinimumFuel,
                    SolutionPath = JsonSerializer.Serialize(response.Path),
                    CreatedAt = DateTime.UtcNow
                };

                _context.TreasureMatrices.Add(treasureMatrix);
                await _context.SaveChangesAsync();
            }

            return Ok(response);
        }

        [HttpPost("solve/greedy")]
        public async Task<ActionResult<TreasureHuntResponse>> SolveTreasureHuntGreedy([FromBody] TreasureHuntRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Use greedy algorithm
            var response = _greedyService.SolveTreasureHunt(request);

            if (response.Success)
            {
                // Save to database with a note that it's greedy
                var treasureMatrix = new TreasureMatrix
                {
                    N = request.N,
                    M = request.M,
                    P = request.P,
                    MatrixData = JsonSerializer.Serialize(request.Matrix),
                    Result = response.MinimumFuel,
                    SolutionPath = JsonSerializer.Serialize(response.Path),
                    CreatedAt = DateTime.UtcNow
                };

                _context.TreasureMatrices.Add(treasureMatrix);
                await _context.SaveChangesAsync();
            }

            return Ok(response);
        }

        [HttpPost("compare")]
        public async Task<ActionResult<object>> CompareBothAlgorithms([FromBody] TreasureHuntRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var greedyResponse = _greedyService.SolveTreasureHunt(request);
            var optimalResponse = _optimalService.SolveTreasureHunt(request);

            var comparison = new
            {
                Input = new { request.N, request.M, request.P, request.Matrix },
                Greedy = new 
                { 
                    MinimumFuel = greedyResponse.MinimumFuel,
                    Path = greedyResponse.Path,
                    Success = greedyResponse.Success,
                    ErrorMessage = greedyResponse.ErrorMessage
                },
                Optimal = new 
                { 
                    MinimumFuel = optimalResponse.MinimumFuel,
                    Path = optimalResponse.Path,
                    Success = optimalResponse.Success,
                    ErrorMessage = optimalResponse.ErrorMessage
                },
                Improvement = optimalResponse.Success && greedyResponse.Success 
                    ? $"{((greedyResponse.MinimumFuel - optimalResponse.MinimumFuel) / greedyResponse.MinimumFuel * 100):F2}%"
                    : "N/A"
            };

            // Save optimal result to database
            if (optimalResponse.Success)
            {
                var treasureMatrix = new TreasureMatrix
                {
                    N = request.N,
                    M = request.M,
                    P = request.P,
                    MatrixData = JsonSerializer.Serialize(request.Matrix),
                    Result = optimalResponse.MinimumFuel,
                    SolutionPath = JsonSerializer.Serialize(optimalResponse.Path),
                    CreatedAt = DateTime.UtcNow
                };

                _context.TreasureMatrices.Add(treasureMatrix);
                await _context.SaveChangesAsync();
            }

            return Ok(comparison);
        }

        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<TreasureMatrix>>> GetHistory()
        {
            var history = await _context.TreasureMatrices
                .OrderByDescending(x => x.CreatedAt)
                .Take(50)
                .ToListAsync();

            return Ok(history);
        }

        [HttpGet("history/{id}")]
        public async Task<ActionResult<TreasureMatrix>> GetHistoryItem(int id)
        {
            var item = await _context.TreasureMatrices.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpDelete("history/{id}")]
        public async Task<IActionResult> DeleteHistoryItem(int id)
        {
            var item = await _context.TreasureMatrices.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            _context.TreasureMatrices.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("replay/{id}")]
        public async Task<ActionResult<TreasureHuntResponse>> ReplayFromHistory(int id)
        {
            var item = await _context.TreasureMatrices.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            try
            {
                var matrix = JsonSerializer.Deserialize<int[][]>(item.MatrixData);
                var request = new TreasureHuntRequest
                {
                    N = item.N,
                    M = item.M,
                    P = item.P,
                    Matrix = matrix ?? Array.Empty<int[]>()
                };

                // Use optimal algorithm for replay
                var response = _optimalService.SolveTreasureHunt(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error replaying solution: {ex.Message}");
            }
        }

        [HttpGet("test")]
        public async Task<ActionResult<object>> RunTests()
        {
            var testResults = new List<object>();

            // Test case 1
            var test1 = new TreasureHuntRequest
            {
                N = 3, M = 3, P = 3,
                Matrix = new int[][] {
                    new int[] { 3, 2, 2 },
                    new int[] { 2, 2, 2 },
                    new int[] { 2, 2, 1 }
                }
            };

            var greedy1 = _greedyService.SolveTreasureHunt(test1);
            var optimal1 = _optimalService.SolveTreasureHunt(test1);

            testResults.Add(new {
                TestCase = 1,
                Expected = 4 * Math.Sqrt(2),
                Greedy = greedy1.MinimumFuel,
                Optimal = optimal1.MinimumFuel,
                GreedyPath = greedy1.Path,
                OptimalPath = optimal1.Path
            });

            // Test case 2
            var test2 = new TreasureHuntRequest
            {
                N = 3, M = 4, P = 3,
                Matrix = new int[][] {
                    new int[] { 2, 1, 1, 1 },
                    new int[] { 1, 1, 1, 1 },
                    new int[] { 2, 1, 1, 3 }
                }
            };

            var greedy2 = _greedyService.SolveTreasureHunt(test2);
            var optimal2 = _optimalService.SolveTreasureHunt(test2);

            testResults.Add(new {
                TestCase = 2,
                Expected = 5.0,
                Greedy = greedy2.MinimumFuel,
                Optimal = optimal2.MinimumFuel,
                GreedyPath = greedy2.Path,
                OptimalPath = optimal2.Path
            });

            // Test case 3
            var test3 = new TreasureHuntRequest
            {
                N = 3, M = 4, P = 12,
                Matrix = new int[][] {
                    new int[] { 1, 2, 3, 4 },
                    new int[] { 8, 7, 6, 5 },
                    new int[] { 9, 10, 11, 12 }
                }
            };

            var greedy3 = _greedyService.SolveTreasureHunt(test3);
            var optimal3 = _optimalService.SolveTreasureHunt(test3);

            testResults.Add(new {
                TestCase = 3,
                Expected = 11.0,
                Greedy = greedy3.MinimumFuel,
                Optimal = optimal3.MinimumFuel,
                GreedyPath = greedy3.Path,
                OptimalPath = optimal3.Path
            });

            return Ok(testResults);
        }
    }
} 