using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class KeyloggerException : Exception
    {
        public KeyloggerException() { }
        public KeyloggerException(string message) : base(message) { }
        public KeyloggerException(string message, Exception inner) : base(message, inner) { }
    }
}