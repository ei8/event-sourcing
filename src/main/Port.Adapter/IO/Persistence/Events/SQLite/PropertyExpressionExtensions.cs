using System.IO;
using System.Security.Cryptography;
using System;

namespace ei8.EventSourcing.Port.Adapter.IO.Persistence.Events.SQLite
{
    // TODO:0 transfer to an appropriate namespace
    public static class PropertyExpressionExtensions
    {
        public static void Decrypt<T>(this PropertyExpression<T, string> property, T instance, byte[] key)
        {
            property.Setter(instance, PropertyExpressionExtensions.DecryptString(property.Getter(instance), key));
        }

        public static void Encrypt<T>(this PropertyExpression<T, string> property, T instance, byte[] key)
        {
            property.Setter(instance, PropertyExpressionExtensions.EncryptString(property.Getter(instance), key));
        }

        private static string EncryptString(string str, byte[] keys)
        {
            byte[] encrypted;
            using (var aes = Aes.Create())
            {
                aes.Key = keys;

                aes.GenerateIV(); // The get method of the 'IV' property of the 'SymmetricAlgorithm' automatically generates an IV if it is has not been generate before. 

                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(aes.IV, 0, aes.IV.Length);
                    ICryptoTransform encoder = aes.CreateEncryptor();
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encoder, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(str);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        private static string DecryptString(string str, byte[] key)
        {
            byte[] cipherText = Convert.FromBase64String(str);

            string decrypted;
            using (var aes = Aes.Create())
            {
                // Setting a key size disposes the previously-set key. 
                // Setting a key size will generate a new key. 
                // Setting a key size is redundant if a key going to be set after this statement. 
                // aes.KeySize = 256; 

                aes.Key = key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (MemoryStream msDecryptor = new MemoryStream(cipherText))
                {
                    byte[] readIV = new byte[16];
                    msDecryptor.Read(readIV, 0, 16);
                    aes.IV = readIV;
                    ICryptoTransform decoder = aes.CreateDecryptor();
                    using (CryptoStream csDecryptor = new CryptoStream(msDecryptor, decoder, CryptoStreamMode.Read))
                    using (StreamReader srReader = new StreamReader(csDecryptor))
                    {
                        decrypted = srReader.ReadToEnd();
                    }
                }
            }
            return decrypted;
        }
    }
}
