using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    [ConfigData]
    public abstract class Data
    {
        public abstract void SetData(object data);
    }
}
