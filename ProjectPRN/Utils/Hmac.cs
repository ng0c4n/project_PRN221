using System.Security.Cryptography;
using System.Text;

namespace ProjectPRN.Utils
{
    public class Hmac
    {
        string algorithm;
        string data;
        string checkSumKey;
        public Hmac()
        {
            algorithm = "sha256";

        }
        /*public string getHashHMAC()
        {
            var hmac = HMAC.Create(algorithm);
            var keyByte =Encoding.UTF8.GetBytes(checkSumKey);
            var dataByte = Encoding.UTF8.GetBytes(data);
            var hash = hmac.ComputeHash(dataByte, keyByte);
        }*/
    }
}
