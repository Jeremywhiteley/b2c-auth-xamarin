using System;
namespace B2CAuth
{
	public interface IWebViewClient
	{
		void Authorize(string url, Func<string, bool> redirectUrlValidator, Action<string, bool> redirectUrlCallback);
	}
}

