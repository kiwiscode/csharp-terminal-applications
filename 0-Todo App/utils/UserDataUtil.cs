using System.Text.Json;
public static class UserDataUtils
{


    public static void SaveUsers(List<User> userList)
    {
        string json = JsonSerializer.Serialize(userList, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("users.json", json);
    }

    public static List<User> LoadUsers()
    {
        if (!File.Exists("users.json")) return new List<User>();

        string json = File.ReadAllText("users.json");
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }
}