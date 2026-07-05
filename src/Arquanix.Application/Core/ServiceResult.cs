namespace Arquanix.Application.Core;


public enum ServiceResultStatus
{
    Success,
    ValidationError,
    NotFound,
}


public class ServiceResult
{
    public ServiceResultStatus Status { get; protected set; } = ServiceResultStatus.Success;

    public List<string> Errors { get; protected set; } = new();

    public bool Success => Status == ServiceResultStatus.Success;

    public static ServiceResult Ok() => new() { Status = ServiceResultStatus.Success };

    public static ServiceResult NotFound() => new() { Status = ServiceResultStatus.NotFound };

    public static ServiceResult Invalid(IEnumerable<string> errors) =>
        new() { Status = ServiceResultStatus.ValidationError, Errors = errors.ToList() };
}


public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; private set; }

    public static ServiceResult<T> Ok(T data) =>
        new() { Status = ServiceResultStatus.Success, Data = data };

    public static new ServiceResult<T> NotFound() =>
        new() { Status = ServiceResultStatus.NotFound };

    public static new ServiceResult<T> Invalid(IEnumerable<string> errors) =>
        new() { Status = ServiceResultStatus.ValidationError, Errors = errors.ToList() };
}
