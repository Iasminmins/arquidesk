using ArquiDesk.Application.Interfaces;
using ArquiDesk.Domain.Entities;
using ArquiDesk.Domain.Enums;
using ArquiDesk.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArquiDesk.Web.Controllers;

[Authorize(Roles = $"{UserRoles.Administrador},{UserRoles.Arquiteto},{UserRoles.Projetista}")]
public class NegotiationsController(IUnitOfWork unitOfWork) : Controller
{
    public async Task<IActionResult> Index(NegotiationStatus? status)
    {
        var query = unitOfWork.Negotiations.Query().AsNoTracking();
        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        ViewBag.Status = status;
        ViewBag.Statuses = EnumSelectListHelper.ToSelectList<NegotiationStatus>();
        return View(await query.OrderBy(x => x.NextContactAt ?? DateTime.MaxValue).ThenByDescending(x => x.CreatedAt).ToListAsync());
    }

    public IActionResult Create()
    {
        ViewBag.Statuses = EnumSelectListHelper.ToSelectList<NegotiationStatus>();
        return View(new Negotiation());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Negotiation model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Statuses = EnumSelectListHelper.ToSelectList<NegotiationStatus>();
            return View(model);
        }

        await unitOfWork.Negotiations.AddAsync(model);
        await unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var negotiation = await unitOfWork.Negotiations.GetByIdAsync(id);
        if (negotiation is null)
        {
            return NotFound();
        }

        ViewBag.Statuses = EnumSelectListHelper.ToSelectList<NegotiationStatus>();
        return View(negotiation);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Negotiation model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Statuses = EnumSelectListHelper.ToSelectList<NegotiationStatus>();
            return View(model);
        }

        var negotiation = await unitOfWork.Negotiations.GetByIdAsync(id);
        if (negotiation is null)
        {
            return NotFound();
        }

        negotiation.ClientName = model.ClientName;
        negotiation.Contact = model.Contact;
        negotiation.Cost = model.Cost;
        negotiation.LastOfferedValue = model.LastOfferedValue;
        negotiation.CashValue = model.CashValue;
        negotiation.UpdatedValue = model.UpdatedValue;
        negotiation.OwnerName = model.OwnerName;
        negotiation.Status = model.Status;
        negotiation.NextContactAt = model.NextContactAt;
        negotiation.Notes = model.Notes;

        unitOfWork.Negotiations.Update(negotiation);
        await unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
