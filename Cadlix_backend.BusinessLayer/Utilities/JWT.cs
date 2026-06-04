using System;
using System.Collections.Concurrent;
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
    private static readonly ConcurrentDictionary<string, RefreshTokenEntry> _refreshTokens = new();
    private static readonly TimeSpan AccessTokenExpiry = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan RefreshTokenExpiry = TimeSpan.FromDays(7);

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

    public string GenerateRefreshToken(int userId)
    {
        var tokenBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(tokenBytes);
        var token = Convert.ToBase64String(tokenBytes);

        _refreshTokens[token] = new RefreshTokenEntry
        {
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.Add(RefreshTokenExpiry),
        };

        return token;
    }

    public int? ValidateRefreshToken(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            return null;

        if (!_refreshTokens.TryGetValue(refreshToken, out var entry))
            return null;

        if (entry.ExpiresAt < DateTime.UtcNow)
        {
            _refreshTokens.TryRemove(refreshToken, out _);
            return null;
        }

        _refreshTokens.TryRemove(refreshToken, out _);

        return entry.UserId;
    }

    public DateTime GetAccessTokenExpiresAt() => DateTime.UtcNow.Add(AccessTokenExpiry);

    private class RefreshTokenEntry
    {
        public int UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
