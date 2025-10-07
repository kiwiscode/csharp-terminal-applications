using static Globals;
using Spectre.Console;


static class Program
{
    static void Main()
    {


        if (!File.Exists("users.json"))
        // Mock users
        {
            UserList.AddRange(new List<User>
        {
            new User { Username = "John", Password = PasswordUtils.HashPassword("1234") },
            new User { Username = "Jane", Password = PasswordUtils.HashPassword("5678") },
            new User { Username = "Mike", Password = PasswordUtils.HashPassword("abcde") }
        });
            // Save users
            UserDataUtils.SaveUsers(UserList);

        }


        do
        {
            bool repeatAuthModeQuestion = false;
            bool keepDashboard = false;
            string? dashboardAction = "";
            string? error = "";


            do
            {

                bool confirm = AnsiConsole.Confirm("[bold yellow]Already using Todo App? Log in[/]");

                if (confirm)
                {




                    username = AnsiConsole.Ask<string>("[bold rgb(85,88,253)]Your username:[/]");


                    var password = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold rgb(85,88,253)]Your password:[/]")
                     .Secret()
                    );


                    // login
                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {

                        int result = UserController.LoginUser(username, password);

                        if (result == HttpStatus.NOT_FOUND)
                        {
                            ThrowErrorUtils.ThrowError(HttpStatus.NOT_FOUND, "User not found!");
                        }
                        else if (result == HttpStatus.UNAUTHORIZED)
                        {
                            ThrowErrorUtils.ThrowError(HttpStatus.UNAUTHORIZED, "Invalid credentials!");
                        }
                        else if (result == HttpStatus.OK)
                        {
                            using (var progress = new ProgressBar())
                            {
                                for (int i = 0; i <= 100; i++)
                                {
                                    progress.Report((double)i / 100);
                                    Thread.Sleep(20);
                                }
                            }
                            AnsiConsole.MarkupLine($"[bold green]👋 Welcome to your dashboard, [underline yellow]{username}[/]![/]");
                            repeatAuthModeQuestion = false;
                            keepDashboard = true;
                            dashboardAction = "/tasks";
                        }
                    }



                }
                else
                {


                    username = AnsiConsole.Ask<string>("[bold rgb(85,88,253)]Please enter your username:[/]");
                    var password = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold rgb(85,88,253)]Please enter your password:[/]")
                     .Secret()
                    );

                    // register
                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {


                        int result = UserController.RegisterUser(username, PasswordUtils.HashPassword(password));


                        if (result == HttpStatus.CONFLICT)
                        {

                            ThrowErrorUtils.ThrowError(HttpStatus.CONFLICT, "User already exists!");
                            repeatAuthModeQuestion = true;
                        }
                        else if (result == HttpStatus.CREATED)
                        {
                            using (var progress = new ProgressBar())
                            {
                                for (int i = 0; i <= 100; i++)
                                {
                                    progress.Report((double)i / 100);
                                    Thread.Sleep(20);
                                }
                            }
                            AnsiConsole.MarkupLine($"[bold green]👋 Welcome to your dashboard, [underline yellow]{username}[/]![/]");
                            repeatAuthModeQuestion = false;
                            keepDashboard = true;
                            dashboardAction = "/tasks";
                        }
                    }

                }

            } while (repeatAuthModeQuestion);




            void RenderError(string errorMessage)
            {
                error = errorMessage;
            }

            if (!File.Exists("tasks.json"))
            {
                TaskDataUtils.CreateTaskJSONFile();
            }

            do
            {
                if (dashboardAction == "/create-task")
                {
                    var taskCategory = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[bold rgb(85,88,253)]Please select a category:[/]")
                            .PageSize(10)
                            .AddChoices("[rgb(254,203,95)]Work[/]", "[rgb(225,133,242)]Personal[/]",
                                        "[rgb(190,229,253)]Family[/]", "[rgb(166,255,235)]Pet[/]")
                    );




                    string cleanCategory = taskCategory.Replace("[rgb(254,203,95)]", "")
                                                       .Replace("[rgb(225,133,242)]", "")
                                                       .Replace("[rgb(190,229,253)]", "")
                                                       .Replace("[rgb(166,255,235)]", "")
                                                       .Replace("[/]", "");

                    (string fg, string bg) = cleanCategory switch
                    {
                        "Work" => ("rgb(0,0,0)", "rgb(254,203,95)"),
                        "Personal" => ("rgb(0,0,0)", "rgb(225,133,242)"),
                        "Family" => ("rgb(0,0,0)", "rgb(190,229,253)"),
                        "Pet" => ("rgb(0,0,0)", "rgb(166,255,235)"),
                        _ => ("white", "black")
                    };

                    AnsiConsole.MarkupLine($"[bold rgb(85,88,253)]Selected category:[/] [{fg} on {bg}] {cleanCategory} [/]");

                    string taskDescription = AnsiConsole.Ask<string>("[bold rgb(85,88,253)]+ New task:[/]");
                    Console.Write("\u001b[1A");
                    Console.Write("\u001b[0J");
                    AnsiConsole.MarkupLine($"[bold rgb(120,120,255)]Task:[/] [italic]{taskDescription}[/]");
                    Console.WriteLine();

                    bool confirm = AnsiConsole.Confirm("[bold yellow]Do you want to create this task?[/]");

                    if (confirm)
                    {

                        // save task to the tasks.json with users username
                        int result = TaskController.AddTask(username, taskDescription, cleanCategory, "Todo");

                        if (result == HttpStatus.BAD_REQUEST)
                        {
                            RenderError("You cannot create more than 10 tasks for this user.");
                        }
                        else if (result == HttpStatus.CREATED)
                        {
                            AnsiConsole.MarkupLine($"[green]Task created successfully:[/] {taskDescription} ([italic]{taskCategory}[/])");

                            AnsiConsole.Progress()
                                .Start(ctx =>
                                {
                                    var showTasks = ctx.AddTask("[green]Tasks loading[/]", maxValue: 100);
                                    while (!showTasks.IsFinished)
                                    {
                                        showTasks.Increment(1.5);
                                        Thread.Sleep(50);
                                    }
                                });
                        }
                        dashboardAction = "/tasks";


                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Task creation canceled.[/]");

                        AnsiConsole.Progress()
                            .Start(ctx =>
                            {
                                var showTasks = ctx.AddTask("[green]Tasks loading[/]", maxValue: 100);
                                while (!showTasks.IsFinished)
                                {
                                    showTasks.Increment(1.5);
                                    Thread.Sleep(50);
                                }
                            });
                        dashboardAction = "/tasks";
                    }



                }
                else if (dashboardAction == "/update-task")
                {
                    var (userTasks, result) = TaskController.GetTasks(username);

                    string cleanAction = "";


                    if (result == HttpStatus.NOT_FOUND || userTasks == null || !userTasks.Any())
                    {
                        AnsiConsole.MarkupLine("[red]No tasks found to update.[/]");
                        dashboardAction = "/tasks";
                    }
                    else
                    {
                        var taskChoices = userTasks.Select((t, index) =>
                           new
                           {
                               Display = $"[yellow]{index + 1}.[/] [cyan]{t.Description}[/] [grey]({t.Category})[/] [dim]{t.Status}[/]",
                               Id = t.Id
                           }).ToList();

                        var selected = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[bold rgb(85,88,253)]Select a task to update:[/]")
                                .PageSize(10)
                                .AddChoices(taskChoices.Select(t => t.Display))
                        );

                        var selectedTask = taskChoices.First(t => t.Display == selected);
                        var taskToUpdate = userTasks.First(t => t.Id == selectedTask.Id);

                        var fieldToUpdate = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("[bold rgb(85,88,253)]Which field do you want to update?[/]")
        .AddChoices(
            "[yellow]Category[/]",
            "[cyan]Description[/]",
            "[green]Status[/]"
        )
);
                        cleanAction = fieldToUpdate
                            .Replace("[yellow]", "")
                            .Replace("[cyan]", "")
                            .Replace("[green]", "")
                            .Replace("[/]", "")
                            .Trim();
                        if (cleanAction == "Description")
                        {
                            var newDescription = AnsiConsole.Ask<string>("[bold rgb(85,88,253)]Enter new description:[/]");
                            bool confirm = AnsiConsole.Confirm($"[bold yellow]Update description to[/] [italic]{newDescription}[/]?");
                            if (confirm)
                            {
                                taskToUpdate.Description = newDescription;
                                TaskController.UpdateTask(username, taskToUpdate);
                                AnsiConsole.MarkupLine("[green]Description updated successfully![/]");
                            }
                        }
                        else if (cleanAction == "Category")
                        {
                            var categoryChoices = new Dictionary<string, string>
    {
        { "Work", "[rgb(0,0,0) on rgb(254,203,95)] Work [/]" },
        { "Personal", "[rgb(0,0,0) on rgb(225,133,242)] Personal [/]" },
        { "Family", "[rgb(0,0,0) on rgb(190,229,253)] Family [/]" },
        { "Pet", "[rgb(0,0,0) on rgb(166,255,235)] Pet [/]" }
    };

                            var newCategoryDisplay = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("[bold rgb(85,88,253)]Select a new category:[/]")
                                    .PageSize(10)
                                    .AddChoices(categoryChoices.Values)
                            );

                            var newCategory = categoryChoices.First(c => c.Value == newCategoryDisplay).Key;

                            bool confirm = AnsiConsole.Confirm($"[bold yellow]Update category to[/] [italic]{newCategory}[/]?");
                            if (confirm)
                            {
                                taskToUpdate.Category = newCategory;
                                TaskController.UpdateTask(username, taskToUpdate);

                                (string fg, string bg) = newCategory switch
                                {
                                    "Work" => ("rgb(0,0,0)", "rgb(254,203,95)"),
                                    "Personal" => ("rgb(0,0,0)", "rgb(225,133,242)"),
                                    "Family" => ("rgb(0,0,0)", "rgb(190,229,253)"),
                                    "Pet" => ("rgb(0,0,0)", "rgb(166,255,235)"),
                                    _ => ("white", "black")
                                };

                                AnsiConsole.MarkupLine($"[green]Category updated successfully to:[/] [{fg} on {bg}] {newCategory} [/]");
                            }
                        }
                        else if (cleanAction == "Status")
                        {
                            var newStatus = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("[bold rgb(85,88,253)]Select a new status:[/]")
        .PageSize(10)
        .AddChoices(
            "[yellow]Todo[/]",
            "[rgb(85,170,255)]In Progress[/]",
            "[green]Completed[/]"
        )
);

                            string cleanStatus = newStatus
                                .Replace("[yellow]", "")
                                .Replace("[rgb(85,170,255)]", "")
                                .Replace("[green]", "")
                                .Replace("[/]", "")
                                .Trim();

                            string statusColored = cleanStatus switch
                            {
                                "Todo" => "[yellow]Todo[/]",
                                "In Progress" => "[rgb(85,170,255)]In Progress[/]",
                                "Completed" => "[green]Completed[/]",
                                _ => cleanStatus
                            };

                            bool confirm = AnsiConsole.Confirm($"[bold yellow]Update status to[/] {statusColored}?");
                            if (confirm)
                            {
                                taskToUpdate.Status = cleanStatus;
                                TaskController.UpdateTask(username, taskToUpdate);

                                AnsiConsole.MarkupLine("[green]Status updated successfully![/]");
                            }

                        }



                        AnsiConsole.Progress()
                                .Start(ctx =>
                                {
                                    var showTasks = ctx.AddTask("[green]Tasks loading[/]", maxValue: 100);
                                    while (!showTasks.IsFinished)
                                    {
                                        showTasks.Increment(1.5);
                                        Thread.Sleep(50);
                                    }
                                });

                        dashboardAction = "/tasks";



                    }

                }
                else if (dashboardAction == "/delete-task")
                {
                    var (userTasks, result) = TaskController.GetTasks(username);

                    if (result == HttpStatus.NOT_FOUND || userTasks == null || !userTasks.Any())
                    {
                        AnsiConsole.MarkupLine("[red]No tasks found to delete.[/]");
                        dashboardAction = "/tasks";
                    }
                    else
                    {
                        var taskChoices = userTasks.Select((t, index) =>
                            new
                            {
                                Display = $"[yellow]{index + 1}.[/] [cyan]{t.Description}[/] [grey]({t.Category})[/] [dim]{t.Status}[/]",
                                Id = t.Id
                            }).ToList();

                        var selected = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[bold red]Select a task to delete:[/]")
                                .PageSize(10)
                                .AddChoices(taskChoices.Select(t => t.Display))
                        );

                        var selectedTask = taskChoices.First(t => t.Display == selected);

                        bool confirmDelete = AnsiConsole.Confirm($"[bold red]Are you sure you want to delete this task?[/]\n{selected}");

                        if (confirmDelete)
                        {
                            int deleteResult = TaskController.DeleteTask(username, selectedTask.Id);

                            if (deleteResult == HttpStatus.OK)
                            {
                                AnsiConsole.MarkupLine("[green]Task deleted successfully![/]");

                                AnsiConsole.Progress()
                                                .Start(ctx =>
                                                {
                                                    var showTasks = ctx.AddTask("[green]Tasks loading[/]", maxValue: 100);
                                                    while (!showTasks.IsFinished)
                                                    {
                                                        showTasks.Increment(1.5);
                                                        Thread.Sleep(50);
                                                    }
                                                });

                            }

                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[yellow]Task deletion canceled.[/]");
                            AnsiConsole.Progress()
                            .Start(ctx =>
                            {
                                var showTasks = ctx.AddTask("[green]Tasks loading[/]", maxValue: 100);
                                while (!showTasks.IsFinished)
                                {
                                    showTasks.Increment(1.5);
                                    Thread.Sleep(50);
                                }
                            });
                        }

                        dashboardAction = "/tasks";
                    }
                }

                else if (dashboardAction == "/delete-all")
                {
                    bool confirm = AnsiConsole.Confirm("[bold yellow]Do you want to delete all tasks?[/]");

                    if (confirm)
                    {


                        int result = TaskController.DeleteAllTask(username);

                        if (result == HttpStatus.OK)
                        {
                            AnsiConsole.MarkupLine("[green]All tasks have been deleted successfully![/]");


                            AnsiConsole.Progress()
                                .Start(ctx =>
                                {
                                    var showTasks = ctx.AddTask("[green]Tasks loading[/]", maxValue: 100);
                                    while (!showTasks.IsFinished)
                                    {
                                        showTasks.Increment(1.5);
                                        Thread.Sleep(50);
                                    }
                                });

                            dashboardAction = "/tasks";

                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Tasks deletion canceled.[/]");

                            AnsiConsole.Progress()
                                .Start(ctx =>
                                {
                                    var showTasks = ctx.AddTask("[green]Tasks loading[/]", maxValue: 100);
                                    while (!showTasks.IsFinished)
                                    {
                                        showTasks.Increment(1.5);
                                        Thread.Sleep(50);
                                    }
                                });
                            dashboardAction = "/tasks";
                        }



                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Task creation canceled.[/]");

                        AnsiConsole.Progress()
                            .Start(ctx =>
                            {
                                var showTasks = ctx.AddTask("[green]Tasks loading[/]", maxValue: 100);
                                while (!showTasks.IsFinished)
                                {
                                    showTasks.Increment(1.5);
                                    Thread.Sleep(50);
                                }
                            });
                        dashboardAction = "/tasks";
                    }
                }
                else if (dashboardAction == "/tasks")
                {


                    var (userTasks, result) = TaskController.GetTasks(username);

                    var table = new Table();
                    table.Title = new TableTitle("[bold underline rgb(85,88,253)]Your Tasks[/]");
                    table.Border = TableBorder.Rounded;
                    table.Expand();

                    table.AddColumn(new TableColumn("[bold yellow]Category[/]").Centered());
                    table.AddColumn(new TableColumn("[bold cyan]Description[/]").Centered());
                    table.AddColumn(new TableColumn("[bold green]Status[/]").Centered());
                    if (result == HttpStatus.NOT_FOUND)
                    {
                        table.ShowRowSeparators();
                        table.AddRow(
                        new Markup("[grey]No tasks found[/]"),
                        new Markup("[grey]-[/]"),
                        new Markup("[grey]-[/]")
                );
                    }
                    else
                    {
                        foreach (var task in userTasks!)
                        {
                            (string fg, string bg) = task.Category switch
                            {
                                "Work" => ("rgb(0,0,0)", "rgb(254,203,95)"),
                                "Personal" => ("rgb(0,0,0)", "rgb(225,133,242)"),
                                "Family" => ("rgb(0,0,0)", "rgb(190,229,253)"),
                                "Pet" => ("rgb(0,0,0)", "rgb(166,255,235)"),
                                _ => ("white", "black")
                            };

                            string categoryMarkup = $"[{fg} on {bg}] {task.Category} [/]";

                            string statusMarkup = task.Status switch
                            {
                                "Todo" => "[yellow]Todo[/]",
                                "In Progress" => "[rgb(85,170,255)]In Progress[/]",
                                "Completed" => "[green]Completed[/]",
                                _ => task.Status
                            };
                            table.ShowRowSeparators();
                            table.AddRow(categoryMarkup, task.Description, statusMarkup);


                        }

                    }
                    AnsiConsole.Write(table);
                    dashboardAction = "/task-settings";
                }
                else if (dashboardAction == "/task-settings")
                {


                    var (userTasks, result) = TaskController.GetTasks(username);

                    string cleanAction = "";



                    if (result == HttpStatus.NOT_FOUND)
                    {
                        var settingAction = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[bold rgb(85,88,253)]What do you want to do?[/]")
                            .PageSize(10)
                            .AddChoices(
                                "[bold green]/create-task[/]",
                                "[bold orange1]/logout[/]",
                                "[bold red1]/exit[/]"
                            )
                        );
                        cleanAction = settingAction
                           .Replace("[bold green]", "")
                           .Replace("[bold orange1]", "")
                           .Replace("[bold red1]", "")
                           .Replace("[/]", "")
                           .Trim();

                    }
                    else
                    {
                        var settingAction = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[bold rgb(85,88,253)]What do you want to do?[/]")
                            .PageSize(10)
                            .AddChoices(
                                "[bold green]/create-task[/]",
                                "[bold yellow]/update-task[/]",
                                "[bold red]/delete-task[/]",
                                "[bold magenta]/delete-all[/]",
                                "[bold orange1]/logout[/]",
                                "[bold red1]/exit[/]"
                            )
                        );
                        cleanAction = settingAction
                           .Replace("[bold green]", "")
                           .Replace("[bold yellow]", "")
                           .Replace("[bold red]", "")
                           .Replace("[bold magenta]", "")
                           .Replace("[bold orange1]", "")
                           .Replace("[bold red1]", "")
                           .Replace("[/]", "")
                           .Trim();
                    }




                    if (cleanAction == "/exit")
                    {
                        error = "";
                        exitApp = true;
                        break;
                    }
                    if (cleanAction == "/logout")
                    {
                        error = "";
                        break;
                    }

                    if (cleanAction == "/create-task")
                    {
                        error = "";
                        dashboardAction = "/create-task";
                    }
                    else if (cleanAction == "/delete-task")
                    {
                        error = "";
                        dashboardAction = "/delete-task";
                    }
                    else if (cleanAction == "/update-task")
                    {
                        error = "";
                        dashboardAction = "/update-task";

                    }
                    else if (cleanAction == "/delete-all")
                    {
                        error = "";
                        dashboardAction = "/delete-all";
                    }



                }
                else
                {
                    Console.WriteLine("Unknown command. Try again.");
                    dashboardAction = "/tasks";
                }

                if (!string.IsNullOrEmpty(error))
                {
                    AnsiConsole.MarkupLine($"[red]{error}[/]");
                }
            } while (keepDashboard);

        } while (!exitApp);

    }
}