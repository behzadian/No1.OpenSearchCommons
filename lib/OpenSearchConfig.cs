namespace No1.OpenSearchCommons;

public record OpenSearchConfig(
	string ClientCertificateFile,
	string ClientPrivateKeyFile,
	string RootCertificateAuthorityFile,
	string HealthUrl
);