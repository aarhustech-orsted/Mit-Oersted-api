using Google.Cloud.Firestore;

namespace Mit_Oersted.Domain.Entities.Models
{
    [FirestoreData]
    public class AddressModel
    {
        public string Id { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; }

        [FirestoreProperty]
        public string AddressString { get; set; }
    }
}
