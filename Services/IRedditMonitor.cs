namespace Services
{
    public interface IRedditMonitor
    {
        void Start(CancellationToken stoppingToken);
    }
}
