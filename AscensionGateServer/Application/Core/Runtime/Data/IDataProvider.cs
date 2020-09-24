using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    public interface IDataProvider
    {
        void InitData(out Dictionary<Type,Data> dict);
    }
}
