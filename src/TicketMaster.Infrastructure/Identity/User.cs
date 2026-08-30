using Microsoft.AspNetCore.Identity;

namespace TicketMaster.Infrastructure.Identity;


public class User : IdentityUser<Guid>
{
    public string? RefreshToken { get; set; } 
    public DateTime RefreshTokenExpiry { get; set; }


    public User() {}

    public User(string email, string passwordHashed)
    {
        Email = email;
        PasswordHash = passwordHashed;
    }

    public User(string email)
    {
        Email = email;
    }

}