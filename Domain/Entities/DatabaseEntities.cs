using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Mit_Oersted.Domain.Entities
{
    public class DatabaseEntities
    {
        private readonly IConfiguration _config;

        public DatabaseEntities(IConfiguration config)
        {
            _config = config;
            InitializeDb();
        }

        public string ProjectId { get; private set; }
        public FirestoreDb FirestoreDb { get; private set; }

        private void InitializeDb()
        {
            ProjectId = _config.GetSection("ProjectId").Value;
            FirestoreDb = FirestoreDb.Create(ProjectId);
        }
    }
}
