
using Microsoft.EntityFrameworkCore;
using NotificationEngine.Features.Events;

namespace NotificationEngine.Data;
public class NotificationEngineContext : DbContext
{
    public DbSet<Event> Events { get; set; }    
}