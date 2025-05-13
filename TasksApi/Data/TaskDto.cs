namespace TasksApi.Data;

public class TaskDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ProjectName { get; set; }
    public string TypeName { get; set; }
    public DateTime Deadline { get; set; }
}