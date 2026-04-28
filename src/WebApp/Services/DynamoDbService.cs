using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Configuration;
using WebApp.Models;

namespace WebApp.Services
{
    public class DynamoDbService : IDynamoDbService
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly string _tableName;

        public DynamoDbService(IAmazonDynamoDB dynamoDb, IConfiguration configuration)
        {
            _dynamoDb = dynamoDb;
            _tableName = configuration["DynamoDB:TableName"] ?? "Items";
        }

        public async Task<List<Item>> GetAllItemsAsync()
        {
            var request = new ScanRequest { TableName = _tableName };
            var response = await _dynamoDb.ScanAsync(request);

            var items = new List<Item>();
            foreach (var attributeMap in response.Items)
            {
                items.Add(MapToItem(attributeMap));
            }
            return items;
        }

        public async Task<Item> GetItemAsync(string id)
        {
            var request = new GetItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = id } }
                }
            };

            var response = await _dynamoDb.GetItemAsync(request);
            return response.Item.Count == 0 ? null : MapToItem(response.Item);
        }

        public async Task PutItemAsync(Item item)
        {
            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "Id",          new AttributeValue { S = item.Id } },
                    { "Title",       new AttributeValue { S = item.Title } },
                    { "Description", new AttributeValue { S = item.Description ?? string.Empty } },
                    { "CreatedAt",   new AttributeValue { S = item.CreatedAt.ToString("O") } }
                }
            };

            await _dynamoDb.PutItemAsync(request);
        }

        public async Task DeleteItemAsync(string id)
        {
            var request = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = id } }
                }
            };

            await _dynamoDb.DeleteItemAsync(request);
        }

        private static Item MapToItem(Dictionary<string, AttributeValue> map)
        {
            return new Item
            {
                Id          = map.TryGetValue("Id",          out var id)          ? id.S          : string.Empty,
                Title       = map.TryGetValue("Title",       out var title)       ? title.S       : string.Empty,
                Description = map.TryGetValue("Description", out var desc)        ? desc.S        : string.Empty,
                CreatedAt   = map.TryGetValue("CreatedAt",   out var createdAt)
                              && DateTime.TryParse(createdAt.S, out var dt)
                              ? dt
                              : DateTime.UtcNow
            };
        }
    }
}
