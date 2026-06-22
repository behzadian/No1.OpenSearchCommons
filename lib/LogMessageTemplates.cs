using Microsoft.Extensions.Logging;

namespace No1.OpenSearchCommons;

internal partial class LogMessageTemplates
{
	protected LogMessageTemplates() {
	}

	[LoggerMessage(Level = LogLevel.Trace, Message = "A http client with key {key} has been created for OpenSearchConfig")]
	internal static partial void OpenSearchHttpClientCreated(ILogger logger, string key);

	[LoggerMessage(Level = LogLevel.Warning, Message = "Unable to create {key} http client service, because either config (`{config}`) or its healthUrl `{healthUrl}` is null or empty. You must add below configurations: OpenSearchConfig::HealthUrl")]
	internal static partial void OpenSearchHttpClientNotCreated(ILogger logger, string key, OpenSearchConfig? config, string? healthUrl);
}