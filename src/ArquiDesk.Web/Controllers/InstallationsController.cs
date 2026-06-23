using ArquiDesk.Application.Interfaces;
using ArquiDesk.Domain.Entities;
using ArquiDesk.Domain.Enums;
using ArquiDesk.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArquiDesk.Web.Controllers;

[Authorize(Roles = UserRoles.EquipeInterna)]
public class InstallationsController(IUnitOfWork unitOfWork) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.Statuses = EnumSelectListHelper.ToSelectList<InstallationStatus>();
        var installations = await unitOfWork.Installations.Query()
            .AsNoTracking()
            .OrderBy(x => x.InstallationDate ?? DateTime.MaxValue)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync();

        return View(installations);
    }

    public IActionResult Create()
    {
        ViewBag.Statuses = EnumSelectListHelper.ToSelectList<InstallationStatus>();
        return View(new Installation());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Installation model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Statuses = EnumSelectListHelper.ToSelectList<InstallationStatus>();
            return View(model);
        }

        await unitOfWork.Installations.AddAsync(model);
        await unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var installation = await unitOfWork.Installations.GetByIdAsync(id);
        if (installation is null)
        {
            return NotFound();
        }

        ViewBag.Statuses = EnumSelectListHelper.ToSelectList<InstallationStatus>();
        return View(installation);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Installation model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Statuses = EnumSelectListHelper.ToSelectList<InstallationStatus>();
            return View(model);
        }

        var installation = await unitOfWork.Installations.GetByIdAsync(id);
        if (installation is null)
        {
            return NotFound();
        }

        installation.ClientName = model.ClientName;
        installation.Contact = model.Contact;
        installation.FactoryBillingDate = model.FactoryBillingDate;
        installation.OrderArrived = model.OrderArrived;
        installation.InstallationDate = model.InstallationDate;
        installation.InstallerName = model.InstallerName;
        installation.Status = model.Status;
        installation.Notes = model.Notes;

        unitOfWork.Installations.Update(installation);
        await unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
