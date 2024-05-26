using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using TheGames.Data;
using TheGames.Shared.Models;

namespace TheGames.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting("Fixed")]
public class GameController(ApplicationDbContext ctx) : ControllerBase
{
    [HttpGet("")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IQueryable<Game>> Get()
    {
        return Ok(ctx.Game.Include(x => x.Publisher));
    }

    [HttpGet("{key}")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Game>> GetAsync(long key)
    {
        var game = await ctx.Game.Include(x => x.Publisher).FirstOrDefaultAsync(x => x.Id == key);

        if (game == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(game);
        }
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Game>> PostAsync(Game game)
    {
        var record = await ctx.Game.FindAsync(game.Id);
        if (record != null)
        {
            return Conflict();
        }
    
        await ctx.Game.AddAsync(game);

        await ctx.SaveChangesAsync();

        return Created($"/game/{game.Id}", game);
    }

    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Game>> PutAsync(long key, Game update)
    {
        var game = await ctx.Game.FirstOrDefaultAsync(x => x.Id == key);

        if (game == null)
        {
            return NotFound();
        }

        ctx.Entry(game).CurrentValues.SetValues(update);

        await ctx.SaveChangesAsync();

        return Ok(game);
    }

    [HttpPatch("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Game>> PatchAsync(long key, Delta<Game> delta)
    {
        var game = await ctx.Game.FirstOrDefaultAsync(x => x.Id == key);

        if (game == null)
        {
            return NotFound();
        }

        delta.Patch(game);

        await ctx.SaveChangesAsync();

        return Ok(game);
    }

    [HttpDelete("{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long key)
    {
        var game = await ctx.Game.FindAsync(key);

        if (game != null)
        {
            ctx.Game.Remove(game);
            await ctx.SaveChangesAsync();
        }

        return NoContent();
    }
}
