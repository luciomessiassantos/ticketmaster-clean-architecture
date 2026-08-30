using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TicketMaster.Infrastructure.Identity;

public class AppIdentityDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    
}