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

                string certPem = "-----BEGIN CERTIFICATE-----"
                                + "MIIFIzCCBAugAwIBAgISA51Xy+UUbRVSZR/6g5gD5x2hMA0GCSqGSIb3DQEBCwUA"
                                + "MDIxCzAJBgNVBAYTAlVTMRYwFAYDVQQKEw1MZXQncyBFbmNyeXB0MQswCQYDVQQD"
                                + "EwJSMzAeFw0yMTAzMjMwODQwMDFaFw0yMTA2MjEwODQwMDFaMBoxGDAWBgNVBAMT"
                                + "D2xvY2FsLmNoOTlxLmNvbTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEB"
                                + "AM+Z/TVr6YD3SzLOTCVpImDhocQlseuQz6ebB0pR9ArtBL7KXUgkQAIqVEvbFnTI"
                                + "JznsrZ21b522LioLCGd5fi6JnQ91Xd2UQCgeuvYhLBhNGDSLyE6BAHVBSl1cz+5w"
                                + "BSS4ejO64ItI9oFp1IXD13X3vn919kOfuVZrDgyTCFyvA7ATRYNdY9jJF2+SZU56"
                                + "cdOe1qUeg3c6fTnFd4nt+ikrt2Tya0SHDhJMiNJRp9wLTnfEowiGYqza+pCRmRtQ"
                                + "DQLDc5kwvOLirPFI50CJVb2r7uZw+4jUHlWTyKaRwoTheDPyJG6VG3E0rmZKsMlB"
                                + "wtFRWTHRQCE6hTw2v0PBhGECAwEAAaOCAkkwggJFMA4GA1UdDwEB/wQEAwIFoDAd"
                                + "BgNVHSUEFjAUBggrBgEFBQcDAQYIKwYBBQUHAwIwDAYDVR0TAQH/BAIwADAdBgNV"
                                + "HQ4EFgQUn4lTEiL7ev1hYRcDwiMasV5d05cwHwYDVR0jBBgwFoAUFC6zF7dYVsuu"
                                + "UAlA5h+vnYsUwsYwVQYIKwYBBQUHAQEESTBHMCEGCCsGAQUFBzABhhVodHRwOi8v"
                                + "cjMuby5sZW5jci5vcmcwIgYIKwYBBQUHMAKGFmh0dHA6Ly9yMy5pLmxlbmNyLm9y"
                                + "Zy8wGgYDVR0RBBMwEYIPbG9jYWwuY2g5OXEuY29tMEwGA1UdIARFMEMwCAYGZ4EM"
                                + "AQIBMDcGCysGAQQBgt8TAQEBMCgwJgYIKwYBBQUHAgEWGmh0dHA6Ly9jcHMubGV0"
                                + "c2VuY3J5cHQub3JnMIIBAwYKKwYBBAHWeQIEAgSB9ASB8QDvAHYAlCC8Ho7VjWyI"
                                + "cx+CiyIsDdHaTV5sT5Q9YdtOL1hNosIAAAF4XnST/gAABAMARzBFAiEA9p4lqBTo"
                                + "Filr1S0L6dosbsScCBC9c5MHX6nOp0VRw7kCIDWg5QU9QS1el6r5FAl1a2UUlrCq"
                                + "ZH3eCGX4Ne0DFJMNAHUAfT7y+I//iFVoJMLAyp5SiXkrxQ54CX8uapdomX4i8NcA"
                                + "AAF4XnSUNgAABAMARjBEAiAT7SJ5ajdvpGDnbSO9kApqDusfm2ZzmnORQNxj3fxi"
                                + "kAIgGDO74zHvRVfrKpRcqZjYGcQV1jqSHeA90eXZksst7dkwDQYJKoZIhvcNAQEL"
                                + "BQADggEBAHaKGqWaIf2NA8SIBXG85AWv1Dd2EpWOpl5CArlp3eKAXOjkgHBaUIBv"
                                + "E84lSnGxZU37EhLMYVgcd+/xK6j3+k1ykVyfE+g0vG4HRE2TiKqN+zHy/qgAzlrp"
                                + "W6nYq9ZIjyZhQGdsHzmHhcMFRRe/kPCiZzhsjyfhaJ/iz3iSyviQuOkvO7Y6UTIa"
                                + "9gp2bVLXPHopTfAoMavnrom6xIJurmiIW8Apd3rIQTHiF5QxY2HpppPLFG8SYSER"
                                + "AoINpuJ7UKcp/CNvkPQhzf6SzAeGmcNmFkmzJRRFLsq0r/Pzfb0xWGQkGQsf20bB"
                                + "hEHfLWz7zF7hVPkwQOk1V+f7IGg0lVw="
                                + "-----END CERTIFICATE-----"; //File.ReadAllText(cert.FullName);
                string keyPem = "-----BEGIN PRIVATE KEY-----"
                                + "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDPmf01a+mA90sy"
                                + "zkwlaSJg4aHEJbHrkM+nmwdKUfQK7QS+yl1IJEACKlRL2xZ0yCc57K2dtW+dti4q"
                                + "CwhneX4uiZ0PdV3dlEAoHrr2ISwYTRg0i8hOgQB1QUpdXM/ucAUkuHozuuCLSPaB"
                                + "adSFw9d1975/dfZDn7lWaw4MkwhcrwOwE0WDXWPYyRdvkmVOenHTntalHoN3On05"
                                + "xXeJ7fopK7dk8mtEhw4STIjSUafcC053xKMIhmKs2vqQkZkbUA0Cw3OZMLzi4qzx"
                                + "SOdAiVW9q+7mcPuI1B5Vk8imkcKE4Xgz8iRulRtxNK5mSrDJQcLRUVkx0UAhOoU8"
                                + "Nr9DwYRhAgMBAAECggEAMaqWkHRLveXr4LDION8JMYJpkCKwiTnw5BlLyjUHHKIN"
                                + "05NYc8zwX1Q/LJKysae88rez5ZolpOaT7s0K6q5/SKT1pWAf38X6/14yvnW7Nekj"
                                + "1FZoypdEQ8kmAGYW/OVh5aIY4k1gMRJ4nwdxp7rtzjRCJ/5Rt2X8Eje0eb+nbXym"
                                + "qcC18VRm7yPldcCVENM8VwnKBCe87//T4gRmqYtFPSy+OaV/6GeXvRWt5zXQE/U3"
                                + "Mbo1h+jgqwOS/s537OkIllRA0W8anG1DISa+z/E2OubuKqeYfL68EjZmYenIyuWt"
                                + "o5YObLst0wY5ntCzG4mnZcrprEBDnCqOaHMdMXNObQKBgQD9WTnb+Xjk5Q/Mg4/i"
                                + "0zbWlZcABqaIyT9zNO7+cZYxalAp0+NR5GX4e0bivFTMs9ONp7L9GBTauoAUsBsi"
                                + "ZGhqY6+elWTGo3N29RLDtTFrwd0sMk2sEODMjFIcJyL/44n13gWPWn+TEOr0ZfN8"
                                + "k4+rUwpTWahziOdNwVR/TToIYwKBgQDRxjJ78GyQygrSoEgmVp6+hY9CIxGi8ITx"
                                + "mGsGyWJFHyhu2R326NkgWuWpy0cC3peLZ/2LdDd19Jq0uX9iCZ74LAUJcqEqpo+5"
                                + "8Bk0/LzuDB3EwFrWktrMaDbOZ7CMgnE/uy0oX3mYaA8h+YKvftB+qO+yBBThv59w"
                                + "1rOXa6vhawKBgQCF3dCxyCc2I0bu1JGB1AME7bQFAn6ahmQtdsnVzqUC5V8ISvqx"
                                + "edLbsh/pjIPuShK9pd/w1fmm/abRx5/+0zlNBfF1BRFj2FgZgjNxD9vWSXCZetcJ"
                                + "1T1DN93nHFLlDREHxlH/xlzkk6riisBWkg33BADPu+9DZRJ7Rm1keqTwawKBgHaz"
                                + "TAXNueEEMrOUdr8a7KCqOrIV3fjxWIjM0mL4uV6bjumKeeYpRBOG28YEfJ4nDmGX"
                                + "6mRq0qcwTgpNnMMA5q9PWVpLPt8/eLyiG+Fb0hxxRrb6kWwxlRVtMvYAvmoDtcl2"
                                + "RMs/mzKeT1HOOiDMBXZmaZu1q4tCPxo+o0jfaFcZAoGAJ5j2vq0PwXdQh5iGWM03"
                                + "M5dHQQZjCMb3G6vWRzYzI668mf1hHYb2KC5vn5HLOm4h6hrZbBxx6Ll+BDD4UFwq"
                                + "90p7WAB+6BwSJFlDaDXWgnoFXy5P9fho+wjwgnu18Yunr0S+FgiYi6zN6oBSaRP7"
                                + "yAeVOVvcW2cOJmGsU11Um8U="
                                + "-----END PRIVATE KEY-----";//File.ReadAllText(privkey.FullName);

                Certificate = X509Certificate2.CreateFromPem(certPem, keyPem);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
