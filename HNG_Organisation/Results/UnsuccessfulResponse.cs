namespace HNG_Organisation.Results;

public class UnsuccessfulResponse
{
    public string Status { get; set; } = "Bad request";
    public string Message { get; set; } = "Registration unsuccessful";
    public int StatusCode { get; set; } = 400;
}
