using System.Net;

namespace SimpleAuth.Server.ExceptionHandling;

public class ErrorResultModel
{
    public ErrorResultModel(string message, HttpStatusCode status, string code, object? payload = null)
    {
        Message = message;
        Status = status;
        Code = code;
        Payload = payload;
    }

    public string Message { get; set; }

    public HttpStatusCode Status { get; set; }

    public string Code { get; set; }

    public object? Payload { get; set; }
}