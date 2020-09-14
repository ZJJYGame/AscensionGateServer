using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cosmos;
namespace AscensionGateServer
{
    public class ResourceManager : Module<ResourceManager>
    {
        IResourceProvider resProvider;
        public void SetProvider(IResourceProvider provider)
        {
            resProvider = provider;
        }
        Dictionary<string, string> resDataDict = new Dictionary<string, string>();
        public override void OnInitialization()
        {
            resProvider.LoadResource();
        }
    }
}
