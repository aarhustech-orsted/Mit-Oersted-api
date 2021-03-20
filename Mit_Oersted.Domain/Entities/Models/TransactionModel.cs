using Google.Cloud.Firestore;
using System;

namespace Mit_Oersted.Domain.Entities.Models
{
    [FirestoreData]
    public class TransactionModel
    {
        public string Id { get; set; }

        [FirestoreProperty]
        public DateTime Created { get; set; }

        [FirestoreProperty]
        public string EventType { get; set; }

        [FirestoreProperty]
        public string EventData { get; set; }
    }
}
