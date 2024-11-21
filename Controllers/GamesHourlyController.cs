using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/games")]
public class GamesHourlyController : ControllerBase
{
    private readonly SteamChartsDbContext _dbContext;

    public GamesHourlyController(SteamChartsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("hourly")]
    public async Task<IActionResult> StoreHourlyGames([FromBody] List<HourlyGame> games)
    {
        if (games == null || !games.Any())
            return BadRequest("Nenhum dado foi enviado.");

        try
        {
            foreach (var game in games)
                _dbContext.HourlyGames.Add(game);


            await _dbContext.SaveChangesAsync();
            return Ok("Dados salvos com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao salvar os dados: {ex.Message}");
            return StatusCode(500, "Erro ao salvar os dados.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetHourlyGames()
    {
        var data = await _dbContext.HourlyGames
            .OrderBy(h => h.Date)
            .ToListAsync();

        return Ok(data);
    }
}