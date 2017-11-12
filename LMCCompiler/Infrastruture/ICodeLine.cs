namespace LMCCompiler.Infrastruture
{
    public interface ICodeLine
    {
        string LineLabel{get;set;}
        string Command { get; set; }
        string Number { get; set; }
        byte[] AsmCode { get; set; }
    }
}
