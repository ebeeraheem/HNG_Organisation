using HNG_Organisation.Models;

namespace HNG_Organisation.Results;

public class GetUserResponse
{
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public UserResponseData Data { get; set; } = new();
}
