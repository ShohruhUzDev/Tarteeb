//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Tarteeb.Api.Models.Foundations.Tokens;
using Tarteeb.Api.Models.Foundations.Users;

namespace Tarteeb.Api.Brokers.Tokens
{
    public class TokenBroker : ITokenBroker
    {
        private readonly TokenConfiguration tokenConfiguration;

        public TokenBroker(IConfiguration configuration)
        {
            this.tokenConfiguration = new TokenConfiguration();
            configuration.Bind("Jwt", this.tokenConfiguration);
        }

        public string GenerateJWT(User user)
        {

            byte[] convertedKeyToBytes =
                Encoding.UTF8.GetBytes(this.tokenConfiguration.Key);

            var securityKey =
                new SymmetricSecurityKey(convertedKeyToBytes);

            var cridentials =
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name,user.FirstName)
            };

            var token = new JwtSecurityToken(
                this.tokenConfiguration.Issuer,
                this.tokenConfiguration.Audience,
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cridentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string HashToken(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}