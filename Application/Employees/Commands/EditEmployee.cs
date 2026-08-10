using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Employees.Commands;

public class EditEmployee
{
    public class Command : IRequest<bool>
    {
        public required Employee Employee { get; set; }
    }

    public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var employee = await context.Employees.FindAsync([request.Employee.Id], cancellationToken);

            if (employee == null) return false;

            // AutoMapper przepisuje wartości z obiektu z żądania do obiektu z bazy danych
            mapper.Map(request.Employee, employee);

            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}