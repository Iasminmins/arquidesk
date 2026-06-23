using ArquiDesk.Application.Interfaces;
using ArquiDesk.Domain.Entities;
using ArquiDesk.Domain.Enums;
using ArquiDesk.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArquiDesk.Web.Controllers;

[Authorize(Roles = $"{UserRoles.Administrador},{UserRoles.Arquiteto},{UserRoles.Projetista}")]
public class LeadsController(IUnitOfWork unitOfWork) : Controller
{
    public async Task<IActionResult> Index(string? search, LeadStatus? status)
    {
        var query = unitOfWork.Leads.Query().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Name.Contains(search) ||
                (x.Contact != null && x.Contact.Contains(search)) ||
                (x.Observations != null && x.Observations.Contains(search)));
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        ViewBag.Search = search;
        ViewBag.Status = status;
        ViewBag.Statuses = EnumSelectListHelper.ToSelectList<LeadStatus>();

        var leads = await query.OrderBy(x => x.NextFollowUpAt ?? DateTime.MaxValue).ThenByDescending(x => x.CreatedAt).ToListAsync();
        return View(leads);
    }

    public IActionResult Create()
    {
        ViewBag.Statuses = EnumSelectListHelper.ToSelectList<LeadStatus>();
        return View(new Lead());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Lead model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Statuses = EnumSelectListHelper.ToSelectList<LeadStatus>();
            return View(model);
        }

        await unitOfWork.Leads.AddAsync(model);
        await unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var lead = await unitOfWork.Leads.GetByIdAsync(id);
        if (lead is null)
        {
            return NotFound();
        }

        ViewBag.Statuses = EnumSelectListHelper.ToSelectList<LeadStatus>();
        return View(lead);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Lead model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Statuses = EnumSelectListHelper.ToSelectList<LeadStatus>();
            return View(model);
        }

        var lead = await unitOfWork.Leads.GetByIdAsync(id);
        if (lead is null)
        {
            return NotFound();
        }

        lead.Name = model.Name;
        lead.Contact = model.Contact;
        lead.Answered = model.Answered;
        lead.Interested = model.Interested;
        lead.ProjectSent = model.ProjectSent;
        lead.Status = model.Status;
        lead.NextFollowUpAt = model.NextFollowUpAt;
        lead.OwnerName = model.OwnerName;
        lead.Observations = model.Observations;

        unitOfWork.Leads.Update(lead);
        await unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
