using App.Data;
using App.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Features.Tickets;

public class TicketService(AppDbContext db)
{

    public async Task<TicketResponse> CreateTicketAsync(CreateTicketRequest request)
    {
        var employee = await db.Employees
            .Include(e => e.Profession)
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId)
            ?? throw new TicketException("Врач не найден", 404);

        var service = await db.Services
            .FirstOrDefaultAsync(s => s.Id == request.ServiceId)
            ?? throw new TicketException("Услуга не найдена", 404);

        var canDoService = await db.EmployeeServices
            .AnyAsync(es => es.EmployeeId == request.EmployeeId && es.ServiceId == request.ServiceId);

        if (!canDoService)
            throw new TicketException("Этот врач не оказывает выбранную услугу");

        var workDate = DateOnly.FromDateTime(request.ScheduledAt);
        var schedule = await db.Schedules
            .FirstOrDefaultAsync(s => s.EmployeeId == request.EmployeeId && s.WorkDate == workDate);

        if (schedule is null)
            throw new TicketException("Врач не работает в выбранный день");

        var shiftStart = workDate.ToDateTime(schedule.ShiftStart);
        var shiftEnd = workDate.ToDateTime(schedule.ShiftEnd);
        var appointmentEnd = request.ScheduledAt.AddMinutes(service.DurationMin);

        if (request.ScheduledAt < shiftStart || appointmentEnd > shiftEnd)
            throw new TicketException("Выбранное время вне рабочего графика врача");

        var hasConflict = await db.Appointments.AnyAsync(a =>
            a.EmployeeId == request.EmployeeId &&
            a.Status != "cancelled" &&
            a.Status != "no_show" &&
            a.ScheduledAt < appointmentEnd &&
            a.ScheduledAt.AddMinutes(service.DurationMin) > request.ScheduledAt);

        
        var exactConflict = await db.Appointments.AnyAsync(a =>
            a.EmployeeId == request.EmployeeId &&
            a.ScheduledAt == request.ScheduledAt &&
            a.Status != "cancelled" &&
            a.Status != "no_show");

        if (exactConflict || hasConflict)
            throw new TicketException("Выбранное время уже занято. Выберите другое время");

        var client = await db.Clients.FirstOrDefaultAsync(c => c.Phone == request.Phone);

        if (client is null)
        {
            if (string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.FirstName))
                throw new TicketException("Для нового пациента укажите фамилию и имя");

            client = new Client
            {
                LastName = request.LastName.Trim(),
                FirstName = request.FirstName.Trim(),
                MiddleName = request.MiddleName?.Trim(),
                Phone = request.Phone.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Clients.Add(client);
            await db.SaveChangesAsync();
        }

        var ticketNumber = await GenerateTicketNumberAsync();

        var appointment = new Appointment
        {
            TicketNumber = ticketNumber,
            ClientId = client.Id,
            EmployeeId = employee.Id,
            ServiceId = service.Id,
            ScheduledAt = request.ScheduledAt,
            Status = "waiting",
            PriceFinal = service.Price,
            CreatedAt = DateTime.UtcNow
        };

        db.Appointments.Add(appointment);
        await db.SaveChangesAsync();

        return new TicketResponse(
            AppointmentId: appointment.Id,
            TicketNumber: appointment.TicketNumber,
            Status: appointment.Status,
            ScheduledAt: appointment.ScheduledAt,
            DurationMin: service.DurationMin,
            Price: service.Price,
            ClientName: $"{client.LastName} {client.FirstName} {client.MiddleName}".Trim(),
            EmployeeName: $"{employee.LastName} {employee.FirstName} {employee.MiddleName}".Trim(),
            ServiceName: service.Name,
            RoomNumber: null
        );
    }

   
    public async Task<TicketResponse> GetByTicketNumberAsync(string ticketNumber)
    {
        var appointment = await db.Appointments
            .Include(a => a.Client)
            .Include(a => a.Employee)
            .Include(a => a.Service)
            .Include(a => a.Room)
            .FirstOrDefaultAsync(a => a.TicketNumber == ticketNumber)
            ?? throw new TicketException("Талон не найден", 404);

        return new TicketResponse(
            AppointmentId: appointment.Id,
            TicketNumber: appointment.TicketNumber,
            Status: appointment.Status,
            ScheduledAt: appointment.ScheduledAt,
            DurationMin: appointment.Service.DurationMin,
            Price: appointment.PriceFinal ?? appointment.Service.Price,
            ClientName: $"{appointment.Client.LastName} {appointment.Client.FirstName} {appointment.Client.MiddleName}".Trim(),
            EmployeeName: $"{appointment.Employee.LastName} {appointment.Employee.FirstName} {appointment.Employee.MiddleName}".Trim(),
            ServiceName: appointment.Service.Name,
            RoomNumber: appointment.Room?.Number
        );
    }

    private async Task<string> GenerateTicketNumberAsync()
    {
        
        var year = DateTime.UtcNow.Year;
        var prefix = $"T-{year}-";

        var last = await db.Appointments
            .Where(a => a.TicketNumber.StartsWith(prefix))
            .OrderByDescending(a => a.TicketNumber)
            .Select(a => a.TicketNumber)
            .FirstOrDefaultAsync();

        var nextNumber = 1;
        if (last is not null && int.TryParse(last.Replace(prefix, ""), out var n))
            nextNumber = n + 1;

        return $"{prefix}{nextNumber:D5}";
    }
}