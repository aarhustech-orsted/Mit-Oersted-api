using Google.Cloud.Firestore;

namespace Mit_Oersted.Domain.Entities.Models
{
    [FirestoreData]
    public class Address
    {
        public string Id { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; }

        [FirestoreProperty]
        public string RoadName { get; set; }

        [FirestoreProperty]
        public string HouseNumber { get; set; }

        [FirestoreProperty]
        public string PostalCode { get; set; }

        [FirestoreProperty]
        public string City { get; set; }

        [FirestoreProperty]
        public string InvoiceFolder { get; set; }
    }
}
