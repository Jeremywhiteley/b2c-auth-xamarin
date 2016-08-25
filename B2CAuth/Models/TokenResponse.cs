using System;
using Newtonsoft.Json;

namespace B2CAuth
{
	public class TokenResponse
	{
		[JsonProperty("not_before")]
		public long NotBefore { get; set; }

		[JsonProperty("token_type")]
		public string TokenType { get; set; }

		[JsonProperty("id_token")]
		public string IdToken { get; set; }

		[JsonProperty("id_token_expires_in")]
		public long IdTokenExpiresIn { get; set; }

		[JsonProperty("profile_info")]
		public string ProfileInfo { get; set; }

		[JsonProperty("refresh_token")]
		public string RefreshToken { get; set; }

		[JsonProperty("refresh_token_expires_in")]
		public long RefreshTokenExpiresIn { get; set; }

		/// <summary>
		/// Gets the identifier token expires date.
		/// </summary>
		/// <value>The identifier token expires date.</value>
		public DateTimeOffset IdTokenExpiresDate
		{
			get
			{
				return FromUnixTimeSeconds(NotBefore + IdTokenExpiresIn);
			}
		}

		/// <summary>
		/// Gets the refresh token expires date.
		/// </summary>
		/// <value>The refresh token expires date.</value>
		public DateTimeOffset RefreshTokenExpiresDate
		{
			get
			{
				return FromUnixTimeSeconds(NotBefore + RefreshTokenExpiresIn);
			}
		}

		private static DateTimeOffset FromUnixTimeSeconds(long seconds)
		{
			var dateTimeOffset = new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
			dateTimeOffset = dateTimeOffset.AddSeconds(seconds);
			return dateTimeOffset;
		}
	}
}

