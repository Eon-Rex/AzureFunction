using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace MobiwikFunctionApp
{

    public class EncDecLogic
    {
        private static readonly string ASYMMETRIC_RSA_ALGORITHM = "RSA/ECB/PKCS1Padding";
        private static readonly string SYMMETRIC_AES_ALGORITHM = "AES/CBC/PKCS5Padding";

        public static string EncryptRSA(string msgToBeEncrypted, string publicKeyFilePath)
        {
            string encodedMessage = null;
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportCspBlob(File.ReadAllBytes(publicKeyFilePath));
                byte[] secretMessageBytes = Encoding.UTF8.GetBytes(msgToBeEncrypted);
                byte[] encryptedMessageBytes = rsa.Encrypt(secretMessageBytes, false);
                encodedMessage = Convert.ToBase64String(encryptedMessageBytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return encodedMessage;
        }

        public static string DecryptRSA(string encryptedString, string privateKeyFilePath)
        {
            string decryptedMessage = null;
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportCspBlob(File.ReadAllBytes(privateKeyFilePath));
                byte[] decodedMessageBytes = Convert.FromBase64String(encryptedString);
                byte[] decryptedMessageBytes = rsa.Decrypt(decodedMessageBytes, false);
                decryptedMessage = Encoding.UTF8.GetString(decryptedMessageBytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return decryptedMessage;
        }

        public static string EncryptAES(string plainText, string key)
        {
            string encryptedMsg = null;
            try
            {
                byte[] clean = Encoding.UTF8.GetBytes(plainText);
                int ivSize = 16;
                byte[] iv = new byte[ivSize];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(iv);

                using (var aes = Aes.Create())
                {
                    aes.Key = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(key));
                    aes.IV = iv;

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(clean, 0, clean.Length);
                        }
                        encryptedMsg = Convert.ToBase64String(iv.Concat(ms.ToArray()).ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return encryptedMsg;
        }

        public static string DecryptAES(string encryptedMsg, string key)
        {
            string decryptedMsg = null;
            try
            {
                byte[] encryptedIvTextBytes = Convert.FromBase64String(encryptedMsg);
                int ivSize = 16;
                byte[] iv = new byte[ivSize];
                Array.Copy(encryptedIvTextBytes, 0, iv, 0, iv.Length);

                using (var aes = Aes.Create())
                {
                    aes.Key = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(key));
                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(encryptedIvTextBytes, ivSize, encryptedIvTextBytes.Length - ivSize);
                        }
                        decryptedMsg = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return decryptedMsg;
        }

        public class EncryptedPOJO
        {
            public string RsaEncryptedKey { get; set; }
            public string AesEncryptedMsg { get; set; }

            public override string ToString()
            {
                return $"EncryptedPOJO{{rsaEncryptedKey='{RsaEncryptedKey}', aesEncryptedMsg='{AesEncryptedMsg}'}}";
            }
        }
    }

}