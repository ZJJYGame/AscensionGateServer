using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AscensionGateServer
{
    /// <summary>
    /// 加密编码工具
    /// </summary>
    public sealed  class Encryptor
    {
        #region Sync
        /// <summary>
        /// 对称加密；
        /// 将明文对称加密成密文
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <returns>密文</returns>
        public static string Encrypt(string plaintext)
        {
            string ciphertex = Utility.Encryption.DESEncrypt(plaintext, ApplicationConst._TokenSecretKey, ApplicationConst.KcpIV);
            return ciphertex;
        }
        /// <summary>
        /// 对称加密；
        /// 将明文对称加密后转换成buffer；
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <returns>二进制buffer</returns>
        public static byte[] Encrypt2Byte(string plaintext)
        {
            string ciphertex = Utility.Encryption.DESEncrypt(plaintext, ApplicationConst._TokenSecretKey, ApplicationConst.KcpIV);
            return Utility.Encode.ConvertToByte(ciphertex);
        }
        /// <summary>
        /// HmacSHA256加密；
        /// 生成加密后的token
        /// </summary>
        /// <param name="plaintext">token明文</param>
        /// <returns>token密文</returns>
        public static string EncryptToken(string plaintext)
        {
            var tokenStr = Utility.Encryption.HmacSHA256(plaintext, ApplicationConst._TokenSecretKey);
            return tokenStr;
        }
        /// <summary>
        /// HmacSHA256加密；
        /// 生成加密后的token的buffer
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <returns>token的加密buffer</returns>
        public static byte[] Token2Byte(string plaintext)
        {
            var tokenStr = Utility.Encryption.HmacSHA256(plaintext, ApplicationConst._TokenSecretKey);
            return Utility.Encode.ConvertToByte(tokenStr);
        }
        /// <summary>
        /// 对称解密；
        /// 将buffer对称解密成字段；
        /// </summary>
        /// <param name="buffer">二进制buffer</param>
        /// <returns>解密后的字段</returns>
        public static string Decrypt(byte[] buffer)
        {
            var ciphertext = Utility.Encode.ConvertToString(buffer);
            var plaintext = Utility.Encryption.DESDecrypt(ciphertext, ApplicationConst._TokenSecretKey, ApplicationConst.KcpIV);
            return plaintext;
        }
        /// <summary>
        /// 对称解密；
        /// 将对称加密的密文解密成明文；
        /// </summary>
        /// <param name="ciphertext">密文</param>
        /// <returns>明文</returns>
        public static string Decrypt(string ciphertext)
        {
            var plaintext = Utility.Encryption.DESDecrypt(ciphertext, ApplicationConst._TokenSecretKey, ApplicationConst.KcpIV);
            return plaintext;
        }
        #endregion

        #region Async
        /// <summary>
        /// 对称加密；
        /// 将明文对称加密成密文
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <returns>密文</returns>
        public async static Task<string> EncryptAsync(string plaintext)
        {
            return await Task.Run<string>(() => { return Encrypt(plaintext); });
        }
        /// <summary>
        /// 对称加密；
        /// 将明文对称加密后转换成buffer；
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <returns>二进制buffer</returns>
        public async static Task<byte[]> Encrypt2ByteAsync(string plaintext)
        {
            return await Task.Run(() => { return Encrypt2Byte(plaintext); });
        }
        /// <summary>
        /// HmacSHA256加密；
        /// 生成加密后的token
        /// </summary>
        /// <param name="plaintext">token明文</param>
        /// <returns>token密文</returns>
        public async static Task<string> EncryptTokenAsync(string plaintext)
        {
            return await Task.Run(() => { return EncryptToken(plaintext); });
        }

        /// <summary>
        /// HmacSHA256加密；
        /// 生成加密后的token的buffer
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <returns>token的加密buffer</returns>
        public async static Task<byte[]> Token2ByteAsync(string plaintext)
        {
            return await Task.Run(() => { return Token2Byte(plaintext); });
        }
        /// <summary>
        /// 对称解密；
        /// 将buffer对称解密成字段；
        /// </summary>
        /// <param name="buffer">二进制buffer</param>
        /// <returns>解密后的字段</returns>
        public async static Task<string> DecryptAsync(byte[] buffer)
        {
            return await Task.Run<string>(() => { return Decrypt(buffer); });
        }
        /// <summary>
        /// 对称解密；
        /// 将对称加密的密文解密成明文；
        /// </summary>
        /// <param name="ciphertext">密文</param>
        /// <returns>明文</returns>
        public async static Task<string> DecryptAsync(string ciphertext)
        {
            return await Task.Run<string>(() => { return Decrypt(ciphertext); });
        }
        #endregion
    }
}