using HNG_Organisation.Errors;

namespace HNG_Organisation.Results;

public class RegisterResult
{
    public List<ErrorDetail> Errors { get; set; } = [];
}
