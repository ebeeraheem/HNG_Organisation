using HNG_Organisation.Errors;
using HNG_Organisation.Models;
using HNG_Organisation.Results;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace HNG_Organisation.Extensions;

public static class CustomValidation
{
    public static RegisterResult IsValidRegisterModel(this RegisterModel model)
    {
        var result = new RegisterResult();

        if (string.IsNullOrWhiteSpace(model.FirstName))
        {
            result.Errors.Add(new ErrorDetail
            {
                Field = nameof(model.FirstName),
                Message = "First Name is required."
            });
        }

        if (string.IsNullOrWhiteSpace(model.LastName))
        {
            result.Errors.Add(new ErrorDetail
            {
                Field = nameof(model.LastName),
                Message = "Last Name is required."
            });
        }

        if (string.IsNullOrWhiteSpace(model.Email))
        {
            result.Errors.Add(new ErrorDetail
            {
                Field = nameof(model.Email),
                Message = "Email is required."
            });
        }
        else if (!IsValidEmail(model.Email))
        {
            result.Errors.Add(new ErrorDetail
            {
                Field = nameof(model.Email),
                Message = "Email is not valid."
            });
        }

        if (string.IsNullOrWhiteSpace(model.Password))
        {
            result.Errors.Add(new ErrorDetail
            {
                Field = nameof(model.Password),
                Message = "Password is required."
            });
        }

        if (string.IsNullOrWhiteSpace(model.Phone))
        {
            result.Errors.Add(new ErrorDetail
            {
                Field = nameof(model.Phone),
                Message = "Phone number is required."
            });
        }
        else if (!IsValidPhone(model.Phone))
        {
            result.Errors.Add(new ErrorDetail
            {
                Field = nameof(model.Phone),
                Message = "Phone number is not valid."
            });
        }

        return result;
    }

    private static bool IsValidEmail(string email)
    {
        // Use a simple regex or any other email validation logic here
        return new EmailAddressAttribute().IsValid(email);
    }

    private static bool IsValidPhone(string phone)
    {
        // Use a simple regex or any other phone validation logic here
        return Regex.IsMatch(phone, @"^\+?\d{10,}$");
    }
}
