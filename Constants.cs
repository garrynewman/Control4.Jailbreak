﻿namespace Garry.Control4.Jailbreak
{
    public static class Constants
    {
        public const int Version = 5;

        /// <summary>
        /// The cert for composer needs to be named cacert-*.pem
        /// </summary>
        public const string ComposerCertName = "cacert-jailbreak.pem";

        /// <summary>
        /// Needs to start with Composer_ and can be anything after
        /// </summary>
        public const string CertificateCn = "Composer_GarryJailbreak";

        /// <summary>
        /// Should always be this unless they change something internally
        /// </summary>
        public const string CertPassword = "R8lvpqtgYiAeyO8j8Pyd";

        /// <summary>
        /// How many days until the certificate expires. Doesn't seem any harm in setting this to
        /// a huge value so you don't have to re-crack every year.
        /// </summary>
        public const int CertificateExpireDays = 3650;

        /// <summary>
        /// Where OpenSSL is installed (it's installed with Composer)
        /// </summary>
        public const string OpenSslExe = @"C:\Program Files (x86)\Control4\Composer\Pro\RemoteAccess\bin\openssl.exe";

        /// <summary>
        /// Where OpenSSL's Config is located (it's installed with Composer)
        /// </summary>
        public const string OpenSslConfig = @"Certs\openssl.cfg";

        /// <summary>
        /// What version of Director/Composer we're aiming at
        /// </summary>
        public const string TargetDirectorVersion = @"3.2.3";
    }
}