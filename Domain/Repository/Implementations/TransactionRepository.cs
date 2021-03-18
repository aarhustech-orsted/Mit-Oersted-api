using Google.Cloud.Firestore;
using Mit_Oersted.Domain.Entities;
using Mit_Oersted.Domain.Pagination;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mit_Oersted.Domain.Repository.Implementations
{
    public class TransactionRepository : BaseRepository, ITransactionRepository
    {
        private readonly DatabaseEntities _entities;
        private static readonly string _collection = "transactions";

        public TransactionRepository(DatabaseEntities entities)
        {
            _entities = entities ?? throw new ArgumentNullException(nameof(entities));
        }

        public async void Add(Entities.Models.TransactionModel entity)
        {
            await _entities.FirestoreClient?.Collection(_collection)?.AddAsync(entity);
        }

        public async Task<IEnumerable<Entities.Models.TransactionModel>> GetAllAsync()
        {
            Query Query = _entities.FirestoreClient?.Collection(_collection);
            QuerySnapshot QuerySnapshot = await Query?.GetSnapshotAsync();
            var list = new List<Entities.Models.TransactionModel>();

            foreach (DocumentSnapshot documentSnapshot in QuerySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    string json = JsonConvert.SerializeObject(documentSnapshot.ToDictionary());
                    list.Add(JsonConvert.DeserializeObject<Entities.Models.TransactionModel>(json));
                }
            }

            return list;
        }

        public PaginationResult<Entities.Models.TransactionModel> GetByFilter(PaginationQuery paginationQuery)
        {
            return GetPaginatedResult(
                    paginationQuery,
                    GetAllAsync().Result.AsQueryable(),
                    (elements, filter) => elements.Where(x => x.EventType != null && x.EventType.Contains(paginationQuery.Filter)),
                    elements => elements.OrderBy(x => x.Created));
        }
    }
}
