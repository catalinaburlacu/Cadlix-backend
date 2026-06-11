using System;
using System.Security.Cryptography;
using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.DTOs.Frontend;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Cadlix_backend.BusinessLayer.Utilities;

public class JWT
{
    private static readonly TimeSpan AccessTokenExpiry = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan RefreshTokenExpiry = TimeSpan.FromDays(30);

    public string GenerateJWTToken(UserDTO user)
    {
        var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Level.ToString()),
        };

        var jwtToken = new JwtSecurityToken(
            issuer: "cadlix_backend_api",
            audience: "cadlix_backend_api_clients",
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.Add(AccessTokenExpiry),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(
                   Encoding.UTF8.GetBytes("TEST_SECRET_KEY_EXTENDED_FOR_256BIT")
                ),
                SecurityAlgorithms.HmacSha256Signature)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }

    public string GenerateRefreshToken()
    {
        var tokenBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(tokenBytes);
        return Convert.ToBase64String(tokenBytes);
    }

    public DateTime GetRefreshTokenExpiresAt() => DateTime.UtcNow.Add(RefreshTokenExpiry);

    public DateTime GetAccessTokenExpiresAt() => DateTime.UtcNow.Add(AccessTokenExpiry);
}
