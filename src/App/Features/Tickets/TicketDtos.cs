namespace App.Features.Tickets;

public record CreateTicketRequest(
    string Phone,
    string? LastName,
    string? FirstName,
    string? MiddleName,
    int EmployeeId,
    int ServiceId,
    DateTime ScheduledAt);

public record TicketResponse(
    int AppointmentId,
    string TicketNumber,
    string Status,
    DateTime ScheduledAt,
    int DurationMin,
    int Price,
    string ClientName,
    string EmployeeName,
    string ServiceName,
    string? RoomNumber);

public record TimeSlotDto(DateTime Start, DateTime End);

public class TicketException : Exception
{
    public int StatusCode { get; }

    public TicketException(string message, int statusCode = 400) : base(message)
    {
        StatusCode = statusCode;
    }
}