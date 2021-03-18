using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace Mit_Oersted.Domain.Entities.Models
{
    [FirestoreData]
    public class UserModel
    {
        public string Id { get; set; }

        [FirestoreProperty("name")]
        [Required]
        public string Name { get; set; }

        [FirestoreProperty("email")]
        [Required]
        public string Email { get; set; }

        [FirestoreProperty("address")]
        [Required]
        public string Address { get; set; }

        [FirestoreProperty("phone")]
        [Required]
        public string Phone { get; set; }
    }
}
