using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Mit_Oersted.WebApi.Certs
{
    public class WorkWithPEMCert
    {
        public readonly X509Certificate2 Certificate;

        public WorkWithPEMCert(string certificatePath, string certificateKeyPath)
        {
            if (!File.Exists(certificatePath))
            {
                Console.WriteLine($"Error certificatePath does not exist!!: {certificateKeyPath}");
            }
            if (!File.Exists(certificateKeyPath))
            {
                Console.WriteLine($"Error certificateKeyPath does not exist!!: {certificateKeyPath}");
            }
            if (!File.Exists(certificateKeyPath) || !File.Exists(certificatePath))
            {
                string[] files = Directory.GetFiles("/https/live/local.ch99q.com");
                Console.WriteLine(string.Join(Environment.NewLine, files));
            }

            string certPem = File.ReadAllText(certificatePath);
            string keyPem = File.ReadAllText(certificateKeyPath);

            Certificate = X509Certificate2.CreateFromPem(certPem, keyPem);
        }
    }
}
