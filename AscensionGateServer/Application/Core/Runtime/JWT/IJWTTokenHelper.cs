using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    public interface IJWTTokenHelper
    {
        /// <summary>
        /// 使用默认的key；
        /// 使用数据对象生成token；
        /// </summary>
        /// <param name="value">未序列化的数据对象</param>
        /// <returns>token</returns>
        string EncodeToken(object value);
        /// <summary>
        /// 使用数据对象生成token；
        /// </summary>
        /// <param name="value">未序列化的数据对象</param>
        /// <param name="key">自定义的key</param>
        /// <returns>token</returns>
        string EncodeToken( object value, string key);
        /// <summary>
        /// 解码token；
        /// 使用默认的key；
        /// </summary>
        /// <param name="token">token数据</param>
        /// <returns>序列化的数据</returns>
        string DecodeToken(string token);
        /// <summary>
        /// 解码token
        /// </summary>
        /// <param name="token">token数据</param>
        /// <param name="key">自定义的key</param>
        /// <returns>序列化的数据</returns>
        string DecodeToken(string token,string key);
        /// <summary>
        /// 解码token；
        /// 使用默认的key；
        /// </summary>
        /// <typeparam name="T">目标数据类型</typeparam>
        /// <param name="token">token数据</param>
        /// <returns>反序列化后的数据对象</returns>
        T DecodeToken<T>(string token);
        /// <summary>
        /// 解码token
        /// </summary>
        /// <typeparam name="T">目标数据类型</typeparam>
        /// <param name="token">token数据</param>
        /// <param name="key">自定义的key</param>
        /// <returns>反序列化后的数据对象</returns>
        T DecodeToken<T>(string token,string key);
    }
}
