using Application.DTOs;

namespace Application.IServices
{
    public interface IChartService
    {
        Task<Result<string?>> GeneratePieChartAsync();
    }
}
