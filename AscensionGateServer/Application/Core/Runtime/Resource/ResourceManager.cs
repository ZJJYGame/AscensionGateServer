using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cosmos;
using System.Reflection;
using FluentNHibernate.Utils;

namespace AscensionGateServer
{
    [OuterModule]
    public class ResourceManager : Module<ResourceManager>, ISimpleKeyValue<string, string>
    {
        IResourceProvider resProvider;
        Dictionary<string, string> resDataDict;
        public Dictionary<string, string> ResDataDict { get { return resDataDict; } }
        public override void OnInitialization()
        {
            InitHelper();
        }
        public bool ContainsKey(string key)
        {
            return resDataDict.ContainsKey(key);
        }
        public bool TryAdd(string key, string value)
        {
            return resDataDict.TryAdd(key, value);
        }
        public bool TryGetValue(string key, out string value)
        {
            return resDataDict.TryGetValue(key, out value);
        }
        public bool TryRemove(string key)
        {
            return resDataDict.Remove(key);
        }
        public bool TryRemove(string key, out string value)
        {
            return resDataDict.Remove(key,out value);
        }
        void InitHelper()
        {
            var obj = Utility.Assembly.GetInstanceByAttribute<TargetHelperAttribute>(typeof(IResourceProvider));
            resProvider = obj as IResourceProvider;
            resDataDict= resProvider?.LoadResource() as Dictionary<string, string>;
        }
    }
}
