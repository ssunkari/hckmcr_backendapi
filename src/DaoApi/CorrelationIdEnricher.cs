using CorrelationId;
using Serilog.Core;
using Serilog.Events;

namespace Zuto.Uk.Sample.API
{
    public class CorrelationIdEnricher : ILogEventEnricher
    {
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public CorrelationIdEnricher(ICorrelationContextAccessor correlationContextAccessor)
        {
            _correlationContextAccessor = correlationContextAccessor;
        }
        public void Enrich(LogEvent le, ILogEventPropertyFactory lepf)
        {
            le.AddOrUpdateProperty(lepf.CreateProperty("correlationId", _correlationContextAccessor.CorrelationContext.CorrelationId));
        }
    }
}