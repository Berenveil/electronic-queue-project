namespace App.Data.Entities;

public class Profession
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<Service> Services { get; set; } = [];
    public ICollection<Employee> Employees { get; set; } = [];
}