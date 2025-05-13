using TasksApi.Data;
using TasksApi.Repositories;

namespace TasksApi.Services;

public class TasksService : ITasksService
{
    private readonly ITasksRepository _repository;

    public TasksService(ITasksRepository repository, IConfiguration configuration)
    {
        _repository = repository;
    }

    public async Task<TeamMemberDetailsDto> GetTeamMemberDetailsAsync(int id)
    {
        var teamMember = await _repository.GetTeamMember(id);
        if (teamMember is null)
            throw new TeamMemberDoesNotExistException(id);
        var assignedTasks = await _repository.GetTasksAssignedToTeamMember(id);
        var createdTasks = await _repository.GetTasksCreatedByTeamMember(id);
        return new TeamMemberDetailsDto
        {
            TeamMember = teamMember,
            TasksAssigned = assignedTasks,
            TasksCreated = createdTasks
        };
    }

    public async Task DeleteProjectAsync(int id)
    {
        if (!await _repository.ProjectExists(id))
            throw new ProjectDoesNotExistException(id);
        await _repository.DeleteProjectWithTasks(id);
    }
}