using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Mit_Oersted.WebApi.Certs
{
    public class WorkWithPEMCert
    {
        public readonly X509Certificate2 Certificate;

        public WorkWithPEMCert(string certificatePath)
        {
            try
            {
                var cert = new DirectoryInfo(certificatePath).GetFiles(@"cert.pem")[0];
                var privkey = new DirectoryInfo(certificatePath).GetFiles(@"privkey.pem")[0];

                Console.WriteLine(cert.FullName);
                Console.WriteLine(privkey.FullName);

                if (!cert.Exists)
                {
                    Console.WriteLine("Error can't find cert.pem");
                    return;
                }
                if (!privkey.Exists)
                {
                    Console.WriteLine("Error can't find privkey.pem");
                    return;
                }

                string certPem = File.ReadAllText(cert.FullName);
                string keyPem = File.ReadAllText(privkey.FullName);

                Certificate = X509Certificate2.CreateFromPem(certPem, keyPem);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
