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
            var tmpcertificatePath = Path.GetFullPath(certificatePath);
            var tmpcertificateKeyPath = Path.GetFullPath(certificateKeyPath);

            if (!File.Exists(tmpcertificatePath))
            {
                Console.WriteLine($"Error certificatePath does not exist!!: {certificateKeyPath}");
            }
            if (!File.Exists(tmpcertificateKeyPath))
            {
                Console.WriteLine($"Error certificateKeyPath does not exist!!: {certificateKeyPath}");
            }
            if (!File.Exists(tmpcertificatePath) || !File.Exists(tmpcertificateKeyPath))
            {
                string[] files = Directory.GetFiles("/https/live/local.ch99q.com");
                Console.WriteLine(string.Join(Environment.NewLine, files));
            }

            string certPem = File.ReadAllText(tmpcertificatePath);
            string keyPem = File.ReadAllText(tmpcertificateKeyPath);

            Certificate = X509Certificate2.CreateFromPem(certPem, keyPem);
        }
    }
}
