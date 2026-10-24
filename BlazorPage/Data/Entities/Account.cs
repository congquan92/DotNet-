namespace BlazorPage.Data.Entities;

public class Account
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // For simplicity in student project
    public string Role { get; set; } = "User"; // "Admin" or "User"
    public string Status { get; set; } = "Active"; // "Active" or "Locked"

    public Customer? Customer { get; set; }
}
