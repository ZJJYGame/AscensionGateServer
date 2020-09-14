using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Web;
using Cosmos;
namespace AscensionGateServer
{
    public class GateEncodeHelper : IJWTEncodeHelper
    {
        readonly string _SecretKey = "292C08109FD07280B3E4B6AAF35C89A0";
        /// <summary>
        /// 使用默认的key；
        /// 使用数据对象生成token；
        /// </summary>
        /// <param name="value">未序列化的数据对象</param>
        /// <returns>token</returns>
        public string EncodeToken(object value)
        {
            return EncodeToken(value, ApplicationConst._JWTTokenKey);
        }
        /// <summary>
        /// 使用数据对象生成token；
        /// </summary>
        /// <param name="value">未序列化的数据对象</param>
        /// <param name="key">自定义的key</param>
        /// <returns>token</returns>
        public string EncodeToken( object value, string key)
        {
            var json = Utility.Json.ToJson(value);
            string token = new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(_SecretKey)
            .AddClaim(key, json).Encode();
            return token;
        }
        /// <summary>
        /// 解码token；
        /// 使用默认的key；
        /// </summary>
        /// <param name="token">token数据</param>
        /// <returns>序列化的数据</returns>
        public string DecodeToken(string token)
        {
            return DecodeToken(token, ApplicationConst._JWTTokenKey);
        }
        /// <summary>
        /// 解码token
        /// </summary>
        /// <param name="token">token数据</param>
        /// <param name="key">自定义的key</param>
        /// <returns>序列化的数据</returns>
        public string DecodeToken(string token, string key)
        {
            try
            {
                var dict = new JwtBuilder()
                .WithSecret(_SecretKey)
                .Decode<Dictionary<string,object>>(token);
                object data;
                var result = dict.TryGetValue(key, out data);
                if (result)
                    return data.ToString();
                else
                    return null;
            }
            catch (TokenExpiredException e)
            {
                Utility.Debug.LogError(e);
                return null;
            }
        }
        /// <summary>
        /// 解码token；
        /// 使用默认的key；
        /// </summary>
        /// <typeparam name="T">目标数据类型</typeparam>
        /// <param name="token">token数据</param>
        /// <returns>反序列化后的数据对象</returns>
        public T DecodeToken<T>(string token)
        {
            return DecodeToken<T>(token, ApplicationConst._JWTTokenKey);
        }
        /// <summary>
        /// 解码token
        /// </summary>
        /// <typeparam name="T">目标数据类型</typeparam>
        /// <param name="token">token数据</param>
        /// <param name="key">自定义的key</param>
        /// <returns>反序列化后的数据对象</returns>
        public T DecodeToken<T>(string token, string key)
        {
            try
            {
                var dict = new JwtBuilder()
               .WithSecret(_SecretKey)
               .Decode<Dictionary<string, object>>(token);
                object data;
                var result = dict.TryGetValue(key, out data);
                if (result)
                    return Utility.Json.ToObject<T>(data.ToString());
                else
                    return default(T);
            }
            catch (TokenExpiredException e)
            {
                Utility.Debug.LogError(e);
                return default(T);
            }
        }
    }
}