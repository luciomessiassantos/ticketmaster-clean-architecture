using System.Text.Json.Serialization;

namespace TicketMasterAPI.src.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EventState
{
    Draft,
    Available,
    Unavailable,
    Cancelled
}

