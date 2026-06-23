using ArquiDesk.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArquiDesk.Web.Controllers.Api;

[ApiController]
[Route("api/tickets")]
[Authorize]
public class TicketsApiController(IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? q, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Tickets.Query().Include(x => x.Project).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(x => x.Description.Contains(q) || x.Number.ToString().Contains(q));
        }

        var result = await query
            .OrderByDescending(x => x.OpenedAt)
            .Take(20)
            .Select(x => new
            {
                x.Id,
                x.Number,
                Project = x.Project != null ? x.Project.Name : string.Empty,
                x.Status,
                x.Priority,
                x.SlaDueAt
            })
            .ToListAsync(cancellationToken);

        return Ok(result);
    }
}
