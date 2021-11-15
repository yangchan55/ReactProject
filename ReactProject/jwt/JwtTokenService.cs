using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Xml;

namespace ReactProject.jwt
{
    public class JwtTokenService : ITokenService
    {
		private readonly string _publicRawKey;
		private readonly string _privateRawKey;

		private RSA _publicRsa;
		private RSA _privateRsa;

		public JwtTokenService(IConfiguration configuration)
		{
			_publicRawKey = configuration["publicKey"];
			_privateRawKey = configuration["privateKey"];

			if (string.IsNullOrEmpty(_privateRawKey))
			{
				throw new Exception("_privateRawKey가 null 입니다.");
			}

			_privateRsa = RSA.Create();
			_publicRsa = RSA.Create();

			_privateRsa.FromXML(_privateRawKey);
			_publicRsa.FromXML(_publicRawKey);
		}

		public JwtSecurityToken CreateAgentToken(string userId, string userEmail, int expireAfterDay = 30)
		{
			RsaSecurityKey privateKey = new RsaSecurityKey(_privateRsa);

			var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);

			var header = new JwtHeader(credentials);
			var claims = new HashSet<Claim>
			{
				new Claim("userId", userId),
				new Claim("userEmail", userEmail),

			};

			var payload = new JwtPayload(
				"ReactProject",
				userId,
				claims,
				DateTime.Now,
				DateTime.Now.AddDays(expireAfterDay)
			);

			return new JwtSecurityToken(header, payload);
		}

		public ClaimsPrincipal ValidateAgentToken(string token, TokenValidationParameters validationParameters,
			out SecurityToken validatedToken)
		{
			RsaSecurityKey publicKey = new RsaSecurityKey(_publicRsa);
			validationParameters.IssuerSigningKey = publicKey;

			var handler = new JwtSecurityTokenHandler();
			ClaimsPrincipal principal = handler.ValidateToken(token, validationParameters, out validatedToken);

			/*
            String userId = principal.FindFirst("userId").Value;
            String licenseType = principal.FindFirst("licenseType").Value;
            String deviceId = principal.FindFirst("deviceId").Value;
            String aud = principal.FindFirst("aud").Value;
            String Issuer = validatedToken.Issuer;
            */


			return principal;
		}
	}

	public static class Util
	{
		public static void FromXML(this RSA rsa, string xmlString)
		{
			var parameters = new RSAParameters();

			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xmlString);

			if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
			{
				foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
				{
					switch (node.Name)
					{
						case "Modulus":
							parameters.Modulus = (string.IsNullOrEmpty(node.InnerText)
								? null
								: Convert.FromBase64String(node.InnerText));
							break;
						case "Exponent":
							parameters.Exponent = (string.IsNullOrEmpty(node.InnerText)
								? null
								: Convert.FromBase64String(node.InnerText));
							break;
						case "P":
							parameters.P = (string.IsNullOrEmpty(node.InnerText)
								? null
								: Convert.FromBase64String(node.InnerText));
							break;
						case "Q":
							parameters.Q = (string.IsNullOrEmpty(node.InnerText)
								? null
								: Convert.FromBase64String(node.InnerText));
							break;
						case "DP":
							parameters.DP = (string.IsNullOrEmpty(node.InnerText)
								? null
								: Convert.FromBase64String(node.InnerText));
							break;
						case "DQ":
							parameters.DQ = (string.IsNullOrEmpty(node.InnerText)
								? null
								: Convert.FromBase64String(node.InnerText));
							break;
						case "InverseQ":
							parameters.InverseQ = (string.IsNullOrEmpty(node.InnerText)
								? null
								: Convert.FromBase64String(node.InnerText));
							break;
						case "D":
							parameters.D = (string.IsNullOrEmpty(node.InnerText)
								? null
								: Convert.FromBase64String(node.InnerText));
							break;
					}
				}
			}
			else
			{
				throw new Exception("Invalid XML RSA key.");
			}

			rsa.ImportParameters(parameters);
		}
	}
}
