using System;
using System.Collections.Generic;
using System.Text;
using Cosmos;
namespace AscensionGateServer
{
    [CustomeModule]
    public class DataManager : Module<DataManager>, ISimpleKeyValue<Type, IData>
    {
        IDataProvider dataProvider;
        Dictionary<Type, IData> dataDict = new Dictionary<Type, IData>();
        public override void OnInitialization()
        {
            InitProvider();
        }
        public override void OnPreparatory()
        {
            dataProvider?.InitData(out dataDict);
        }
        public bool ContainsKey(Type key)
        {
            return dataDict.ContainsKey(key);
        }
        public bool TryAdd(Type key, IData Value)
        {
            return dataDict.TryAdd(key, Value);
        }
        public bool TryGetValue(Type key, out IData value)
        {
            return dataDict.TryGetValue(key, out value);
        }
        public bool TryRemove(Type key)
        {
            return dataDict.Remove(key);
        }
        public bool TryRemove(Type key, out IData value)
        {
            return dataDict.Remove(key, out value);
        }
        public bool ContainsKey<T>()
            where T:class,IData
        {
            return ContainsKey(typeof(T));
        }
        public bool TryAdd<T>( T value)
            where T : class, IData
        {
            return TryAdd(typeof(T), value);
        }
        public bool TryGetValue<T>( out T value)
            where T : class, IData
        {
            value = default;
            IData data;
            var result= TryGetValue(typeof(T), out data);
            if (result)
                value = data as T;
            return result;
        }
        public bool TryRemove<T>()
            where T : class, IData
        {
            return TryRemove(typeof(T));
        }
        public bool TryRemove<T>(out T value)
            where T : class, IData
        {
            value = default;
            IData data;
            var result = TryRemove(typeof(T),out data);
            if (result)
                value = data as T;
            return result;
        }
        void InitProvider()
        {
            var obj = Utility.Assembly.GetInstanceByAttribute<TargetHelperAttribute>(typeof(IDataProvider));
            dataProvider = obj as IDataProvider;
        }
    }
}
