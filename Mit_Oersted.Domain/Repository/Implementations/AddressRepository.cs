using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using Mit_Oersted.Domain.Entities;
using Mit_Oersted.Domain.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mit_Oersted.Domain.Repository.Implementations
{
    public class AddressRepository : IAddressRepository
    {
        private readonly DatabaseEntities _entities;
        private static readonly string _collection = "addresses";
        private readonly ILogger<AddressRepository> _logger;

        public AddressRepository(
            DatabaseEntities entities,
            ILogger<AddressRepository> logger)
        {
            _entities = entities ?? throw new ArgumentNullException();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<AddressModel>> GetAllAsync()
        {
            Query Query = _entities.FirestoreClient?.Collection(_collection);
            QuerySnapshot QuerySnapshot = await Query?.GetSnapshotAsync();
            var list = new List<AddressModel>();

            foreach (DocumentSnapshot documentSnapshot in QuerySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    Dictionary<string, object> dictionary = documentSnapshot.ToDictionary();

                    list.Add(new AddressModel()
                    {
                        Id = documentSnapshot.Id,
                        AddressString = dictionary["addressString"].ToString(),
                        UserId = dictionary["userId"].ToString()
                    });
                }
            }

            return list.OrderBy(x => x.UserId).ToList();
        }

        public async Task<AddressModel> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) { return null; }

            DocumentReference docRef = _entities.FirestoreClient?.Collection(_collection)?.Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> dictionary = snapshot.ToDictionary();
                return new AddressModel()
                {
                    Id = snapshot.Id,
                    AddressString = dictionary["addressString"].ToString(),
                    UserId = dictionary["userId"].ToString()
                };
            }

            return null;
        }

        public async Task<string> AddAsync(AddressModel model)
        {
            DocumentReference docRef = _entities.FirestoreClient?.Collection(_collection).Document(model.Id);
            var newModel = new Dictionary<string, object>
            {
                { "userId", model.UserId },
                { "addressString", model.AddressString }
            };
            await docRef.SetAsync(newModel);

            return docRef.Id;
        }

        public async void RemoveAsync(AddressModel model)
        {
            await _entities.FirestoreClient?.Collection(_collection)?.Document(model.Id.ToString()).DeleteAsync();
        }

        public async void UpdateAsync(string id, Dictionary<string, object> updates)
        {
            await _entities.FirestoreClient?.Collection(_collection)?.Document(id)?.UpdateAsync(updates);
        }

        public bool IsAddressAlreadyInUse(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) { return false; }

            AddressModel model = GetByIdAsync(Base64Encode(id)).Result;
            return model != null;
        }
        public string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
