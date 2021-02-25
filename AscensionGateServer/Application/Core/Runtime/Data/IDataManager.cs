using System;
using System.Collections.Generic;
using System.Text;
using Cosmos;
namespace AscensionGateServer
{
    public interface IDataManager:IModuleManager
    {
        bool ContainsKey(Type key);
        bool TryAdd(Type key, object value);
        bool TryGetValue(Type key, out object value);
        bool TryRemove(Type key);
        bool TryRemove(Type key, out object value);
        bool ContainsKey<T>() where T : class;
        bool TryAdd<T>(T value) where T : class;
        bool TryGetValue<T>(out T value) where T : class;
        bool TryRemove<T>() where T : class;
        bool TryRemove<T>(out T value) where T : class;
        bool TryAdd(string key, string value);
        bool TryGetValue(string key, out string value);
        bool TryRemove(string key);
        bool TryRemove(string key, out string value);
        bool ContainsKey(string key);
    }
}
