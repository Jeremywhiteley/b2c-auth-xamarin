using System;
using UIKit;

namespace B2CAuth.iOS
{
	public class IosWebViewClient : IWebViewClient
	{
		public IosWebViewClient(Func<UIViewController> callerViewControllerFunc)
		{
			CallerViewControllerFunc = callerViewControllerFunc;
		}
		public Func<UIViewController> CallerViewControllerFunc { get; set; }

		public void Authorize(string url, Func<string, bool> redirectUrlValidator, Action<string, bool> redirectUrlCallback)
		{
			//get caller
			UIViewController callerVC = null;
			if (CallerViewControllerFunc != null)
			{
				callerVC = CallerViewControllerFunc();
			}

			//present
			if (callerVC != null)
			{
				AuthenticationViewController authVC = new AuthenticationViewController(url, redirectUrlValidator, redirectUrlCallback);
				UINavigationController navVC = new UINavigationController(authVC);
				callerVC.PresentViewController(navVC, true, null);
			}	else
			{
				throw new Exception("Missing CallerViewController for IosWebViewClient. Should set the current RootViewController that present another UIViewController");
			}
		}
	}
}

