using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppCryptoMM
{
    public class Crypto
    {

        public static RSA GetRSA()
        {
            RSA rsa = RSA.Create();
            rsa.KeySize = 1024;
            return rsa;
        }
        public static Aes GetAES()
        {
            Aes aes = Aes.Create();
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.KeySize = 256;
            aes.Padding = PaddingMode.None;
            return aes;
        }
    }
}
