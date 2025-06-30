using Application.Enums;

namespace Application.DTOs
{
    public record Result<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public Error Error { get; set; }

        public Result()
        {
            Data = default;
            IsSuccess = false;
        }

        public static Result<T> Success(T value) => new Result<T> { Data = value, IsSuccess = true };
        public static Result<T> Failure(ErrorCodeEnum errorCode, string errorMessage) => new Result<T> { Error = new Error(errorCode, errorMessage) };
    }
}
