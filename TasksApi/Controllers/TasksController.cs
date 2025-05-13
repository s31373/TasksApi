using TasksApi.Data;
using TasksApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace TasksApi.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITasksService _tasksService;

    public TasksController(ITasksService tasksService)
    {
        _tasksService = tasksService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTeamMember(int id)
    {
        try
        {
            var task = await _tasksService.GetTeamMemberDetailsAsync(id);
            if (task == null)
                return NotFound();
            return Ok(task);
        }
        catch (TeamMemberDoesNotExistException)
        {
            return BadRequest($"Team member with id {id} does not exist");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        try
        {
            await _tasksService.DeleteProjectAsync(id);
            return Ok();
        }
        catch (ProjectDoesNotExistException)
        {
            return BadRequest($"Project with id {id} does not exist");
        }
    }
}