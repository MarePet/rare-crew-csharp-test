using Application.Enums;

namespace Application.DTOs
{
    public sealed record Error(ErrorCodeEnum ErrorCode, string? ErrorMessage = null);
}
