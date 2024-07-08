using HNG_Organisation.Models;

namespace HNG_Organisation.Results;

public class OrganisationSuccessResponse
{
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public OrganisationSuccessData Data { get; set; } = new();
}
