using ArquiDesk.Domain.Entities;
using ArquiDesk.Domain.Enums;
using ArquiDesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArquiDesk.Infrastructure.Data;

public class ApplicationDbSeeder(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
{
    public async Task SeedAsync()
    {
        foreach (var role in UserRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var admin = await userManager.FindByEmailAsync("admin@arquidesk.com");
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = "admin@arquidesk.com",
                Email = "admin@arquidesk.com",
                EmailConfirmed = true,
                FullName = "Administrador ArquiDesk",
                Department = "Gestao"
            };

            await userManager.CreateAsync(admin, "Admin@123456");
            await userManager.AddToRoleAsync(admin, UserRoles.Administrador);
        }

        var sampleClient = await context.Clients
            .OrderBy(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (sampleClient == null)
        {
            sampleClient = new Client
            {
                Name = "Cliente Demonstracao",
                Document = "000.000.000-00",
                Email = "cliente@demo.com",
                Phone = "(11) 99999-0000",
                Address = "Rua dos Projetos, 100"
            };

            await context.Clients.AddAsync(sampleClient);
        }

        if (!await context.Leads.AnyAsync())
        {
            var lead = new Lead
            {
                Name = "Cliente Demonstracao",
                Contact = "(11) 99999-0000",
                Answered = true,
                Interested = true,
                ProjectSent = true,
                Status = LeadStatus.ProjetoEnviado,
                Observations = "Lead inicial de demonstracao"
            };

            await context.Leads.AddAsync(lead);
        }

        if (!await context.Projects.AnyAsync())
        {
            var project = new Project
            {
                Name = "Apartamento Jardim",
                Client = sampleClient,
                Address = "Rua dos Projetos, 100",
                StartDate = DateTime.Today.AddDays(-12),
                ExpectedDeliveryDate = DateTime.Today.AddDays(30),
                ResponsibleUserId = admin.Id,
                Status = ProjectStatus.EmProjeto
            };

            await context.Projects.AddAsync(project);
            await context.Tickets.AddAsync(new Ticket
            {
                Number = 1,
                Project = project,
                RequesterUserId = admin.Id,
                ResponsibleUserId = admin.Id,
                Type = TicketType.AlteracaoDeProjeto,
                Priority = TicketPriority.Media,
                Description = "Alterar medidas do armário da suíte conforme nova planta enviada.",
                OpenedAt = DateTime.UtcNow.AddHours(-8),
                SlaDueAt = DateTime.UtcNow.AddHours(64),
                Status = TicketStatus.EmAnalise
            });
        }

        await context.SaveChangesAsync();

        var sampleTicket = await context.Tickets.FirstOrDefaultAsync(x => x.Number == 1);
        if (sampleTicket != null && sampleTicket.Description.Contains("armario da suite"))
        {
            sampleTicket.Description = "Alterar medidas do armário da suíte conforme nova planta enviada.";
            await context.SaveChangesAsync();
        }
    }
}