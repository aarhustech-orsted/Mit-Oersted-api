using Mit_Oersted.Domain.Entities;
using Mit_Oersted.Domain.Pagination;
using Google.Cloud.Firestore;
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

        public async void Add(Entities.Models.Transaction entity)
        {
            await _entities.FirestoreDb?.Collection(_collection)?.AddAsync(entity);
        }

        public async Task<IEnumerable<Entities.Models.Transaction>> GetAllAsync()
        {
            Query Query = _entities.FirestoreDb?.Collection(_collection);
            QuerySnapshot QuerySnapshot = await Query?.GetSnapshotAsync();
            var list = new List<Entities.Models.Transaction>();

            foreach (DocumentSnapshot documentSnapshot in QuerySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    string json = JsonConvert.SerializeObject(documentSnapshot.ToDictionary());
                    list.Add(JsonConvert.DeserializeObject<Entities.Models.Transaction>(json));
                }
            }

            return list;
        }

        public PaginationResult<Entities.Models.Transaction> GetByFilter(PaginationQuery paginationQuery)
        {
            return GetPaginatedResult(
                    paginationQuery,
                    GetAllAsync().Result.AsQueryable(),
                    (elements, filter) => elements.Where(x => x.EventType != null && x.EventType.Contains(paginationQuery.Filter)),
                    elements => elements.OrderBy(x => x.Created));
        }
    }
}
