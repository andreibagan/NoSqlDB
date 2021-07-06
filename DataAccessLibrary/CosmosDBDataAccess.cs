using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary
{
    public class CosmosDBDataAccess
    {
        private readonly string _endpointUrl;
        private readonly string _primaryKey;
        private readonly string _databaseName;
        private readonly string _containerName;
        private CosmosClient _cosmosClient;
        private Database _database;
        private Container _container;

        public CosmosDBDataAccess(string endpointUrl, string primaryKey, string databaseName, string containerName)
        {
            _endpointUrl = endpointUrl;
            _primaryKey = primaryKey;
            _databaseName = databaseName;
            _containerName = containerName;

            _cosmosClient = new CosmosClient(_endpointUrl, _primaryKey);
            _database = _cosmosClient.GetDatabase(_databaseName);
            _container = _database.GetContainer(_containerName);
        }

        public async Task<List<T>> LoadRecordsAsync<T>()
        {
            string sql = "select * from c";

            QueryDefinition queryDefinition = new QueryDefinition(sql);
            FeedIterator<T> feedIterator = _container.GetItemQueryIterator<T>(queryDefinition);

            List<T> output = new List<T>();

            while(feedIterator.HasMoreResults)
            {
                FeedResponse<T> currentResultSet = await feedIterator.ReadNextAsync();

                foreach (var item in currentResultSet)
                {
                    output.Add(item);
                }
            }

            return output;
        }

        public async Task<T> LoadRecordByIdAsync<T>(string id)
        {
            string sql = "select * from c where c.id = @Id";

            QueryDefinition queryDefinition = new QueryDefinition(sql).WithParameter("@Id", id);
            FeedIterator<T> feedIterator = _container.GetItemQueryIterator<T>(queryDefinition);

            while (feedIterator.HasMoreResults)
            {
                FeedResponse<T> currentResultSet = await feedIterator.ReadNextAsync();

                foreach (var item in currentResultSet)
                {
                    return item;
                }
            }

            throw new Exception("Item not found");
        }

        public async Task UpsertRecordAsync<T>(T record)
        {
            await _container.UpsertItemAsync(record);
        }   

        public async Task DeleteRecordAsync<T>(string id, string partitionKey)
        {
            await _container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
        }
    }
}
