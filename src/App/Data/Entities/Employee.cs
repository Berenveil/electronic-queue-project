namespace App.Data.Entities;

public class Employee
{
    public int Id { get; set; }
    public int ProfessionId { get; set; }
    public string LastName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string? MiddleName { get; set; }

    public Profession Profession { get; set; } = null!;
    public ICollection<EmployeeService> EmployeeServices { get; set; } = [];
    public ICollection<Schedule> Schedules { get; set; } = [];
    public ICollection<Appointment> Appointments { get; set; } = [];
}