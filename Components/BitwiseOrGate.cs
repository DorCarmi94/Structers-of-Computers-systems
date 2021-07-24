using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A two input bitwise gate takes as input two WireSets containing n wires, and computes a bitwise function - z_i=f(x_i,y_i)
    class BitwiseOrGate : BitwiseTwoInputGate
    {
        //your code here
        private OrGate[] orArr;
        private int m_Size;
        public BitwiseOrGate(int iSize)
            : base(iSize)
        {
            //your code here
            orArr = new OrGate[iSize];
            m_Size = iSize;
            for (int i = 0; i < iSize; i++)
            {
                orArr[i] = new OrGate();
                orArr[i].ConnectInput1(this.Input1[i]);
                orArr[i].ConnectInput2(this.Input2[i]);
                this.Output[i].ConnectInput(orArr[i].Output);

            }
        }

        //an implementation of the ToString method is called, e.g. when we use Console.WriteLine(or)
        //this is very helpful during debugging
        public override string ToString()
        {
            return "Or " + Input1 + ", " + Input2 + " -> " + Output;
        }

        public override bool TestGate()
        {
            bool ans = true;
            int count = 0;
            //Calculate input 1
            double pow = Math.Pow(2, m_Size);
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

                        if (Input1[m].Value == 1 || Input2[m].Value == 1)
                        {
                            if (Output[m].Value != 1)
                            {
                                ans = false;
                            }
                        }
                        else
                        {
                            if (Output[m].Value == 1)
                            {
                                ans = false;
                            }
                        }
                    }

                    //Console.WriteLine(ans);
                }

            }
            //Console.WriteLine("Count: "+count);
            return ans;
        }
    }
}
