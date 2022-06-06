using Practice.WorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<ChallengeStatusChecker>();
    })
    .Build();

await host.RunAsync();
