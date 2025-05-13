namespace TasksApi.Data;

public class TeamMemberDetailsDto
{
    public TeamMemberDto TeamMember { get; set; }
    public List<TaskDto> TasksAssigned { get; set; }
    public List<TaskDto> TasksCreated { get; set; }
}
