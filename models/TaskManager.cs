namespace Models;

using System.Text;
using System.Text.Json;

public class TaskManager
{
    public string Path { get; init; }
    public List<Task> Tasks { get; private set; }
    public TaskManager(string path)
    {
        Path = path;
        Tasks = GetTasks();
    }

    private List<Task> GetTasks()
    {
        if (!File.Exists(Path))
            return new List<Task>();

        try
        {
            string json = File.ReadAllText(Path);
            var tasks = JsonSerializer.Deserialize<List<Task>>(json);
            return tasks ?? new List<Task>();
        }
        catch
        {
            return new List<Task>();
        }
    }

    public void AddTask(string description)
    {
        var task = CreateTask(description);
        Tasks.Add(task);
        Save();
    }

    public void UpdateTask(int id, string description)
    {
        var task = Tasks.Where(t => t.Id == id).FirstOrDefault();
        if (task != null)
        {
            task.Description = description;
            task.UpdatedAt = DateTime.Now;
            Save();
        }
    }

    public void DeleteTask(int id)
    {
        Tasks = Tasks.Where(t => t.Id != id).ToList();
        Save();
    }

    private Task CreateTask(string description)
    {
        int maxId = Tasks.Count > 0 ? Tasks.Max(t => t.Id) + 1 : 1;
        return new Task(maxId, description, 0, DateTime.Now, DateTime.Now);
    }

    public void ChangeTaskStatus(int id, Status status)
    {
        Tasks.Where(t => t.Id == id).FirstOrDefault()?.Status = status!;
        Save();
    }

    private void Save()
    {
        string json = JsonSerializer.Serialize(Tasks);
        File.WriteAllText(Path, json);
    }

    public string GetFormattedList(Status? status = null)
    {
        if (Tasks.Count == 0)
            return "";

        string dateFormat = "HH:mm:ss dd.MM.yyyy";
        var statusFormat = new Dictionary<Status, string>() { { Status.Todo, "Todo" }, { Status.InProgress, "In progress" }, { Status.Done, "Done" } };
        var sb = new StringBuilder();
        sb.Append("List of tasks:\n");
        var tasks = status is not null ? Tasks.Where(t => t.Status == status) : Tasks;
        foreach (var task in tasks)
            sb.Append($"[{task.Id}] {task.Description} | {statusFormat[task.Status]} | U: {task.UpdatedAt.ToString(dateFormat)} C: {task.CreatedAt.ToString(dateFormat)}\n");

        return sb.ToString().TrimEnd('\n');
    }
}