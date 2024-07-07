using HNG_Organisation.Errors;

namespace HNG_Organisation.Results;

public class LoginResult
{
    public List<ErrorDetail> Errors { get; set; } = [];
}
