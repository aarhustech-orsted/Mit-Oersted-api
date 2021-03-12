using Google.Cloud.Firestore;

namespace Mit_Oersted.Domain.Entities.Models
{
    [FirestoreData]
    public class User
    {
        public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreProperty]
        public string Address { get; set; }

        [FirestoreProperty]
        public string Phone { get; set; }
    }
}
