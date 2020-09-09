using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    /// <summary>
    /// 加密编码工具
    /// </summary>
    public sealed class Encryption
    {
        public static string Decrypt(byte[] buffer)
        {
            var ciphertext = Utility.Encode.ConvertToString(buffer);
            var plaintext = Utility.Encryption.DESDecrypt(ciphertext, AppConst._KcpSecretKey, AppConst.KcpIV);
            return plaintext;
        }
        public static string Decrypt(string ciphertext)
        {
            var plaintext = Utility.Encryption.DESDecrypt(ciphertext, AppConst._KcpSecretKey, AppConst.KcpIV);
            return plaintext;
        }
        public static string Encrypt(string plaintext)
        {
            string ciphertex = Utility.Encryption.DESEncrypt(plaintext, AppConst._KcpSecretKey, AppConst.KcpIV);
            return ciphertex;
        }
        public static byte[] Encrypt2Byte(string plaintext)
        {
            string ciphertex = Utility.Encryption.DESEncrypt(plaintext, AppConst._KcpSecretKey, AppConst.KcpIV);
            return Utility.Encode.ConvertToByte(ciphertex);
        }
        public static string Token(string plaintext )
        {
            var tokenStr = Utility.Encryption.HmacSHA256(plaintext, AppConst._TokenSecretKey);
            return tokenStr;
        }
        public static byte[]  Token2Byte(string plaintext)
        {
            var tokenStr = Utility.Encryption.HmacSHA256(plaintext, AppConst._TokenSecretKey);
            return Utility.Encode.ConvertToByte(tokenStr);
        }
    }
}