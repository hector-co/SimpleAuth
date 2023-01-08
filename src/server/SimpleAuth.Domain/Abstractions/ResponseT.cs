namespace SimpleAuth.Domain.Abstractions;

public class Response<TValue> : Response
{
    protected Response(TValue? value)
    {
        Value = value;
    }

    protected Response(Error error) : base(error)
    {
        Value = default;
    }

    public TValue? Value { get; }

    public static Response<TValue> Success(TValue value)
    {
        return new Response<TValue>(value);
    }

    public new static Response<TValue> Failure(Error error)
    {
        return new Response<TValue>(error);
    }

    public static Response<TValue> Failure(string code, string message, Exception? innerException = null)
    {
        return new Response<TValue>(new Error(code, message, innerException));
    }
}