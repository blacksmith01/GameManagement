using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameManagement.Services
{
    public class LifeTimeEventHandler : IHostedService
    {
        BulkActionService bulkActionService;
        IHostApplicationLifetime lifeTime;
        public LifeTimeEventHandler(IServiceProvider sp)
        {
            bulkActionService = sp.GetRequiredService<BulkActionService>();
            lifeTime = sp.GetRequiredService<IHostApplicationLifetime>();

            lifeTime.ApplicationStarted.Register(ApplicationStarted);
            lifeTime.ApplicationStopping.Register(ApplicationStopping);
            lifeTime.ApplicationStopped.Register(ApplicationStopped);
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            bulkActionService.Start();

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.Info("Stopping...");

            await bulkActionService.Stop();

            Logger.Info("Stopped");
        }

        void ApplicationStarted()
        {
            Logger.Info("ApplicationStarted");
        }
        void ApplicationStopping()
        {
            Logger.Info("ApplicationStopping");
        }
        void ApplicationStopped()
        {
            Logger.Info("ApplicationStopped");
        }


    }
}
