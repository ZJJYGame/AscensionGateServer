using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    public interface IResourceProvider
    {
        object LoadResource();
    }
}
