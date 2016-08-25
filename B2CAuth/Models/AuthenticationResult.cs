using System;
namespace B2CAuth
{
	public class AuthenticationResult
	{
		public TokenResponse Token { get; private set; }

		public AuthResultType ResultType { get; private set; }

		public string ErrorMessage { get; private set; }

		public AuthenticationResult (TokenResponse token)
		{
			this.Token = token;
			this.ResultType = AuthResultType.Success;
		}

		public AuthenticationResult(AuthResultType resultType, string errorMessage = null)
		{
			this.Token = null;
			this.ResultType = resultType;
			this.ErrorMessage = errorMessage;
		}
	}
}

