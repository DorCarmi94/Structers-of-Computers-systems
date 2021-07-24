using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Assembler a = new Assembler();

            //to run tests, call the "TranslateAssemblyFile" function like this:
            //string sourceFileLocation = the path to your source file
            //string destFileLocation = the path to your dest file
            //a.TranslateAssemblyFile(sourceFileLocation, destFileLocation);
            string sourceFileLocation = @"D:\University\3rd Simester\Structers of Computers systems\Homework\Ex2\Ex2.3\Code\Assembly examples\SquareMacro.asm";
            string destFileLocation = @"D:\University\3rd Simester\Structers of Computers systems\Homework\Ex2\Ex2.3\Code\Assembly examples\SquareMacro.mc";
            a.TranslateAssemblyFile(sourceFileLocation, destFileLocation);
            //a.TranslateAssemblyFile(@"Add.asm", @"Add.mc");
        }
    }
}
