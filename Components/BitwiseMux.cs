using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A bitwise gate takes as input WireSets containing n wires, and computes a bitwise function - z_i=f(x_i)
    class BitwiseMux : BitwiseTwoInputGate
    {
        public Wire ControlInput { get; private set; }

        //your code here
        private MuxGate[] muxArr;
        private int m_Size;
        public BitwiseMux(int iSize)
            : base(iSize)
        {

            ControlInput = new Wire();
            //your code here
            muxArr = new MuxGate[iSize];
            m_Size = iSize;
            for (int i = 0; i < iSize; i++)
            {
                muxArr[i] = new MuxGate();
                muxArr[i].ConnectInput1(this.Input1[i]);
                muxArr[i].ConnectInput2(this.Input2[i]);
                muxArr[i].ConnectControl(this.ControlInput);
                this.Output[i].ConnectInput(muxArr[i].Output);
            }
        }

        public void ConnectControl(Wire wControl)
        {
            ControlInput.ConnectInput(wControl);
        }



        public override string ToString()
        {
            return "Mux " + Input1 + "," + Input2 + ",C" + ControlInput.Value + " -> " + Output;
        }




        public override bool TestGate()
        {
            bool ans = true;
            int count = 0;
            //Calculate input 1
            double pow = Math.Pow(2, m_Size);
            for (int q = 0; q <= 1; q++)
            {
                this.ControlInput.Value = q;
                //Console.WriteLine("--------Contorl: "+q);
                for (int i = 0; ans && i < Convert.ToInt64(pow); i++)
                {
                    int first = i;
                    for (int j = 0; ans && j < m_Size; j++)
                    {
                        this.Input1[j].Value = first % 2;
                        first = first / 2;
                    }
                    //Calculate input 2
                    for (int k = 0; ans && k < Convert.ToInt64(pow); k++)
                    {
                        int second = k;
                        for (int j = 0; j < m_Size; j++)
                        {
                            this.Input2[j].Value = second % 2;
                            second = second / 2;
                        }

                        //Test the output
                        //Console.WriteLine(this.ToString());
                        count++;
                        for (int m = 0; ans && m < m_Size; m++)
                        {
                            if(this.ControlInput.Value==0 && Output[m].Value!=Input1[m].Value)
                            {
                                ans = false;
                            }
                            if (this.ControlInput.Value == 1 && Output[m].Value != Input2[m].Value)
                            {
                                ans = false;
                            }


                        }

                        //Console.WriteLine(ans);
                    }

                }
            }
            //Console.WriteLine("Count: "+count);
            return ans;
        }
    }
}
