using System;
using Foundation;
using UIKit;
using WebKit;

namespace B2CAuth.iOS
{
	internal class AuthenticationViewController : UIViewController, IWKNavigationDelegate
	{
		string url;
		Func<string, bool> redirectUrlValidator;
		Action<string, bool> redirectUrlCallback;
		UIWebView webView;
		UIActivityIndicatorView activity;

		public AuthenticationViewController(string url, Func<string, bool> redirectUrlValidator, Action<string, bool> redirectUrlCallback)
		{
			this.url = url;
			this.redirectUrlValidator = redirectUrlValidator;
			this.redirectUrlCallback = redirectUrlCallback;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			//navigation bar
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (sender, e) =>
			{
				webView.StopLoading();
				this.redirectUrlCallback(null, true);
				this.DismissViewController(true, null);
			});

			var activityStyle = UIActivityIndicatorViewStyle.White;
			activity = new UIActivityIndicatorView(activityStyle);
			NavigationItem.RightBarButtonItem = new UIBarButtonItem(activity);

			//init webView
			webView = new UIWebView(this.View.Bounds);
			webView.AutoresizingMask = UIViewAutoresizing.All;
			this.View.AddSubview(webView);
			webView.ShouldStartLoad = (webView, request, navigationType) =>
			{
				string requestUrl = request.Url.AbsoluteString;
				if (redirectUrlValidator(requestUrl))
				{
					ValidateRedirectUrl(requestUrl);
					return false;
				}	else
				{
					return true;
				}
			};
			webView.LoadStarted += (sender, e) => 
			{
				activity.StartAnimating();
			};
			webView.LoadFinished += (sender, e) => 
			{
				activity.StopAnimating();
			};
			webView.LoadError += (sender, e) => 
			{
				activity.StopAnimating();
			};

			//load
			webView.LoadRequest(NSUrlRequest.FromUrl(NSUrl.FromString(url)));
		}

		private void ValidateRedirectUrl(string redirectUrl)
		{
			this.DismissViewController(true, null);
			this.redirectUrlCallback(redirectUrl, false);
		}
	}
}

