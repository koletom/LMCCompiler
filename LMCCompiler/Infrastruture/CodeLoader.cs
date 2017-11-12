namespace LMCCompiler.Infrastruture
{
    using System.IO;
    class CodeLoader : ICodeLoader
    {
        public CodeLoader()
        {
           
        }

        public CodeLoader(string[] sourceCode)
        {
            SourceCode = sourceCode;
        }

        public CodeLoader(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new CodeFileNotFoundException();
            }
            SourceCode = File.ReadAllLines(fileName);
        }

        public string[] SourceCode { get; set; }
    }
}
