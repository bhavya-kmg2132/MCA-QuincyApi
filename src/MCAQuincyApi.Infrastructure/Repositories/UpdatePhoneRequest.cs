namespace MCAQuincyApi.API.Models;

public class UpdatePhoneRequest
{
    // Allowing null to clear a number
    public string PhoneNumber { get; set; }
}