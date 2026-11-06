namespace App.Data.Entities;

public class Schedule
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateOnly WorkDate { get; set; }
    public TimeOnly ShiftStart { get; set; }
    public TimeOnly ShiftEnd { get; set; }

    public Employee Employee { get; set; } = null!;
}