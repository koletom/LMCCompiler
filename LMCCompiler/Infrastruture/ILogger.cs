using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMCCompiler.Infrastruture
{
    public interface ILogger
    {
        void Add(string logLine);
        IEnumerable<string> List();
    }
}
