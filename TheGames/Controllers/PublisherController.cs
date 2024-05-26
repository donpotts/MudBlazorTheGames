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
public class PublisherController(ApplicationDbContext ctx) : ControllerBase
{
    [HttpGet("")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IQueryable<Publisher>> Get()
    {
        return Ok(ctx.Publisher);
    }

    [HttpGet("{key}")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Publisher>> GetAsync(long key)
    {
        var publisher = await ctx.Publisher.FirstOrDefaultAsync(x => x.Id == key);

        if (publisher == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(publisher);
        }
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Publisher>> PostAsync(Publisher publisher)
    {
        var record = await ctx.Publisher.FindAsync(publisher.Id);
        if (record != null)
        {
            return Conflict();
        }
    
        await ctx.Publisher.AddAsync(publisher);

        await ctx.SaveChangesAsync();

        return Created($"/publisher/{publisher.Id}", publisher);
    }

    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Publisher>> PutAsync(long key, Publisher update)
    {
        var publisher = await ctx.Publisher.FirstOrDefaultAsync(x => x.Id == key);

        if (publisher == null)
        {
            return NotFound();
        }

        ctx.Entry(publisher).CurrentValues.SetValues(update);

        await ctx.SaveChangesAsync();

        return Ok(publisher);
    }

    [HttpPatch("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Publisher>> PatchAsync(long key, Delta<Publisher> delta)
    {
        var publisher = await ctx.Publisher.FirstOrDefaultAsync(x => x.Id == key);

        if (publisher == null)
        {
            return NotFound();
        }

        delta.Patch(publisher);

        await ctx.SaveChangesAsync();

        return Ok(publisher);
    }

    [HttpDelete("{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long key)
    {
        var publisher = await ctx.Publisher.FindAsync(key);

        if (publisher != null)
        {
            ctx.Publisher.Remove(publisher);
            await ctx.SaveChangesAsync();
        }

        return NoContent();
    }
}
