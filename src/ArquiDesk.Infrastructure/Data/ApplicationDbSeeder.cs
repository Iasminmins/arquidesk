using ArquiDesk.Domain.Entities;
using ArquiDesk.Domain.Enums;
using ArquiDesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

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

        if (!context.Leads.Any())
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

            var project = new Project
            {
                Name = "Apartamento Jardim",
                Lead = lead,
                Address = "Rua dos Projetos, 100",
                StartDate = DateTime.Today.AddDays(-12),
                ExpectedDeliveryDate = DateTime.Today.AddDays(30),
                ResponsibleUserId = admin.Id,
                Status = ProjectStatus.EmProjeto
            };

            context.Projects.Add(project);
            context.Tickets.Add(new Ticket
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

            await context.SaveChangesAsync();
        }

        var sampleTicket = context.Tickets.FirstOrDefault(x => x.Number == 1);
        if (sampleTicket != null && sampleTicket.Description.Contains("armario da suite"))
        {
            sampleTicket.Description = "Alterar medidas do armário da suíte conforme nova planta enviada.";
            await context.SaveChangesAsync();
        }
    }
}
