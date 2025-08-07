using System.Security.Cryptography;

namespace backuppv2.Services;

public class FileHasher
{
  public static string ComputeHash(string filepath)
  {
    using var sha256 = SHA256.Create();
    using var stream = File.OpenRead(filepath);
    var hash = sha256.ComputeHash(stream);
    return Convert.ToHexString(hash);
  }
}