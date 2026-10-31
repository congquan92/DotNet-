using Microsoft.AspNetCore.Identity;

namespace McpPage.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public Customer? Customer { get; set; }
}
