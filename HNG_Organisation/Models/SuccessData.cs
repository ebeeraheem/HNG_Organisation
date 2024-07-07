using HNG_Organisation.Entities;

namespace HNG_Organisation.Models;

public class SuccessData
{
    public string AccessToken { get; set; } = string.Empty;
    public User User { get; set; } = new();
}
