using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using Protocol;
namespace AscensionGateServer
{
    public class CommandEventCore:ConcurrentStandardEventCore<byte,long,OperationData,CommandEventCore>
    {
    }
}
