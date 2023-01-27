namespace SimpleAuth.Domain.Common;

public class Response
{
    protected Response()
    {
        IsSuccess = true;
        Error = null;
    }

    protected Response(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    protected Response(string code, string message, Exception? innerException = null)
    {
        IsSuccess = false;
        Error = new Error(code, message, innerException);
    }

    public bool IsSuccess { get; }
    public Error? Error { get; }

    public static Response Success()
    {
        return new Response();
    }

    public static Response Failure(Error error)
    {
        return new Response(error);
    }

    public static Response Failure(string code, string message, Exception? innerException = null)
    {
        return new Response(code, message, innerException);
    }

    public static Response<TValue> Success<TValue>(TValue value)
    {
        return Response<TValue>.Success(value);
    }

    public static Response<TValue> Failure<TValue>(Error error)
    {
        return Response<TValue>.Failure(error);
    }

    public static Response<TValue> Failure<TValue>(string code, string message, Exception? innerException = null)
    {
        return Response<TValue>.Failure(code, message, innerException);
    }
}
