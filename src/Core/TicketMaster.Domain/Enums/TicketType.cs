using System.Text.Json.Serialization;

namespace TicketMasterAPI.src.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TicketType
{
    FullPrice,
    HalfPrice,
    Beneficiary
}