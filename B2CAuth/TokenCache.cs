using System;
using Plugin.Settings.Abstractions;

namespace B2CAuth
{
	internal class TokenCache
	{
		//Use refactor later
		string keyNotBefore = "keyNotBefore";
		string keyTokenType = "keyTokenType";
		string keyIdToken = "keyIdToken";
		string keyIdTokenExpiresIn = "keyIdTokenExpiresIn";
		string keyProfileInfo = "keyProfileInfo";
		string keyRefreshToken = "keyRefreshToken";
		string keyRefreshTokenExpiresIn = "keyRefreshTokenExpiresIn";

		private ISettings _settings;
		public TokenCache()
		{
			_settings = Plugin.Settings.CrossSettings.Current;
		}

		private TokenResponse _token;
		public TokenResponse Token
		{
			get
			{
				_token = _token ?? GetSettings();
				return _token;
			}
			set
			{
				if (_token != value)
				{
					_token = value;
					UpdateSettings(_token);
				}
			}
		}

		/// <summary>
		/// Updates the settings.
		/// </summary>
		/// <param name="value">Value.</param>
		void UpdateSettings(TokenResponse value)
		{
			if (value != null)
			{
				_settings.AddOrUpdateValue<long>(keyNotBefore, value.NotBefore);
				_settings.AddOrUpdateValue<string>(keyTokenType, value.TokenType);
				_settings.AddOrUpdateValue<string>(keyIdToken, value.IdToken);
				_settings.AddOrUpdateValue<long>(keyIdTokenExpiresIn, value.IdTokenExpiresIn);
				_settings.AddOrUpdateValue<string>(keyProfileInfo, value.ProfileInfo);
				_settings.AddOrUpdateValue<string>(keyRefreshToken, value.RefreshToken);
				_settings.AddOrUpdateValue<long>(keyRefreshTokenExpiresIn, value.RefreshTokenExpiresIn);
			}	else
			{
				_settings.Remove(keyNotBefore);
				_settings.Remove(keyTokenType);
				_settings.Remove(keyIdToken);
				_settings.Remove(keyIdTokenExpiresIn);
				_settings.Remove(keyProfileInfo);
				_settings.Remove(keyRefreshToken);
				_settings.Remove(keyRefreshTokenExpiresIn);
			}
		}

		TokenResponse GetSettings()
		{
			string idToken = _settings.GetValueOrDefault<string>(keyIdToken);
			if (!string.IsNullOrWhiteSpace(idToken))
			{
				return new TokenResponse
				{
					IdToken = idToken,
					NotBefore = _settings.GetValueOrDefault<long>(keyNotBefore),
					TokenType = _settings.GetValueOrDefault<string>(keyTokenType),
					IdTokenExpiresIn = _settings.GetValueOrDefault<long>(keyIdTokenExpiresIn),
					ProfileInfo = _settings.GetValueOrDefault<string>(keyProfileInfo),
					RefreshToken = _settings.GetValueOrDefault<string>(keyRefreshToken),
					RefreshTokenExpiresIn = _settings.GetValueOrDefault<long>(keyRefreshTokenExpiresIn)
				};
			}

			return null;
		}
	}
}

