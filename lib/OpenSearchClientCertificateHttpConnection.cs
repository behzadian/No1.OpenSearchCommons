using No1.asp.NET.Commons.Utility;
using OpenSearch.Net;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace No1.OpenSearchCommons;

public class OpenSearchClientCertificateHttpConnection() : HttpConnection
{
	private static readonly OpenSearchConfig Config = ConfigurationUtility.GetNeededConfig<OpenSearchConfig>();
	private readonly X509Certificate2 clientCertificate = X509Certificate2.CreateFromPemFile(Config.ClientCertificateFile, Config.ClientPrivateKeyFile);
	private readonly X509Certificate rootCaCertificate = X509CertificateLoader.LoadCertificateFromFile(Config.RootCertificateAuthorityFile);

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5398:Avoid hardcoded SslProtocols values", Justification = "_")]
	protected override HttpMessageHandler CreateHttpClientHandler(RequestData requestData) {
		ArgumentNullException.ThrowIfNull(requestData);
		var handler = new HttpClientHandler {
			AutomaticDecompression = requestData.HttpCompression ? DecompressionMethods.GZip | DecompressionMethods.Deflate : DecompressionMethods.None,
			ClientCertificateOptions = ClientCertificateOption.Manual,
			SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
		};

		// Client certificate for mTLS authentication
		handler.ClientCertificates.Add(this.clientCertificate);

		// Proper validation using your Root CA
		handler.ServerCertificateCustomValidationCallback = CertificateValidations.AuthorityIsRoot(this.rootCaCertificate);

		return handler;
	}
}