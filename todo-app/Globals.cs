global using todo_app.Models;
global using todo_app.Constants;

public static class Globals
{
    // Register or Log In
    public static string? authMode = "";

    // List to store users
    public static List<User> UserList { get; set; } = new();

}