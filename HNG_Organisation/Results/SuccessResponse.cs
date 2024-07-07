using HNG_Organisation.Models;

namespace HNG_Organisation.Results;

public class SuccessResponse
{
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public SuccessData Data { get; set; } = new();
}
