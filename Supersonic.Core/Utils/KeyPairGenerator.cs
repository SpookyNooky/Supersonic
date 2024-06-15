using System.Security.Cryptography;
using System.Text;

namespace Supersonic.Core.Utils
{
    public static class KeyPairGenerator
    {
        public static (string PublicKey, string PrivateKey, string Address) GenerateKeyPair()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                var publicKey = rsa.ToXmlString(false); // Public key only
                var privateKey = rsa.ToXmlString(true); // Public and private key

                var address = GenerateAddressFromPublicKey(publicKey);

                return (publicKey, privateKey, address);
            }
        }

        private static string GenerateAddressFromPublicKey(string publicKey)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(publicKey));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
