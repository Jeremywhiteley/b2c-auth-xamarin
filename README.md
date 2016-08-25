# Xamarin Authentication for Azure AD B2C
Easily Authentication library for replace MSAL dotnet, since having issue on Social login (Facebook/LinkedIn).

```
async void Authenticate(string policy)
{

	string authority = "https://login.microsoftonline.com/ryanngbeta.onmicrosoft.com";
	string clientId = "9b7f9dee-c454-4101-ad1d-a9eb6595a6ec";
	IAuthClient authClient = new AuthClient(authority, clientId);
	authClient.WebViewClient = new IosWebViewClient(() => this);

	//request
	string prefillEmail = "";//could be NULL
	AuthenticationResult authResult = await authClient.AcquireTokenAsync(policy, () =>
	{
		//POST request Access Token -> started
		SVProgressHUDBinding.SVProgressHUD.Show();
	}, () =>
	{
		//POST request Access Token -> finished
		SVProgressHUDBinding.SVProgressHUD.Dismiss();
	}, prefillEmail);

	switch (authResult.ResultType)
	{
		case AuthResultType.Success:
			new UIAlertView("Success", authResult.Token.IdToken.Substring(0, 50) + "...", null, "Ok").Show();
			break;
		case AuthResultType.Failed:
			new UIAlertView("Failed", authResult.ErrorMessage, null, "Ok").Show();
			break;
		default:
			break;
	}
}
```


# TODO
- [ ] Android WebView
- [ ] Sample for Android native, Xamarin.Forms
- [ ] Nuget package and Xamarin Components Store
