namespace Application.DTOs.Response
{
    public record EmployeeTimeEntryResponseDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public double? TotalHours { get; set; }
    }
}
