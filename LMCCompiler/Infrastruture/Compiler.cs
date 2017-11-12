//01xxxx ADD A xxxx RAM címen lévő érték hozzáadása az akkumolátorhoz és beállítja a negatív ill. 0 jelzőt
//02xxxx SUB Kivonja az xxxx RAM címen lévő értéket az akkumolátor aktuális értékéből és beállítja a negatív ill. 0 jelzőt
//03xxxx STA Beírja az akkumolátor aktuális értékét az xxxx RAM címre
//04xxxx LOA Betölti az xxxx RAM címen lévő értéket az akkumolátorba
//05xxxx DAT Beállítja az akkumulátor értékét xx-re mivel jelenleg 8bites az adatszélesség ezért csak 256nál kissebb érték lesz figyelembe véve
//06xxxx BRA Átállítja a program mutatót az XXXX értékre és onnan folytatódik a program futása
//07xxxx BRZ Ha a null jelző 1 akkor átállítja a program mutatót az XXXX értékre és onnan folytatódik a program futása
//08xxxx BRP Ha a negatív jelző  0 akkor átállítja a program mutatót az XXXX értékre és onnan folytatódik a program futása
//90xxxx OUT Kiírja az akkumolátor aktuális értékét a kimenetre. A jelenlegi compilernek szükséges megadni minden esetben egy attribútum értéket ezért a parancs beírása után mindenképpen adjunk meg egy tetszóleges számot
//990000 BRK Program vége. A jelenlegi compilernek szükséges megadni minden esetben egy attribútum értéket ezért a parancs beírása után mindenképpen adjunk meg egy tetszóleges számot

namespace LMCCompiler.Infrastruture
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Linq;
    using System;
    using System.Collections;

    class Compiler
    {
        string _codeFilePath;
        ICodeLoader _code;
        ILogger _logger;

        static Dictionary<string, byte> _commandCodes = new Dictionary<string, byte>()
            {
                {"ADD",1},
                {"SUB",2},
                {"STA",3},
                {"LOA",4},
                {"DAT",5},
                {"BRA",6},
                {"BRZ",7},
                {"BRP",8},
                {"OUT",90},
                {"BRK",255}
            };

        string _codeLinePattern = @"(^((?<Label>[\#]\w*)\s*(?<code>ADD|SUB|STA|LOA|DAT|BRA|BRZ|BRP|OUT|BRK)\s*(?<number>([0-9]{1,4})|([\#]\w*)))$)|(^(\s*(?<code>ADD|SUB|STA|LOA|DAT|BRA|BRZ|BRP|OUT|BRK)\s*(?<number>([0-9]{1,4})|([\#]\w*)))$)";
        List<ICodeLine> _cleanCode = new List<ICodeLine>();
        public Compiler(ICodeLoader codeLoader, ILogger logger)
        {
            _code = codeLoader;
            _logger = logger;
        }
        bool checkSyntax()
        {
            bool result = true;
            int pc = 0;
            _cleanCode.Clear();
            foreach (var textLine in _code.SourceCode)
            {
                ++pc;
                var l = textLine.Trim().Replace("\t"," ");
                if (string.IsNullOrEmpty(textLine))
                {
                    continue;
                }

                var m = Regex.Match(textLine, _codeLinePattern);

                if (!m.Success)
                {
                    _logger.Add($"Check syntax in {pc} line : {textLine}");
                    result = false;
                    continue;
                }

                var codeLine = new CodeLine()
                {
                    LineLabel = m.Groups["Label"].Value ?? "",
                    Command = m.Groups["code"].Value,
                    Number = m.Groups["number"].Value
                };

                if (codeLine.Number.StartsWith("#"))
                {
                    if (!Regex.IsMatch(codeLine.Command, "BRA|BRZ|BRP"))
                    {
                        _logger.Add($"Check syntax in {pc} line : {textLine}");
                        result = false;
                        continue;
                    }
                }

                _cleanCode.Add(codeLine);
            }

            return result;
        }

        bool compile()
        {
            bool result = true;
            Dictionary<string, ushort> labelLine = new Dictionary<string, ushort>();

            for (ushort pc = 0; pc < _cleanCode.Count(); pc++)
            {
                if (string.IsNullOrEmpty(_cleanCode[pc].LineLabel))
                {
                    continue;
                }

                labelLine.Add(_cleanCode[pc].LineLabel, (ushort)(pc * 3));
            }

            foreach (ICodeLine codeLine in _cleanCode)
            {
                codeLine.AsmCode = new byte[3];
                codeLine.AsmCode[0] = _commandCodes[codeLine.Command];

                var byteNumber = new byte[2];

                if (codeLine.Number.StartsWith("#"))
                {
                    if (!labelLine.ContainsKey(codeLine.Number))
                    {
                        _logger.Add($"Nem létező cimke: {codeLine.Number}");
                        result = false;
                        continue;
                    };

                    byteNumber = BitConverter.GetBytes(labelLine[codeLine.Number]);
                }
                else
                {
                    byteNumber = BitConverter.GetBytes(ushort.Parse(codeLine.Number));
                }

                codeLine.AsmCode[1] = byteNumber[1]; codeLine.AsmCode[2] = byteNumber[0];
            }

            return result;
        }

        void Run()
        {
            if (!checkSyntax())
            {
                throw new SyntaxErrorException("Szintaktikailag hibás a forrás file\n");
            }

            if (!compile())
            {
                throw new CompileErrorException("Fordítási hiba\n");
            }
        }

        public byte[] GetBinaryCode(int binSize=1024)
        {
            Run();

            var result = new byte[binSize];
            var pc = 0;
            foreach (var codeLine in _cleanCode)
            {
                if (pc > binSize)
                {
                    break;
                }

                var asmCode = codeLine.AsmCode;
                result[pc] = asmCode[0];
                result[pc+1] = asmCode[1];
                result[pc+2] = asmCode[2];
                pc += 3;
            }

            return result;
        }
    }
}
