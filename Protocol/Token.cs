using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public class Token:IDisposable
    {
        public virtual string TokenString { get; set; }
        public void Dispose()
        {
            TokenString = null;
        }
        public override string ToString()
        {
            return $"Token : {TokenString} ;";
        }
    }
}
