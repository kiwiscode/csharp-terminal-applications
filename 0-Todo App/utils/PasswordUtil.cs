


using System.Security.Cryptography;

public static class PasswordUtils
{

    // Secure input password
    public static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[0..^1]; // delete last character
                Console.Write("\b \b");     // delete from screen
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password += key.KeyChar;
                Console.Write("*"); // press asterist
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();

        return password;
    }
    // Create hash
    public static string HashPassword(string password)
    {
        // Create random salt
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Create hash with the help of PBKDF2
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32); // 256-bit hash

        // Merge salt + hash and encode Base64
        byte[] hashBytes = new byte[48]; // 16 + 32
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);


        return Convert.ToBase64String(hashBytes);
    }

    // Verify hash
    public static bool VerifyPassword(string password, string storedHash)
    {
        byte[] hashBytes = Convert.FromBase64String(storedHash);

        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32);

        for (int i = 0; i < 32; i++)
        {
            if (hashBytes[i + 16] != hash[i])
            {
                return false;
            }
        }
        return true;
    }


}