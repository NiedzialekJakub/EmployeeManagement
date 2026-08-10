using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Employees.Queries;

public class GetEmployeeList
{
    public class Query : IRequest<List<Employee>> { }

    public class Handler(AppDbContext context) : IRequestHandler<Query, List<Employee>>
    {
        public async Task<List<Employee>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await context.Employees.ToListAsync(cancellationToken);
        }
    }
}