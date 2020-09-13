using System;
using Microsoft.Azure.Cosmos;

namespace cosmosdb
{
    class Program
    {
        //你的cosmos DB EndpointUri
        private static readonly string EndpointUri = "https://_______________.documents.azure.com:443/";
        //你的cosmos db PrimaryKey
        private static readonly string PrimaryKey = "aFN7hUoxX2____________________DGOqWClUsA==";
        private static CosmosClient cosmosClient;
        private static Database database;
        private static Container container;
        private static string databaseId = "CRM";
        private static string containerId = "Customers";

        static void Main(string[] args)
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

        private static void InsertNewData()
        {
            for (int i = 1; i <= 100; i++)
            {
                var cust = new Customer()
                {
                    id = Guid.NewGuid().ToString(),
                    Title = "錸客多巴有限公司",
                    TEL = "02-12345678",
                    Address = "台北市中正區忠孝一路四段３８號",
                    NO = (new Random()).Next(11111111, 99999999).ToString()
                };

                var item = container.CreateItemAsync<Customer>(cust, new PartitionKey(cust.Title)).Result;
                Console.WriteLine("Created item {0}: \n{1}\n", i, Newtonsoft.Json.JsonConvert.SerializeObject(item));
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
    }
}
