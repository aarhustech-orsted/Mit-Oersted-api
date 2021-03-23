﻿using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Mit_Oersted.WebApi.Certs
{
    public class WorkWithPEMCert
    {
        public readonly X509Certificate2 Certificate;

        public WorkWithPEMCert(string certificatePath, string certificateKeyPath)
        {
            string[] files = Directory.GetFiles("/https/live/local.ch99q.com");
            Console.WriteLine("What can I see in the dir: " + string.Join(Environment.NewLine, files));

            if (!File.Exists(certificatePath))
            {
                Console.WriteLine($"Error certificatePath does not exist!!: {certificateKeyPath}");
            }
            if (!File.Exists(certificatePath))
            {
                Console.WriteLine($"Error certificateKeyPath does not exist!!: {certificateKeyPath}");
            }

            Console.WriteLine($"certificatePath: {certificatePath}");
            string certPem = File.ReadAllText(certificatePath);

            Console.WriteLine($"certificatePath: {certificatePath}");
            string keyPem = File.ReadAllText(certificatePath);

            Certificate = X509Certificate2.CreateFromPem(certPem, keyPem);
        }
    }
}
