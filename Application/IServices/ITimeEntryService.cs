using Application.DTOs;
using Application.DTOs.Response;

namespace Application.IServices
{
    public interface ITimeEntryService
    {
        Task<Result<ICollection<EmployeeTimeEntryResponseDTO>>> GetTimeEntriesAsync();
    }
}
