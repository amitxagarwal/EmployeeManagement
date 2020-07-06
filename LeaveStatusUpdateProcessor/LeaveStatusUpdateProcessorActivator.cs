using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace LeaveStatusUpdateProcessor
{
    public class LeaveStatusUpdateProcessorActivator: IJobActivatorEx
    {
        private readonly IServiceProvider serviceProvider;

        public LeaveStatusUpdateProcessorActivator(IServiceProvider services)
        {
            this.serviceProvider = services;
        }

        public T CreateInstance<T>(IFunctionInstanceEx functionInstance)
        {
            var disposer = functionInstance.InstanceServices.GetRequiredService<ScopeDisposable>();
            disposer.Scope = serviceProvider.CreateScope();
            return serviceProvider.GetRequiredService<T>();
        }

        public T CreateInstance<T>()
        {
            // This will never get called because we're implementing IJobActivatorEx
            throw new NotSupportedException("Cannot create an instance outside of scopes");
        }

        [SuppressMessage("ReSharper", "CA1034",
                Justification = "It’s clearer that way")]
        public sealed class ScopeDisposable : IDisposable
        {
            public IServiceScope Scope { get; set; }

            public void Dispose()
            {
                Scope?.Dispose();
            }
        }
    }
}
