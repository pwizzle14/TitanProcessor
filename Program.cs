using GoldenvaleDAL.ClassObjects;
using GoldenvaleDAL.DataLayerWorker;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.DependencyInjection;
using System.Timers;
using TitanProcessor;
using TitanProcessor.Services;
using TitanProcessor.TitanCore;
using static System.Net.Mime.MediaTypeNames;

{
    var serviceProvider = new ServiceCollection()
               // .AddSingleton<ILogger, Logger>()
                .AddSingleton<IDataLayerWorker, DataLayerWorker>()
                .AddSingleton<ITitanProcessorWorker, TitanProcessorWorker>()
                .BuildServiceProvider();


    var dataworker = serviceProvider.GetService<IDataLayerWorker>();

    TitanProcessorWorker worker = new TitanProcessorWorker(dataworker, null);

    worker.StartWatcher();

    while (Console.ReadLine() != "q") 
    {
        switch (Console.ReadLine())
        {
            case "stop":
                {
                    worker.StopWatcher();
                    break;
                }
            case "start":
                {
                    worker.StartWatcher();
                    break;
                }

            default:
                break;
        }
    }

}