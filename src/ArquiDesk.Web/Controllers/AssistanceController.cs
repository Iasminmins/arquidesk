using ArquiDesk.Application.Interfaces;
using ArquiDesk.Domain.Entities;
using ArquiDesk.Domain.Enums;
using ArquiDesk.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArquiDesk.Web.Controllers;

[Authorize]
public class AssistanceController(IUnitOfWork unitOfWork) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.Statuses = EnumSelectListHelper.ToSelectList<AssistanceStatus>();
        var requests = await unitOfWork.AssistanceRequests.Query()
            .AsNoTracking()
            .OrderBy(x => x.AssistanceDate ?? DateTime.MaxValue)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync();

        return View(requests);
    }

    public IActionResult Create()
    {
        ViewBag.Statuses = EnumSelectListHelper.ToSelectList<AssistanceStatus>();
        return View(new AssistanceRequest());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AssistanceRequest model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Statuses = EnumSelectListHelper.ToSelectList<AssistanceStatus>();
            return View(model);
        }

        await unitOfWork.AssistanceRequests.AddAsync(model);
        await unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var request = await unitOfWork.AssistanceRequests.GetByIdAsync(id);
        if (request is null)
        {
            return NotFound();
        }

        ViewBag.Statuses = EnumSelectListHelper.ToSelectList<AssistanceStatus>();
        return View(request);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, AssistanceRequest model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Statuses = EnumSelectListHelper.ToSelectList<AssistanceStatus>();
            return View(model);
        }

        var request = await unitOfWork.AssistanceRequests.GetByIdAsync(id);
        if (request is null)
        {
            return NotFound();
        }

        request.ClientName = model.ClientName;
        request.Contact = model.Contact;
        request.VisitCompleted = model.VisitCompleted;
        request.Completed = model.Completed;
        request.OrderPlaced = model.OrderPlaced;
        request.AssistanceDate = model.AssistanceDate;
        request.Status = model.Status;
        request.Notes = model.Notes;

        unitOfWork.AssistanceRequests.Update(request);
        await unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
