using LotteryAPI.Calculating;
using LotteryAPI.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LotteryAPI.Controllers
{
    [ApiController]
    [Route("hungarian_five_lottery/[controller]")]
    public class HungarianFiveLotteryController : ControllerBase
    {
        private readonly ILogger<HungarianFiveLotteryController> _logger;
        private readonly DataContext _context;

        public HungarianFiveLotteryController(DataContext context, ILogger<HungarianFiveLotteryController> logger)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("{year}")]
        public async Task<ActionResult<HungarianFiveLotteryResults>> Get(int year)
        {
            _logger.LogInformation("API CALL : hungarian_five_lottery_results");
            var result = await _context.HungarianFiveLotteryResults.Where(x => x.Year == year).ToArrayAsync();
            if (!result.Any())
            {
                return NoContent();
            }
            return Ok(result);
        }

        [HttpGet("calculate")]
        public async Task<ActionResult<HungarianFiveLotteryResults>> Calculate()
        {
            _logger.LogInformation("Hungarian five lottery calculating...");
            CalculateHungarianFivelotteryStatistics chfls = new(_context);
            try
            {
                await _context.SaveChangesAsync();
                chfls.Calculate();
            }
            catch (Exception ex)
            {
                if (ex.InnerException is WebException)
                {
                    _logger.LogWarning("",ex.Message);
                    return Accepted("WebClient exception: " + ex.Message);
                }
                _logger.LogWarning("",ex.Message);
                return Accepted("Calculate not happend.");
            }
            _logger.LogInformation("Hungarian five lottery calculate is complete.");
            return Ok(await _context.HungarianFiveLotteryResults.FirstOrDefaultAsync());
        }
    }
}