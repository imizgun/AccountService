
using System.Text.Json;
using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Shared.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AccountService.Background.Rabbit.Filters;

public class EnvelopeFilter<T>(
    IInboxDeadLetterRepository deadLetters,
    ILogger<EnvelopeFilter<T>> logger) : IFilter<ConsumeContext<T>> where T : DefaultEvent {
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next) {
        var version = context.Message.Meta.Version;
        logger.LogInformation("[EnvelopeFilter] Processing message. Version: {Version}", version);

        var messageId = context.Message.EventId != Guid.Empty
            ? context.Message.EventId
            : context.MessageId ?? Guid.Empty;

        if (messageId == Guid.Empty || string.IsNullOrWhiteSpace(version) || !IsSupported(version)) {
            try {
                await deadLetters.AddAsync(new InboxDeadLetter {
                    MessageId = messageId == Guid.Empty ? Guid.NewGuid() : messageId,
                    Handler = typeof(T).Name,
                    ReceivedAt = DateTime.UtcNow,
                    Payload = JsonSerializer.Serialize<object>(context.Message),
                    Error = $"Invalid envelope/version: schemaVersion={version}"
                });

                logger.LogWarning(
                    "Invalid envelope/version. Message moved to dead letter. schema={schemaVersion}, messageId={messageId}",
                    version, messageId);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Failed to save message to dead letter queue");
            }

            throw new InvalidOperationException($"Invalid message envelope: version={version}, messageId={messageId}");
        }

        logger.LogInformation("[EnvelopeFilter] Filtered successfully");
        await next.Send(context);
    }

    public void Probe(ProbeContext context) => context.CreateFilterScope("envelope-guard");

    private static bool IsSupported(string? v) => v is "v1";
}