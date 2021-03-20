namespace Mit_Oersted.Domain.Models
{
    public class Webapidata
    {
        public string ProjectId { get; set; }
        public string ProjectApiKey { get; set; }
        public string BucketName { get; set; }
        public Cryptodata Crypto { get; set; }
    }

    public class Cryptodata
    {
        public int SALT_BYTE_SIZE { get; set; }
        public int HASH_BYTE_SIZE { get; set; }
        public int PBKDF2_ITERATIONS { get; set; }
        public int ITERATION_INDEX { get; set; }
        public int SALT_INDEX { get; set; }
        public int PBKDF2_INDEX { get; set; }
        public string SALT { get; set; }
        public string INITVECTOR { get; set; }
    }
}
