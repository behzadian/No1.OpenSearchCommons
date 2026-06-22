using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenSearch.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace No1.OpenSearchCommons;

public static class OpenSearchExtensions
{
	internal const string OpenSearchKey = nameof(OpenSearchKey);

	public static void RegisterOpenSearchCertificateSignedHttpClient(this WebApplicationBuilder builder) {
		using var loggerFactory = LoggerFactory.Create(logging => {
			logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
			logging.AddConsole();
		});
		var logger = loggerFactory.CreateLogger(typeof(OpenSearchExtensions));

		ArgumentNullException.ThrowIfNull(builder);
		var osConfig = builder.Configuration.GetSection(nameof(OpenSearchConfig)).Get<OpenSearchConfig>();
		if (osConfig != null && !string.IsNullOrEmpty(osConfig.HealthUrl)) {
			builder.Services.AddHttpClient(
				OpenSearchKey,
				client => client.BaseAddress = new Uri(osConfig.HealthUrl)
			).ConfigurePrimaryHttpMessageHandler(() => Handler(osConfig));
			LogMessageTemplates.OpenSearchHttpClientCreated(logger, OpenSearchKey);
		} else {
			LogMessageTemplates.OpenSearchHttpClientNotCreated(logger, OpenSearchKey, osConfig, osConfig?.HealthUrl);
		}
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5398:Avoid hardcoded SslProtocols values", Justification = "_")]
	private static HttpClientHandler Handler(OpenSearchConfig osConfig) {
		var handler = new HttpClientHandler {
			ClientCertificateOptions = ClientCertificateOption.Manual,
			SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
		};

		// Load client cert (PEM/key) and add to handler
		var clientCert = X509Certificate2.CreateFromPemFile(osConfig.ClientCertificateFile, osConfig.ClientPrivateKeyFile);
		handler.ClientCertificates.Add(clientCert);

		// Load root CA and use validation callback that checks authority
		var rootCa = X509CertificateLoader.LoadCertificateFromFile(osConfig.RootCertificateAuthorityFile);
		handler.ServerCertificateCustomValidationCallback = CertificateValidations.AuthorityIsRoot(rootCa);

		return handler;
	}
}