using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMCCompiler.Infrastruture
{
    class Logger : ILogger
    {
        List<string> logLines = new List<string>();
        public void Add(string logLine)
        {
            logLines.Add(logLine);
        }

        public IEnumerable<string> List()
        {
            return logLines;
        }
    }
}
