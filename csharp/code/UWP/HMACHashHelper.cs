using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace UWPClassLibrary.Helpers
{
    public class HmacHashHelper
    {
        public static string GetHmacHashCode(string stringToSign, string hashKey)
        {
            try
            {
                var macAlgorithmProvider = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);
                var messageBuffer = CryptographicBuffer.ConvertStringToBinary(stringToSign, BinaryStringEncoding.Utf8);
                var keyBuffer = CryptographicBuffer.ConvertStringToBinary(hashKey, BinaryStringEncoding.Utf8);
                var hmacKey = macAlgorithmProvider.CreateKey(keyBuffer);
                var signedMessage = CryptographicEngine.Sign(hmacKey, messageBuffer);
                return CryptographicBuffer.EncodeToHexString(signedMessage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }
    }
}
