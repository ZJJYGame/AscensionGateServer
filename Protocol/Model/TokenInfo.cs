using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public class TokenInfo:IDisposable
    {
        public long Conv { get; set; }
        public string Token { get; set; }
        public void Dispose()
        {
            Conv = 0;
            Token = null;
        }
        public override string ToString()
        {
            return $"TokenString : {Token} ;";
        }
    }
}
