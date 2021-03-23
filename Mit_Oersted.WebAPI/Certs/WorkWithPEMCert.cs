using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Mit_Oersted.WebApi.Certs
{
    public class WorkWithPEMCert
    {
        public readonly X509Certificate2 Certificate;

        public WorkWithPEMCert(string certificatePath, string certificateKeyPath)
        {
            string certPem = File.ReadAllText(certificatePath);
            string keyPem = File.ReadAllText(certificateKeyPath);

            Certificate = X509Certificate2.CreateFromPem(certPem, keyPem);
        }
    }
}
