using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Boilerplate.Api.Common;

// Dev-only. Real tokens are issued by the User Service with the same signing key.
public static class JwtTokenFactory
{
    public static string CreateToken(
        IConfiguration configuration,
        Guid userId)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["SigningKey"]!));
        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                userId.ToString()),
        };

        var token = new JwtSecurityToken(
            jwtSection["Issuer"],
            jwtSection["Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
