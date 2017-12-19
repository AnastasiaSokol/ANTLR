using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Antlr.Runtime.Misc;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                Antlr.Runtime.ANTLRFileStream inStream = new Antlr.Runtime.ANTLRFileStream(args[0]);
                testLexer lexer = new testLexer(inStream);
                Emitter emitter = new Emitter();
                Antlr.Runtime.CommonTokenStream tokenStream = new Antlr.Runtime.CommonTokenStream(lexer);
                testParser parser = new testParser(tokenStream, emitter);

                //вызываем разбор правил programm
                parser.program();

                emitter.SaveMSIL(args[1]);
            }
            else
            {
                Console.WriteLine("usege: <program> <inputfile> <outputfile>");
                Console.ReadKey();
            }
        }
    }
}
