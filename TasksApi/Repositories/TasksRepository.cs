using TasksApi.Data;
using Microsoft.Data.SqlClient;

namespace TasksApi.Repositories;

public class TasksRepository : ITasksRepository
{
    private readonly string _connectionString;

    public TasksRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default");
    }

    public async Task<List<TaskDto>> GetTasksCreatedByTeamMember(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        const string query = @"
            SELECT
                t.Name,
                t.Description,
                p.Name,
                tt.Name,
                t.Deadline
            FROM Task t
            JOIN Project p on t.IdProject = p.IdProject
            JOIN TaskType tt on t.IdTaskType = tt.IdTaskType
            WHERE IdCreator = @id
            ORDER BY t.Deadline DESC
        ";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        await using var reader = await command.ExecuteReaderAsync();
        List<TaskDto> tasks = new List<TaskDto>();
        while (await reader.ReadAsync())
        {
            var dto = new TaskDto
            {
                Id = id,
                Name = reader.GetString(0),
                Description = reader.GetString(1),
                ProjectName = reader.GetString(2),
                TypeName = reader.GetString(3),
                Deadline = reader.GetDateTime(4)
            };
            tasks.Add(dto);
        }

        return tasks;
    }

    public async Task<List<TaskDto>> GetTasksAssignedToTeamMember(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        const string query = @"
            SELECT
                t.Name,
                t.Description,
                p.Name,
                tt.Name,
                t.Deadline
            FROM Task t
            JOIN Project p on t.IdProject = p.IdProject
            JOIN TaskType tt on t.IdTaskType = tt.IdTaskType
            WHERE IdAssignedTo = @id
            ORDER BY t.Deadline DESC
        ";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        await using var reader = await command.ExecuteReaderAsync();
        List<TaskDto> tasks = new List<TaskDto>();
        while (await reader.ReadAsync())
        {
            var dto = new TaskDto
            {
                Id = id,
                Name = reader.GetString(0),
                Description = reader.GetString(1),
                ProjectName = reader.GetString(2),
                TypeName = reader.GetString(3),
                Deadline = reader.GetDateTime(4)
            };
            tasks.Add(dto);
        }

        return tasks;
    }

    public async Task<TeamMemberDto?> GetTeamMember(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        const string query = @"
            SELECT
                tm.FirstName,
                tm.LastName,
                tm.Email
            FROM TeamMember tm
            WHERE tm.IdTeamMember = @id
        ";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;
        return new TeamMemberDto
        {
            FirstName = reader.GetString(0),
            LastName = reader.GetString(1),
            Email = reader.GetString(2)
        };
    }

    public async Task DeleteProjectWithTasks(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();
        try
        {
            await using var deleteProject = new SqlCommand(
                @"
                    DELETE FROM Project
                    WHERE IdProject = @id
                ",
                connection, transaction
            );
            await using var deleteTasks = new SqlCommand(
                @"
                    DELETE FROM Task
                    WHERE IdProject = @id
                ",
                connection, transaction
            );
            deleteProject.Parameters.AddWithValue("@id", id);
            deleteTasks.Parameters.AddWithValue("@id", id);
            await deleteTasks.ExecuteNonQueryAsync();
            await deleteProject.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
        }
    }

    public async Task<bool> ProjectExists(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        const string query = @"
            SELECT IIF(EXISTS (SELECT 1
                FROM Project
                WHERE IdProject = @id), 1, 0
            ) AS ProjectExists
        ";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        var result = (int)(await command.ExecuteScalarAsync() ?? 0);
        return result == 1;
    }
}