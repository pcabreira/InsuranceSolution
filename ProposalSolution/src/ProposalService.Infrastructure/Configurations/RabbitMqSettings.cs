public class RabbitMqSettings
{
    public string Host { get; set; } = "";
    public int Port { get; set; }
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Exchange { get; set; } = "";

    public Dictionary<string, QueueConfig> Queues { get; set; } = new();
}

public class QueueConfig
{
    public string Name { get; set; } = "";
    public string RoutingKey { get; set; } = "";
}
