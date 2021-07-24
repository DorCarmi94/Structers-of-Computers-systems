using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements an adder, receving as input two n bit numbers, and outputing the sum of the two numbers
    class MultiBitAdder : Gate
    {
        //Word size - number of bits in each input
        public int Size { get; private set; }

        


        public WireSet Input1 { get; private set; }
        public WireSet Input2 { get; private set; }
        public WireSet Output { get; private set; }
        //An overflow bit for the summation computation
        public Wire Overflow { get; private set; }

        private FullAdder[] fullAdders;
        public MultiBitAdder(int iSize)
        {
            Size = iSize;
            Input1 = new WireSet(Size);
            Input2 = new WireSet(Size);
            Output = new WireSet(Size);
            //your code here
            

            fullAdders = new FullAdder[Size];
            fullAdders[0] = new FullAdder();
            fullAdders[0].Input1.ConnectInput(this.Input1[0]);
            fullAdders[0].Input2.ConnectInput(this.Input2[0]);
            fullAdders[0].CarryInput.Value = 0;
            this.Output[0].ConnectInput(fullAdders[0].Output);

            for (int i = 1; i < Size; i++)
            {
                fullAdders[i] = new FullAdder();
                fullAdders[i].ConnectInput1(this.Input1[i]);
                fullAdders[i].ConnectInput2(this.Input2[i]);
                this.Output[i].ConnectInput(fullAdders[i].Output);
                fullAdders[i].CarryInput.ConnectInput(fullAdders[i - 1].CarryOutput);
            }
            this.Overflow = fullAdders[fullAdders.Length - 1].CarryOutput;

        }

        public override string ToString()
        {
            return Input1 + "(" + Input1.Get2sComplement() + ")" + " + " + Input2 + "(" + Input2.Get2sComplement() + ")" + " = " + Output + "(" + Output.Get2sComplement() + ")";
        }

        public void ConnectInput1(WireSet wInput)
        {
            Input1.ConnectInput(wInput);
        }
        public void ConnectInput2(WireSet wInput)
        {
            Input2.ConnectInput(wInput);
        }


        public override bool TestGate()
        {
            
            
            Random rnd = new Random();
            int powNumber = (int)Math.Pow(2, Size-1);

            


            for (int i = 0; i < 100; i++)
            {


                int a = rnd.Next(powNumber * (-1), powNumber);
                int b = rnd.Next(powNumber * (-1), powNumber);
                WireSet wa = new WireSet(this.Size);
                WireSet wb = new WireSet(this.Size);

                
                if (a + b <= 63 && a + b >= -64)
                    {
                    wa.Set2sComplement(a);
                    wb.Set2sComplement(b);
                    for (int j = 0; j < Size; j++)
                    {
                        Input1[j].Value = wa[j].Value;
                        Input2[j].Value = wb[j].Value;
                    }

                    int sum = this.Output.Get2sComplement();
                    if (a + b != sum)
                    {
                        return false;
                    }
                }
            }
            return true;

        }
    }
}
