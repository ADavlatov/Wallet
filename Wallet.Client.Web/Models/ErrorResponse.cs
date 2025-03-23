using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models;

public class ValidationError
{
    [JsonPropertyName("propertyName")] public string PropertyName { get; set; }
    [JsonPropertyName("errorMessage")] public string ErrorMessage { get; set; }
    [JsonPropertyName("attemptedValue")] public string AttemptedValue { get; set; }
    [JsonPropertyName("customState")] public object CustomState { get; set; }
    [JsonPropertyName("severity")] public int Severity { get; set; }
    [JsonPropertyName("errorCode")] public string ErrorCode { get; set; }
    [JsonPropertyName("formattedMessagePlaceholderValues")] public FormattedMessagePlaceholderValues FormattedMessagePlaceholderValues { get; set; }
}

public class FormattedMessagePlaceholderValues
{
    [JsonPropertyName("minLength")] public int MinLength { get; set; }
    [JsonPropertyName("maxLength")] public int MaxLength { get; set; }
    [JsonPropertyName("totalLength")] public int TotalLength { get; set; }
    [JsonPropertyName("propertyName")] public string PropertyName { get; set; }
    [JsonPropertyName("propertyValue")] public string PropertyValue { get; set; }
    [JsonPropertyName("propertyPath")] public string PropertyPath { get; set; }
}

public class ErrorResponse
{
    [JsonPropertyName("message")] public string Message { get; set; }
}