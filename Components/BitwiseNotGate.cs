using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This bitwise gate takes as input one WireSet containing n wires, and computes a bitwise function - z_i=f(x_i)
    class BitwiseNotGate : Gate
    {
        public WireSet Input { get; private set; }
        public WireSet Output { get; private set; }
        public int Size { get; private set; }

        //your code here
        private NotGate[] notArr;
        
        public BitwiseNotGate(int iSize)
        {
            Size = iSize;
            Input = new WireSet(Size);
            Output = new WireSet(Size);
            //your code here
            notArr = new NotGate[Size];
            
            for (int i = 0; i < iSize; i++)
            {
                notArr[i] = new NotGate();
                notArr[i].ConnectInput(this.Input[i]);
                this.Output[i].ConnectInput(notArr[i].Output);

            }
        }

        public void ConnectInput(WireSet ws)
        {
            Input.ConnectInput(ws);
        }

        //an implementation of the ToString method is called, e.g. when we use Console.WriteLine(not)
        //this is very helpful during debugging
        public override string ToString()
        {
            return "Not " + Input + " -> " + Output;
        }

        public override bool TestGate()
        {
            bool ans = true;
            int count = 0;
            //Calculate input 1
            double pow = Math.Pow(2, Size);
            for (int i = 0; ans && i < Convert.ToInt64(pow); i++)
            {
                int first = i;
                for (int j = 0; ans && j < Size; j++)
                {
                    this.Input[j].Value = first % 2;
                    first = first / 2;
                }
                

                    //Test the output
                    //Console.WriteLine(this.ToString());
                    count++;
                    for (int m = 0; ans && m < Size; m++)
                    {

                        if (Input[m].Value == 1 && Output[m].Value == 1)
                        {
                                ans = false;
                        }
                        else if(Input[m].Value == 1 && Output[m].Value == 1)
                        {

                                ans = false;
                        }
                    }

                    //Console.WriteLine(ans);
                

            }
            //Console.WriteLine("Count: "+count);
            return ans;
        }
    }
}
