using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace ReactProject.jwt
{
    public interface ITokenService
    {
        JwtSecurityToken CreateAgentToken(string userId, string userEmail, int expireAfterDay = 30);

        ClaimsPrincipal ValidateAgentToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken);
    }
}
