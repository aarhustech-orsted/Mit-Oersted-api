using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Mit_Oersted.Domain.Models;
using System.IO;
using System.Text.Json;

namespace Mit_Oersted.Domain.Entities
{
    public class DatabaseEntities
    {
        private readonly IConfiguration _config;

        public DatabaseEntities(IConfiguration config)
        {
            _config = config;
            Initialize();
        }

        public string ProjectId { get; private set; }
        public FirestoreDb FirestoreClient { get; private set; }
        public StorageClient StorageClient { get; private set; }

        private void Initialize()
        {
            var webapidata = JsonSerializer.Deserialize<Webapidata>(File.ReadAllText(_config.GetSection("webapi").Value));

            ProjectId = webapidata.ProjectId;
            FirestoreClient = FirestoreDb.Create(ProjectId);

            StorageClient = StorageClient.Create();
        }
    }
}
