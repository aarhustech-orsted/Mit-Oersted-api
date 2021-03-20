using Microsoft.Extensions.Configuration;
using Mit_Oersted.Domain.Models;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Mit_Oersted.Domain.Security
{
    public class Cryptographic : ICryptographic
    {
        private readonly IConfiguration _config;

        private readonly int SALT_BYTE_SIZE = 0;
        private readonly int HASH_BYTE_SIZE = 0;
        private readonly int PBKDF2_ITERATIONS = 0;

        private readonly int ITERATION_INDEX = 0;
        private readonly int SALT_INDEX = 0;
        private readonly int PBKDF2_INDEX = 0;

        private static byte[] _saltBytes;
        private static byte[] _initVectorBytes;

        public Cryptographic(IConfiguration config)
        {
            _config = config;
            var webapidata = JsonSerializer.Deserialize<Webapidata>(File.ReadAllText(_config.GetSection("webapi").Value));
            SALT_BYTE_SIZE = webapidata.Crypto.SALT_BYTE_SIZE;
            HASH_BYTE_SIZE = webapidata.Crypto.HASH_BYTE_SIZE;
            PBKDF2_ITERATIONS = webapidata.Crypto.PBKDF2_ITERATIONS;

            ITERATION_INDEX = webapidata.Crypto.ITERATION_INDEX;
            SALT_INDEX = webapidata.Crypto.SALT_INDEX;
            PBKDF2_INDEX = webapidata.Crypto.PBKDF2_INDEX;

            _saltBytes = Encoding.UTF8.GetBytes(webapidata.Crypto.SALT);
            _initVectorBytes = Encoding.UTF8.GetBytes(webapidata.Crypto.INITVECTOR);
        }

        public string CreateHash(string stringToHash)
        {
            RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SALT_BYTE_SIZE];
            csprng.GetBytes(salt);

            byte[] hash = PBKDF2(stringToHash, salt, PBKDF2_ITERATIONS, HASH_BYTE_SIZE);
            return PBKDF2_ITERATIONS + ":" + Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        public bool ValidateHashString(string stringToTest, string correctHash)
        {
            char[] delimiter = { ':' };
            string[] split = correctHash.Split(delimiter);
            int iterations = int.Parse(split[ITERATION_INDEX]);
            byte[] salt = Convert.FromBase64String(split[SALT_INDEX]);
            byte[] hash = Convert.FromBase64String(split[PBKDF2_INDEX]);

            byte[] testHash = PBKDF2(stringToTest, salt, iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }

        public string Encrypt(string plainText, string password)
        {
            return Convert.ToBase64String(EncryptToBytes(Encoding.UTF8.GetBytes(plainText), password));
        }

        public string Decrypt(string cipherText, string password)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText.Replace(' ', '+'));
            return Decrypt(cipherTextBytes, password).TrimEnd('\0');
        }

        private static byte[] EncryptToBytes(byte[] plainTextBytes, string password)
        {
            int keySize = 256;

            byte[] initialVectorBytes = _initVectorBytes;
            byte[] saltValueBytes = _saltBytes;
            byte[] keyBytes = new Rfc2898DeriveBytes(password, saltValueBytes).GetBytes(keySize / 8);

            using RijndaelManaged symmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC
            };

            using ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initialVectorBytes);
            using MemoryStream memStream = new MemoryStream();
            using CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();

            return memStream.ToArray();
        }

        private static string Decrypt(byte[] cipherTextBytes, string password)
        {
            int keySize = 256;

            byte[] initialVectorBytes = _initVectorBytes;
            byte[] saltValueBytes = _saltBytes;
            byte[] keyBytes = new Rfc2898DeriveBytes(password, saltValueBytes).GetBytes(keySize / 8);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            using RijndaelManaged symmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC
            };

            using ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initialVectorBytes);
            using MemoryStream memStream = new MemoryStream(cipherTextBytes);
            using CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read);
            int byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++) { diff |= (uint)(a[i] ^ b[i]); }
            return diff == 0;
        }

        private static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
        {
            return new Rfc2898DeriveBytes(password, salt)
            {
                IterationCount = iterations
            }.GetBytes(outputBytes);
        }
    }
}
