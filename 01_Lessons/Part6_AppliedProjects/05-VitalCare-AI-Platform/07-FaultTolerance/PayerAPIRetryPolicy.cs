// MODULE 10: Retry Policy for Payer APIs
// HEALTHCARE EQUIVALENT OF: RetryPolicy.cs (JMA)
// JMA: DMS API → retry 3x. HERE: Payer eligibility API → retry 3x
// KEY DIFFERENCE: Payer APIs are often slow/unreliable during peak hours
// A failed eligibility check → pend to pharmacist, never deny outright

using Polly;
using Polly.Retry;

namespace VitalCare.FaultTolerance;

public class PayerAPIRetryPolicy
{
    private readonly ResiliencePipeline _pipeline;

    public PayerAPIRetryPolicy(ILogger<PayerAPIRetryPolicy> logger)
    {
        _pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay            = TimeSpan.FromSeconds(2),   // payer APIs need more recovery time than DMS
                BackoffType      = DelayBackoffType.Exponential,
                UseJitter        = true,
                ShouldHandle     = new PredicateBuilder()
                    .Handle<HttpRequestException>()
                    .Handle<TaskCanceledException>()
                    .Handle<TimeoutException>(),
                OnRetry = args =>
                {
                    logger.LogWarning(
                        "[PAYER RETRY] Attempt {Attempt} | Error: {Error} | Next retry in {Delay}ms",
                        args.AttemptNumber + 1,
                        args.Outcome.Exception?.Message,
                        args.RetryDelay.TotalMilliseconds);
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    // INTERVIEW: "What happens if the payer API fails after 3 retries?"
    // "We pend the PA to a clinical pharmacist — never deny based on a system failure.
    //  In healthcare, a system outage should never result in patient care denial.
    //  The pharmacist reviews manually until the payer API recovers."
    public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken ct = default)
        => await _pipeline.ExecuteAsync(operation, ct);
}
