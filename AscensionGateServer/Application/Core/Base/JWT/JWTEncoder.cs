using Cosmos;
using System.Threading.Tasks;

namespace AscensionGateServer
{
    public static partial class JWTEncoder
    {
        static IJWTEncodeHelper tokenHelper;
        static JWTEncoder()
        {
            var obj = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute>(typeof(IJWTEncodeHelper));
            tokenHelper = obj as IJWTEncodeHelper;
        }
        #region Sync
        /// <summary>
        /// 使用数据对象生成token；
        /// </summary>
        /// <param name="value">未序列化的数据对象</param>
        /// <param name="key">自定义的key</param>
        /// <returns>token</returns>
        public static string EncodeToken(object value, string key)
        {
            return tokenHelper.EncodeToken(value, key);
        }
        /// <summary>
        /// 使用默认的key；
        /// 使用数据对象生成token；
        /// </summary>
        /// <param name="value">未序列化的数据对象</param>
        /// <returns>token</returns>
        public static string EncodeToken(object value)
        {
            return tokenHelper.EncodeToken(value);
        }
        /// <summary>
        /// 解码token；
        /// 使用默认的key；
        /// </summary>
        /// <param name="token">token数据</param>
        /// <returns>序列化的数据</returns>
        public static string DecodeToken(string token)
        {
            return tokenHelper.DecodeToken(token);
        }
        /// <summary>
        /// 解码token
        /// </summary>
        /// <param name="token">token数据</param>
        /// <param name="key">自定义的key</param>
        /// <returns>序列化的数据</returns>
        public static string DecodeToken(string token, string key)
        {
            return tokenHelper.DecodeToken(token, key);
        }
        /// <summary>
        /// 解码token；
        /// 使用默认的key；
        /// </summary>
        /// <typeparam name="T">目标数据类型</typeparam>
        /// <param name="token">token数据</param>
        /// <returns>反序列化后的数据对象</returns>
        public static T DecodeToken<T>(string token)
        {
            return tokenHelper.DecodeToken<T>(token);
        }
        /// <summary>
        /// 解码token
        /// </summary>
        /// <typeparam name="T">目标数据类型</typeparam>
        /// <param name="token">token数据</param>
        /// <param name="key">自定义的key</param>
        /// <returns>反序列化后的数据对象</returns>
        public static T DecodeToken<T>(string token, string key)
        {
            return tokenHelper.DecodeToken<T>(token, key);
        }
        #endregion
        #region Async
        /// <summary>
        /// 异步；
        /// 使用数据对象生成token；
        /// </summary>
        /// <param name="value">未序列化的数据对象</param>
        /// <param name="key">自定义的key</param>
        /// <returns>token</returns>
        public async static Task<string> EncodeTokenAsync(object value, string key)
        {
            return await Task.Run<string>(() => tokenHelper.EncodeToken(value, key));
        }
        /// <summary>
        /// 异步；
        /// 使用默认的key；
        /// 使用数据对象生成token；
        /// </summary>
        /// <param name="value">未序列化的数据对象</param>
        /// <returns>token</returns>
        public async static Task<string> EncodeTokenAsync(object value)
        {
            return await Task.Run<string>(() => tokenHelper.EncodeToken(value));
        }
        /// <summary>
        /// 异步；
        /// 解码token；
        /// 使用默认的key；
        /// </summary>
        /// <param name="token">token数据</param>
        /// <returns>序列化的数据</returns>
        public async static Task<string> DecodeTokenAsync(string token)
        {
            return await Task.Run<string>(() => tokenHelper.DecodeToken(token));
        }
        /// <summary>
        /// 异步；
        /// 解码token
        /// </summary>
        /// <param name="token">token数据</param>
        /// <param name="key">自定义的key</param>
        /// <returns>序列化的数据</returns>
        public async static Task<string> DecodeTokenAsync(string token, string key)
        {
            return await Task.Run<string>(() => tokenHelper.DecodeToken(token, key));
        }
        /// <summary>
        /// 异步；
        /// 解码token；
        /// 使用默认的key；
        /// </summary>
        /// <typeparam name="T">目标数据类型</typeparam>
        /// <param name="token">token数据</param>
        /// <returns>反序列化后的数据对象</returns>
        public async static Task<T> DecodeTokenAsync<T>(string token)
        {
            return await Task.Run<T>(() => tokenHelper.DecodeToken<T>(token));
        }
        /// <summary>
        /// 异步；
        /// 解码token
        /// </summary>
        /// <typeparam name="T">目标数据类型</typeparam>
        /// <param name="token">token数据</param>
        /// <param name="key">自定义的key</param>
        /// <returns>反序列化后的数据对象</returns>
        public async static Task<T> DecodeTokenAsync<T>(string token, string key)
        {
            return await Task.Run<T>(() => tokenHelper.DecodeToken<T>(token, key));
        }
        #endregion
    }
}
