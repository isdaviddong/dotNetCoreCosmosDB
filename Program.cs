using System;
using Microsoft.Azure.Cosmos;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace cosmosdb
{
    class Program
    {
        //你的cosmos DB EndpointUri
        private static readonly string EndpointUri = "https://_______________.documents.azure.com:443/";
        //你的cosmos db PrimaryKey
        private static readonly string PrimaryKey = "6YbWwz83UlU2Lb___________________syYAje8x28thmzzvHDMcsg==";
        private static CosmosClient cosmosClient;
        private static Database database;
        private static Container container;
        private static string databaseId = "CRM";
        private static string containerId = "Customers";
        static readonly HttpClient client = new HttpClient();
        static readonly string CompanyDataUrl = "https://data.taipei/api/v1/dataset/296acfa2-5d93-4706-ad58-e83cc951863c?scope=resourceAquire";

        static async Task Main(string[] args)
        {
            cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions()
            { ApplicationName = "CosmosDBTest" });

            Console.WriteLine("\n準備建立DB與Container...任按一鍵繼續");
            Console.ReadKey();
            //建立DB與Container
            CreateDbAndContainer();

            Console.WriteLine("\n準備新增資料...任按一鍵繼續");
            Console.ReadKey();
            //新增資料
            InsertNewData();

            Console.WriteLine("\n準備查詢資料...任按一鍵繼續");
            Console.ReadKey();
            //查詢資料
            QueryData();
        }

        private static async Task InsertNewData()
        {
            var customers = await GetCustomers();
            var count = 1;
            foreach (var cust in customers)
            {
                cust.id = Guid.NewGuid().ToString();
                cust.TEL = "02-12345678";
                var item = container.CreateItemAsync<Customer>(cust, new PartitionKey(cust.Title)).Result;
                Console.WriteLine($"Created item {count++}: \n{Newtonsoft.Json.JsonConvert.SerializeObject(item)}\n");
            }
            Console.WriteLine("\n100 items has been added...");
        }

        private static void QueryData()
        {
            //查詢
            var sqlQueryText = "SELECT * FROM c WHERE CONTAINS(c.NO,'12') ";
            Console.WriteLine("執行查詢: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Customer> queryResultSetIterator = container.GetItemQueryIterator<Customer>(queryDefinition);

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Customer> currentResultSet = queryResultSetIterator.ReadNextAsync().Result;
                foreach (Customer Customer in currentResultSet)
                {
                    Console.WriteLine("\nitem {0}", Customer.NO);
                }
            }
        }

        private static void CreateDbAndContainer()
        {
            // Create a new database
            database = cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).Result;
            Console.WriteLine(" Database Created: {0}\n", database.Id);

            // Create a new container
            container = database.CreateContainerIfNotExistsAsync(containerId, "/Title").Result;
            Console.WriteLine(" Container Created: {0}\n", container.Id);
        }

        static async Task<Customer[]> GetCustomers()
        {
            var response = await client.GetAsync(CompanyDataUrl);
            response.EnsureSuccessStatusCode();
            var resultBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Wrapper>(resultBody);
            return result.Result.Results;
        }
    }
}
