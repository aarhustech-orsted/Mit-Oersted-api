using Google;
using Google.Api.Gax;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mit_Oersted.Domain.Entities;
using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mit_Oersted.Domain.Repository.Implementations
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly DatabaseEntities _entities;
        private readonly ILogger<InvoiceRepository> _logger;
        private readonly IConfiguration _config;

        public InvoiceRepository(
            DatabaseEntities entities,
            ILogger<InvoiceRepository> logger,
            IConfiguration config)
        {
            _entities = entities ?? throw new ArgumentNullException();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config;
        }

        public async Task<List<InvoiceModel>> GetAllAsync()
        {
            var webapidata = JsonSerializer.Deserialize<Webapidata>(File.ReadAllText(_config.GetSection("webapi").Value));
            PagedAsyncEnumerable<Objects, Google.Apis.Storage.v1.Data.Object> listOfObjects = _entities.StorageClient.ListObjectsAsync(webapidata.BucketName);

            List<Google.Apis.Storage.v1.Data.Object> listOfFiles = await listOfObjects.ToListAsync();
            var listOfOctetStream = listOfFiles.Where(x => x.ContentType != "application/x-www-form-urlencoded;charset=UTF-8").ToList();
            var result = new List<InvoiceModel>();

            foreach (Google.Apis.Storage.v1.Data.Object file in listOfOctetStream)
            {
                result.Add(new InvoiceModel()
                {
                    Bucket = webapidata.BucketName,
                    Name = file.Name,
                    ContentType = file.ContentType,
                    Size = int.Parse(file.Size.ToString() ?? "0"),
                    Metadata = (Dictionary<string, string>)file.Metadata,
                    DownloadUrl = SignDownloadUrl(file.Bucket, file.Name)
                });
            }

            return result;
        }

        public async Task<List<InvoiceModel>> GetFolderByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) { return null; }

            var webapidata = JsonSerializer.Deserialize<Webapidata>(File.ReadAllText(_config.GetSection("webapi").Value));
            PagedAsyncEnumerable<Objects, Google.Apis.Storage.v1.Data.Object> listOfObjects = _entities.StorageClient.ListObjectsAsync(webapidata.BucketName);

            List<Google.Apis.Storage.v1.Data.Object> listOfFiles = await listOfObjects.ToListAsync();
            var list = listOfFiles.Where(x => x.Name.Contains(id) && x.Name != $"{id}/").ToList();
            var result = new List<InvoiceModel>();

            foreach (Google.Apis.Storage.v1.Data.Object file in list)
            {
                result.Add(new InvoiceModel()
                {
                    Bucket = webapidata.BucketName,
                    Name = file.Name,
                    ContentType = file.ContentType,
                    Size = int.Parse(file.Size.ToString() ?? "0"),
                    Metadata = (Dictionary<string, string>)file.Metadata,
                    DownloadUrl = SignDownloadUrl(file.Bucket, file.Name)
                });
            }

            return result;
        }

        public async Task<InvoiceModel> GetFileByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) { return null; }

            var webapidata = JsonSerializer.Deserialize<Webapidata>(File.ReadAllText(_config.GetSection("webapi").Value));
            try
            {
                var file = await _entities.StorageClient.GetObjectAsync(webapidata.BucketName, id);

                if (file != null)
                {
                    return new InvoiceModel()
                    {
                        Bucket = webapidata.BucketName,
                        Name = file.Name,
                        ContentType = file.ContentType,
                        Size = int.Parse(file.Size.ToString() ?? "0"),
                        Metadata = (Dictionary<string, string>)file.Metadata,
                        DownloadUrl = SignDownloadUrl(file.Bucket, file.Name)
                    };
                }

                return null;
            }
            catch (GoogleApiException gaex)
            {
                _logger.LogError(gaex.Message);
                return null;
            }
        }

        public async Task<string> AddAsync(string id, Dictionary<string, string> metadata, byte[] fileData)
        {
            var webapidata = JsonSerializer.Deserialize<Webapidata>(File.ReadAllText(_config.GetSection("webapi").Value));
            using var dataStream = new MemoryStream(fileData);
            var storageFile = await _entities.StorageClient.UploadObjectAsync(new Google.Apis.Storage.v1.Data.Object
            {
                Bucket = webapidata.BucketName,
                Name = id,
                ContentType = "application/octet-stream",
                Metadata = metadata
            }, dataStream);

            dataStream.Dispose();

            return storageFile.Id;
        }

        public async void RemoveAsync(string id)
        {
            var webapidata = JsonSerializer.Deserialize<Webapidata>(File.ReadAllText(_config.GetSection("webapi").Value));
            await _entities.StorageClient.DeleteObjectAsync(webapidata.BucketName, id);
        }

        public async void UpdateAsync(string id, Dictionary<string, string> updates)
        {
            var webapidata = JsonSerializer.Deserialize<Webapidata>(File.ReadAllText(_config.GetSection("webapi").Value));
            await _entities.StorageClient.PatchObjectAsync(new Google.Apis.Storage.v1.Data.Object
            {
                Bucket = webapidata.BucketName,
                Name = id,
                ContentType = "application/octet-stream",
                Metadata = updates
            });
        }

        public bool IsInvoiceAlreadyInUse(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) { return false; }

            InvoiceModel model = GetFileByIdAsync(id).Result;
            return model != null;
        }

        private string SignDownloadUrl(string bucket, string name)
        {
            UrlSigner urlSigner = UrlSigner.FromServiceAccountPath(_config.GetSection("GOOGLE_APPLICATION_CREDENTIALS").Value);
            return urlSigner.Sign(bucket, name, TimeSpan.FromHours(1), HttpMethod.Get);
        }
    }
}
