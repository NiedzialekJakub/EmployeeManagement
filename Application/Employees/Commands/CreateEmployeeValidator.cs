using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Employees.Commands;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployee.Command>
{
    public CreateEmployeeValidator(AppDbContext context)
    {

        RuleFor(x => x.Employee.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MustAsync(async (email, ct) => !await context.Employees.AnyAsync(e => e.Email == email, ct))
            .WithMessage("Email address is already in use.");

        RuleFor(x => x.Employee.HireDate)
            .NotEmpty().WithMessage("Hire date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Hire date cannot be in the future.");

        RuleFor(x => x.Employee.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(status => status is "active" or "inactive")
            .WithMessage("Status must be either 'active' or 'inactive'.");

        RuleFor(x => x.Employee.PhoneNo)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number must follow E.164 format (e.g. +48123456789).");

        // additional 1
        RuleFor(x => x.Employee.Status)
            .NotEmpty()
            .Must(status => status is "active" or "inactive")
            .WithMessage("Status must be strictly 'active' or 'inactive'.");

        // additional 2
        RuleFor(x => x.Employee.Pincode)
            .NotEmpty()
            .Matches(@"^[A-Za-z0-9\s-]{3,10}$").WithMessage("Invalid pincode/postal code format.");

        // additional 3
        RuleFor(x => x.Employee.ProfilePicture)
            .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Profile picture must be a valid absolute HTTP/HTTPS URL.");

        // additional 4
        RuleFor(x => x.Employee.Name)
            .NotEmpty()
            .Length(3, 100)
            .Must(name => name != null && name.Trim().Contains(' '))
            .WithMessage("Name must contain both first and last name.");

        RuleFor(x => x.Employee.Address).NotEmpty().WithMessage("Address is required.");
        RuleFor(x => x.Employee.City).NotEmpty().WithMessage("City is required.");
        RuleFor(x => x.Employee.State).NotEmpty().WithMessage("State is required.");
        RuleFor(x => x.Employee.Country).NotEmpty().WithMessage("Country is required.");
    }
}