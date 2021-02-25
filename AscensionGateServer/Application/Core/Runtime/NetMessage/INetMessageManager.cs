using System;
using System.Collections.Generic;
using System.Text;
using Cosmos;
using Protocol;
namespace AscensionGateServer
{
    public interface INetMessageManager:IModuleManager
    {
        void SendMessageAsync(long conv, OperationData  operationData);

    }
}
