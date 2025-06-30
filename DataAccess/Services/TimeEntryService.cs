using Application.ApplicationSettingSections;
using Application.DTOs;
using Application.DTOs.Response;
using Application.Enums;
using Application.IServices;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace DataAccess.Services
{
    public class TimeEntryService(HttpClient httpClient, IOptions<ExternalURLs> options) : ITimeEntryService
    {
        private readonly HttpClient httpClient = httpClient;
        private readonly IOptions<ExternalURLs> options = options;
        private readonly string? URL = options.Value.TimeEntriesURL;

        public async Task<Result<ICollection<EmployeeTimeEntryResponseDTO>>> GetTimeEntriesAsync()
        {
            Result<ICollection<EmployeeTimeEntryResponseDTO>?> result = new();
            try
            {
                var response = await httpClient.GetAsync(URL);

                if (response == null || !response.IsSuccessStatusCode)
                {
                    return Result<ICollection<EmployeeTimeEntryResponseDTO>>.Failure(errorCode: ErrorCodeEnum.BadRequest, errorMessage: "Failed to retrieve time entries from external server.");
                }

                var entries = await response.Content.ReadFromJsonAsync<ICollection<EmployeeDTO>>();
                
                if (entries == null || !entries.Any())
                {
                   result.Data = new List<EmployeeTimeEntryResponseDTO>();
                }
                else
                {
                    result.Data = entries.GroupBy(x => x.EmployeeName)
                        .Select(x => new EmployeeTimeEntryResponseDTO
                        {
                            Name = x.Key, 
                            TotalHours = Math.Round(x.Sum(emp => (emp.EndTimeUtc - emp.StarTimeUtc).TotalHours), 2)
                        }).OrderByDescending(x => x.TotalHours).ToList();
                }

                result.IsSuccess = true;
                return Result<ICollection<EmployeeTimeEntryResponseDTO>>.Success(result.Data);
            }
            catch (Exception)
            {
                return Result<ICollection<EmployeeTimeEntryResponseDTO>>.Failure(errorCode: ErrorCodeEnum.InternalServerError, errorMessage: "Internal server error.");
            }
        }
    }
}
