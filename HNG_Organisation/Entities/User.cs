using Microsoft.AspNetCore.Identity;

namespace HNG_Organisation.Entities;

public class User : IdentityUser
{
    public string UserId { get; set; } = Guid.NewGuid().ToString();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    // The phone, email and password are inherited from IdentityUser
    public List<Organisation> Organisations { get; set; } = [];
}
