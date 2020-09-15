using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cosmos;
namespace AscensionGateServer
{
    [TargetHelper]
    public class ConfigDataProvider : IResourceProvider
    {
        string folderPath = Environment.CurrentDirectory + "/ConfigData";
        Dictionary<string, string> resDataDict = new Dictionary<string, string>();
        public object LoadResource()
        {
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            foreach (var f in dir.GetFiles())
            {
                var str = Utility.IO.ReadTextFileContent(folderPath, f.Name);
                resDataDict.Add(f.Name, str);
                //Utility.Debug.LogInfo($"\n{f.Name}\n{str}\n");
            }
            return resDataDict;
        }
    }
}
