using Cosmos;
using Cosmos.Config;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AscensionGateServer
{
    [TargetHelper]
    public class GateDataProvider : IDataProvider
    {
        public void InitData(out Dictionary<Type, IData> dict)
        {
            dict = new Dictionary<Type, IData>();
            var datSet = Utility.Assembly.GetInstancesByAttribute<ConfigDataAttribute>(typeof(IData));
            var dataDict = GameManager.OuterModule<ResourceManager>().ResDataDict;
            for (int i = 0; i < datSet.Length; i++)
            {
                string json;
                var fullName = Utility.Text.Append(datSet[i].GetType().Name, ".json");
                if (dataDict.TryGetValue(fullName, out json))
                {
                    try
                    {
                        var obj = Utility.Json.ToObject(json, datSet[i].GetType());
                        if (obj != null)
                        {
                            dict.TryAdd(datSet[i].GetType(), obj as IData);
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.Debug.LogError($"IDataProvider ToObject fail . Type :{datSet[i].GetType().Name} ;{e}");
                    }
                }
            }
        }
    }
}
