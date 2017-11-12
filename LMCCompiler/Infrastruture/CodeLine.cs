namespace LMCCompiler.Infrastruture
{
    class CodeLine:ICodeLine
    {      
        public string LineLabel { get; set; }
        public string Command { get; set; }
        public string Number { get; set; }
        public byte[] AsmCode { get; set; }
    }
}
