using HNG_Organisation.Models;

namespace HNG_Organisation.Results;

public class SuccessResponse
{
    public string Status { get; set; } = "success";
    public string Message { get; set; } = "Registration successful";

    public SuccessData Data { get; set; } = new();
}
