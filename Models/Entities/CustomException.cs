using System.Text.Json;

namespace EasyWheelsApi.Models.Entities
{
    public class CustomException(string title, string message, int statusCode) : Exception(message)
    {
        public int StatusCode { get; set; } = statusCode;
        public string Title { get; set; } = title;

        public override string ToString() =>
            $"CustomException: Title: {Title}, Message: {Message}, StatusCode: {StatusCode}";
    }
}
