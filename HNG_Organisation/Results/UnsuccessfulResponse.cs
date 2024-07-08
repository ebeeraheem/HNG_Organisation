namespace HNG_Organisation.Results;

public class UnsuccessfulResponse
{
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
}
