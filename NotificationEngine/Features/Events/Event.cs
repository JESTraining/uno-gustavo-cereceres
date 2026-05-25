namespace NotificationEngine.Features.Events;
public class Event
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}