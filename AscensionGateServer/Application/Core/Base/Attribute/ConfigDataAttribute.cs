using System;
using System.Collections.Generic;
using System.Text;
namespace AscensionGateServer
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface,AllowMultiple =false,Inherited =true)]
    public class ConfigDataAttribute:Attribute
    {
    }
}
