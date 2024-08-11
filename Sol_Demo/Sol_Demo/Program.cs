// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var userIds = Enumerable.Range(1, 25).ToList();
int batchSize = 2;
var result = await new ProcessUserClient().ProcessUserIdsAsync(userIds, batchSize);

foreach (var user in result)
{
    Console.WriteLine($"Id: {user.Id}, Name: {user.Name}");
}


// Sample User DTO
public record UserDTO(int Id,string Name);

// Mock client with a GetUserAsync method
public class UserClient{
    public async Task<UserDTO> GetUserAsync(int id){
        await Task.Delay(100);
        return new UserDTO(Id: id, Name: $"User{id}");
    }
}

public class ProcessUserClient{
    public async Task<List<UserDTO>> ProcessUserIdsAsync(List<int> userIds, int batchSize){
        var userClient = new UserClient();
        var userTasks = new List<Task<UserDTO>>();
        int numberOfBatches = (int)Math.Ceiling((double)userIds.Count / batchSize);

        for (int i = 0; i < numberOfBatches; i++)
        {
            var currentIds = userIds.Skip(i * batchSize).Take(batchSize).ToList();
            var tasks = currentIds.Select(id => userClient.GetUserAsync(id)).ToList();
            userTasks.AddRange(tasks.ToList());
        }

        var users = (await Task.WhenAll(userTasks)).ToList();
        return users;
    }
}

