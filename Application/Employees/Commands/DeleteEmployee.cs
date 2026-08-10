using MediatR;
using Persistence;

namespace Application.Employees.Commands;

public class DeleteEmployee
{
    public class Command : IRequest<bool>
    {
        public required Guid Id { get; set; }
    }

    public class Handler(AppDbContext context) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var employee = await context.Employees.FindAsync([request.Id], cancellationToken);

            if (employee == null) return false;

            context.Employees.Remove(employee);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}