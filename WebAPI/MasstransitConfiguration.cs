namespace WebAPI;

public class MasstransitConfiguration
{
    public Producer Producer { get; set; }
    public BrokerConfiguration Broker { get; set; }
}

public class BrokerConfiguration
{
    public string Username { get; set; }
    public string Password { get; set; } 
    public string Host { get; set; }
    public ushort Port { get; set; }
    public string VirtualHost { get; set; }
}
public class Producer
{
    public string ExchangeName { get; set; }
}

