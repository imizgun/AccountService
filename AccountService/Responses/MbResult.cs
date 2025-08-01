namespace AccountService.Responses;

public class MbResult<T>
{
	public bool IsSuccess { get; init; }
	public T? Result { get; init; }
	public string? Message { get; init; }

	public static MbResult<T> Ok(T result, string? message = null) => new()
	{
		IsSuccess = true,
		Result = result,
		Message = message
	};

	public static MbResult<T> Fail(string message) => new()
	{
		IsSuccess = false,
		Result = default,
		Message = message
	};
}
