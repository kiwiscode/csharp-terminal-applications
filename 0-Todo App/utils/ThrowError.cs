


public static class ThrowErrorUtils
{
    public static void ThrowError(int status, string message)
    {
        Console.Error.WriteLine($"Error {status}: {message}");
    }
}