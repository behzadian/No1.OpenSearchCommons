using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace No1.OpenSearchCommons;

public class OpenSearchHealthCheck(IHttpClientFactory factory, OpenSearchConfig config) : IHealthCheck
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "_")]
	public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
		try {
			var client = factory.CreateClient(OpenSearchExtensions.OpenSearchKey);
			var healthUri = new Uri(config.HealthUrl);
			var resp = await client.GetAsync(healthUri, cancellationToken).ConfigureAwait(true);
			if ((int)resp.StatusCode >= 200 && (int)resp.StatusCode <= 299) {
				return HealthCheckResult.Healthy();
			}

			var body = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(true);
			return HealthCheckResult.Unhealthy($"Discover endpoint returned {(int)resp.StatusCode}: {body}");
		} catch (Exception ex) {
			return HealthCheckResult.Unhealthy(ex.Message, ex);
		}
	}
}