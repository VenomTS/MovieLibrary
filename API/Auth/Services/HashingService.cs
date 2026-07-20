using System.Security.Cryptography;
using System.Text;

namespace API.Auth.Services;

public class HashingService
{
    public void CreateHash(string text, out byte[] hash, out byte[] salt)
    {
        using var hmac = new HMACSHA512();
        salt = hmac.Key;
        hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(text));
    }

    public bool VerifyHash(string text, byte[] hash, byte[] salt)
    {
        using var hmac = new HMACSHA512(salt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(text));
        return computedHash.SequenceEqual(hash);
    }
}