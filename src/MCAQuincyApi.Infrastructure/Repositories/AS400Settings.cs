namespace MCAQuincyApi.Infrastructure.Repositories;

public class AS400Settings
{
    public string Host { get; set; } = default!;
    public string User { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Library { get; set; } = default!;
    public string Table { get; set; } = default!;
}