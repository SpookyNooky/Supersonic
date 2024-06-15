using System.Security.Cryptography;
using System.Text;

namespace Supersonic.Core.Entities
{
    public class Transaction
    {
        public string Id { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public decimal Amount { get; set; }
        public string Signature { get; set; }
        public List<Transaction> Parents { get; set; } = new List<Transaction>();

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Signature) || string.IsNullOrWhiteSpace(FromAddress))
                return false;

            using (var rsa = RSA.Create())
            {
                try
                {
                    var transactionData = $"{FromAddress}{ToAddress}{Amount}";
                    var dataBytes = Encoding.UTF8.GetBytes(transactionData);
                    var signatureBytes = Convert.FromBase64String(Signature);

                    rsa.FromXmlString(FromAddress); // Assuming FromAddress contains the public key in XML format
                    return rsa.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                }
                catch
                {
                    return false;
                }
            }
        }

        public void SignTransaction(string privateKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.FromXmlString(privateKey);
                var transactionData = $"{FromAddress}{ToAddress}{Amount}";
                var dataBytes = Encoding.UTF8.GetBytes(transactionData);
                var signatureBytes = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                Signature = Convert.ToBase64String(signatureBytes);
            }
        }
    }
}
