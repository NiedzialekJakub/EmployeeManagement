using Domain;
using MediatR;
using Persistence;

namespace Application.Employees.Queries;

public class GetEmployee
{
    public class Query : IRequest<Employee?>
    {
        public required Guid Id { get; set; }
    }

    public class Handler(AppDbContext context) : IRequestHandler<Query, Employee?>
    {
        public async Task<Employee?> Handle(Query request, CancellationToken cancellationToken)
        {
            return await context.Employees.FindAsync([request.Id], cancellationToken);
        }
    }
}