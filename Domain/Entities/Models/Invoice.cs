using Google.Cloud.Firestore;

namespace Mit_Oersted.Domain.Entities.Models
{
    [FirestoreData]
    public class Invoice
    {
        public string Id { get; set; }

        [FirestoreProperty]
        public string FolderName { get; set; }

        [FirestoreProperty]
        public string[] Files { get; set; }
    }
}
