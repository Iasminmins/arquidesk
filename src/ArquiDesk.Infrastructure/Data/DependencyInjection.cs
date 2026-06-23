using ArquiDesk.Application.Interfaces;
using ArquiDesk.Application.Services;
using ArquiDesk.Infrastructure.Identity;
using ArquiDesk.Infrastructure.Repositories;
using ArquiDesk.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ArquiDesk.Infrastructure.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddArquiDeskInfrastructure(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<ApplicationDbSeeder>();
        return services;
    }
}
