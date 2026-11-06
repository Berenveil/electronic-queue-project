namespace App.Data.Entities;

public class Room
{
    public int Id { get; set; }
    public string Number { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = [];
}