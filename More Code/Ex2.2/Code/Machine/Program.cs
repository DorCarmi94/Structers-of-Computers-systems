using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleComponents;

namespace Machine
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            
            Machine16 machine = new Machine16(false, false);
            machine.Code.LoadFromFile(@"D:\University\3rd Simester\Structers of Computers systems\Homework\Ex2\Ex2.2\Assembly examples\Add.hack");
            machine.Data[2] = 100;
            machine.Data[3] = 15;
            DateTime dtStart = DateTime.Now;
            machine.Reset();
            for (int i = 0; i < 7; i++)
            {
                machine.CPU.PrintState();
                Console.WriteLine();
                Clock.ClockDown();
                Clock.ClockUp();
            }
            

            Console.WriteLine("Done " + (DateTime.Now - dtStart).TotalSeconds);
            Console.ReadLine();
        }
    }
}
