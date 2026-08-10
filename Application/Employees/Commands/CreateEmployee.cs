using Domain;
using MediatR;
using Persistence;

namespace Application.Employees.Commands;

public class CreateEmployee
{
    public class Command : IRequest<Guid>
    {
        public required Employee Employee { get; set; }
    }

    public class Handler(AppDbContext context) : IRequestHandler<Command, Guid>
    {
        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.Employee.Id == Guid.Empty)
            {
                request.Employee.Id = Guid.NewGuid();
            }

            if (request.Employee.CreatedAt == default)
            {
                request.Employee.CreatedAt = DateTime.UtcNow;
            }

            context.Employees.Add(request.Employee);
            await context.SaveChangesAsync(cancellationToken);

            return request.Employee.Id;
        }
    }
}