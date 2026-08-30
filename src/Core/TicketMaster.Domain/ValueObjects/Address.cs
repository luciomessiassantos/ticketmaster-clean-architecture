namespace TicketMasterAPI.src.Domain.ValueObjects;

public class Address
{
    public required string Country { get; set; }
    public required string State { get; set; }
    public required string City { get; set; }
    public required string Neighborhood { get; set; }
    public required string Street { get; set; }
    public required string UF { get; set; }
    public required string ZipCode { get; set; }
    public required string Place { get; set; }
}


public class AddressPatch
{
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? Neighborhood { get; set; }
    public string? Street { get; set; }
    public string? UF { get; set; }
    public string? ZipCode { get; set; }
    public string? Place { get; set; }
}