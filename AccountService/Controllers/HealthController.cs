using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Shared.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AccountService.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController(
    HealthCheckService healthChecks,
    IOutboxMessageRepository repository,
    ILogger<HealthController> logger) : ControllerBase
{
    /// <summary>
    /// Проверка, что приложение живо (liveness).
    /// Не ходит во внешние зависимости.
    /// </summary>
    [HttpGet("live")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<MbResult<object>> Live() => Ok(MbResult<object>.Ok(null, "Alive"));


    /// <summary>
    /// Проверка готовности приложения (readiness).
    /// Запускает все зарегистрированные health checks (БД, RabbitMQ и т.д.)
    /// </summary>
    [HttpGet("ready")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<MbResult<object>>> Ready(CancellationToken cancellationToken)
    {
        var report = await healthChecks.CheckHealthAsync(cancellationToken);
        var outboxMessages = await repository.GetUnprocessedMessagesAsync(cancellationToken);

        if (outboxMessages < 100)
            return report.Status == HealthStatus.Healthy
                ? Ok(MbResult<object>.Ok(null, "Application is healthy"))
                : StatusCode(StatusCodes.Status503ServiceUnavailable, MbResult<object>.Fail(report.Status.ToString()));

        logger.LogWarning("Outbox contains {Count} unprocessed messages, which may indicate a problem.", outboxMessages);
        return StatusCode(StatusCodes.Status503ServiceUnavailable,
            MbResult<object>.Fail($"Outbox contains {outboxMessages} unprocessed messages, which may indicate a problem."));

    }
}