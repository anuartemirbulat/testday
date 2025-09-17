using MapServer;
using ObjectLayerModule;
using RegionModule;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

var redisCon = builder.Configuration["RedisConnection"]!;

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisCon));

builder.Services.AddSingleton<IObjectLayer, ObjectLayer>();
builder.Services.AddSingleton<IRegionLayer, RegionLayer>();
builder.Services.AddSingleton<GameMapServer>();
builder.Services.AddHostedService<MapServerWorker>();


var host = builder.Build();
host.Run();