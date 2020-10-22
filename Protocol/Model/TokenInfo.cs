using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    [MessagePackObject]
    public class TokenInfo:IDisposable
    {
        [Key(0)]
        public long Conv { get; set; }
        [Key(1)]
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
