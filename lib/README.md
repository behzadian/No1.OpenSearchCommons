# No1.OpenSearchCommons

Tools and libraries required by .NET applications to use OpenSearch

## Contents

This library has two important classes:

- `OpenSearchHealthCheck` can be added as health checker to asp.net health checks:
```csharp
builder.Services
	.AddHealthChecks()
	.AddCheck<OpenSearchHealthCheck>("OpenSearch");
```

- `OpenSearchExtensions` class has a helper method that registers specific http client for connecting to OpenSearch server.
OpenSearch requires you to generate custom keys, certificates, pem files, ... to trust to clients.

After you created the keys, you can specify your client key and certificate files and also root certificate authority file in configuration file:
```json
{
	"OpenSearchConfig": {
		"ClientCertificateFile": "/usr/share/opensearch/config/client.pem",
		"ClientPrivateKeyFile": "/usr/share/opensearch/config/client-key.pem",
		"RootCertificateAuthorityFile": "/usr/share/opensearch/config/root-ca.pem",
		"HealthUrl":  "https://[domain]:[port]/_cluster/health"
	}
}
```

Then get service by name (`OpenSearchExtensions.OpenSearchKey`):
```csharp
public class OpenSearchHealthCheck(IHttpClientFactory factory) : IHealthCheck
{
	public HttpClient GetClient() {
		return  factory.CreateClient(OpenSearchExtensions.OpenSearchKey);
	}
}
```

Above HttpClient provides client certificate to the OpenSearch server and also verifies that the server has valid certificates.
This HttpClient has been used for OpenSearchHealthCheck, but can be used elsewhere too.

You have problem in configuring OpenSearch? then read my another repo [Boots.OpenSearchBoot](https://github.com/behzadian/Boots.OpenSearchBoot)


## How to use?

Add below dependency:

```shell
dotnet package add No1.OpenSearchCommons
```

Then add below line in the `Program.cs` before building `app`:
```csharp
using No1.OpenSearchCommons;
//...
builder.RegisterOpenSearchCertificateSignedHttpClient();
```

For adding OpenSearch to Health, add below code too:

```csharp
builder.Services
	.AddHealthChecks()
	.AddCheck<OpenSearchHealthCheck>("OpenSearch");
```