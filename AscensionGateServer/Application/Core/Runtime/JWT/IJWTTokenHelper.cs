using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    public interface IJWTTokenHelper
    {
        string EncodeToken(string key, object value);
        string DecodeToken(string token);
        T DecodeToken<T>(string token);
    }
}
