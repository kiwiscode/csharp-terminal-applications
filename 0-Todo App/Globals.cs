global using todo_app.Models;
global using todo_app.Constants;

public static class Globals
{
    // Register or Log In
    public static string? authMode = "";

    // List to store users
    public static List<User> UserList { get; set; } = new();

    // List to store tasks
    public static List<_Task> TaskList { get; set; } = new();

    public static string? username = "";
    public static bool exitApp = false;

}