using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A bitwise gate takes as input WireSets containing n wires, and computes a bitwise function - z_i=f(x_i)
    class BitwiseDemux : Gate
    {
        public int Size { get; private set; }
        public WireSet Output1 { get; private set; }
        public WireSet Output2 { get; private set; }
        public WireSet Input { get; private set; }
        public Wire Control { get; private set; }

        //your code here
        private Demux[] demuxArr;
        
        public BitwiseDemux(int iSize)
        {
            Size = iSize;
            Control = new Wire();
            Input = new WireSet(Size);
            Output1 = new WireSet(Size);
            Output2 = new WireSet(Size);
            //your code here
            demuxArr = new Demux[iSize];
            
            for (int i = 0; i < iSize; i++)
            {
                demuxArr[i] = new Demux();
                demuxArr[i].ConnectInput(this.Input[i]);
                demuxArr[i].ConnectControl(this.Control);
                this.Output1[i].ConnectInput(demuxArr[i].Output1);
                this.Output2[i].ConnectInput(demuxArr[i].Output2);
            }
        }

        public void ConnectControl(Wire wControl)
        {
            Control.ConnectInput(wControl);
        }
        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }

        public override bool TestGate()
        {
            bool ans = true;
            int count = 0;
            //Calculate input 1
            double pow = Math.Pow(2, Size);
            for (int q = 0; q <= 1; q++)
            {
                this.Control.Value = q;
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

                        if (this.Control.Value == 0 && Output1[m].Value != Input[m].Value)
                        {
                            ans = false;
                        }
                        if (this.Control.Value == 1 && Output2[m].Value != Input[m].Value)
                        {
                            ans = false;
                        }
                    }

                    //Console.WriteLine(ans);


                }
            }
            //Console.WriteLine("Count: "+count);
            return ans;
        }

        public override string ToString()
        {
            return "Demux " + Input +  ",C" + Control.Value + " -> " + "Out1: "+Output1+ " Out2: "+ Output2;
        }
    }
}
