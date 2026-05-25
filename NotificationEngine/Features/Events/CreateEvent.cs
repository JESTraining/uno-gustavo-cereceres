using MediatR;
using Microsoft.AspNetCore.Identity;
using NotificationEngine.Data;

namespace NotificationEngine.Features.Events;

public class CreateEvent
{
    public record Command(string Description, string Type):IRequest<Response>;

    public record Response(Guid Id, string Description, string Type);

    public class Handler: IRequestHandler<Command, Response>
    {
        private readonly NotificationEngineContext _context;

        public Handler(NotificationEngineContext context)
        {
            _context = context;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Description = request.Description,
                Type = request.Type
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return new Response(newEvent.Id, newEvent.Description, newEvent.Type);
        }
    }
}