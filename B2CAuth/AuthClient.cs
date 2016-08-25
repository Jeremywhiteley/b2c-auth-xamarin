using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace B2CAuth
{
	public class AuthClient : IAuthClient
	{
		public string Authority { get; private set; }

		public string ClientId { get; private set; }

		public string Scopes { get; set; }

		public string RedirectUrl { get; set; }

		public TokenResponse Token { get { return _tokenCache.Token; } }

		public IWebViewClient WebViewClient { get; set; }
		private TokenCache _tokenCache;

		public AuthClient(string authority, string clientId)
		{
			this.Authority = authority;
			this.ClientId = clientId;

			//fix authorize
			if (this.Authority != null && this.Authority.EndsWith("/", StringComparison.Ordinal))
			{
				this.Authority = this.Authority.Remove(this.Authority.Length - 1);
			}

			//init
			_tokenCache = new TokenCache();

			//default values
			Scopes = "openid offline_access";
			RedirectUrl = "urn:ietf:wg:oauth:2.0:oob";
		}

		public Task<AuthenticationResult> AcquireTokenAsync(string policy, Action startFetchingTokenProvider, Action endFetchingProvider, string loginHint)
		{
			Log.Info("AcquireTokenAsync: {0} -> {1}", policy, loginHint);
			string authorizeEndpoint = string.Format(EndpointConstants.AuthorizeEndpointTemplate, this.Authority, policy, this.ClientId, Uri.EscapeDataString(this.Scopes), Uri.EscapeDataString(this.RedirectUrl));
			if (!string.IsNullOrWhiteSpace(loginHint))
			{
				authorizeEndpoint += "&login_hint=" + loginHint;
			}

			//check WebView
			if (WebViewClient == null)
			{
				throw new Exception("Missing WebViewClient. Should be IosWebViewClient or DroidWebViewClient");
			}

			//authorize via WebView
			Log.Info("authorizeEndpoint: {0}", authorizeEndpoint);
			TaskCompletionSource<AuthenticationResult> task = new TaskCompletionSource<AuthenticationResult>();
			WebViewClient.Authorize(authorizeEndpoint, (string requestUrl) => 
			{
				Log.Info("{0}", requestUrl);
				if (requestUrl.StartsWith(RedirectUrl, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}

				return false;
			}, async (string redirectUrl, bool isUICanceled) =>
			{
				AuthenticationResult result = null;
				if (isUICanceled)
				{ 
					result = new AuthenticationResult(AuthResultType.Canceled);
				}	else
				{
					if (redirectUrl != null)
					{
						var idx = Math.Max(redirectUrl.IndexOf("?code=", StringComparison.Ordinal), redirectUrl.IndexOf("&code=", StringComparison.Ordinal));
						if (idx >= 0)
						{
							var code = redirectUrl.Substring(idx + "?code=".Length);

							//fetch token
							startFetchingTokenProvider?.Invoke();
							AuthenticationResult authResult = await RetrieveAuthResult(policy, code, OAuth2GrantType.AuthorizationCode);
							endFetchingProvider?.Invoke();
							result = authResult;
						}
					}
					else
					{
						result = new AuthenticationResult(AuthResultType.Failed, "AuthClient Error: Empty Redirect Url");
					}
				}

				task.SetResult(result);
			});

			return task.Task;
		}

		public async Task<AuthenticationResult> AcquireTokenSilentAsync(string policy, bool forceRefresh)
		{
			Log.Info("AcquireTokenSilentAsync: {0} -> {1}", policy, forceRefresh);
			//get token
			var token = _tokenCache.Token;
			if (token != null)
			{
				if (!forceRefresh)
				{
					//check storage
					string existingIdToken = token.IdToken;
					DateTimeOffset tokenExpiryDate = token.IdTokenExpiresDate;

					if (!string.IsNullOrWhiteSpace(existingIdToken) && tokenExpiryDate >= DateTimeOffset.Now)
					{
						return new AuthenticationResult(token);
					}
				}

				//check exist refresh token
				string existRefreshToken = token.RefreshToken;
				DateTimeOffset refreshTokenExpiryDate = token.RefreshTokenExpiresDate;
				if (!string.IsNullOrWhiteSpace(existRefreshToken) && refreshTokenExpiryDate >= DateTimeOffset.Now)
				{
					AuthenticationResult authResult = await RetrieveAuthResult(policy, existRefreshToken, OAuth2GrantType.AuthorizationCode);
					return authResult;
				}

				return new AuthenticationResult(AuthResultType.Failed, "Invalid refresh token");
			}

			return new AuthenticationResult(AuthResultType.Failed, "Invalid token Cache");
		}

		private async Task<AuthenticationResult> RetrieveAuthResult(string policy, string code, string grantType)
		{
			Log.Info("RetrieveAuthResult: {0} -> {1} -> {2}", policy, code, grantType);
			string tokenEndpoint = string.Format(EndpointConstants.TokenEndpointTemplate, this.Authority, policy);

			var client = new HttpClient();

			//create content
			HttpRequestMessage request = CreateRequestMessage(code, grantType, tokenEndpoint);

			//parse response
			var response = await client.SendAsync(request);
			string strRes = null;
			if (response.Content != null)
			{
				strRes = await response.Content.ReadAsStringAsync();
			}
			if (response.IsSuccessStatusCode)
			{
				try
				{
					if (strRes != null)
					{
						TokenResponse token = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(strRes);
						if (token != null)
						{
							CacheTokenIfSuccess(token);
							return new AuthenticationResult(token);
						}
					}

					return new AuthenticationResult(AuthResultType.Failed, "Invalid response access token content");
				}
				catch (Exception ex)
				{
					return new AuthenticationResult(AuthResultType.Failed, ex.Message);
				}
			}
			else
			{
				return new AuthenticationResult(AuthResultType.Failed, strRes);
			}
		}

		private HttpRequestMessage CreateRequestMessage(string code, string grantType, string tokenEndpoint)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
			Dictionary<string, string> bodyParameters = new Dictionary<string, string>
			{
				{ "grant_type", grantType },
				{ "client_id", this.ClientId },
				{ "scope", this.Scopes },
				{ "code",  code},
				{ "redirect_uri",  this.RedirectUrl},
			};

			request.Content = new FormUrlEncodedContent(bodyParameters);
			return request;
		}

		private void CacheTokenIfSuccess(TokenResponse token)
		{
			if (token != null)
			{
				_tokenCache.Token = token;
			}
		}

		public async Task Clear()
		{
			_tokenCache.Token = null;
		}
	}
}

