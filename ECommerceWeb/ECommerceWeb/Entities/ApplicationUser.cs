using Microsoft.AspNetCore.Identity;

namespace ECommerceWeb.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public Customer? Customer { get; set; }
}
