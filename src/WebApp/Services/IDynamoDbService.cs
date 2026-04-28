using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Services
{
    public interface IDynamoDbService
    {
        Task<List<Item>> GetAllItemsAsync();
        Task<Item> GetItemAsync(string id);
        Task PutItemAsync(Item item);
        Task DeleteItemAsync(string id);
    }
}
