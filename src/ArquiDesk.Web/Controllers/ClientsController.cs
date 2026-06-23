using AutoMapper;
using ArquiDesk.Application.DTOs;
using ArquiDesk.Application.Interfaces;
using ArquiDesk.Domain.Entities;
using ArquiDesk.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArquiDesk.Web.Controllers;

[Authorize(Roles = $"{UserRoles.Administrador},{UserRoles.Arquiteto}")]
public class ClientsController(IUnitOfWork unitOfWork, IMapper mapper) : Controller
{
    public async Task<IActionResult> Index()
    {
        var clients = await unitOfWork.Clients.Query().OrderBy(x => x.Name).ToListAsync();
        return View(clients.Select(mapper.Map<ClientDto>).ToList());
    }

    public IActionResult Create() => View(new ClientDto());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClientDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await unitOfWork.Clients.AddAsync(mapper.Map<Client>(model));
        await unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
