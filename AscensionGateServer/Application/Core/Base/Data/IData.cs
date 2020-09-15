using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    [ConfigData]
    public interface IData
    {
        void SetData(object data);
    }
}
