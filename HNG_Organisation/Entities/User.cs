using Microsoft.AspNetCore.Identity;

namespace HNG_Organisation.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    // The userId, email, password and phone properties are inherited from IdentityUser
}
