using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StatsdClient;

namespace SampleDataCollector
{
    public class DataCollector : BackgroundService
    {
        private readonly ILogger<DataCollector> _logger;
        public DataCollector(
            ILogger<DataCollector> logger
        )
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"DataCollector is starting.");

            stoppingToken.Register(() =>
                    _logger.LogDebug($"DataCollector background task is stopping."));

            bool.TryParse(Environment.GetEnvironmentVariable("INCLUDE_GC"), out var includeGc);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"DataCollector task doing background work.");
                var assm = Assembly.GetExecutingAssembly().GetName().Name;
                using (var proc = Process.GetCurrentProcess())
                {
                    Func<long, double> bytesToMb = (bytes) => bytes / (1024.0 * 1024.0);
                    DogStatsd.Gauge($"dotnet.{assm}.process.threads", proc.Threads.Count);
                    DogStatsd.Gauge($"dotnet.{assm}.process.memory.workingset.mb", bytesToMb(proc.WorkingSet64));
                    DogStatsd.Gauge($"dotnet.{assm}.process.memory.workingset.raw", proc.WorkingSet64);
                    DogStatsd.Gauge($"dotnet.{assm}.process.memory.virtual.mb", bytesToMb(proc.VirtualMemorySize64));
                    DogStatsd.Gauge($"dotnet.{assm}.process.memory.virtual.raw", proc.VirtualMemorySize64);

                    if (includeGc)
                    {
                        var gcMemory = GC.GetTotalMemory(false);
                        DogStatsd.Gauge($"dotnet.{assm}.process.memory.gc.mb", bytesToMb(gcMemory));
                        DogStatsd.Gauge($"dotnet.{assm}.process.memory.gc.raw", gcMemory);
                    }

                }
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }

            _logger.LogDebug($"DataCollector background task is stopping.");

        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await Task.CompletedTask;
        }
    }


    // Copyright (c) .NET Foundation. Licensed under the Apache License, Version 2.0. 
    /// <summary>
    /// Base class for implementing a long running <see cref="IHostedService"/>.
    /// </summary>
    public abstract class BackgroundService : IHostedService, IDisposable
    {
        private Task _executingTask;

        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        /// <summary>
        /// This method is called when the <see cref="IHostedService"/> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)"/> is called.</param>
        /// <returns>A <see cref="Task"/> that represents the long running operations.</returns>
        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            // Store the task we're executing
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            // If the task is completed then return it, this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }

        }

        public virtual void Dispose()
        {
            _stoppingCts.Cancel();
        }
    }
}