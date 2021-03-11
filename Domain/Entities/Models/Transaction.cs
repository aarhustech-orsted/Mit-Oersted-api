using Google.Cloud.Firestore;
using System;

namespace Domain.Entities.Models
{
    [FirestoreData]
    public class Transaction
    {
        public int Id { get; set; }

        [FirestoreProperty]
        public DateTime Created { get; set; }

        [FirestoreProperty]
        public string EventType { get; set; }

        [FirestoreProperty]
        public string EventData { get; set; }
    }
}
