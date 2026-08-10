using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public static class DbInitializer
{
    public static async Task SeedData(AppDbContext context)
    {
        if(await context.Employees.AnyAsync()) return;

        var employees = new List<Employee>
        {
            new()
            {
                Name = "Jan Kowalski",
                HireDate = DateTime.UtcNow.AddYears(-2),
                Email = "jan.kowalski@gmail.com",
                PhoneNo = "+48123456789",
                ProfilePicture = "https://example.com/photos/jkowalski.jpg",
                Status = "active",
                Address = "10 Marszałkowska",
                State = "Mazowieckie",
                Country = "Poland",
                City = "Warsaw",
                Pincode = "00001",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Anna Nowak",
                HireDate = DateTime.UtcNow.AddYears(-1),
                Email = "anna.nowak@gmail.com",
                PhoneNo = "+48987654321",
                ProfilePicture = "https://example.com/photos/anowak.jpg",
                Status = "active",
                Address = "50 Piotrkowska",
                State = "Mazowieckie",
                Country = "Poland",
                City = "Radom",
                Pincode = "90001",
                CreatedAt = DateTime.UtcNow
            }

        };
        
        await context.Employees.AddRangeAsync(employees);
        await context.SaveChangesAsync();
    }
}