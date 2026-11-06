namespace App.Data.Entities;

public class Appointment
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = null!;
    public int ClientId { get; set; }
    public int EmployeeId { get; set; }
    public int ServiceId { get; set; }
    public int? RoomId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string Status { get; set; } = "waiting";
    public int? PriceFinal { get; set; }
    public DateTime CreatedAt { get; set; }
    public Client Client { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
    public Service Service { get; set; } = null!;
    public Room? Room { get; set; }
}