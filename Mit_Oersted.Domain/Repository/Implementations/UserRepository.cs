using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using Mit_Oersted.Domain.Entities;
using Mit_Oersted.Domain.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mit_Oersted.Domain.Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseEntities _entities;
        private static readonly string _collection = "users";
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            DatabaseEntities entities,
            ILogger<UserRepository> logger)
        {
            _entities = entities ?? throw new ArgumentNullException();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<UserModel>> GetAllAsync()
        {
            Query Query = _entities.FirestoreClient?.Collection(_collection);
            QuerySnapshot QuerySnapshot = await Query?.GetSnapshotAsync();
            var list = new List<UserModel>();

            foreach (DocumentSnapshot documentSnapshot in QuerySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    Dictionary<string, object> dictionary = documentSnapshot.ToDictionary();

                    list.Add(new UserModel()
                    {
                        Id = documentSnapshot.Id,
                        Email = dictionary["email"].ToString(),
                        Name = dictionary["name"].ToString(),
                        Phone = dictionary["phone"].ToString(),
                        Address = dictionary["address"].ToString()
                    });
                }
            }

            return list.OrderBy(x => x.Email).ToList();
        }

        public async Task<UserModel> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) { return null; }

            DocumentReference docRef = _entities.FirestoreClient?.Collection(_collection)?.Document(id.ToString());
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> dictionary = snapshot.ToDictionary();
                return new UserModel()
                {
                    Id = snapshot.Id,
                    Email = dictionary["email"].ToString(),
                    Name = dictionary["name"].ToString(),
                    Phone = dictionary["phone"].ToString(),
                    Address = dictionary["address"].ToString()
                };
            }

            return null;
        }

        public async Task<UserModel> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) { return null; }

            CollectionReference colRef = _entities.FirestoreClient?.Collection(_collection);
            Query query = colRef?.WhereEqualTo("email", email);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    var dbModel = documentSnapshot.ConvertTo<UserModel>();
                    dbModel.Id = documentSnapshot.Id;
                    return dbModel;
                }
            }

            return null;
        }

        public async Task<string> AddAsync(UserModel model)
        {
            DocumentReference docRef = await _entities.FirestoreClient?.Collection(_collection)?.AddAsync(model);
            return docRef.Id;
        }

        public async void RemoveAsync(UserModel model)
        {
            await _entities.FirestoreClient?.Collection(_collection)?.Document(model.Id.ToString()).DeleteAsync();
        }

        public async void UpdateAsync(string id, Dictionary<string, object> updates)
        {
            await _entities.FirestoreClient?.Collection(_collection)?.Document(id)?.UpdateAsync(updates);
        }

        public bool IsEmailAlreadyInUse(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) { return false; }

            UserModel model = GetByEmailAsync(email).Result;
            return model != null;
        }
    }
}
