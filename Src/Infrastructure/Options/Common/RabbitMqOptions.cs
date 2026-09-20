namespace Backend.Src.Infrastructure.Options.Common;

public class RabbitMqOptions
{
    public string? Uri { get; set; }
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = "/";
    public string ExchangeName { get; set; } = "llanqui";
}