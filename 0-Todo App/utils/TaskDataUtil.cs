using System.Text.Json;
public static class TaskDataUtils
{
    public static void CreateTaskJSONFile()
    {
        File.WriteAllText("tasks.json", "[]");
    }

    public static List<_Task> LoadTasks()
    {
        if (!File.Exists("tasks.json")) return new List<_Task>();

        string json = File.ReadAllText("tasks.json");
        return JsonSerializer.Deserialize<List<_Task>>(json) ?? new List<_Task>();
    }

    public static void SaveTasks(List<_Task> taskList)
    {
        string json = JsonSerializer.Serialize(taskList, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("tasks.json", json);
    }
}