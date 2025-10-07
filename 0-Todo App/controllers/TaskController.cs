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
            Id = Guid.NewGuid(),
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

    public static int DeleteTask(string? username, Guid taskId)
    {

        var loadedTasks = TaskDataUtils.LoadTasks();

        var filteredTasks = loadedTasks.Where((t) => t.Id != taskId).ToList();

        TaskDataUtils.SaveTasks(filteredTasks);

        return HttpStatus.OK;
    }


    public static int DeleteAllTask(string? username)
    {
        var loadedTasks = TaskDataUtils.LoadTasks();

        var filteredTasks = loadedTasks.Where((t) => t.Username != username).ToList();

        TaskDataUtils.SaveTasks(filteredTasks);

        return HttpStatus.OK;
    }

    public static int UpdateTask(string? username, _Task updatedTask)
    {
        var loadedTasks = TaskDataUtils.LoadTasks();

        if (loadedTasks == null || !loadedTasks.Any())
        {
            return HttpStatus.NOT_FOUND;
        }

        var existingTask = loadedTasks.FirstOrDefault(t => t.Id == updatedTask.Id && t.Username == username);

        if (existingTask == null)
        {
            return HttpStatus.NOT_FOUND;
        }

        existingTask.Description = updatedTask.Description;
        existingTask.Category = updatedTask.Category;
        existingTask.Status = updatedTask.Status;

        TaskDataUtils.SaveTasks(loadedTasks);

        return HttpStatus.OK;
    }


}