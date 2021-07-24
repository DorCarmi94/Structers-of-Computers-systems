using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class Program
    {
        static void Main(string[] args)
        {
            //This is an example of a testing code that you should run for all the gates that you create
            SingleBitRegister bitRegister = new SingleBitRegister();
            if(!bitRegister.TestGate())
            {
                Console.WriteLine("bugbug bit");
            }
            if (!bitRegister.TestGate())
            {
                Console.WriteLine("bugbug bit");
            }
            if (!bitRegister.TestGate())
            {
                Console.WriteLine("bugbug bit");
            }
            if (!bitRegister.TestGate())
            {
                Console.WriteLine("bugbug bit");
            }
            if (!bitRegister.TestGate())
            {
                Console.WriteLine("bugbug bit");
            }
            if (!bitRegister.TestGate())
            {
                Console.WriteLine("bugbug bit");
            }

            MultiBitRegister multiBitRegister = new MultiBitRegister(5);
            if (!multiBitRegister.TestGate())
            {
                Console.WriteLine("bugbug multi");
            }
            if (!multiBitRegister.TestGate())
            {
                Console.WriteLine("bugbug multi");
            }
            if (!multiBitRegister.TestGate())
            {
                Console.WriteLine("bugbug multi");
            }
            if (!multiBitRegister.TestGate())
            {
                Console.WriteLine("bugbug multi");
            }
            if (!multiBitRegister.TestGate())
            {
                Console.WriteLine("bugbug multi");
            }
            if (!multiBitRegister.TestGate())
            {
                Console.WriteLine("bugbug multi");
            }
            if (!multiBitRegister.TestGate())
            {
                Console.WriteLine("bugbug multi");
            }
            if (!multiBitRegister.TestGate())
            {
                Console.WriteLine("bugbug multi");
            }
            if (!multiBitRegister.TestGate())
            {
                Console.WriteLine("bugbug multi");
            }

            
                Memory memory = new Memory(4, 8);
                if (!memory.TestGate())
                {
                    Console.WriteLine("bugbug");
                }
            if (!memory.TestGate())
            {
                Console.WriteLine("bugbug");
            }
            if (!memory.TestGate())
            {
                Console.WriteLine("bugbug");
            }
            if (!memory.TestGate())
            {
                Console.WriteLine("bugbug");
            }
            if (!memory.TestGate())
            {
                Console.WriteLine("bugbug");
            }
            if (!memory.TestGate())
            {
                Console.WriteLine("bugbug");
            }
            if (!memory.TestGate())
            {
                Console.WriteLine("bugbug");
            }
            if (!memory.TestGate())
            {
                Console.WriteLine("bugbug");
            }
            if (!memory.TestGate())
            {
                Console.WriteLine("bugbug");
            }
            if (!memory.TestGate())
            {
                Console.WriteLine("bugbug");
            }


            Console.WriteLine("done");
                Console.ReadLine();
            
            
        }
        public void testALU()
        {
            String ans = "df";
            MultiBitAdder multiBitAdder = new MultiBitAdder(5);
            multiBitAdder.TestGate();
            multiBitAdder.TestGate();
            while (String.Compare(ans, "bye") != 0)
            {
                ALU alu = new ALU(5);

                Console.WriteLine("Testing:...");
                Console.WriteLine("Test alu:" + alu.TestGate() + " Output: " + alu.Output.ToString());
                Console.WriteLine("Test alu:" + alu.TestGate() + " Output: " + alu.Output.ToString());
                Console.WriteLine("Enter 0 to random test again");
                Console.WriteLine("Enter 1 to enter inputs");
                int choice = Convert.ToInt32(Console.ReadLine());
                if (choice != 0)
                {
                    Console.WriteLine("Enter yout input:");

                    Console.WriteLine("Enter x:");
                    int x = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter y:");
                    int y = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter zx:");
                    int zx = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter nx:");
                    int nx = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter zy:");
                    int zy = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter ny:");
                    int ny = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter f:");
                    int f = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter no:");
                    int no = Convert.ToInt32(Console.ReadLine());
                    Console.Clear();



                    alu.InputX.Set2sComplement(x);
                    alu.InputY.Set2sComplement(y);
                    alu.ZeroX.Value = zx;
                    alu.NotX.Value = nx;
                    alu.ZeroY.Value = zy;
                    alu.NotY.Value = ny;
                    alu.F.Value = f;
                    alu.NotOutput.Value = no;




                    WireSet w = new WireSet(5);


                    Console.WriteLine("-----Control Inputs-----");
                    Console.WriteLine("ZeroX: {" + alu.ZeroX + "} NotX: {" + alu.NotX + "}");
                    Console.WriteLine("ZeroY: {" + alu.ZeroY + "} NotY: {" + alu.NotY + "}");
                    if (f == 0)
                    {
                        Console.WriteLine("F: {" + alu.F + "} -> AND, NotOutput: {" + alu.NotOutput + "}");
                    }
                    else
                    {
                        Console.WriteLine("F: {" + alu.F + "} -> ADD-PLUS, NotOutput: {" + alu.NotOutput + "}");
                    }

                    Console.WriteLine("-----X-----");
                    Console.WriteLine("X: " + alu.InputX.ToString());
                    Console.WriteLine("X': " + alu.w_xtag.ToString());
                    Console.WriteLine("X'': " + alu.w_xtagim.ToString());
                    Console.WriteLine("-----Y-----");
                    Console.WriteLine("Y: " + alu.InputY.ToString());
                    Console.WriteLine("Y': " + alu.w_ytag.ToString());
                    Console.WriteLine("Y'': " + alu.w_ytagim.ToString());

                    Console.WriteLine("-----Aritmethic-----");
                    Console.WriteLine("X'' and Y'': " + alu.w_xtagimANDytagim.ToString());
                    Console.WriteLine("X'' + Y'': " + alu.w_xtagimADDytagim);

                    Console.WriteLine("-----Final-----");
                    Console.WriteLine("F': " + alu.w_ftag.ToString());
                    Console.WriteLine("No: " + alu.w_No.ToString());
                    Console.WriteLine("No': " + alu.NoTag.ToString());

                    Console.WriteLine("-----Outputs-----");
                    Console.WriteLine("Output: " + alu.Output.ToString());
                    Console.WriteLine("Zero Output: " + alu.Zero);
                    Console.WriteLine("Negative Output: " + alu.Negative);


                    Console.WriteLine("done");
                    Console.WriteLine("Enter 'bye' to exit:");
                    ans = Console.ReadLine();
                }
                Console.Clear();

            }
        }
        public void tests1() { 
            //Now we ruin the nand gates that are used in all other gates. The gate should not work properly after this.
            

            //Bring the Nand gate back to work
            
            NAndGate.Corrupt = false;
            OrGate or = new OrGate();
            or.Input1.Value = 0;
            or.Input2.Value = 1;
            if (!or.TestGate())
                Console.WriteLine("bugbug");

            XorGate xor = new XorGate();
            xor.Input1.Value = 0;
            xor.Input2.Value = 1;
            if (!xor.TestGate())
                Console.WriteLine("bugbug");

            MultiBitAndGate multiAnd = new MultiBitAndGate(4);
            if(!multiAnd.TestGate())
            {
                Console.WriteLine("bugbug");
            }

            MultiBitOrGate multiOr = new MultiBitOrGate(4);
            if (!multiOr.TestGate())
            {
                Console.WriteLine("bugbug");
            }

            Console.WriteLine("done");
            Console.ReadLine();
        }

        
        public void tests2()
        {
            MuxGate mux = new MuxGate();
            //Test that the unit testing works properly
            if (!mux.TestGate())
                Console.WriteLine("bugbug");


            Demux demux = new Demux();
            //Test that the unit testing works properly
            if (!demux.TestGate())
                Console.WriteLine("bugbug");

            BitwiseAndGate bitWiseAnd = new BitwiseAndGate(3);
            bitWiseAnd.TestGate();

            BitwiseOrGate bitWiseOr = new BitwiseOrGate(3);
            bitWiseOr.TestGate();

            BitwiseNotGate bitWiseNot = new BitwiseNotGate(3);
            bitWiseNot.TestGate();

            BitwiseMux bitWiseMux = new BitwiseMux(3);
            bitWiseMux.TestGate();

            BitwiseDemux bitWiseDeMux = new BitwiseDemux(3);
            bitWiseDeMux.TestGate();



            BitwiseMultiwayMux multiwayMux = new BitwiseMultiwayMux(3, 3);
            multiwayMux.TestGate();

            BitwiseMultiwayDemux multiwayDeMux = new BitwiseMultiwayDemux(2, 3);
            multiwayDeMux.TestGate();

            WireSet wireSet = new WireSet(5);
            wireSet.SetValue(15);
            Console.WriteLine(wireSet.GetValue());
            Console.WriteLine(wireSet.Get2sComplement());
            wireSet.Set2sComplement(-3);
            Console.WriteLine(wireSet.GetValue());
            Console.WriteLine(wireSet.Get2sComplement());
        }

        public void testsAdders()
        {
            HalfAdder halfAdder = new HalfAdder();
            if (!halfAdder.TestGate())
            {
                Console.WriteLine("bugbug ha");
            }

            FullAdder fullAdder = new FullAdder();
            if (!fullAdder.TestGate())
            {
                Console.WriteLine("bugbug fa");
            }

            MultiBitAdder bitAdder = new MultiBitAdder(7);
            WireSet w1 = new WireSet(7);
            w1.Set2sComplement(1);
            WireSet w2 = new WireSet(7);
            w2.SetValue(28);
            //bitAdder.ConnectInput1(w1);
            //bitAdder.ConnectInput2(w2);

            if (!bitAdder.TestGate())
            {
                Console.WriteLine("bugbug ma");
            }

            //Console.WriteLine(bitAdder.Output.GetValue());
            //Console.WriteLine(bitAdder.ToString());
            //Console.WriteLine(bitAdder.Overflow.Value); 

            Console.WriteLine(bitAdder.Output.Get2sComplement());
        }
    }
}
