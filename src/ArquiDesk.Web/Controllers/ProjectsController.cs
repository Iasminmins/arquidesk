using AutoMapper;
using ArquiDesk.Application.DTOs;
using ArquiDesk.Application.Interfaces;
using ArquiDesk.Domain.Entities;
using ArquiDesk.Domain.Enums;
using ArquiDesk.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ArquiDesk.Web.Controllers;

[Authorize(Roles = $"{UserRoles.Administrador},{UserRoles.Arquiteto}")]
public class ProjectsController(IUnitOfWork unitOfWork, IMapper mapper) : Controller
{
    public async Task<IActionResult> Index()
    {
        var projects = await unitOfWork.Projects.Query()
            .Include(x => x.Lead)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return View(projects.Select(mapper.Map<ProjectDto>).ToList());
    }

    public async Task<IActionResult> Create()
    {
        await FillLeadsAsync();
        return View(new ProjectDto { StartDate = DateTime.Today, ExpectedDeliveryDate = DateTime.Today.AddDays(30) });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectDto model)
    {
        if (!ModelState.IsValid)
        {
            await FillLeadsAsync();
            return View(model);
        }

        var project = mapper.Map<Project>(model);
        await unitOfWork.Projects.AddAsync(project);
        await unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task FillLeadsAsync()
    {
        var leads = await unitOfWork.Leads.Query().OrderBy(x => x.Name).ToListAsync();
        ViewBag.Clients = new SelectList(leads, "Id", "Name");
        ViewBag.Statuses = EnumSelectListHelper.ToSelectList<ProjectStatus>();
    }
}
