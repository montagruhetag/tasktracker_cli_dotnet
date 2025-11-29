const string HELP_MESSAGE = """
Available commands:
  1. add [task name] -> create the new task
  2. update [task id] [new task name] -> update task name by id
  3. delete [task id] -> delete task by id
  4. mark [task id] [todo|in-progress|done] -> change task status
  6. list -> display tasks
  7. list [todo|in-progress|done] -> display tasks with filtered by status
""";

if (TryGetArg(args, 0, typeof(string), out object result))
{
    string command = (string)result;
    var taskManager = new Models.TaskManager("tasks.json");
    var commands = new Dictionary<string, Func<string[], Models.TaskManager, bool>>
    {
        {"add", Add},
        {"update", Update},
        {"delete", Delete},
        {"list", List},
        {"mark", Mark},
    };

    if (!commands.ContainsKey(command))
    {
        Console.WriteLine("Command not found");
        return;
    }

    var func = commands[command];
    func(args, taskManager);
}
else
{
    Console.WriteLine(HELP_MESSAGE);
}

static bool Mark(string[] args, Models.TaskManager taskManager)
{
    if (TryGetArg(args, 1, typeof(int), out object result1) && TryGetArg(args, 2, typeof(string), out object result2))
    {
        int id = (int)result1;
        Models.Status? status = ParseStatus((string)result2);
        if (status is null)
        {
            Console.WriteLine("Status not found");
            return false;
        }
        taskManager.ChangeTaskStatus(id, (Models.Status)status);
    }
    return false;
}

static bool Add(string[] args, Models.TaskManager taskManager)
{
    if (TryGetArg(args, 1, typeof(string), out object result))
    {
        string description = (string)result;
        taskManager.AddTask(description);
        return true;
    }
    return false;
}

static bool Update(string[] args, Models.TaskManager taskManager)
{
    if (TryGetArg(args, 1, typeof(int), out object result1) && TryGetArg(args, 2, typeof(string), out object result2))
    {
        int id = (int)result1;
        string description = (string)result2;
        taskManager.UpdateTask(id, description);
        return true;
    }
    return false;
}

static bool Delete(string[] args, Models.TaskManager taskManager)
{
    if (TryGetArg(args, 1, typeof(int), out object result))
    {
        int id = (int)result;
        taskManager.DeleteTask(id);
        return true;
    }
    return false;
}

static bool List(string[] args, Models.TaskManager taskManager)
{
    Models.Status? status = null;
    if (TryGetArg(args, 1, typeof(string), out object? result))
        status = ParseStatus((string)result);

    Console.WriteLine(taskManager.GetFormattedList(status));
    return true;
}

static bool TryGetArg(string[] args, int pos, Type type, out object arg)
{
    arg = new object();
    if (args.Length > pos)
    {
        if (type == typeof(int))
        {
            if (int.TryParse(args[pos], out int result))
                arg = result;
            else
                return false;
        }
        else
            arg = args[pos];
        return true;
    }
    return false;
}

static Models.Status? ParseStatus(string str)
{
    Models.Status? status = null;
    status = str switch
    {
        "todo" => Models.Status.Todo,
        "in-progress" => Models.Status.InProgress,
        "done" => Models.Status.Done,
        _ => null
    };
    return status;
}
