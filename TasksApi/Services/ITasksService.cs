using TasksApi.Data;

namespace TasksApi.Services;

public interface ITasksService
{
    Task<TeamMemberDetailsDto?> GetTeamMemberDetailsAsync(int id);
    Task DeleteProjectAsync(int id);
}