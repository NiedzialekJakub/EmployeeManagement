using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Employees.Commands;

public class EditEmployeeValidator : AbstractValidator<EditEmployee.Command>
{
    public EditEmployeeValidator(AppDbContext context)
    {

        RuleFor(x => x.Employee.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MustAsync(async (command, email, ct) => 
                !await context.Employees.AnyAsync(e => e.Email == email && e.Id != command.Employee.Id, ct))
            .WithMessage("Email address is already used by another employee.");

        RuleFor(x => x.Employee.HireDate)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Hire date cannot be in the future.");

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

        RuleFor(x => x.Employee.PhoneNo).NotEmpty();
        RuleFor(x => x.Employee.Address).NotEmpty();
        RuleFor(x => x.Employee.City).NotEmpty();
        RuleFor(x => x.Employee.State).NotEmpty();
        RuleFor(x => x.Employee.Country).NotEmpty();
    }
}