using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketMaster.Application.Common.Interfaces;
using TicketMaster.Domain.Interfaces;
using TicketMaster.Infrastructure.Data;
using TicketMaster.Infrastructure.Data.Repositories;
using TicketMaster.Infrastructure.Identity;
using TicketMaster.Infrastructure.Identity.Models;

namespace TicketMaster.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<TicketMasterDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SQLServer")));
            
        services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SQLServer")));

        services.AddAuthentication()
            .AddGoogle(options =>
    {
        options.ClientId = configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
    });
        services.AddAuthorization();
        

        services.AddDataProtection();
        
        services.AddIdentityApiEndpoints<User>()
            .AddEntityFrameworkStores<AppIdentityDbContext>();
        
        
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
            
        return services;
    }
}