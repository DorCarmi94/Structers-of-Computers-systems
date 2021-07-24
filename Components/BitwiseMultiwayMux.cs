using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a mux with k input, each input with n wires. The output also has n wires.

    class BitwiseMultiwayMux : Gate
    {
        //Word size - number of bits in each output
        public int Size { get; private set; }

        //The number of control bits needed for k outputs
        public int ControlBits { get; private set; } //k bits

        public WireSet Output { get; private set; }
        public WireSet Control { get; private set; }

        private int NumberOfInputs; // 2^k inputs
        public WireSet[] Inputs { get; private set; }//Inputs.Length=NumberOfInputs
        

        //your code here
        private BitwiseMux[] muxesTree;
        public BitwiseMultiwayMux(int iSize, int cControlBits)
        {
            Size = iSize;
            Output = new WireSet(Size);
            Control = new WireSet(cControlBits);
            Inputs = new WireSet[(int)Math.Pow(2, cControlBits)];
            this.NumberOfInputs = (int)Math.Pow(2, cControlBits);


            for (int i = 0; i < Inputs.Length; i++)
            {
                Inputs[i] = new WireSet(Size);
                
            }
            ControlBits = cControlBits;
            //your code here
            
            int powOf = (int)Math.Pow(2, cControlBits);
            muxesTree = new BitwiseMux[powOf];
            int muxCounter = 0;
            int inputsCounter = 0;
            int controlCount = 0;
            int muxFrom;
            int muxUntil;
            
            //first phase- leafs
            for (int i = 0; i < powOf; i+=2)
            {
                muxesTree[muxCounter] = new BitwiseMux(Size);
                muxesTree[muxCounter].ConnectInput1(this.Inputs[inputsCounter]);
                muxesTree[muxCounter].ConnectInput2(this.Inputs[inputsCounter+1]);
                muxesTree[muxCounter].ConnectControl(this.Control[controlCount]);
                inputsCounter+=2;
                muxCounter++;
                
            }
            controlCount++;
            muxFrom = 0;
            muxUntil = muxCounter;
            for (int i = 1; i < ControlBits; i++)
            {
                int j;
                int start = muxCounter;
                for ( j= muxFrom; j < muxUntil; j+=2)
                {
                    muxesTree[muxCounter] = new BitwiseMux(Size);
                    muxesTree[muxCounter].ConnectInput1(muxesTree[j].Output);
                    muxesTree[muxCounter].ConnectInput2(muxesTree[j+1].Output);
                    muxesTree[muxCounter].ConnectControl(this.Control[controlCount]);
                    muxCounter++;
                    
                }
                muxFrom = start;
                muxUntil = muxCounter;
                controlCount++;
            }
            Output.ConnectInput(this.muxesTree[muxFrom].Output);

        }


        public void ConnectInput(int i, WireSet wsInput)
        {
            Inputs[i].ConnectInput(wsInput);
        }
        public void ConnectControl(WireSet wsControl)
        {
            Control.ConnectInput(wsControl);
        }

        public override string ToString()
        {
            String str= "MultiwayMux: ";
            str += "Control: {"+this.Control.GetValue()+"}\n";
            for (int i = 0; i < this.NumberOfInputs; i++)
            {
                str += " Input" + i + " -> " + this.Inputs[i].ToString() + "\n";
            }
            str += "Ouptut: ->"+ this.Output.ToString();
            
            return str;
        }

        public override bool TestGate()
        {
            bool ans = true;
            int count = 0;
            //Set inputs
            for (int i = 0; i < this.NumberOfInputs; i++)
            {
                int number = i;
                for (int j = 0; j < this.Size; j++)
                {
                    this.Inputs[i][j].Value = number % 2;
                    number = number / 2;
                }
            }

            //Check all control options
            for (int i = 0; i < NumberOfInputs; i++)
            {
                int ctrl = i;
                for (int j = 0; j < ControlBits; j++)
                {
                    this.Control[j].Value = ctrl % 2;
                    ctrl = ctrl / 2;
                }

                count++;
                //Console.WriteLine(this.ToString());
                //Test Output
                for (int m = 0; ans && m < this.Size; m++)
                {
                    int chosenInput = Control.GetValue();
                    if (Inputs[chosenInput][m].Value!= Output[m].Value)
                    {
                        ans = false;
                    }
                }
                //Console.WriteLine(ans);
            }

            return ans;

        }
    }
}
