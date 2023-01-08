using System.Net;

namespace SimpleAuth.Server.ExceptionHandling;

public class WebApiException : Exception
{
    public const string InternalError = "internal_error";
    public const string ValidationError = "validation_error";
    public const string ParametersFormat = "parameters_format_error";
    public const string DomainException = "domain_error";
    public const string DataAccessError = "data_access_error";

    public WebApiException(string message, HttpStatusCode statusCode, string code, object? payload = null, Exception? innerException = null) : base(message, innerException)
    {
        Status = statusCode;
        Code = code;
        Payload = payload;
    }

    public HttpStatusCode Status { get; }
    public string Code { get; }
    public object? Payload { get; }
}