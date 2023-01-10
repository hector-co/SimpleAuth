using System.Text.Json;

namespace SimpleAuth.Server.Models
{
    public enum StatusMessageType
    {
        Error,
        Warning,
        Success,
        Info
    }

    public class StatusMessageModel
    {
        public StatusMessageModel(string message, StatusMessageType messageType = StatusMessageType.Success, bool autoHide = true)
        {
            MessageType = messageType;
            Message = message;
            AutoHide = autoHide;
        }

        public StatusMessageType MessageType { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool AutoHide { get; set; } = true;

        public string ToJsonString()
        {
            return JsonSerializer.Serialize(this);
        }

        public static StatusMessageModel? FromJsonString(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            return JsonSerializer.Deserialize<StatusMessageModel>(json);
        }

        public static string ErrorMessage(string message)
        {
            return new StatusMessageModel(message, StatusMessageType.Error).ToJsonString();
        }

        public static string WarningMessage(string message)
        {
            return new StatusMessageModel(message, StatusMessageType.Warning).ToJsonString();
        }

        public static string SuccessMessage(string message)
        {
            return new StatusMessageModel(message, StatusMessageType.Success).ToJsonString();
        }

        public static string InfoMessage(string message)
        {
            return new StatusMessageModel(message, StatusMessageType.Info).ToJsonString();
        }
    }
}
