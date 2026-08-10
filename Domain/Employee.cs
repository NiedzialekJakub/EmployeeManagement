using System;

namespace Domain;

public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public DateTime HireDate { get; set; }
    public required string Email { get; set; }
    public required string PhoneNo { get; set; }
    public string? ProfilePicture { get; set; }
    public required string Status { get; set; } // "active" / "inactive"
    public required string Address { get; set; }
    public required string State { get; set; }
    public required string Country { get; set; }
    public required string City { get; set; }
    public required string Pincode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}