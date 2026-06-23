using AutoMapper;
using ArquiDesk.Application.DTOs;
using ArquiDesk.Application.Interfaces;
using ArquiDesk.Domain.Entities;
using ArquiDesk.Domain.Enums;
using ArquiDesk.Web.Helpers;
using ArquiDesk.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ArquiDesk.Web.Controllers;

[Authorize]
public class TicketsController(IUnitOfWork unitOfWork, ITicketService ticketService, IFileStorageService fileStorageService, IMapper mapper) : Controller
{
    public async Task<IActionResult> Index(TicketFilterViewModel filter)
    {
        var query = unitOfWork.Tickets.Query().Include(x => x.Project).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(x => x.Description.Contains(filter.Search) || x.Number.ToString().Contains(filter.Search));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        if (filter.Priority.HasValue)
        {
            query = query.Where(x => x.Priority == filter.Priority.Value);
        }

        if (filter.Type.HasValue)
        {
            query = query.Where(x => x.Type == filter.Type.Value);
        }

        filter.Total = await query.CountAsync();
        var tickets = await query
            .OrderByDescending(x => x.OpenedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
        filter.Items = tickets.Select(mapper.Map<TicketDto>).ToList();

        return View(filter);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var ticket = await unitOfWork.Tickets.Query()
            .Include(x => x.Project)
            .Include(x => x.Comments.OrderBy(c => c.CreatedAt))
            .Include(x => x.Attachments)
            .Include(x => x.ChangeLogs.OrderByDescending(c => c.CreatedAt))
            .FirstOrDefaultAsync(x => x.Id == id);

        return ticket == null ? NotFound() : View(ticket);
    }

    public async Task<IActionResult> Create()
    {
        await FillLookupsAsync();
        return View(new TicketDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TicketDto model)
    {
        if (!ModelState.IsValid)
        {
            await FillLookupsAsync();
            return View(model);
        }

        var ticket = mapper.Map<Ticket>(model);
        ticket.RequesterUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        ticket.CreatedBy = ticket.RequesterUserId;

        await ticketService.CreateAsync(ticket);
        return RedirectToAction(nameof(Details), new { id = ticket.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(Guid ticketId, string message, bool isInternal)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        await unitOfWork.TicketComments.AddAsync(new TicketComment
        {
            TicketId = ticketId,
            Message = message,
            IsInternal = isInternal,
            UserId = userId,
            CreatedBy = userId
        });
        await unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = ticketId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(Guid ticketId, TicketStatus status)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        await ticketService.UpdateStatusAsync(ticketId, status, userId);
        return RedirectToAction(nameof(Details), new { id = ticketId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(Guid ticketId, IFormFile file)
    {
        if (file.Length > 0)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            await using var stream = file.OpenReadStream();
            var attachment = await fileStorageService.SaveTicketAttachmentAsync(ticketId, userId, stream, file.FileName, file.ContentType);
            await unitOfWork.TicketAttachments.AddAsync(attachment);
            await unitOfWork.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), new { id = ticketId });
    }

    private async Task FillLookupsAsync()
    {
        var projects = await unitOfWork.Projects.Query().OrderBy(x => x.Name).ToListAsync();
        ViewBag.Projects = new SelectList(projects, "Id", "Name");
        ViewBag.Types = EnumSelectListHelper.ToSelectList<TicketType>();
        ViewBag.Priorities = EnumSelectListHelper.ToSelectList<TicketPriority>();
    }
}
