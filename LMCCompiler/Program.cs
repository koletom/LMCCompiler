using LMCCompiler.Infrastruture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new Logger();

            try
            {
                var codeLoader = new CodeLoader("TestCode.asm");
                var comp = new Compiler(codeLoader, logger);
                var program = comp.GetBinaryCode();
                File.WriteAllBytes("TestCode.bin", program);
                File.WriteAllText("TestCode.hex", bitsToVhdlData(program));
            }
            catch (CodeFileNotFoundException)
            {
                Console.WriteLine("Nincs fordítandó file\n\n");
            }
            catch (SyntaxErrorException se)
            {
                Console.WriteLine(se.Message + Environment.NewLine);
                Console.WriteLine(String.Join(Environment.NewLine, logger.List()));
            }
            catch (CompileErrorException ce)
            {
                Console.WriteLine(ce.Message + Environment.NewLine);
                Console.WriteLine(String.Join(Environment.NewLine, logger.List()));
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message + Environment.NewLine);
                Console.WriteLine(exp.StackTrace);
            }

        }

        static string bitsToVhdlData(byte[] bits)
        {
            var result = new StringBuilder();

            var lineCharsCount = 0;
            foreach (var bit in bits)
            {                
                if (lineCharsCount == 20)
                {
                    result.Append(Environment.NewLine);
                    lineCharsCount = 0;
                }

                result.Append($"X\"{bit.ToString("X2")}\", ");
                ++lineCharsCount;
            }

            var r = result.ToString();

            return "(" + r.Substring(0, r.Count() - 2) + ")";
        }
    }
}
