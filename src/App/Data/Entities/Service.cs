namespace App.Data.Entities;

public class Service
{
    public int Id { get; set; }
    public int ProfessionId { get; set; }
    public string Name { get; set; } = null!;
    public int Price { get; set; }
    public int DurationMin { get; set; }

    public Profession Profession { get; set; } = null!;
    public ICollection<EmployeeService> EmployeeServices { get; set; } = [];
    public ICollection<Appointment> Appointments { get; set; } = [];
}