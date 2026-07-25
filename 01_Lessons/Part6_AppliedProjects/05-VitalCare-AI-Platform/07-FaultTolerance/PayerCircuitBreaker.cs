// MODULE 10: Circuit Breaker for Payer APIs
// HEALTHCARE EQUIVALENT OF: CircuitBreaker.cs (JMA)
// JMA: DMS down → RSM escalation. HERE: Payer API down → pharmacist escalation
// KEY DIFFERENCE: Circuit open → ALL PAs pend to pharmacist (never auto-deny)

using Polly;
using Polly.CircuitBreaker;

namespace VitalCare.FaultTolerance;

public class PayerCircuitBreaker
{
    private readonly ResiliencePipeline _pipeline;
    private bool _isOpen = false;

    public PayerCircuitBreaker(ILogger<PayerCircuitBreaker> logger)
    {
        _pipeline = new ResiliencePipelineBuilder()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio      = 0.5,
                MinimumThroughput = 5,
                SamplingDuration  = TimeSpan.FromSeconds(30),
                BreakDuration     = TimeSpan.FromSeconds(90),   // payer APIs need longer recovery

                ShouldHandle = new PredicateBuilder()
                    .Handle<HttpRequestException>()
                    .Handle<TimeoutException>(),

                OnOpened = args =>
                {
                    _isOpen = true;
                    // INTERVIEW: When payer circuit opens → all PAs pend to pharmacist
                    // NEVER auto-deny — system outage is not a clinical denial reason
                    logger.LogCritical(
                        "[PAYER CIRCUIT] OPENED — payer API unreachable. " +
                        "All PA eligibility checks will pend to clinical pharmacist for {Duration}s.",
                        args.BreakDuration.TotalSeconds);
                    return ValueTask.CompletedTask;
                },

                OnClosed    = _ => { _isOpen = false; logger.LogInformation("[PAYER CIRCUIT] CLOSED — payer API recovered."); return ValueTask.CompletedTask; },
                OnHalfOpened = _ => { logger.LogInformation("[PAYER CIRCUIT] HALF-OPEN — probing payer API recovery."); return ValueTask.CompletedTask; }
            })
            .Build();
    }

    public bool IsOpen => _isOpen;

    public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken ct = default)
    {
        try { return await _pipeline.ExecuteAsync(operation, ct); }
        catch (BrokenCircuitException)
        {
            // INTERVIEW: "Payer circuit open" is NOT a denial reason
            // Return a sentinel value that tells the supervisor to pend, not deny
            throw new PayerUnavailableException("Payer API circuit open — PA must pend to clinical pharmacist.");
        }
    }
}

public class PayerUnavailableException(string message) : Exception(message);
