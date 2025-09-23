using static Globals;


public class TaskController
{
    public static (_Task[]? Tasks, int Status) GetTasks(string? username)
    {
        var loadedTasks = TaskDataUtils.LoadTasks();

        if (loadedTasks == null || !loadedTasks.Any())
        {
            return (null, HttpStatus.NOT_FOUND);
        }

        _Task[] tasks = loadedTasks
            .Where(t => t.Username == username)
            .ToArray();

        if (tasks.Length == 0)
        {
            return (null, HttpStatus.NOT_FOUND);
        }

        return (tasks, HttpStatus.OK);
    }


    public static int AddTask(string? username, string taskDescription, string taskCategory, string status)
    {

        var loadedTasks = TaskDataUtils.LoadTasks();

        int userTaskCount = loadedTasks.Count(t => t.Username == username);

        if (userTaskCount == 10)
        {

            return HttpStatus.BAD_REQUEST;
        }

        var newTask = new _Task
        {
            Username = username,
            Description = taskDescription,
            Category = taskCategory,
            Status = status
        };

        // Add to the list
        loadedTasks.Add(newTask);

        // Add to the json file
        TaskDataUtils.SaveTasks(loadedTasks);

        return HttpStatus.CREATED;
    }
}