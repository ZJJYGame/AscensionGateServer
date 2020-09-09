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
    public class GateTokenHelper : IJWTTokenHelper
    {
        readonly string _SecretKey = "292C08109FD07280B3E4B6AAF35C89A0";
        public string EncodeToken(string key, object value)
        {
            string token = new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm()) // 使用算法
            .WithSecret(_SecretKey)
            .AddClaim(key, value).Encode();
            return token;
        }
        public string DecodeToken(string token)
        {
            try
            {
                string json = new JwtBuilder()
                .WithSecret(_SecretKey)
                .Decode(token);
                return json;
            }
            catch (TokenExpiredException e)
            {
                Utility.Debug.LogError(e);
                return null;
            }
        }
        public T DecodeToken<T>(string token)
        {
            try
            {
                 string  json= new JwtBuilder()
                .WithSecret(_SecretKey)
                .Decode(token);
                List<string>str= Utility.Json.ToObject<List<string>>(json);
                var t= Utility.Json.ToObject<T>(str[1]);
                return t;
            }
            catch (TokenExpiredException e)
            {
                Utility.Debug.LogError(e);
                return default(T);
            }
        }
    }
}