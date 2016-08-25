using System;
namespace B2CAuth
{
	public static class EndpointConstants
	{
		public const string AuthorizeEndpointTemplate = "{0}/oauth2/v2.0/authorize?p={1}&client_id={2}&scope={3}&redirect_uri={4}&response_type=code&prompt=login";

		public const string TokenEndpointTemplate = "{0}/oauth2/v2.0/token?p={1}";

		public const string EndSessionEndpointTemplate = "{0}/oauth2/v2.0/logout?p={1}";
	}

	public class OAuth2GrantType
	{
		public const string AuthorizationCode = "authorization_code";

		public const string RefreshToken = "refresh_token";

		public const string ClientCredentials = "client_credentials";

		public const string Saml11Bearer = "urn:ietf:params:oauth:grant-type:saml1_1-bearer";

		public const string Saml20Bearer = "urn:ietf:params:oauth:grant-type:saml2-bearer";

		public const string JwtBearer = "urn:ietf:params:oauth:grant-type:jwt-bearer";

		public const string Password = "password";

		public const string DeviceCode = "device_code";
	}
}

