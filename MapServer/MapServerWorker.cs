namespace MapServer;

public class MapServerWorker : BackgroundService
{
    private readonly GameMapServer _gameMapServer;

    public MapServerWorker(GameMapServer gameMapServer)
    {
        _gameMapServer = gameMapServer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _gameMapServer.PollEvents(); 
            await Task.Delay(10, stoppingToken); 
        }
    }
}
