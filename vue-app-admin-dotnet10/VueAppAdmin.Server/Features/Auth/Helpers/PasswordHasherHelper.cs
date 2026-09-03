using Microsoft.AspNetCore.Identity;

namespace VueAppAdmin.Server.Features.Auth.Helpers;

public static class PasswordHasherHelper
{
    private static readonly IPasswordHasher<object> _hasher = new PasswordHasher<object>();
    private static readonly object _user = new();

    public static string HashPassword(string plainText)
        => _hasher.HashPassword(_user, plainText);

    public static bool VerifyPassword(string plainText, string hash)
        => _hasher.VerifyHashedPassword(_user, hash, plainText) != PasswordVerificationResult.Failed;
}
