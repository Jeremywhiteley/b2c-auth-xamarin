using System;
using System.Threading.Tasks;

namespace B2CAuth
{
	public interface IAuthClient
	{
		Task<AuthenticationResult> AcquireTokenAsync(string policy, Action startFetchingTokenProvider, Action endFetchingProvider, string loginHint = null);

		Task<AuthenticationResult> AcquireTokenSilentAsync(string policy, bool forceRefresh);

		TokenResponse Token { get; }

		IWebViewClient WebViewClient { get; set; }

		Task Clear();
	}
}

