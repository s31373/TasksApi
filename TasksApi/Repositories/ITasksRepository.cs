using TasksApi.Data;

namespace TasksApi.Repositories;

public interface ITasksRepository
{
    Task<List<TaskDto>> GetTasksCreatedByTeamMember(int id);
    Task<List<TaskDto>> GetTasksAssignedToTeamMember(int id);
    Task<TeamMemberDto?> GetTeamMember(int id);
    Task DeleteProjectWithTasks(int id);
    Task<bool> ProjectExists(int id);
}