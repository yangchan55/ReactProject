using Microsoft.IdentityModel.Tokens;
using ReactProject.jwt;
using System;


namespace ReactProject.Model.Common
{
    public class UserInfo
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
    }

    public class UserToken
    {
        public string userToken { get; set; }

		public UserInfo ValidateToken(ITokenService _jwtTokenService)
		{
			String _userEmail;
			String _userId;
			try
			{
				SecurityToken validatedToken;
				var principal = _jwtTokenService.ValidateAgentToken(userToken, new TokenValidationParameters
				{
					RequireExpirationTime = true,
					RequireSignedTokens = true,
					ValidateAudience = false,
					ValidateIssuer = false,
					ValidateLifetime = false
				}, out validatedToken);

				_userEmail = principal.FindFirst("userEmail").Value;
				_userId = principal.FindFirst("userId").Value;


				/*
				if (_userEmail == null || _userEmail.Length < 6)
				{
					throw new RestApiException(ExceptionType.BadRequest, LangCode);
				}

				if (_userId == null || _userId.Length < 6)
				{
					throw new RestApiException(ExceptionType.BadRequest, LangCode);
				}
				*/

				return new UserInfo
				{
					UserEmail = _userEmail,
					UserId = _userId
				};
			}
			catch (Exception e)
			{
				throw e;
			}


		}
	}
}
