using System;
using B2CAuth.iOS;
using UIKit;

namespace B2CAuth.SampleiOS
{
	public partial class ViewController : UIViewController
	{
		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
			this.View.BackgroundColor = UIColor.White;

			UIButton btn1 = new UIButton(UIButtonType.System);
			btn1.Frame = new CoreGraphics.CGRect(50, 100, 150, 40);
			View.AddSubview(btn1);
			btn1.SetTitle("B2C_1_SignIn", UIControlState.Normal);
			btn1.TouchUpInside += (sender, e) =>
			{
				Authenticate("B2C_1_SignIn");
			};

			UIButton btn2 = new UIButton(UIButtonType.System);
			btn2.Frame = new CoreGraphics.CGRect(50, 150, 150, 40);
			btn2.SetTitle("B2C_1_SignInSignUp", UIControlState.Normal);
			View.AddSubview(btn2);
			btn2.TouchUpInside += (sender, e) =>
			{
				Authenticate("B2C_1_SignInSignUp");
			};
		}

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

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

