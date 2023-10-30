using BCrypt.Net;

namespace Zaphyros.Core;

public static class PasswordHelper
{
    private static HashType HashType => HashType.SHA512;
    private static bool EnhancedEntropy => false;

    // Function to generate a random salt
    private static string GenerateSalt(int seed)
    {
        return BCrypt.Net.BCrypt.GenerateSalt(seed, 'x');
    }

    // Function to hash the password
    public static string HashPassword(string password)
    {
        string salt = GenerateSalt(12);
        return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }

    // Function to verify the password
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
