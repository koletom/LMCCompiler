using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMCCompiler.Infrastruture
{
    class SyntaxErrorException : Exception
    {
        public SyntaxErrorException(string message) : base(message)
        {
        }
    }
}
